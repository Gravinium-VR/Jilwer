using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace Gravinium.Jilwer.Editor.Compiler.Integration
{
    internal sealed class JilwerSourceTransaction : IDisposable
    {
        private sealed class Backup
        {
            public string Path;
            public byte[] Contents;
            public DateTime LastWriteUtc;
        }

        private readonly IReadOnlyList<JilwerGeneratedSource> _sources;
        private readonly List<Backup> _backups = new();

        private bool _applied;

        public JilwerSourceTransaction(IReadOnlyList<JilwerGeneratedSource> sources) => _sources = sources;

        public void Apply()
        {
            if (_applied) throw new InvalidOperationException($"Transaction has already been applied.");
            
            AssetDatabase.DisallowAutoRefresh();

            try
            {
                foreach (JilwerGeneratedSource source in _sources)
                {
                    string path = source.SourcePath;

                    Backup backup = new Backup
                    {
                        Path = path,
                        Contents = File.ReadAllBytes(path),
                        LastWriteUtc = File.GetCreationTimeUtc(path),
                    };

                    _backups.Add(backup);

                    File.WriteAllText(path, source.GeneratedSource, new UTF8Encoding(false));
                }

                _applied = true;
            }
            catch
            {
                Restore();
                throw;
            }
        }

        public void Dispose() => Restore();

        public void Restore()
        {
            if (_backups.Count == 0) return;

            try
            {
                foreach (Backup backup in _backups)
                {
                    File.WriteAllBytes(backup.Path, backup.Contents);

                    File.SetLastWriteTimeUtc(backup.Path, backup.LastWriteUtc);
                }
            }
            finally
            {
                _backups.Clear();
                _applied = false;
                AssetDatabase.AllowAutoRefresh();
            }
        }
    }
}