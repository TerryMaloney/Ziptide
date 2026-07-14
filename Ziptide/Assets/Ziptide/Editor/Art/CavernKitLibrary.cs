#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Art
{
    /// <summary>
    /// HARDWIRING 1.4e — the CAVERN kit's first fulfillment of the ART REGISTRY seam (the traversal
    /// lane's sibling of <see cref="BuildingKitLibrary"/> — DISTINCT id prefix "cavernModule:", so the
    /// two kits can never clash; the Forge-baked textured kit re-registers the same ids later and
    /// supersedes via last-registration-wins). Geometry-only, flat URP colors, deterministic — no
    /// randomness in a module; variety is the PLANNER's job (chamber radius/positions), not the kit's.
    /// Modules are unit-scale at origin; the builder scales/positions them.
    /// </summary>
    public static class CavernKitLibrary
    {
        private static readonly Color Rock = new Color(0.23f, 0.21f, 0.20f);
        private static readonly Color RockDark = new Color(0.16f, 0.15f, 0.145f);
        private static readonly Color Crystal = new Color(0.35f, 0.75f, 0.85f); // the cave's own light

        [InitializeOnLoadMethod]
        private static void RegisterOnLoad() => EnsureRegistered();

        /// <summary>Idempotent registration (re-runnable after a registry Clear()).</summary>
        public static void EnsureRegistered()
        {
            ArtModuleRegistry.Register("cavernModule:rock/FloorPad", BuildFloorPad);
            ArtModuleRegistry.Register("cavernModule:rock/Stalactite", BuildStalactite);
            ArtModuleRegistry.Register("cavernModule:rock/ShaftWall", BuildShaftWall);
        }

        /// <summary>A chamber floor: a unit disc (scale XZ by chamber radius) with a low broken rim
        /// so the pad reads as carved stone, not a placed platter.</summary>
        private static GameObject BuildFloorPad()
        {
            var root = new GameObject("Kit_Cavern_FloorPad");

            var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = "Floor";
            // The cylinder primitive ships a CapsuleCollider, and a capsule cannot flatten below its
            // own radius: squashed to this 0.12-thick disc and scaled to a chamber, it degenerates to
            // an invisible SPHERE the radius of the whole chamber — a phantom dome that swallowed the
            // W011 spawn (SPAWN_OVERLAP_SOLID, the headset-build blocker) and would have the player
            // standing on air. A thin box is the disc's real envelope.
            var capsule = disc.GetComponent<Collider>();
            if (capsule != null) Object.DestroyImmediate(capsule);
            disc.AddComponent<BoxCollider>(); // auto-sizes to the cylinder mesh bounds → the disc's box
            disc.transform.SetParent(root.transform, false);
            disc.transform.localScale = new Vector3(2f, 0.12f, 2f); // unit radius = 1 at root scale 1
            ItemFactory.ApplyURPColor(disc, Rock);

            // Rim stones: 8 low blocks around the edge, deterministic ring (no randomness — the kit law).
            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI * 2f / 8f;
                var stone = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stone.name = "Rim" + i;
                var c = stone.GetComponent<Collider>();
                if (c != null) Object.DestroyImmediate(c); // dressing — the disc is the walk surface
                stone.transform.SetParent(root.transform, false);
                stone.transform.localPosition = new Vector3(Mathf.Cos(a) * 0.96f, 0.14f, Mathf.Sin(a) * 0.96f);
                stone.transform.localRotation = Quaternion.Euler(0f, a * Mathf.Rad2Deg + 20f, 0f);
                stone.transform.localScale = new Vector3(0.28f, 0.18f + 0.06f * (i % 3), 0.14f);
                ItemFactory.ApplyURPColor(stone, RockDark);
            }
            return root;
        }

        /// <summary>A ceiling spike with a faint crystal tip — the cave's light source language.</summary>
        private static GameObject BuildStalactite()
        {
            var root = new GameObject("Kit_Cavern_Stalactite");

            var spike = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spike.name = "Spike";
            var c = spike.GetComponent<Collider>();
            if (c != null) Object.DestroyImmediate(c); // overhead dressing never blocks
            spike.transform.SetParent(root.transform, false);
            spike.transform.localRotation = Quaternion.Euler(0f, 45f, 180f);
            spike.transform.localScale = new Vector3(0.35f, 1.6f, 0.35f);
            ItemFactory.ApplyURPColor(spike, RockDark);

            var tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tip.name = "CrystalTip";
            var tc = tip.GetComponent<Collider>();
            if (tc != null) Object.DestroyImmediate(tc);
            tip.transform.SetParent(root.transform, false);
            tip.transform.localPosition = new Vector3(0f, -0.95f, 0f);
            tip.transform.localRotation = Quaternion.Euler(0f, 45f, 180f);
            tip.transform.localScale = new Vector3(0.12f, 0.3f, 0.12f);
            ItemFactory.ApplyURPColor(tip, Crystal);
            return root;
        }

        /// <summary>A rough vertical rock face (unit: 1 wide × 1 tall × 0.4 deep at root scale 1) —
        /// the climbable shaft wall. Solid collider on the slab; the builder adds ClimbableSurface
        /// (which paints its own stud handholds on the face).</summary>
        private static GameObject BuildShaftWall()
        {
            var root = new GameObject("Kit_Cavern_ShaftWall");

            var slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slab.name = "Face";
            slab.transform.SetParent(root.transform, false);
            slab.transform.localScale = new Vector3(1f, 1f, 0.4f);
            ItemFactory.ApplyURPColor(slab, Rock);

            // Two deterministic ledges breaking the face's silhouette.
            for (int i = 0; i < 2; i++)
            {
                var ledge = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ledge.name = "Ledge" + i;
                var lc = ledge.GetComponent<Collider>();
                if (lc != null) Object.DestroyImmediate(lc);
                ledge.transform.SetParent(root.transform, false);
                ledge.transform.localPosition = new Vector3(i == 0 ? -0.22f : 0.25f, i == 0 ? -0.18f : 0.24f, -0.22f);
                ledge.transform.localScale = new Vector3(0.4f, 0.08f, 0.1f);
                ItemFactory.ApplyURPColor(ledge, RockDark);
            }
            return root;
        }
    }
}
#endif
