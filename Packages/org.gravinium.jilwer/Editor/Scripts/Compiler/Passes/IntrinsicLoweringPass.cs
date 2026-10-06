using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gravinium.Jilwer.Editor.Compiler.Passes
{
    internal sealed class IntrinsicLoweringPass : IJilwerLowerPass
    {
        private const string IntrinsicAttributeName = "Gravinium.Jilwer.Core.JilwerIntrinsic";
        
        public bool CanRun(CSharpCompilation compilation)
        {
            return compilation.GetTypeByMetadataName(IntrinsicAttributeName) != null;
        }

        public SyntaxNode Rewrite(SyntaxNode root, SemanticModel semanticModel, JilwerCompilationContext context)
        {
            INamedTypeSymbol intrinsicAttribute =
                semanticModel.Compilation.GetTypeByMetadataName(IntrinsicAttributeName);

            if (intrinsicAttribute == null) return root;

            Rewriter rewriter = new Rewriter(semanticModel, context, intrinsicAttribute);

            return rewriter.Visit(root);
        }

        private sealed class Rewriter : CSharpSyntaxRewriter
        {
            private readonly SemanticModel _semanticModel;
            private readonly JilwerCompilationContext _context;
            private readonly INamedTypeSymbol _intrinsicAttribute;

            public Rewriter(SemanticModel semanticModel, JilwerCompilationContext context,
                INamedTypeSymbol intrinsicAttribute)
            {
                _semanticModel = semanticModel;
                _context = context;
                _intrinsicAttribute = intrinsicAttribute;
            }

            public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                IMethodSymbol method = _semanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;

                InvocationExpressionSyntax rewritten = (InvocationExpressionSyntax)base.VisitInvocationExpression(node);

                if (method == null) return rewritten;

                if (!HasIntrinsicAttribute(method)) return rewritten;

                if (_semanticModel.GetEnclosingSymbol(node.SpanStart) is not IMethodSymbol caller)
                    throw new InvalidOperationException($"Unable to determine caller for intrinsic '{method.Name}'");

                if (caller.IsStatic)
                    throw new InvalidOperationException($"Jilwer intrinsic '{method.Name}' cannot be used " +
                                                        $"inside static method '{caller.Name}' because " +
                                                        $"it requires an instance runtime.");
                
                _context.Require(caller.ContainingType, JilwerRequirement.Runtime);

                return InjectRuntimeArgument(rewritten);
            }

            private bool HasIntrinsicAttribute(IMethodSymbol method)
            {
                return method.GetAttributes().Any(attribute =>
                    SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, _intrinsicAttribute));
            }

            private static InvocationExpressionSyntax InjectRuntimeArgument(InvocationExpressionSyntax node)
            {
                ArgumentSyntax runtimeArgument =
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(RuntimeStatePass.RuntimeFieldName));

                SeparatedSyntaxList<ArgumentSyntax> arguments = node.ArgumentList.Arguments.Insert(0, runtimeArgument);

                return node.WithArgumentList(node.ArgumentList.WithArguments(arguments));
            }
        }
    }
}