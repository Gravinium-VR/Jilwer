using System.Collections.Generic;

namespace Gravinium.Jilwer.Editor.Compiler
{
    internal sealed class JilwerCompilationResult
    {
        public string AssemblyName { get; }

        public IReadOnlyList<JilwerGeneratedSource> Sources { get; }

        public JilwerCompilationContext Context { get; }

        public JilwerCompilationResult(string assemblyName, IReadOnlyList<JilwerGeneratedSource> sources,
            JilwerCompilationContext context)
        {
            AssemblyName = assemblyName;
            Sources = sources;
            Context = context;
        }
}
}