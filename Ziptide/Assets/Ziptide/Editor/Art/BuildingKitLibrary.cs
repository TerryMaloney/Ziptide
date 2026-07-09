#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Art
{
    /// <summary>
    /// HARDWIRING 0.2 — the FIRST fulfillment of the ART REGISTRY seam. Registers structured wall
    /// modules for the shipped building styles so worlds stop rendering flat primitive cubes:
    /// a wall becomes panel + ribs + skirt + top band (and a framed reveal on window walls), with a
    /// per-style accent (patch plate / standpipe). Geometry-only, flat URP colors — the Forge-baked
    /// TEXTURED kit (art lane, E5.1) re-registers the same ids and supersedes this via the registry's
    /// last-registration-wins rule. Consumers (BuildingBuilder) are untouched.
    ///
    /// Dimensions are the canonical module: 3.0m wide × 3.2m storey (both shipped styles use these;
    /// see Resources/BuildingStyles). The kit root is unit-scale, origin at wall center — matching
    /// the primitive-fallback placement, and BuildingBuilder's window pane Inset() lands in the
    /// reveal this kit frames (clear zone ~0.7×0.6 around local (0, 0.08)).
    /// </summary>
    public static class BuildingKitLibrary
    {
        private const float W = 3.0f;   // module width
        private const float H = 3.2f;   // storey height
        private const float D = 0.25f;  // wall depth (primitive parity — doorway jambs match)

        [InitializeOnLoadMethod]
        private static void RegisterOnLoad() => EnsureRegistered();

        /// <summary>Idempotent registration (re-runnable by tests after a registry Clear()).
        /// Palettes mirror the style assets in Resources/BuildingStyles.</summary>
        public static void EnsureRegistered()
        {
            // salvage_row — scrap-brown, bolted patch plates.
            RegisterStyle("salvage_row",
                wall: new Color(0.34f, 0.30f, 0.26f), trim: new Color(0.24f, 0.23f, 0.22f),
                accent: new Color(0.42f, 0.34f, 0.24f), window: new Color(0.08f, 0.13f, 0.16f));

            // toxic_tenement — mossy grey-green, industrial standpipes.
            RegisterStyle("toxic_tenement",
                wall: new Color(0.30f, 0.32f, 0.28f), trim: new Color(0.24f, 0.23f, 0.22f),
                accent: new Color(0.20f, 0.26f, 0.20f), window: new Color(0.10f, 0.16f, 0.12f));
        }

        private static void RegisterStyle(string styleId, Color wall, Color trim, Color accent, Color window)
        {
            ArtModuleRegistry.Register("buildingModule:" + styleId + "/WallSolid",
                () => BuildWall(styleId, wall, trim, accent, window, windowReveal: false));
            ArtModuleRegistry.Register("buildingModule:" + styleId + "/WallWindow",
                () => BuildWall(styleId, wall, trim, accent, window, windowReveal: true));
        }

        /// <summary>One wall module. Deterministic (no randomness) — same id builds the same wall;
        /// per-wall variety stays the grammar's job. A kit module owns its COMPLETE look: the window
        /// variant bakes its own lit pane, and BuildingBuilder skips its primitive Inset() for kits.</summary>
        private static GameObject BuildWall(string styleId, Color wall, Color trim, Color accent, Color window, bool windowReveal)
        {
            var root = new GameObject("Kit_" + styleId + (windowReveal ? "_WallWindow" : "_WallSolid"));

            if (!windowReveal)
            {
                // Full panel, slightly inset between the ribs.
                Piece(root, "Panel", new Vector3(0f, 0f, 0f), new Vector3(W - 0.3f, H - 0.5f, D - 0.06f), wall, collide: true);
                // Style accent on the solid face: salvage patch plate / tenement stain panel.
                Piece(root, "Accent", new Vector3(0.65f, 0.35f, (D - 0.06f) * 0.5f + 0.015f),
                    new Vector3(0.9f, 0.7f, 0.03f), accent, collide: false);
            }
            else
            {
                // Panel split around a framed window reveal; the kit's own lit pane fills it.
                float holeW = 0.8f, holeH = 0.62f, holeY = 0.08f;
                float sideW = (W - 0.3f - holeW) * 0.5f;
                float below = (H - 0.5f) * 0.5f + holeY - holeH * 0.5f;   // panel height under the hole
                float above = (H - 0.5f) * 0.5f - holeY - holeH * 0.5f;   // panel height over the hole
                Piece(root, "PanelL", new Vector3(-(holeW + sideW) * 0.5f, 0f, 0f), new Vector3(sideW, H - 0.5f, D - 0.06f), wall, collide: true);
                Piece(root, "PanelR", new Vector3((holeW + sideW) * 0.5f, 0f, 0f), new Vector3(sideW, H - 0.5f, D - 0.06f), wall, collide: true);
                Piece(root, "PanelB", new Vector3(0f, holeY - holeH * 0.5f - below * 0.5f, 0f), new Vector3(holeW, below, D - 0.06f), wall, collide: true);
                Piece(root, "PanelT", new Vector3(0f, holeY + holeH * 0.5f + above * 0.5f, 0f), new Vector3(holeW, above, D - 0.06f), wall, collide: false);
                // Sill + header make the window read as BUILT, not painted.
                Piece(root, "Sill", new Vector3(0f, holeY - holeH * 0.5f - 0.04f, 0.06f), new Vector3(holeW + 0.18f, 0.08f, D + 0.1f), trim, collide: false);
                Piece(root, "Header", new Vector3(0f, holeY + holeH * 0.5f + 0.05f, 0.03f), new Vector3(holeW + 0.12f, 0.1f, D + 0.04f), trim, collide: false);
                // The kit-owned lit pane fills the reveal (slightly inset to leave a shadow line).
                Piece(root, "Pane", new Vector3(0f, holeY, 0f), new Vector3(holeW - 0.06f, holeH - 0.06f, D + 0.02f), window, collide: false);
            }

            // Shared shell: vertical ribs at the module edges + base skirt + top band — the depth
            // silhouette that a flat cube can never give.
            Piece(root, "RibL", new Vector3(-(W * 0.5f - 0.09f), 0f, 0f), new Vector3(0.18f, H, D + 0.08f), trim, collide: true);
            Piece(root, "RibR", new Vector3(W * 0.5f - 0.09f, 0f, 0f), new Vector3(0.18f, H, D + 0.08f), trim, collide: true);
            Piece(root, "Skirt", new Vector3(0f, -(H * 0.5f - 0.2f), 0f), new Vector3(W, 0.4f, D + 0.06f), trim * 0.85f, collide: false);
            Piece(root, "Band", new Vector3(0f, H * 0.5f - 0.11f, 0f), new Vector3(W, 0.22f, D + 0.05f), trim, collide: false);

            // Per-style vertical: the tenement standpipe (industrial read at zero texture cost).
            if (styleId == "toxic_tenement")
            {
                var pipe = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pipe.name = "Standpipe";
                Object.DestroyImmediate(pipe.GetComponent<Collider>());
                pipe.transform.SetParent(root.transform, false);
                pipe.transform.localPosition = new Vector3(1.15f, 0f, (D + 0.16f) * 0.5f);
                pipe.transform.localScale = new Vector3(0.09f, H * 0.5f, 0.09f);
                ItemFactory.ApplyURPColor(pipe, accent);
                var pr = pipe.GetComponent<Renderer>();
                if (pr != null) pr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            return root;
        }

        private static void Piece(GameObject root, string name, Vector3 pos, Vector3 size, Color color, bool collide)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            if (!collide) Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(root.transform, false);
            go.transform.localPosition = pos;
            go.transform.localScale = size;
            ItemFactory.ApplyURPColor(go, color);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
#endif
