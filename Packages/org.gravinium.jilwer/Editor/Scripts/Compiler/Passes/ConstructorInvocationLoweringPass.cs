using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gravinium.Jilwer.Editor.Compiler.Passes
{
    internal sealed class ConstructorInvocationLoweringPass : IJilwerLowerPass
    {
        public bool CanRun(CSharpCompilation compilation)
        {
            return true;
        }

        public SyntaxNode Rewrite(SyntaxNode root, SemanticModel semanticModel, JilwerCompilationContext context)
        {
            return new Rewriter(semanticModel, context).Visit(root);
        }

        private sealed class Rewriter : CSharpSyntaxRewriter
        {
            private readonly SemanticModel _semanticModel;
            private readonly JilwerCompilationContext _context;

            public Rewriter(SemanticModel semanticModel, JilwerCompilationContext context)
            {
                _semanticModel = semanticModel;
                _context = context;
            }

            public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
            {
                IMethodSymbol constructor = _semanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;

                ObjectCreationExpressionSyntax rewritten =
                    (ObjectCreationExpressionSyntax)base.VisitObjectCreationExpression(node);

                if (constructor == null) return rewritten;

                INamedTypeSymbol type = constructor.ContainingType;

                if (!IsJilwerType(type)) return rewritten;

                if (node.Initializer != null)
                    throw new NotSupportedException($"Jilwer object initializers are not yet supported: {type.Name}");

                if (node.ArgumentList.Arguments.Any(argument => argument.NameColon != null))
                    throw new NotSupportedException($"Named constructor arguments are not yet supported: {type.Name}");

                string runtimeName;

                ConstructorDeclarationSyntax enclosingConstructor =
                    node.Ancestors().OfType<ConstructorDeclarationSyntax>().FirstOrDefault();

                if (enclosingConstructor != null && IsJilwerType(enclosingConstructor)) runtimeName = "runtime";
                else
                {
                    ISymbol caller = _semanticModel.GetEnclosingSymbol(node.SpanStart);

                    if (!(caller is IMethodSymbol method))
                        throw new NotSupportedException(
                            $"Cannot create '{type.Name}' outside of a supported instance method.");

                    if (method.IsStatic)
                        throw new NotSupportedException(
                            $"Cannot create '{type.Name}' inside static method '{method.Name}' without an explicit JilwerRuntime.");
                    
                    _context.Require(method.ContainingType, JilwerRequirement.Runtime);

                    runtimeName = RuntimeStatePass.RuntimeFieldName;
                }

                ArgumentSyntax runtimeArgument = SyntaxFactory.Argument(SyntaxFactory.IdentifierName(runtimeName));

                ArgumentListSyntax arguments =
                    rewritten.ArgumentList.WithArguments(rewritten.ArgumentList.Arguments.Insert(0, runtimeArgument));

                ExpressionSyntax factory = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    rewritten.Type.WithoutTrivia(), SyntaxFactory.IdentifierName("__JilwerCtor"));

                return SyntaxFactory.InvocationExpression(factory, arguments).WithTriviaFrom(node);
            }

            private static bool IsJilwerType(INamedTypeSymbol type)
            {
                return type.GetAttributes().Any(attribute => attribute.AttributeClass?.Name == "JilwerType");
            }

            private static bool IsJilwerType(ConstructorDeclarationSyntax constructor)
            {
                ClassDeclarationSyntax type = constructor.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
                if (type == null) return false;
                
                return type.AttributeLists
                    .SelectMany(list => list.Attributes)
                    .Any(attribute => attribute.Name.ToString().Split('.').Last() == "JilwerType");
            }
        }
    }
}