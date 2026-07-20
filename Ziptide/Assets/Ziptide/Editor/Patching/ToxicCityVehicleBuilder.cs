#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Ship;

namespace Ziptide.Editor.Patching
{
    /// <summary>Places the three authored starter rides on explicit ToxicCity parking pads.</summary>
    public static class ToxicCityVehicleBuilder
    {
        public const string RootName = "__TOXIC_CITY_VEHICLES";
        public const int ExpectedVehicleCount = 3;
        private static readonly Dictionary<Color, Material> Materials = new Dictionary<Color, Material>();

        public readonly struct Summary
        {
            public readonly int Vehicles;
            public readonly int Pads;
            public Summary(int vehicles, int pads) { Vehicles = vehicles; Pads = pads; }
        }

        public static Summary Build(Transform cityRoot, CityLayoutDefinition kit)
        {
            if (cityRoot == null || kit == null) return default;
            Transform existing = cityRoot.Find(RootName);
            if (existing != null) return Measure(existing);

            Transform root = new GameObject(RootName).transform;
            root.SetParent(cityRoot, false);
            Materials.Clear();

            DistrictDef shipyard = FindDistrict(kit, "Shipyard");
            DistrictDef dispatch = FindDistrict(kit, "Dispatch");
            DistrictDef market = FindDistrict(kit, "Market");

            Spawn(root, "tide_skiff",
                (shipyard != null ? shipyard.anchor : new Vector3(0f, 0f, -30f)) + new Vector3(6f, 0.55f, 3f),
                new Color(0.20f, 0.78f, 0.90f));
            Spawn(root, "dune_hoverbike",
                (dispatch != null ? dispatch.anchor : new Vector3(0f, 0f, -8f)) + new Vector3(-5f, 0.82f, 4f),
                new Color(0.68f, 0.42f, 0.92f));
            Spawn(root, "cavern_crawler",
                (market != null ? market.anchor : new Vector3(26f, 0f, 8f)) + new Vector3(-4f, 0.22f, -5f),
                new Color(0.92f, 0.62f, 0.16f));

            Summary summary = Measure(root);
            Debug.Log("ZIPTIDE: TOXIC_CITY_VEHICLES vehicles=" + summary.Vehicles
                + " pads=" + summary.Pads);
            Materials.Clear();
            return summary;
        }

        private static void Spawn(Transform parent, string id, Vector3 position, Color color)
        {
            Transform bay = new GameObject("VehicleBay_" + id).transform;
            bay.SetParent(parent, false);
            bay.localPosition = position;

            GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pad.name = "ParkingPad_" + id;
            pad.transform.SetParent(bay, false);
            pad.transform.localPosition = new Vector3(0f, -0.10f, 0f);
            pad.transform.localScale = new Vector3(3.2f, 0.08f, 4.0f);
            Collider padCollider = pad.GetComponent<Collider>();
            if (padCollider != null) Object.DestroyImmediate(padCollider);
            Renderer padRenderer = pad.GetComponent<Renderer>();
            if (padRenderer != null)
            {
                padRenderer.sharedMaterial = Mat(color);
                padRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            GameObjectUtility.SetStaticEditorFlags(pad, StaticEditorFlags.BatchingStatic);

            GameObject vehicle = new GameObject("Vehicle_" + id);
            vehicle.transform.SetParent(bay, false);
            vehicle.transform.localPosition = Vector3.zero;
            VehicleRuntime runtime = vehicle.AddComponent<VehicleRuntime>();
            runtime.Configure(id);

            GameObject label = new GameObject("BayLabel");
            label.transform.SetParent(bay, false);
            label.transform.localPosition = new Vector3(0f, 1.95f, -1.55f);
            TextMesh text = label.AddComponent<TextMesh>();
            text.text = id.Replace('_', ' ').ToUpperInvariant() + "\nREADY TO RIDE";
            text.fontSize = 48;
            text.characterSize = 0.024f;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.color = color;
        }

        private static Material Mat(Color color)
        {
            if (Materials.TryGetValue(color, out Material material) && material != null) return material;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            material = new Material(shader) { name = "VehiclePad_" + ColorUtility.ToHtmlStringRGB(color) };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            Materials[color] = material;
            return material;
        }

        private static DistrictDef FindDistrict(CityLayoutDefinition kit, string id)
        {
            if (kit.districts == null) return null;
            for (int i = 0; i < kit.districts.Count; i++)
                if (kit.districts[i] != null && kit.districts[i].id == id) return kit.districts[i];
            return null;
        }

        public static Summary Measure(Transform root)
        {
            if (root == null) return default;
            return new Summary(
                root.GetComponentsInChildren<VehicleRuntime>(true).Length,
                CountPads(root));
        }

        private static int CountPads(Transform root)
        {
            int count = 0;
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                if (t.name.StartsWith("ParkingPad_")) count++;
            return count;
        }
    }
}
#endif
