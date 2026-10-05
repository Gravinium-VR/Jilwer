using System;
using System.Collections.Generic;
using System.Linq;
using Gravinium.Jilwer.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gravinium.Jilwer.Editor
{
    public sealed class IntrinsicRewriter : CSharpSyntaxRewriter
    {
        private const string RuntimeFieldName = "__jilwerRuntime";
        
        private readonly SemanticModel _semanticModel;
        private readonly INamedTypeSymbol _intrinsicAttributeSymbol;

        private readonly HashSet<INamedTypeSymbol> _runtimeTypes = new(SymbolEqualityComparer.Default);
        public IReadOnlyCollection<INamedTypeSymbol> RuntimeTypes => _runtimeTypes;

        public IntrinsicRewriter(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;

            _intrinsicAttributeSymbol =
                semanticModel.Compilation.GetTypeByMetadataName("Gravinium.Jilwer.Core.JilwerIntrinsic")
                ?? throw new InvalidOperationException("Could not resolve Gravinium.Jilwer.Core.JilwerIntrinsic");
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            IMethodSymbol method = _semanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;

            InvocationExpressionSyntax rewritten = (InvocationExpressionSyntax)base.VisitInvocationExpression(node);

            if (method == null) return rewritten;

            AttributeData intrinsic = GetIntrinsic(method);

            if (intrinsic == null) return rewritten;

            if (_semanticModel.GetEnclosingSymbol(node.SpanStart) is not IMethodSymbol caller)
                throw new InvalidOperationException($"Unable to determine caller for intrinsic '{method.Name}'.");
            if (caller.IsStatic) throw new InvalidOperationException( $"Jilwer intrinsic '{method.Name}' cannot currently be used " +
                                                                      "inside a static method because it requires a runtime instance.");

            _runtimeTypes.Add(caller.ContainingType);

            IntrinsicType intrinsicType = GetIntrinsicType(intrinsic);
            return RewriteIntrinsic(rewritten, intrinsicType);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            INamedTypeSymbol classSymbol = _semanticModel.GetDeclaredSymbol(node);
            ClassDeclarationSyntax rewritten = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);

            if (classSymbol == null || !_runtimeTypes.Contains(classSymbol)) return rewritten;

            bool alreadyHasRuntime = rewritten.Members
                .OfType<FieldDeclarationSyntax>()
                .SelectMany(x => x.Declaration.Variables)
                .Any(x => x.Identifier.Text == RuntimeFieldName);

            if (alreadyHasRuntime) return rewritten;

            MemberDeclarationSyntax runtimeField = SyntaxFactory.ParseMemberDeclaration(
                $@"
[UnityEngine.SerializeField]
[UnityEngine.HideInInspector]
private Gravinium.Jilwer.Core.JilwerRuntime {RuntimeFieldName};
"
                );

            return rewritten.AddMembers(runtimeField);
        }

        private AttributeData GetIntrinsic(IMethodSymbol method)
        {
            return Enumerable.FirstOrDefault(method.GetAttributes(), attribute =>
                SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, _intrinsicAttributeSymbol));
        }

        private static IntrinsicType GetIntrinsicType(AttributeData intrinsic)
        {
            if (intrinsic.ConstructorArguments.Length == 0)
                throw new InvalidOperationException("JilwerIntrinsic is missing an IntrinsicType.");

            object value = intrinsic.ConstructorArguments[0].Value;

            return (IntrinsicType)Convert.ToInt32(value);
        }

        private static InvocationExpressionSyntax RewriteIntrinsic(InvocationExpressionSyntax node, IntrinsicType type)
        {
            return type switch
            {
                IntrinsicType.Runtime => RewriteRuntimeIntrinsic(node),
                _ => throw new InvalidOperationException($"Unsupported Jilwer intrinsic type '{type}'")
            };
        }

        private static InvocationExpressionSyntax RewriteRuntimeIntrinsic(InvocationExpressionSyntax node)
        {
            ArgumentSyntax runtimeArg = SyntaxFactory.Argument(SyntaxFactory.IdentifierName(RuntimeFieldName));

            SeparatedSyntaxList<ArgumentSyntax> arguments = node.ArgumentList.Arguments.Insert(0, runtimeArg);

            return node.WithArgumentList(node.ArgumentList.WithArguments(arguments));
        }
    }
}