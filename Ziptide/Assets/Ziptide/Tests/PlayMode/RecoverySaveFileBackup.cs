using System;
using System.Collections.Generic;
using System.IO;
using Ziptide.Core;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Test-only exact byte backup for the profile main/backup/temp files. Actual-scene travel tests
    /// must be allowed to exercise SaveSystem's real persistentDataPath without destroying a local
    /// developer profile or inheriting an unrelated Continue state from an earlier test run.
    /// </summary>
    public sealed class RecoverySaveFileBackup : IDisposable
    {
        private sealed class Entry
        {
            public string Path;
            public bool Existed;
            public byte[] Bytes;
        }

        private readonly List<Entry> _entries;
        private bool _restored;

        private RecoverySaveFileBackup(List<Entry> entries)
        {
            _entries = entries;
        }

        public static RecoverySaveFileBackup CaptureAndClear(string mainPath)
        {
            if (string.IsNullOrEmpty(mainPath))
                throw new ArgumentException("Main save path is required.", nameof(mainPath));

            string[] paths =
            {
                mainPath,
                SaveFileStore.BakPath(mainPath),
                SaveFileStore.TmpPath(mainPath)
            };
            var entries = new List<Entry>(paths.Length);
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                bool existed = File.Exists(path);
                entries.Add(new Entry
                {
                    Path = path,
                    Existed = existed,
                    Bytes = existed ? File.ReadAllBytes(path) : null
                });
                if (existed) File.Delete(path);
            }
            return new RecoverySaveFileBackup(entries);
        }

        public void Dispose()
        {
            if (_restored) return;
            _restored = true;

            for (int i = 0; i < _entries.Count; i++)
            {
                Entry entry = _entries[i];
                if (File.Exists(entry.Path)) File.Delete(entry.Path);
                if (!entry.Existed) continue;

                string directory = Path.GetDirectoryName(entry.Path);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
                File.WriteAllBytes(entry.Path, entry.Bytes ?? Array.Empty<byte>());
            }
        }
    }
}
