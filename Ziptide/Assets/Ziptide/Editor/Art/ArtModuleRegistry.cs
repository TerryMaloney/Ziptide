#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Editor.Art
{
    /// <summary>
    /// THE ART REGISTRY seam (V2.5 H4 — contract in docs/design/ART_REGISTRY.md). Builders request a
    /// LOOK by id ("buildingModule:&lt;styleId&gt;/&lt;module&gt;", "scatterKind:&lt;id&gt;", …) and never know the
    /// fulfillment tech; the art track registers factories (Forge recipes today, imported kits later)
    /// in ITS lane. Unfulfilled ids fall back to the caller's primitive — zero visual change until a
    /// kit exists (the ForgeVisualApplier.TryApply shape). Editor-side on purpose: all consumers are
    /// build-time patchers; a runtime registry joins only if runtime art streaming ever does (§5
    /// deferral triggers in ART_REGISTRY.md).
    /// </summary>
    public static class ArtModuleRegistry
    {
        private static readonly Dictionary<string, Func<GameObject>> _factories =
            new Dictionary<string, Func<GameObject>>();

        /// <summary>Art track: bind an id to a factory producing a ready-to-place GameObject.
        /// Last registration wins (kits can upgrade primitives without touching consumers).</summary>
        public static void Register(string id, Func<GameObject> factory)
        {
            if (string.IsNullOrEmpty(id) || factory == null) return;
            _factories[id] = factory;
        }

        /// <summary>Consumers: try the registry; false = build your primitive fallback.</summary>
        public static bool TryBuild(string id, out GameObject built)
        {
            built = null;
            if (string.IsNullOrEmpty(id) || !_factories.TryGetValue(id, out var f)) return false;
            try { built = f(); }
            catch (Exception ex)
            {
                Debug.LogWarning("[Ziptide] ART_REGISTRY_FACTORY_FAILED id=" + id + ": " + ex.Message);
                return false;
            }
            return built != null;
        }

        public static bool Has(string id) => !string.IsNullOrEmpty(id) && _factories.ContainsKey(id);
        public static void Clear() => _factories.Clear(); // tests / domain reload hygiene

        /// <summary>The current factory for an id (E5.1): lets an upgrader WRAP the existing
        /// fulfillment instead of discarding it — the textured kit decorates the primitive kit's
        /// output, so an unbaked build degrades to the primitive look, never to nothing.</summary>
        public static bool TryGetFactory(string id, out Func<GameObject> factory)
        {
            factory = null;
            return !string.IsNullOrEmpty(id) && _factories.TryGetValue(id, out factory);
        }
    }
}
#endif
