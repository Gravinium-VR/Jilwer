using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gravinium.Jilwer.Editor.Compiler.Passes
{
    internal sealed class ConstructorLoweringPass : IJilwerLowerPass
    {
        public bool CanRun(CSharpCompilation compilation)
        {
            return true;
        }

        public SyntaxNode Rewrite(
            SyntaxNode root,
            SemanticModel semanticModel,
            JilwerCompilationContext context)
        {
            return new Rewriter(semanticModel).Visit(root);
        }

        private sealed class Rewriter : CSharpSyntaxRewriter
        {
            private const string InstanceName = "__jilwerInstance";
            private readonly SemanticModel _semanticModel;

            public Rewriter(SemanticModel semanticModel) => _semanticModel = semanticModel;
            
            public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            {
                ClassDeclarationSyntax type = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();

                if (type == null ||
                    !HasJilwerTypeAttribute(type) ||
                    node.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
                    return base.VisitConstructorDeclaration(node);

                if (node.Initializer != null)
                    throw new NotSupportedException(
                        $"Jilwer constructor initializers are not yet supported: {type.Identifier}");

                int count = type.Members.OfType<ConstructorDeclarationSyntax>()
                    .Count(c => !c.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)));

                if (count != 1)
                    throw new NotSupportedException(
                        $"Jilwer only supports one constructor per type: {type.Identifier}");

                if (node.Body == null)
                    throw new NotSupportedException($"Jilwer requires a body in the constructor: {type.Identifier}");

                string typeName = type.Identifier.ValueText;

                string generated = $@"
public static {typeName} __JilwerCtor()
{{
    var __jilwerError = Gravinium.Jilwer.Core.TypeRegistry.Create(runtime, nameof({typeName}), out UnityEngine.GameObject __jilwerObject);
    if (__jilwerObject == null) return null;

    var {InstanceName} = __jilwerObject.GetComponent<{typeName}>();
    if ({InstanceName} == null) return null;

    return {InstanceName};
}}";

                var method = SyntaxFactory.ParseMemberDeclaration(generated) as MethodDeclarationSyntax;
                if (method == null) throw new InvalidOperationException($"Failed to generate Jilwer constructor.");

                ParameterSyntax runtimeParameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("runtime"))
                    .WithType(SyntaxFactory.ParseTypeName("Gravinium.Jilwer.Core.JilwerRuntime"));

                ParameterListSyntax parameters =
                    node.ParameterList.WithParameters(node.ParameterList.Parameters.Insert(0, runtimeParameter));

                var bodyRewriter = new InstanceMemberRewriter(_semanticModel, InstanceName);

                var originalStatements = node.Body.Statements
                    .Select(statement => (StatementSyntax)bodyRewriter.Visit(statement)).ToArray();

                BlockSyntax body = method.Body;
                
                int returnIndex = body.Statements.Count - 1;

                var statements = body.Statements.InsertRange(returnIndex, originalStatements);

                method = method
                    .WithParameterList(parameters)
                    .WithBody(body.WithStatements(statements))
                    .WithModifiers(
                        node.Modifiers.Any()
                            ? node.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                            : SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                    .WithTriviaFrom(node);

                return method;
            }

            private static bool HasJilwerTypeAttribute(ClassDeclarationSyntax type)
            {
                return type.AttributeLists
                    .SelectMany(list => list.Attributes)
                    .Any(attribute => attribute.Name.ToString().Split('.').Last() == "JilwerType");
            }
        }

        private sealed class InstanceMemberRewriter : CSharpSyntaxRewriter
        {
            private readonly SemanticModel _semanticModel;
            private readonly string _instance;

            public InstanceMemberRewriter(SemanticModel semanticModel, string instance)
            {
                _semanticModel = semanticModel;
                _instance = instance;
            }

            public override SyntaxNode VisitThisExpression(ThisExpressionSyntax node)
            {
                return SyntaxFactory.IdentifierName(_instance).WithTriviaFrom(node);
            }

            public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
            {
                ISymbol symbol = _semanticModel.GetSymbolInfo(node).Symbol;
                if (symbol == null ||
                    node.Parent is MemberAccessExpressionSyntax access &&
                    access.Name == node)
                    return base.VisitIdentifierName(node);

                bool instanceMember = symbol switch
                {
                    IFieldSymbol field => !field.IsStatic,
                    IPropertySymbol property => !property.IsStatic,
                    IMethodSymbol method => !method.IsStatic && method.MethodKind == MethodKind.Ordinary,
                    _ => false
                };

                if (!instanceMember) return base.VisitIdentifierName(node);

                return SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(_instance),
                        node.WithoutTrivia())
                    .WithTriviaFrom(node);
            }
        }
    }
}
