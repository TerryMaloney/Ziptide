using System;
using System.IO;

namespace Ziptide.Core
{
    /// <summary>
    /// Crash-proof save file IO (ship-killer sweep, 2026-07-10). The old path was
    /// File.WriteAllText straight over the ONLY copy — a battery death or app kill mid-write
    /// truncates the file, and the player silently gets a fresh profile: total progress wipe.
    /// This store makes that impossible:
    ///   WRITE: serialize to `file.tmp` → swap into place, demoting the previous good file to
    ///          `file.bak`. The main file is never open for writing — it's either the old complete
    ///          version or the new complete version, at every instant.
    ///   READ:  main first; if it's missing or fails the caller's parse, the previous good version
    ///          is still sitting in `.bak`.
    /// Pure static IO — EditMode tests exercise the full corrupt-and-recover dance on temp files.
    /// </summary>
    public static class SaveFileStore
    {
        public static string TmpPath(string path) => path + ".tmp";
        public static string BakPath(string path) => path + ".bak";

        /// <summary>Atomically replace <paramref name="path"/> with <paramref name="contents"/>,
        /// keeping the previous version as .bak. Throws on IO failure (caller logs).</summary>
        public static void WriteAtomic(string path, string contents)
        {
            string tmp = TmpPath(path), bak = BakPath(path);
            File.WriteAllText(tmp, contents);
            if (File.Exists(path))
            {
                // Replace is atomic-on-same-volume where the platform supports it; the manual
                // fallback still never leaves us without at least one complete file.
                try { File.Replace(tmp, path, bak); }
                catch (PlatformNotSupportedException)
                {
                    if (File.Exists(bak)) File.Delete(bak);
                    File.Move(path, bak);
                    File.Move(tmp, path);
                }
            }
            else
            {
                File.Move(tmp, path);
            }
        }

        /// <summary>Read the newest COMPLETE version: main if it parses, else the .bak. The parse
        /// check belongs to the caller (we can't know the format) — pass a validator that returns
        /// true when the text is usable. Returns null when nothing usable exists.</summary>
        public static string ReadBestVersion(string path, Func<string, bool> isUsable, out bool recoveredFromBackup)
        {
            recoveredFromBackup = false;
            string main = TryRead(path);
            if (main != null && isUsable(main)) return main;

            string bak = TryRead(BakPath(path));
            if (bak != null && isUsable(bak))
            {
                recoveredFromBackup = true;
                return bak;
            }
            return null;
        }

        private static string TryRead(string path)
        {
            try { return File.Exists(path) ? File.ReadAllText(path) : null; }
            catch (Exception) { return null; }
        }
    }
}
