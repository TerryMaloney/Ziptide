using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Gameplay.DevTools
{
    /// <summary>
    /// Runtime-loadable list of worlds (+ their named markers) for dev tools that can't use the
    /// editor-only AssetDatabase — chiefly the in-VR <see cref="DevMenu"/>. Regenerate from the
    /// WorldPackDefinitions via <c>Ziptide → Dev → Rebuild Dev World Manifest</c>. Lives under a
    /// Resources folder so it loads on-device.
    /// </summary>
    public class DevWorldManifest : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string sceneName;
            public string displayName;
            public List<string> markerIds = new List<string>();
        }

        public List<Entry> worlds = new List<Entry>();

        public const string ResourceName = "DevWorldManifest";
        private static DevWorldManifest _cached;

        /// <summary>Load the manifest from Resources (cached). Null if it hasn't been generated yet.</summary>
        public static DevWorldManifest Load()
        {
            if (_cached == null)
                _cached = Resources.Load<DevWorldManifest>(ResourceName);
            return _cached;
        }
    }
}
