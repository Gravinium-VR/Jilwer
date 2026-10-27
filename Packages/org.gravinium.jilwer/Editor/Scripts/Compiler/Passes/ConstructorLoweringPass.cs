
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
            return new Rewriter().Visit(root);
        }

        private sealed class Rewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitConstructorDeclaration(
                ConstructorDeclarationSyntax node)
            {
                ClassDeclarationSyntax type = node.Ancestors()
                    .OfType<ClassDeclarationSyntax>()
                    .FirstOrDefault();

                if (type == null ||
                    !HasJilwerTypeAttribute(type) ||
                    node.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)) ||
                    node.Initializer != null)
                    return base.VisitConstructorDeclaration(node);

                int constructorCount = type.Members
                    .OfType<ConstructorDeclarationSyntax>()
                    .Count(c => !c.Modifiers.Any(
                        m => m.IsKind(SyntaxKind.StaticKeyword)));

                if (constructorCount != 1) return base.VisitConstructorDeclaration(node);

                ConstructorDeclarationSyntax rewritten =
                    (ConstructorDeclarationSyntax) base.VisitConstructorDeclaration(node);

                MethodDeclarationSyntax method =
                    SyntaxFactory.MethodDeclaration(
                            SyntaxFactory.PredefinedType(
                                SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                            SyntaxFactory.Identifier("__JilwerCtor"))
                        .WithModifiers(rewritten.Modifiers)
                        .WithParameterList(rewritten.ParameterList)
                        .WithBody(rewritten.Body)
                        .WithExpressionBody(rewritten.ExpressionBody)
                        .WithSemicolonToken(rewritten.SemicolonToken)
                        .WithAttributeLists(rewritten.AttributeLists)
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
    }
}
