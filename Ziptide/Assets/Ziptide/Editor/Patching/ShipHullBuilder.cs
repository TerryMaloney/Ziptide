#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Shared hero-ship fallback used by every berth. It preserves the named refit skeleton and overall
    /// ship bounds while replacing the old block stack with a layered industrial silhouette: curved shell
    /// volumes, ribs, panel bands, framed canopy, heavy nacelles, intake/exhaust rings, service pods,
    /// boarding read, cargo clamps, landing gear and nav hardware. Materials use a closed semantic palette.
    /// </summary>
    public static class ShipHullBuilder
    {
        public const int MinimumHeroRenderers = 52;
        public const int MaximumHeroMaterials = 9;

        private static readonly Color GlassColor = new Color(0.18f, 0.48f, 0.62f);
        private static readonly Color ExhaustColor = new Color(1.00f, 0.42f, 0.12f);
        private static readonly Color IntakeColor = new Color(0.12f, 0.16f, 0.20f);
        private static readonly Color PortLight = new Color(0.95f, 0.20f, 0.20f);
        private static readonly Color StarboardLight = new Color(0.20f, 0.95f, 0.35f);
        private static readonly Color TailLight = new Color(0.95f, 0.95f, 0.90f);
        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        /// <summary>Build under ship; size is beam/height/length and +Z is bow.</summary>
        public static void Build(Transform ship, Vector3 size, GlobalPalette pal)
        {
            if (ship == null || pal == null) return;
            Materials.Clear();
            float w = Mathf.Max(3f, size.x);
            float h = Mathf.Max(2f, size.y);
            float l = Mathf.Max(7f, size.z);
            Color body = pal.building2;
            Color bodyLight = Color.Lerp(pal.building2, pal.concrete, 0.28f);
            Color plate = pal.metal;
            Color trim = pal.accent;
            Color dark = Color.Lerp(pal.metal, Color.black, 0.46f);

            // Refit skeleton: these exact direct-child names remain authoritative.
            Part(ship, "Fuselage_Aft", PrimitiveType.Cube,
                new Vector3(0f, 0f, -l * 0.30f), new Vector3(w * 0.62f, h * 0.52f, l * 0.34f),
                Vector3.zero, body, true, "Body");
            Part(ship, "Fuselage_Mid", PrimitiveType.Cube,
                new Vector3(0f, h * 0.02f, 0f), new Vector3(w * 0.54f, h * 0.50f, l * 0.35f),
                Vector3.zero, bodyLight, true, "BodyLight");
            Part(ship, "Fuselage_Bow", PrimitiveType.Cube,
                new Vector3(0f, -h * 0.03f, l * 0.30f), new Vector3(w * 0.38f, h * 0.38f, l * 0.30f),
                new Vector3(-5f, 0f, 0f), plate, true, "Plate");
            Part(ship, "Nose_Tip", PrimitiveType.Cube,
                new Vector3(0f, -h * 0.09f, l * 0.475f), new Vector3(w * 0.18f, h * 0.16f, l * 0.09f),
                new Vector3(-13f, 0f, 0f), trim, true, "Trim");

            // Curved shell volumes soften the refit skeleton without replacing it.
            Part(ship, "HullShell_Aft", PrimitiveType.Sphere,
                new Vector3(0f, h * 0.02f, -l * 0.30f), new Vector3(w * 0.34f, h * 0.29f, l * 0.19f),
                Vector3.zero, body, false, "Body");
            Part(ship, "HullShell_Mid", PrimitiveType.Sphere,
                new Vector3(0f, h * 0.05f, -l * 0.01f), new Vector3(w * 0.30f, h * 0.28f, l * 0.22f),
                Vector3.zero, bodyLight, false, "BodyLight");
            Part(ship, "HullShell_Bow", PrimitiveType.Sphere,
                new Vector3(0f, -h * 0.02f, l * 0.29f), new Vector3(w * 0.22f, h * 0.22f, l * 0.19f),
                Vector3.zero, plate, false, "Plate");

            // Structural ribs and longitudinal panel bands.
            for (int i = 0; i < 6; i++)
            {
                float z = Mathf.Lerp(-l * 0.42f, l * 0.36f, i / 5f);
                float taper = 1f - Mathf.Max(0f, z / l) * 0.65f;
                Part(ship, "Fuselage_Rib_" + i, PrimitiveType.Cube,
                    new Vector3(0f, h * 0.02f, z),
                    new Vector3(w * 0.58f * taper, h * 0.53f * taper, 0.07f),
                    Vector3.zero, dark, false, "Dark");
            }
            for (int side = -1; side <= 1; side += 2)
            {
                string tag = side < 0 ? "L" : "R";
                Part(ship, "HullBand_" + tag, PrimitiveType.Cube,
                    new Vector3(side * w * 0.29f, -h * 0.02f, -l * 0.02f),
                    new Vector3(w * 0.035f, h * 0.13f, l * 0.70f),
                    Vector3.zero, trim, false, "Trim");
                Part(ship, "ServicePod_" + tag, PrimitiveType.Capsule,
                    new Vector3(side * w * 0.42f, -h * 0.05f, -l * 0.02f),
                    new Vector3(w * 0.14f, l * 0.17f, h * 0.14f),
                    new Vector3(90f, 0f, 0f), plate, true, "Plate");
                Part(ship, "ServicePodCap_F_" + tag, PrimitiveType.Cylinder,
                    new Vector3(side * w * 0.42f, -h * 0.05f, l * 0.15f),
                    new Vector3(w * 0.11f, h * 0.05f, w * 0.11f),
                    new Vector3(90f, 0f, 0f), trim, false, "Trim");
                Part(ship, "ServicePodCap_A_" + tag, PrimitiveType.Cylinder,
                    new Vector3(side * w * 0.42f, -h * 0.05f, -l * 0.19f),
                    new Vector3(w * 0.11f, h * 0.05f, w * 0.11f),
                    new Vector3(90f, 0f, 0f), dark, false, "Dark");
            }

            // Cockpit and framing.
            Part(ship, "Canopy", PrimitiveType.Sphere,
                new Vector3(0f, h * 0.31f, l * 0.15f), new Vector3(w * 0.18f, h * 0.17f, l * 0.13f),
                new Vector3(-8f, 0f, 0f), GlassColor, false, "Glass");
            Part(ship, "Canopy_Frame", PrimitiveType.Cube,
                new Vector3(0f, h * 0.24f, l * 0.07f), new Vector3(w * 0.38f, h * 0.09f, l * 0.31f),
                Vector3.zero, dark, true, "Dark");
            for (int side = -1; side <= 1; side += 2)
            {
                Part(ship, "CanopyRail_" + side, PrimitiveType.Cube,
                    new Vector3(side * w * 0.16f, h * 0.35f, l * 0.15f),
                    new Vector3(0.055f, h * 0.20f, l * 0.22f),
                    new Vector3(-10f, 0f, side * 5f), trim, false, "Trim");
            }
            Part(ship, "CanopyCrossbar", PrimitiveType.Cube,
                new Vector3(0f, h * 0.40f, l * 0.14f), new Vector3(w * 0.35f, 0.055f, 0.07f),
                new Vector3(-8f, 0f, 0f), trim, false, "Trim");

            // Heavy nacelles and engine vocabulary.
            for (int side = -1; side <= 1; side += 2)
            {
                string tag = side < 0 ? "L" : "R";
                float x = side * w * 0.46f;
                Part(ship, "Nacelle_" + tag, PrimitiveType.Capsule,
                    new Vector3(x, -h * 0.04f, -l * 0.31f),
                    new Vector3(w * 0.17f, l * 0.18f, h * 0.18f),
                    new Vector3(90f, 0f, 0f), plate, true, "Plate");
                Part(ship, "Intake_" + tag, PrimitiveType.Cylinder,
                    new Vector3(x, -h * 0.04f, -l * 0.11f),
                    new Vector3(w * 0.15f, h * 0.055f, w * 0.15f),
                    new Vector3(90f, 0f, 0f), IntakeColor, false, "Intake");
                Part(ship, "NacelleRing_F_" + tag, PrimitiveType.Cylinder,
                    new Vector3(x, -h * 0.04f, -l * 0.14f),
                    new Vector3(w * 0.18f, h * 0.045f, w * 0.18f),
                    new Vector3(90f, 0f, 0f), trim, false, "Trim");
                Part(ship, "NacelleRing_A_" + tag, PrimitiveType.Cylinder,
                    new Vector3(x, -h * 0.04f, -l * 0.45f),
                    new Vector3(w * 0.18f, h * 0.045f, w * 0.18f),
                    new Vector3(90f, 0f, 0f), dark, false, "Dark");
                Part(ship, "Exhaust_" + tag, PrimitiveType.Cylinder,
                    new Vector3(x, -h * 0.04f, -l * 0.49f),
                    new Vector3(w * 0.135f, h * 0.035f, w * 0.135f),
                    new Vector3(90f, 0f, 0f), ExhaustColor, false, "Exhaust", true);
                Part(ship, "EngineVane_" + tag, PrimitiveType.Cube,
                    new Vector3(x, h * 0.17f, -l * 0.31f),
                    new Vector3(w * 0.05f, h * 0.24f, l * 0.22f),
                    new Vector3(0f, side * 4f, side * 8f), body, false, "Body");
            }

            // Swept wing assemblies, keeping Wing_/WingTip_ refit names.
            for (int side = -1; side <= 1; side += 2)
            {
                string tag = side < 0 ? "L" : "R";
                Part(ship, "WingRoot_" + tag, PrimitiveType.Cube,
                    new Vector3(side * w * 0.34f, h * 0.02f, -l * 0.08f),
                    new Vector3(w * 0.24f, h * 0.08f, l * 0.34f),
                    new Vector3(0f, side * -10f, side * 4f), dark, true, "Dark");
                Part(ship, "Wing_" + tag, PrimitiveType.Cube,
                    new Vector3(side * w * 0.50f, h * 0.02f, -l * 0.08f),
                    new Vector3(w * 0.34f, h * 0.055f, l * 0.28f),
                    new Vector3(0f, side * -16f, side * 7f), body, true, "Body");
                Part(ship, "WingFlap_" + tag, PrimitiveType.Cube,
                    new Vector3(side * w * 0.53f, -h * 0.01f, -l * 0.19f),
                    new Vector3(w * 0.26f, h * 0.04f, l * 0.10f),
                    new Vector3(0f, side * -18f, side * 8f), plate, false, "Plate");
                Part(ship, "WingTip_" + tag, PrimitiveType.Cube,
                    new Vector3(side * w * 0.67f, h * 0.06f, -l * 0.14f),
                    new Vector3(w * 0.07f, h * 0.12f, l * 0.15f),
                    new Vector3(0f, side * -18f, side * 10f), trim, true, "Trim");
            }

            // Tail, dorsal systems, asymmetrical radar hardware.
            Part(ship, "TailFin", PrimitiveType.Cube,
                new Vector3(0f, h * 0.39f, -l * 0.37f), new Vector3(w * 0.055f, h * 0.47f, l * 0.17f),
                new Vector3(0f, 0f, -4f), body, true, "Body");
            Part(ship, "DorsalSpine", PrimitiveType.Cube,
                new Vector3(0f, h * 0.28f, -l * 0.15f), new Vector3(w * 0.11f, h * 0.10f, l * 0.38f),
                Vector3.zero, trim, true, "Trim");
            for (int side = -1; side <= 1; side += 2)
                Part(ship, "TailPlane_" + side, PrimitiveType.Cube,
                    new Vector3(side * w * 0.21f, h * 0.25f, -l * 0.39f),
                    new Vector3(w * 0.34f, h * 0.045f, l * 0.15f),
                    new Vector3(0f, side * 8f, side * 5f), plate, false, "Plate");
            Part(ship, "AntennaMast", PrimitiveType.Cylinder,
                new Vector3(w * 0.08f, h * 0.57f, -l * 0.26f), new Vector3(0.045f, h * 0.22f, 0.045f),
                Vector3.zero, trim, false, "Trim");
            Part(ship, "RadarDish", PrimitiveType.Cylinder,
                new Vector3(w * 0.08f, h * 0.82f, -l * 0.26f), new Vector3(w * 0.13f, 0.035f, w * 0.13f),
                new Vector3(18f, 0f, 12f), plate, false, "Plate");

            // Belly, cargo clamps, boarding-side identity.
            Part(ship, "CargoPod", PrimitiveType.Cube,
                new Vector3(0f, -h * 0.31f, -l * 0.06f), new Vector3(w * 0.38f, h * 0.20f, l * 0.38f),
                Vector3.zero, plate, true, "Plate");
            for (int i = -1; i <= 1; i += 2)
                Part(ship, "CargoClamp_" + i, PrimitiveType.Cube,
                    new Vector3(i * w * 0.19f, -h * 0.31f, -l * 0.06f),
                    new Vector3(w * 0.055f, h * 0.25f, l * 0.42f), Vector3.zero, trim, false, "Trim");
            Part(ship, "BoardingDoor_Port", PrimitiveType.Cube,
                new Vector3(-w * 0.315f, h * 0.02f, l * 0.02f), new Vector3(0.08f, h * 0.42f, l * 0.17f),
                Vector3.zero, dark, false, "Dark");
            Part(ship, "BoardingDoorFrame_Port", PrimitiveType.Cube,
                new Vector3(-w * 0.33f, h * 0.02f, l * 0.02f), new Vector3(0.055f, h * 0.50f, l * 0.22f),
                Vector3.zero, trim, false, "Trim");
            Part(ship, "BoardingStep_Port", PrimitiveType.Cube,
                new Vector3(-w * 0.47f, -h * 0.35f, l * 0.02f), new Vector3(w * 0.34f, h * 0.07f, l * 0.20f),
                Vector3.zero, plate, true, "Plate");

            // Landing gear and broad feet.
            Strut(ship, "Strut_F", new Vector3(0f, -h * 0.43f, l * 0.27f), h, plate);
            Strut(ship, "Strut_L", new Vector3(-w * 0.34f, -h * 0.43f, -l * 0.29f), h, plate);
            Strut(ship, "Strut_R", new Vector3(w * 0.34f, -h * 0.43f, -l * 0.29f), h, plate);

            LightPart(ship, "Nav_Port", new Vector3(-w * 0.68f, h * 0.07f, -l * 0.13f), PortLight, "NavPort");
            LightPart(ship, "Nav_Starboard", new Vector3(w * 0.68f, h * 0.07f, -l * 0.13f), StarboardLight, "NavStarboard");
            LightPart(ship, "Nav_Tail", new Vector3(0f, h * 0.62f, -l * 0.39f), TailLight, "NavTail");

            Debug.Log("ZIPTIDE: HERO_SHIP_HULL renderers="
                + ship.GetComponentsInChildren<Renderer>(true).Length
                + " materials=" + Materials.Count
                + " size=" + size.ToString("F1"));
            Materials.Clear();
        }

        private static GameObject Part(Transform parent, string name, PrimitiveType primitive,
            Vector3 localPos, Vector3 scale, Vector3 euler, Color color, bool collider,
            string materialSlot, bool emissive = false)
        {
            GameObject go = GameObject.CreatePrimitive(primitive);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            go.transform.localRotation = Quaternion.Euler(euler);
            Collider col = go.GetComponent<Collider>();
            if (col != null && !collider) Object.DestroyImmediate(col);
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = Mat(materialSlot, color, emissive);
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            return go;
        }

        private static void Strut(Transform parent, string name, Vector3 localPos, float shipHeight, Color color)
        {
            GameObject leg = Part(parent, name, PrimitiveType.Cylinder, localPos,
                new Vector3(0.11f, shipHeight * 0.24f, 0.11f), Vector3.zero,
                color, true, "Plate");
            Part(leg.transform, name + "_Foot", PrimitiveType.Cube,
                new Vector3(0f, -1.10f, 0f), new Vector3(2.5f, 0.16f, 2.5f),
                Vector3.zero, color, true, "Plate");
        }

        private static void LightPart(Transform parent, string name, Vector3 localPos, Color color, string slot)
        {
            Part(parent, name, PrimitiveType.Sphere, localPos, Vector3.one * 0.11f,
                Vector3.zero, color, false, slot, true);
        }

        private static Material Mat(string slot, Color color, bool emissive)
        {
            if (Materials.TryGetValue(slot, out Material material) && material != null) return material;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            material = new Material(shader) { name = "HeroShip_" + slot };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (emissive && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.7f);
            }
            Materials[slot] = material;
            return material;
        }
    }
}
#endif
