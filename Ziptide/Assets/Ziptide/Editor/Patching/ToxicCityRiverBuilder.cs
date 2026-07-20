#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Replaces ToxicCity's decorative canal slabs with explicit toxic-river basins. Each basin has a
    /// solid floor, readable banks, animated presentation, warning signage, and one ToxicRiverRuntime.
    /// The river is intentionally enterable; consequence ownership belongs to ToxicRiverRuntime.
    /// </summary>
    public static class ToxicCityRiverBuilder
    {
        public const string RootName = "__TOXIC_RIVERS";
        public const string RiverPrefix = "__TOXIC_RIVER_";
        public const int FlowRibbonCount = 5;
        public const int BubbleCount = 8;

        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        public readonly struct Summary
        {
            public readonly int Rivers;
            public readonly int Renderers;
            public readonly int SolidColliders;

            public Summary(int rivers, int renderers, int solidColliders)
            {
                Rivers = rivers;
                Renderers = renderers;
                SolidColliders = solidColliders;
            }
        }

        public static Summary Build(Transform cityRoot, CityLayoutDefinition kit)
        {
            if (cityRoot == null || kit == null || kit.canals == null) return default;
            Transform existing = cityRoot.Find(RootName);
            if (existing != null) return Measure(existing);

            Transform legacy = cityRoot.Find("Canals");
            if (legacy != null) Object.DestroyImmediate(legacy.gameObject);

            var root = new GameObject(RootName).transform;
            root.SetParent(cityRoot, false);
            Materials.Clear();

            int rivers = 0;
            for (int i = 0; i < kit.canals.Count; i++)
            {
                CanalRegionDef canal = kit.canals[i];
                if (canal == null) continue;
                BuildRiver(root, kit, canal, i);
                rivers++;
            }

            Summary summary = Measure(root);
            Debug.Log("ZIPTIDE: TOXIC_RIVER_BUILD rivers=" + summary.Rivers
                + " renderers=" + summary.Renderers
                + " colliders=" + summary.SolidColliders
                + " materials=" + Materials.Count);
            Materials.Clear();
            return summary;
        }

        private static void BuildRiver(Transform parent, CityLayoutDefinition kit, CanalRegionDef canal, int index)
        {
            float width = Mathf.Max(2f, canal.size.x);
            float length = Mathf.Max(2f, canal.size.y);
            float depth = Mathf.Max(1.25f, canal.depth);
            const float surfaceY = -0.18f;
            float bottomY = -depth;
            bool alongZ = length >= width;

            var river = new GameObject(RiverPrefix + index).transform;
            river.SetParent(parent, false);
            river.localPosition = new Vector3(canal.center.x, kit.walkwayHeight, canal.center.z);

            Color toxic = canal.useOverride ? canal.colorOverride : kit.palette.toxic;
            Color darkToxic = Color.Lerp(toxic, Color.black, 0.48f);
            Color hotToxic = Color.Lerp(toxic, new Color(0.72f, 1f, 0.12f), 0.55f);
            Color bank = Color.Lerp(kit.palette.concrete, kit.palette.metal, 0.48f);
            Color warning = Color.Lerp(kit.palette.accent, new Color(1f, 0.28f, 0.05f), 0.42f);

            Cube(river, "BasinFloor", new Vector3(0f, bottomY - 0.15f, 0f),
                new Vector3(width, 0.30f, length), darkToxic, true, false, "BasinFloor");
            Cube(river, "ToxicUnderlayer", new Vector3(0f, surfaceY - 0.16f, 0f),
                new Vector3(width, 0.18f, length), darkToxic, false, false, "ToxicDark");
            Cube(river, "ToxicSurface", new Vector3(0f, surfaceY, 0f),
                new Vector3(width, 0.09f, length), toxic, false, true, "ToxicSurface");

            const float bankThickness = 0.48f;
            const float bankHeight = 0.52f;
            float bankY = -bankHeight * 0.46f;
            Cube(river, "Bank_W", new Vector3(-width * 0.5f - bankThickness * 0.5f, bankY, 0f),
                new Vector3(bankThickness, bankHeight, length + bankThickness * 2f), bank, true, false, "Bank");
            Cube(river, "Bank_E", new Vector3(width * 0.5f + bankThickness * 0.5f, bankY, 0f),
                new Vector3(bankThickness, bankHeight, length + bankThickness * 2f), bank, true, false, "Bank");
            Cube(river, "Bank_S", new Vector3(0f, bankY, -length * 0.5f - bankThickness * 0.5f),
                new Vector3(width, bankHeight, bankThickness), bank, true, false, "Bank");
            Cube(river, "Bank_N", new Vector3(0f, bankY, length * 0.5f + bankThickness * 0.5f),
                new Vector3(width, bankHeight, bankThickness), bank, true, false, "Bank");

            Transform motion = new GameObject("SurfaceMotion").transform;
            motion.SetParent(river, false);
            for (int i = 0; i < FlowRibbonCount; i++)
            {
                float across = ((i + 0.5f) / FlowRibbonCount - 0.5f) * (alongZ ? width : length) * 0.78f;
                Vector3 pos = alongZ
                    ? new Vector3(across, surfaceY + 0.055f, -length * 0.45f + i * length * 0.18f)
                    : new Vector3(-width * 0.45f + i * width * 0.18f, surfaceY + 0.055f, across);
                Vector3 scale = alongZ
                    ? new Vector3(Mathf.Max(0.12f, width * 0.075f), 0.025f, Mathf.Max(0.8f, length * 0.16f))
                    : new Vector3(Mathf.Max(0.8f, width * 0.16f), 0.025f, Mathf.Max(0.12f, length * 0.075f));
                Cube(motion, "Flow_" + i, pos, scale, hotToxic, false, true, "ToxicFlow");
            }

            for (int i = 0; i < BubbleCount; i++)
            {
                float x = ((((index + 3) * 37 + i * 17) % 101) / 100f - 0.5f) * width * 0.78f;
                float z = ((((index + 5) * 53 + i * 29) % 103) / 102f - 0.5f) * length * 0.78f;
                GameObject bubble = Primitive(motion, "Bubble_" + i, PrimitiveType.Sphere,
                    new Vector3(x, surfaceY + 0.07f + (i % 3) * 0.015f, z),
                    Vector3.one * (0.08f + (i % 4) * 0.018f), hotToxic, false, true, "ToxicFlow");
                bubble.transform.localRotation = Quaternion.Euler(0f, i * 31f, 0f);
            }

            var surfaceMotion = motion.gameObject.AddComponent<ToxicRiverSurfaceRuntime>();
            surfaceMotion.Configure(alongZ, alongZ ? length : width);

            BuildWarning(river, "Warning_A", new Vector3(-width * 0.5f - 0.32f, 0.58f, -length * 0.36f), warning, 90f);
            BuildWarning(river, "Warning_B", new Vector3(width * 0.5f + 0.32f, 0.58f, length * 0.36f), warning, -90f);

            float lower = bottomY - 0.12f;
            float upper = surfaceY + 0.78f;
            var runtime = river.gameObject.AddComponent<ToxicRiverRuntime>();
            runtime.Configure("toxic_city_river_" + index,
                new Vector3(0f, (lower + upper) * 0.5f, 0f),
                new Vector3(width, upper - lower, length));
        }

        private static void BuildWarning(Transform parent, string name, Vector3 position, Color color, float yaw)
        {
            Transform root = new GameObject(name).transform;
            root.SetParent(parent, false);
            root.localPosition = position;
            root.localRotation = Quaternion.Euler(0f, yaw, 0f);
            Cube(root, "Post", new Vector3(0f, -0.28f, 0f), new Vector3(0.10f, 0.65f, 0.10f),
                color, true, false, "Warning");
            Cube(root, "Plate", Vector3.zero, new Vector3(0.95f, 0.38f, 0.08f),
                color, false, true, "Warning");
            var label = new GameObject("Label");
            label.transform.SetParent(root, false);
            label.transform.localPosition = new Vector3(0f, 0f, -0.055f);
            label.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            var text = label.AddComponent<TextMesh>();
            text.text = "TOXIC\nKEEP OUT";
            text.fontSize = 48;
            text.characterSize = 0.018f;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.color = Color.black;
        }

        private static GameObject Cube(Transform parent, string name, Vector3 pos, Vector3 scale,
            Color color, bool collider, bool emissive, string materialSlot)
        {
            return Primitive(parent, name, PrimitiveType.Cube, pos, scale, color, collider, emissive, materialSlot);
        }

        private static GameObject Primitive(Transform parent, string name, PrimitiveType type,
            Vector3 pos, Vector3 scale, Color color, bool collider, bool emissive, string materialSlot)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            Collider col = go.GetComponent<Collider>();
            if (col != null && !collider) Object.DestroyImmediate(col);
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = Mat(materialSlot, color, emissive);
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            bool animated = name.StartsWith("Flow_") || name.StartsWith("Bubble_");
            if (!animated)
                GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic);
            return go;
        }

        private static Material Mat(string slot, Color color, bool emissive)
        {
            if (Materials.TryGetValue(slot, out Material existing) && existing != null) return existing;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            var material = new Material(shader) { name = "ToxicRiver_" + slot };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (emissive && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.45f);
            }
            Materials[slot] = material;
            return material;
        }

        public static Summary Measure(Transform root)
        {
            if (root == null) return default;
            int rivers = 0;
            int renderers = 0;
            int colliders = 0;
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                if (t.name.StartsWith(RiverPrefix)) rivers++;
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
                if (renderer != null) renderers++;
            foreach (Collider collider in root.GetComponentsInChildren<Collider>(true))
                if (collider != null && collider.enabled && !collider.isTrigger) colliders++;
            return new Summary(rivers, renderers, colliders);
        }
    }
}
#endif
