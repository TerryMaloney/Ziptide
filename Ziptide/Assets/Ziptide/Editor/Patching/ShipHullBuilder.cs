#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// THE INTERIM HULL (Quality Bar P4a). Terry's verdict on the 4-cube ship: "a very very poor
    /// version of something built in Roblox." This replaces it with a ~19-part procedural silhouette —
    /// tapered fuselage, raked nose, canopy, engine nacelles with glowing exhausts, wings, tail fin,
    /// landing struts, nav lights (red port / green starboard, aviation convention), dorsal antenna,
    /// belly cargo pod — sized to the same bounding box the boarding station expects, so S1/S2
    /// boarding/fly-out wiring is untouched. This is the STOPGAP: Picasso's LLM asset-forge hull mesh
    /// replaces the whole assembly through the same parent when it lands (HANDOFF ooo).
    /// </summary>
    public static class ShipHullBuilder
    {
        private static readonly Color GlassColor = new Color(0.25f, 0.55f, 0.75f);
        private static readonly Color ExhaustColor = new Color(0.95f, 0.55f, 0.20f);
        private static readonly Color PortLight = new Color(0.95f, 0.20f, 0.20f);
        private static readonly Color StarboardLight = new Color(0.20f, 0.95f, 0.35f);
        private static readonly Color TailLight = new Color(0.95f, 0.95f, 0.90f);

        /// <summary>Build the hull under <paramref name="ship"/>, fitted to <paramref name="size"/>
        /// (x = beam, y = height, z = length; +Z is the bow).</summary>
        public static void Build(Transform ship, Vector3 size, GlobalPalette pal)
        {
            float W = size.x, H = size.y, L = size.z;
            Color body = pal.building2;
            Color plate = pal.metal;
            Color trim = pal.accent;

            // ── Fuselage: three tapering segments, aft → bow ─────────────────────────────────────
            Part(ship, "Fuselage_Aft", new Vector3(0f, 0f, -L * 0.30f),
                new Vector3(W * 0.62f, H * 0.52f, L * 0.34f), body, true);
            Part(ship, "Fuselage_Mid", new Vector3(0f, H * 0.02f, 0f),
                new Vector3(W * 0.52f, H * 0.48f, L * 0.34f), body, true);
            var nose = Part(ship, "Fuselage_Bow", new Vector3(0f, -H * 0.02f, L * 0.30f),
                new Vector3(W * 0.34f, H * 0.36f, L * 0.30f), plate, true);
            nose.transform.localRotation = Quaternion.Euler(-4f, 0f, 0f); // raked nose line

            // Nose tip — the chisel.
            var tip = Part(ship, "Nose_Tip", new Vector3(0f, -H * 0.06f, L * 0.47f),
                new Vector3(W * 0.18f, H * 0.16f, L * 0.10f), trim, true);
            tip.transform.localRotation = Quaternion.Euler(-10f, 0f, 0f);

            // ── Canopy (the cockpit you sit behind in S2) ────────────────────────────────────────
            Part(ship, "Canopy", new Vector3(0f, H * 0.30f, L * 0.14f),
                new Vector3(W * 0.30f, H * 0.24f, L * 0.20f), GlassColor, true);
            Part(ship, "Canopy_Frame", new Vector3(0f, H * 0.22f, L * 0.05f),
                new Vector3(W * 0.36f, H * 0.10f, L * 0.30f), plate, true);

            // ── Engine nacelles + exhaust glow ───────────────────────────────────────────────────
            for (int side = -1; side <= 1; side += 2)
            {
                string tag = side < 0 ? "L" : "R";
                Part(ship, "Nacelle_" + tag, new Vector3(side * W * 0.42f, -H * 0.04f, -L * 0.32f),
                    new Vector3(W * 0.22f, H * 0.30f, L * 0.30f), plate, true);
                Part(ship, "Exhaust_" + tag, new Vector3(side * W * 0.42f, -H * 0.04f, -L * 0.475f),
                    new Vector3(W * 0.16f, H * 0.22f, L * 0.015f), ExhaustColor, false);
                // Wings: swept blades off the mid fuselage.
                var wing = Part(ship, "Wing_" + tag, new Vector3(side * W * 0.46f, H * 0.02f, -L * 0.06f),
                    new Vector3(W * 0.42f, H * 0.05f, L * 0.26f), body, true);
                wing.transform.localRotation = Quaternion.Euler(0f, side * -14f, side * 6f);
                Part(ship, "WingTip_" + tag, new Vector3(side * W * 0.64f, H * 0.05f, -L * 0.12f),
                    new Vector3(W * 0.06f, H * 0.10f, L * 0.14f), trim, true);
            }

            // ── Tail fin + dorsal spine + antenna ────────────────────────────────────────────────
            Part(ship, "TailFin", new Vector3(0f, H * 0.36f, -L * 0.36f),
                new Vector3(W * 0.05f, H * 0.44f, L * 0.16f), body, true);
            Part(ship, "DorsalSpine", new Vector3(0f, H * 0.26f, -L * 0.14f),
                new Vector3(W * 0.10f, H * 0.08f, L * 0.34f), plate, true);
            Part(ship, "Antenna", new Vector3(W * 0.06f, H * 0.52f, -L * 0.30f),
                new Vector3(0.03f, H * 0.30f, 0.03f), trim, false);

            // ── Belly: cargo pod + landing struts ────────────────────────────────────────────────
            Part(ship, "CargoPod", new Vector3(0f, -H * 0.30f, -L * 0.06f),
                new Vector3(W * 0.36f, H * 0.18f, L * 0.36f), plate, true);
            Strut(ship, "Strut_F", new Vector3(0f, -H * 0.42f, L * 0.26f), H, plate);
            Strut(ship, "Strut_L", new Vector3(-W * 0.34f, -H * 0.42f, -L * 0.28f), H, plate);
            Strut(ship, "Strut_R", new Vector3(W * 0.34f, -H * 0.42f, -L * 0.28f), H, plate);

            // ── Nav lights (aviation: red port, green starboard, white tail) ─────────────────────
            Light(ship, "Nav_Port", new Vector3(-W * 0.66f, H * 0.06f, -L * 0.10f), PortLight);
            Light(ship, "Nav_Starboard", new Vector3(W * 0.66f, H * 0.06f, -L * 0.10f), StarboardLight);
            Light(ship, "Nav_Tail", new Vector3(0f, H * 0.56f, -L * 0.38f), TailLight);
        }

        private static GameObject Part(Transform parent, string name, Vector3 localPos, Vector3 scale,
            Color color, bool collider)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            if (!collider)
            {
                var col = go.GetComponent<Collider>();
                if (col != null) Object.DestroyImmediate(col);
            }
            var r = go.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = Mat(color);
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            return go;
        }

        private static void Strut(Transform parent, string name, Vector3 localPos, float shipHeight, Color color)
        {
            var leg = Part(parent, name, localPos, new Vector3(0.12f, shipHeight * 0.5f, 0.12f), color, true);
            Part(leg.transform, name + "_Foot", new Vector3(0f, -0.55f, 0f),
                new Vector3(2.4f, 0.16f, 2.4f), color, true); // local to the scaled leg
        }

        private static void Light(Transform parent, string name, Vector3 localPos, Color color)
        {
            Part(parent, name, localPos, new Vector3(0.10f, 0.10f, 0.10f), color, false);
        }

        private static readonly Dictionary<Color, Material> _mats = new Dictionary<Color, Material>();

        private static Material Mat(Color color)
        {
            if (_mats.TryGetValue(color, out var cached) && cached != null) return cached;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            var m = new Material(shader) { name = "ShipMat_" + ColorUtility.ToHtmlStringRGB(color) };
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
            else if (m.HasProperty("_Color")) m.SetColor("_Color", color);
            _mats[color] = m;
            return m;
        }
    }
}
#endif
