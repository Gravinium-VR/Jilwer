using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gravinium.Jilwer.Editor.Compiler.Passes
{
    internal sealed class RuntimeStatePass : IJilwerLowerPass
    {
        public const string RuntimeFieldName = "__jilwerRuntime";


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

            public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                INamedTypeSymbol classSymbol = _semanticModel.GetDeclaredSymbol(node);

                ClassDeclarationSyntax rewritten = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);

                if (classSymbol == null) return rewritten;

                if (!_context.Requires(classSymbol, JilwerRequirement.Runtime)) return rewritten;

                if (HasRuntimeField(rewritten)) return rewritten;

                MemberDeclarationSyntax runtimeField = SyntaxFactory.ParseMemberDeclaration($@"
[UnityEngine.SerializeField]
[UnityEngine.HideInInspector]
private Gravinium.Jilwer.Core.JilwerRuntime {RuntimeFieldName};
");

                return rewritten.AddMembers(runtimeField);
            }

            private static bool HasRuntimeField(ClassDeclarationSyntax node)
            {
                return node.Members.OfType<FieldDeclarationSyntax>().SelectMany(field => field.Declaration.Variables)
                    .Any(variable => variable.Identifier.Text == RuntimeFieldName);
            }
        }
    }
}