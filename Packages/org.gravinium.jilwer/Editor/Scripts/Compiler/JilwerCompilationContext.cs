using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Gravinium.Jilwer.Editor.Compiler
{
    [Flags]
    internal enum JilwerRequirement
    {
        None = 0,
        Runtime = 1 << 0,
    }
    
    internal sealed class JilwerCompilationContext
    {
        private readonly Dictionary<string, JilwerRequirement> _requirements = new();

        public IReadOnlyDictionary<string, JilwerRequirement> Requirements => _requirements;

        public void Require(INamedTypeSymbol type, JilwerRequirement requirement)
        {
            string key = GetTypeKey(type);

            if (_requirements.TryGetValue(key, out JilwerRequirement existing))
                _requirements[key] = existing | requirement;
            else
                _requirements[key] = requirement;
        }

        public bool Requires(INamedTypeSymbol type, JilwerRequirement requirement)
        {
            string key = GetTypeKey(type);

            return _requirements.TryGetValue(key, out JilwerRequirement existing) && (existing & requirement) != 0;
        }

        private static string GetTypeKey(INamedTypeSymbol type)
        {
            return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
    }
}