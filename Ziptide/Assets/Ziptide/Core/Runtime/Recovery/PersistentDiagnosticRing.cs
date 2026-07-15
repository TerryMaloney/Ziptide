using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Ziptide.Core
{
    /// <summary>
    /// Persistent, bounded capture for ZIPTIDE diagnostics. Android logcat is a rotating system
    /// buffer, so checkpoint evidence cannot depend on the relevant tags still being present when
    /// Terry ends a device pass. RecoveryRuntimeGate installs this sink before its first boot log.
    ///
    /// Two files are retained: the current segment and one previous segment. Each segment is capped
    /// before the next append, keeping the total storage bounded while preserving the most recent
    /// diagnostic history across process exits and logcat rotation.
    /// </summary>
    public static class PersistentDiagnosticRing
    {
        public const long MaxFileBytes = 512 * 1024;
        public const string CurrentFileName = "ziptide-diagnostics.log";
        public const string PreviousFileName = "ziptide-diagnostics.previous.log";
        private const int MaxMessageCharacters = 8192;

        private static readonly object Sync = new object();
        private static bool _installed;
        private static string _currentPath;
        private static string _previousPath;

        public static string CurrentPath => _currentPath;
        public static string PreviousPath => _previousPath;

        /// <summary>
        /// Called from SubsystemRegistration so Enter Play Mode without domain reload cannot retain
        /// duplicate callbacks or stale persistentDataPath values.
        /// </summary>
        public static void ResetSubscription()
        {
            lock (Sync)
            {
                if (_installed)
                    Application.logMessageReceivedThreaded -= Capture;
                _installed = false;
                _currentPath = null;
                _previousPath = null;
            }
        }

        public static void Install()
        {
            lock (Sync)
            {
                if (_installed) return;

                string root = Application.persistentDataPath;
                if (string.IsNullOrEmpty(root))
                    throw new InvalidOperationException("Application.persistentDataPath is unavailable.");

                Directory.CreateDirectory(root);
                _currentPath = Path.Combine(root, CurrentFileName);
                _previousPath = Path.Combine(root, PreviousFileName);

                Application.logMessageReceivedThreaded -= Capture;
                Application.logMessageReceivedThreaded += Capture;
                _installed = true;
            }

            // Emitted after subscription so the file itself proves the sink was active for this boot.
            Debug.Log("ZIPTIDE: DIAG_RING_READY file=" + CurrentFileName
                + " previous=" + PreviousFileName + " maxBytes=" + MaxFileBytes);
        }

        public static bool ShouldCapture(string message)
        {
            if (string.IsNullOrEmpty(message)) return false;
            return message.StartsWith("ZIPTIDE:", StringComparison.Ordinal)
                || message.StartsWith("ZIPTIDE_DIAG", StringComparison.Ordinal);
        }

        public static bool ShouldRotate(long currentLength, int incomingBytes, long maxBytes)
        {
            if (currentLength < 0) throw new ArgumentOutOfRangeException(nameof(currentLength));
            if (incomingBytes < 0) throw new ArgumentOutOfRangeException(nameof(incomingBytes));
            if (maxBytes <= 0) throw new ArgumentOutOfRangeException(nameof(maxBytes));
            return currentLength > 0 && currentLength + incomingBytes > maxBytes;
        }

        public static string FormatLine(DateTimeOffset timestamp, LogType type, string message)
        {
            string clean = message ?? string.Empty;
            if (clean.Length > MaxMessageCharacters)
                clean = clean.Substring(0, MaxMessageCharacters) + "...[truncated]";
            clean = clean.Replace("\r", "\\r").Replace("\n", "\\n");
            return timestamp.UtcDateTime.ToString("O") + "\t" + type + "\t" + clean
                + Environment.NewLine;
        }

        private static void Capture(string condition, string stackTrace, LogType type)
        {
            if (!ShouldCapture(condition)) return;

            string line = FormatLine(DateTimeOffset.UtcNow, type, condition);
            int incomingBytes = Encoding.UTF8.GetByteCount(line);

            try
            {
                lock (Sync)
                {
                    if (!_installed || string.IsNullOrEmpty(_currentPath)) return;

                    long currentLength = File.Exists(_currentPath)
                        ? new FileInfo(_currentPath).Length
                        : 0L;
                    if (ShouldRotate(currentLength, incomingBytes, MaxFileBytes))
                        RotateCurrentSegment();

                    File.AppendAllText(_currentPath, line, Encoding.UTF8);
                }
            }
            catch
            {
                // Never recurse into Unity logging from the log callback. Logcat remains available
                // if storage is temporarily unavailable; the next app boot retries installation.
            }
        }

        private static void RotateCurrentSegment()
        {
            if (File.Exists(_previousPath))
                File.Delete(_previousPath);
            if (File.Exists(_currentPath))
                File.Move(_currentPath, _previousPath);
        }
    }
}
