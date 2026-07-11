#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.IO;
using UnityEngine;

namespace Ziptide.Gameplay.DevTools
{
    /// <summary>
    /// Controller-free access gate for the development-only warp menu.
    ///
    /// On Quest, ADB writes marker files into Application.persistentDataPath. Possession of an
    /// authorized ADB connection is the developer-login boundary; no gameplay input is reserved and
    /// no password/secret is embedded in the APK. The whole type is omitted from shipping builds.
    /// </summary>
    public static class DevAccessGate
    {
        public const string AccessMarkerName = ".ziptide_dev_access";
        public const string OpenMarkerName = ".ziptide_dev_open";

        private static bool _warnedStorageFailure;

        public static bool IsUnlocked
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return IsUnlockedAt(Application.persistentDataPath);
#endif
            }
        }

        public static bool TryConsumeOpenRequest()
        {
            return TryConsumeOpenRequestAt(Application.persistentDataPath);
        }

        public static string AccessMarkerPath(string root)
        {
            return MarkerPath(root, AccessMarkerName);
        }

        public static string OpenMarkerPath(string root)
        {
            return MarkerPath(root, OpenMarkerName);
        }

        /// <summary>Pure/file-system seam used by EditMode tests and the runtime property.</summary>
        public static bool IsUnlockedAt(string root)
        {
            if (string.IsNullOrWhiteSpace(root)) return false;
            try
            {
                return File.Exists(AccessMarkerPath(root));
            }
            catch (Exception ex)
            {
                WarnStorageFailure(ex);
                return false;
            }
        }

        /// <summary>
        /// Returns true only after an unlocked one-shot request has been successfully deleted.
        /// A failed delete never opens the menu repeatedly or bypasses the access marker.
        /// </summary>
        public static bool TryConsumeOpenRequestAt(string root)
        {
            if (!IsUnlockedAt(root)) return false;

            string requestPath;
            try
            {
                requestPath = OpenMarkerPath(root);
                if (!File.Exists(requestPath)) return false;
                File.Delete(requestPath);
                return !File.Exists(requestPath);
            }
            catch (Exception ex)
            {
                WarnStorageFailure(ex);
                return false;
            }
        }

        private static string MarkerPath(string root, string markerName)
        {
            if (string.IsNullOrWhiteSpace(root)) return string.Empty;
            return Path.Combine(root, markerName);
        }

        private static void WarnStorageFailure(Exception ex)
        {
            if (_warnedStorageFailure) return;
            _warnedStorageFailure = true;
            Debug.LogWarning("ZIPTIDE: DEV_ACCESS storage_unavailable type=" + ex.GetType().Name);
        }
    }
}
#endif
