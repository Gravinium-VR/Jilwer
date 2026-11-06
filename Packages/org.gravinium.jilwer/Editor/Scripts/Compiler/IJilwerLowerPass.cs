using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Gravinium.Jilwer.Editor.Compiler
{
    internal interface IJilwerLowerPass
    {
        bool CanRun(CSharpCompilation compilation);

        SyntaxNode Rewrite(SyntaxNode root, SemanticModel semanticModel, JilwerCompilationContext context);
    }
}