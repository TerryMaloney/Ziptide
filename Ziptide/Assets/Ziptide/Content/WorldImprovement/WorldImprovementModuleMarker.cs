using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Serialized identity for one deterministic module output. This class intentionally lives in its
    /// own filename so Unity can persist the component reliably across scene save/reopen and APK builds.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WorldImprovementModuleMarker : MonoBehaviour
    {
        [SerializeField] private string moduleId;
        [SerializeField] private int moduleVersion;
        [SerializeField] private int objectCount;
        [SerializeField] private string[] aspects = Array.Empty<string>();

        public string ModuleId => moduleId;
        public int ModuleVersion => moduleVersion;
        public int ObjectCount => objectCount;
        public string[] Aspects => aspects;

        public void Configure(WorldImprovementModuleSpec spec, int count)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            moduleId = spec.moduleId ?? string.Empty;
            moduleVersion = spec.version;
            objectCount = Mathf.Max(0, count);
            aspects = spec.aspects ?? Array.Empty<string>();
        }
    }
}
