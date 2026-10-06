namespace Gravinium.Jilwer.Editor.Compiler
{
    internal sealed class JilwerGeneratedSource
    {
        public string AssemblyName { get; }
        public string SourcePath { get; }
        
        public string OriginalSource { get; }
        public string GeneratedSource { get; }

        public JilwerGeneratedSource(string assemblyName, string sourcePath, string originalSource,
            string generatedSource)
        {
            AssemblyName = assemblyName;
            SourcePath = sourcePath;
            OriginalSource = originalSource;
            GeneratedSource = generatedSource;
        }
    }
}