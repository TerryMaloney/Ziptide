#if UNITY_EDITOR
using UnityEngine;
using Ziptide.Content;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// FORGE III F3.3 commit 4 — WATER PLACEMENT. Called from WorldDressingBuilder.Build: where a
    /// world has a shipyard berth, fill it with a <see cref="ZiptideWater"/> body — the tidefront/
    /// canal read the game is named for. Uses the EXISTING berth rect (auto-aligns to the layout;
    /// no hand-authored coordinates, no shared-data edit). A LOOK, never a stat: no collider, no
    /// gameplay change. Worlds without a berth get no water (default = zero risk). The component
    /// builds its mesh/material at RUNTIME (Awake) so nothing generated serializes into the scene.
    /// </summary>
    public static class WaterAuthor
    {
        public static int Place(Transform parent, CityLayoutDefinition kit)
        {
            if (kit == null || kit.shipyard == null || !kit.shipyard.enabled) return 0;
            var b = kit.shipyard;

            var go = new GameObject("BerthWater");
            go.transform.SetParent(parent, false);
            // Sit the surface just below the berth lip; y is Terry-tunable (runbook 🎮).
            go.transform.position = new Vector3(b.berthCenter.x, b.berthCenter.y - 0.15f, b.berthCenter.z);

            var w = go.AddComponent<ZiptideWater>();
            w.sizeX = b.berthSize.x;
            w.sizeZ = b.berthSize.y;
            w.cells = 16;
            w.foam = true;
            w.foamBand = 0.6f;

            Debug.Log("[Ziptide] WaterAuthor: berth water " + b.berthSize.x + "x" + b.berthSize.y
                + " at " + go.transform.position);
            return 1;
        }
    }
}
#endif
