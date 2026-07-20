#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// ToxicCity street-life layer: lamps, utility lines, district signs, vents, steam and route beacons.
    /// Presentation only—every primitive is collider-free and uses a six-slot shared palette.
    /// </summary>
    public static class ToxicCityStageB
    {
        public const string RootName = "__CITY_STAGE_B";
        public const string DistrictRootName = "__STREET_LIFE";
        public const int LampsPerDistrict = 4;
        public const int TargetObjectsPerDistrict = 38;
        public const int HardObjectsPerDistrict = 52;
        public const int HardMaterials = 6;

        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        public readonly struct Summary
        {
            public readonly int Districts;
            public readonly int Objects;
            public readonly int Renderers;
            public readonly int Materials;
            public Summary(int districts, int objects, int renderers, int materials)
            {
                Districts = districts;
                Objects = objects;
                Renderers = renderers;
                Materials = materials;
            }
        }

        public static Summary Build(Transform cityRoot, CityLayoutDefinition kit)
        {
            if (cityRoot == null || kit == null || kit.districts == null) return default;
            Transform existing = cityRoot.Find(RootName);
            if (existing != null) return Measure(existing);

            Materials.Clear();
            Transform root = new GameObject(RootName).transform;
            root.SetParent(cityRoot, false);
            int districts = 0;

            for (int i = 0; i < kit.districts.Count; i++)
            {
                DistrictDef def = kit.districts[i];
                if (def == null || string.IsNullOrEmpty(def.id)) continue;
                Transform district = cityRoot.Find("District_" + def.id);
                if (district == null) continue;
                BuildDistrict(district, def, kit, i);
                districts++;
            }

            Summary summary = Measure(root);
            Debug.Log("ZIPTIDE: CITY_STAGE_B_COMPLETE districts=" + districts
                + " objects=" + summary.Objects
                + " renderers=" + summary.Renderers
                + " materials=" + summary.Materials);
            Materials.Clear();
            return new Summary(districts, summary.Objects, summary.Renderers, summary.Materials);
        }

        private static void BuildDistrict(Transform district, DistrictDef def,
            CityLayoutDefinition kit, int districtIndex)
        {
            Transform street = new GameObject(DistrictRootName).transform;
            street.SetParent(district, false);
            Color metal = Color.Lerp(kit.palette.metal, Color.black, 0.20f);
            Color accent = kit.palette.accent;
            Color lamp = Color.Lerp(accent, new Color(1f, 0.72f, 0.30f), 0.45f);
            Color cable = Color.Lerp(kit.palette.rail, Color.black, 0.55f);
            Color steam = new Color(0.58f, 0.72f, 0.68f);
            Color sign = Color.Lerp(kit.palette.building1, kit.palette.facadeWindow, 0.45f);

            float x = Mathf.Max(3f, def.bounds.x * 0.39f);
            float z = Mathf.Max(3f, def.bounds.y * 0.39f);
            Vector3[] corners =
            {
                new Vector3(-x, kit.walkwayHeight, -z),
                new Vector3(x, kit.walkwayHeight, -z),
                new Vector3(x, kit.walkwayHeight, z),
                new Vector3(-x, kit.walkwayHeight, z),
            };
            for (int i = 0; i < corners.Length; i++)
                BuildLamp(street, i, corners[i], metal, accent, lamp);

            Beam(street, "UtilityLine_NS_A", corners[0] + Vector3.up * 2.95f,
                corners[3] + Vector3.up * 2.95f, cable);
            Beam(street, "UtilityLine_NS_B", corners[1] + Vector3.up * 2.95f,
                corners[2] + Vector3.up * 2.95f, cable);
            Beam(street, "UtilityLine_X", corners[2] + Vector3.up * 3.12f,
                corners[3] + Vector3.up * 3.12f, cable);

            BuildDistrictSign(street, def.id,
                new Vector3(-x + 1.15f, kit.walkwayHeight + 1.05f, -z + 0.25f), sign, accent);

            BuildVent(street, 0,
                new Vector3(x - 1.35f, kit.walkwayHeight, z - 1.35f), metal, steam);
            BuildVent(street, 1,
                new Vector3(-x + 1.35f, kit.walkwayHeight, z - 1.35f), metal, steam);

            Sphere(street, "RouteBeacon_A", new Vector3(0f, kit.walkwayHeight + 0.10f, -z + 0.55f),
                Vector3.one * 0.16f, accent, true, "Accent");
            Sphere(street, "RouteBeacon_B", new Vector3(0f, kit.walkwayHeight + 0.10f, z - 0.55f),
                Vector3.one * 0.16f, accent, true, "Accent");

            street.gameObject.AddComponent<CityStreetLifeRuntime>();
            Debug.Log("ZIPTIDE: CITY_STAGE_B district=" + def.id
                + " objects=" + CountObjects(street)
                + " renderers=" + street.GetComponentsInChildren<Renderer>(true).Length);
        }

        private static void BuildLamp(Transform parent, int index, Vector3 basePos,
            Color metal, Color accent, Color glow)
        {
            Transform root = new GameObject("StreetLamp_" + index).transform;
            root.SetParent(parent, false);
            root.localPosition = basePos;
            Cube(root, "Post", new Vector3(0f, 1.30f, 0f), new Vector3(0.09f, 2.60f, 0.09f),
                metal, false, "Metal");
            Cube(root, "Arm", new Vector3(0.20f, 2.57f, 0f), new Vector3(0.46f, 0.07f, 0.07f),
                accent, false, "Accent");
            Sphere(root, "LampGlow_" + index, new Vector3(0.39f, 2.48f, 0f),
                Vector3.one * 0.18f, glow, true, "Lamp");
            Cube(root, "LampHood", new Vector3(0.39f, 2.61f, 0f), new Vector3(0.30f, 0.08f, 0.30f),
                metal, false, "Metal");
        }

        private static void BuildDistrictSign(Transform parent, string districtId,
            Vector3 position, Color sign, Color accent)
        {
            Transform root = new GameObject("DistrictSign").transform;
            root.SetParent(parent, false);
            root.localPosition = position;
            Cube(root, "SignPost", new Vector3(0f, -0.38f, 0f), new Vector3(0.09f, 1.40f, 0.09f),
                accent, false, "Accent");
            Cube(root, "SignPlate", Vector3.zero, new Vector3(1.65f, 0.55f, 0.08f),
                sign, false, "Sign");
            GameObject label = new GameObject("Label");
            label.transform.SetParent(root, false);
            label.transform.localPosition = new Vector3(0f, 0f, -0.052f);
            label.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            TextMesh text = label.AddComponent<TextMesh>();
            text.text = districtId.ToUpperInvariant();
            text.fontSize = 56;
            text.characterSize = 0.023f;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.color = accent;
        }

        private static void BuildVent(Transform parent, int index, Vector3 position,
            Color metal, Color steam)
        {
            Transform root = new GameObject("Vent_" + index).transform;
            root.SetParent(parent, false);
            root.localPosition = position;
            Cube(root, "VentBase", new Vector3(0f, 0.12f, 0f), new Vector3(0.55f, 0.24f, 0.55f),
                metal, false, "Metal");
            Cylinder(root, "VentStack", new Vector3(0f, 0.48f, 0f), new Vector3(0.18f, 0.40f, 0.18f),
                Vector3.zero, metal, false, "Metal");
            for (int puff = 0; puff < 3; puff++)
                Sphere(root, "Steam_" + index + "_" + puff,
                    new Vector3((puff - 1) * 0.08f, 0.90f + puff * 0.22f, 0f),
                    Vector3.one * (0.16f + puff * 0.04f), steam, false, "Steam");
        }

        private static void Beam(Transform parent, string name, Vector3 a, Vector3 b, Color color)
        {
            Vector3 delta = b - a;
            GameObject beam = Cube(parent, name, (a + b) * 0.5f,
                new Vector3(0.035f, 0.035f, delta.magnitude), color, false, "Cable");
            beam.transform.localRotation = delta.sqrMagnitude > 0.001f
                ? Quaternion.LookRotation(delta.normalized, Vector3.up)
                : Quaternion.identity;
        }

        private static GameObject Cube(Transform parent, string name, Vector3 position,
            Vector3 scale, Color color, bool emissive, string slot)
        {
            return Primitive(parent, name, PrimitiveType.Cube, position, scale,
                Vector3.zero, color, emissive, slot);
        }

        private static GameObject Sphere(Transform parent, string name, Vector3 position,
            Vector3 scale, Color color, bool emissive, string slot)
        {
            return Primitive(parent, name, PrimitiveType.Sphere, position, scale,
                Vector3.zero, color, emissive, slot);
        }

        private static GameObject Cylinder(Transform parent, string name, Vector3 position,
            Vector3 scale, Vector3 euler, Color color, bool emissive, string slot)
        {
            return Primitive(parent, name, PrimitiveType.Cylinder, position, scale,
                euler, color, emissive, slot);
        }

        private static GameObject Primitive(Transform parent, string name, PrimitiveType primitive,
            Vector3 position, Vector3 scale, Vector3 euler, Color color, bool emissive, string slot)
        {
            GameObject go = GameObject.CreatePrimitive(primitive);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            go.transform.localRotation = Quaternion.Euler(euler);
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = Mat(slot, color, emissive);
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            bool moving = name.StartsWith("Steam_") || name.StartsWith("LampGlow_");
            if (!moving) GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic);
            return go;
        }

        private static Material Mat(string slot, Color color, bool emissive)
        {
            if (Materials.TryGetValue(slot, out Material material) && material != null) return material;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            material = new Material(shader) { name = "CityStageB_" + slot };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (emissive && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.35f);
            }
            Materials[slot] = material;
            return material;
        }

        public static int CountObjects(Transform streetRoot)
        {
            if (streetRoot == null) return 0;
            return streetRoot.GetComponentsInChildren<Transform>(true).Length - 1;
        }

        public static Summary Measure(Transform stageRoot)
        {
            if (stageRoot == null) return default;
            int districts = 0;
            int objects = 0;
            var materialIds = new HashSet<int>();
            foreach (Transform t in stageRoot.root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name != DistrictRootName) continue;
                districts++;
                objects += CountObjects(t);
            }
            Renderer[] renderers = stageRoot.root.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
                if (renderer != null && renderer.sharedMaterial != null &&
                    renderer.sharedMaterial.name.StartsWith("CityStageB_"))
                    materialIds.Add(renderer.sharedMaterial.GetInstanceID());
            int stageRenderers = 0;
            foreach (Renderer renderer in renderers)
                if (renderer != null && IsStageB(renderer.transform)) stageRenderers++;
            return new Summary(districts, objects, stageRenderers, materialIds.Count);
        }

        private static bool IsStageB(Transform t)
        {
            while (t != null)
            {
                if (t.name == DistrictRootName || t.name == RootName) return true;
                t = t.parent;
            }
            return false;
        }
    }
}
#endif
