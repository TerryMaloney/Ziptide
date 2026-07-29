#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Ship;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Places the three authored starter rides on explicit ToxicCity parking pads. Each vehicle also
    /// receives a saved lightweight silhouette under `__VehicleVisual`; VehicleRuntime replaces that
    /// exact owned root with the full interactive presentation on Start.
    /// </summary>
    public static class ToxicCityVehicleBuilder
    {
        public const string RootName = "__TOXIC_CITY_VEHICLES";
        public const int ExpectedVehicleCount = 3;
        private const string PreviewRootName = "__VehicleVisual";
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
            if (existing != null)
            {
                EnsureSavedPreviews(existing);
                return Measure(existing);
            }

            Transform root = new GameObject(RootName).transform;
            root.SetParent(cityRoot, false);
            Materials.Clear();

            DistrictDef shipyard = FindDistrict(kit, "Shipyard");
            DistrictDef dispatch = FindDistrict(kit, "Dispatch");
            // ⚖ The expedition ride parks at the QUAY, beside the Dockmaster's booth: he hands over
            // the work order and the keys in the same breath, so the drive out to the flats starts
            // where the lead is given rather than across town. Falls back to the old Market pad for
            // layouts authored before the quay existed.
            DistrictDef expeditionYard = FindDistrict(kit, "Quay") ?? FindDistrict(kit, "Market");

            Spawn(root, "tide_skiff",
                (shipyard != null ? shipyard.anchor : new Vector3(0f, 0f, -30f)) + new Vector3(6f, 0.55f, 3f),
                ColorFor("tide_skiff"));
            Spawn(root, "dune_hoverbike",
                (dispatch != null ? dispatch.anchor : new Vector3(0f, 0f, -8f)) + new Vector3(-5f, 0.82f, 4f),
                ColorFor("dune_hoverbike"));
            Spawn(root, "cavern_crawler",
                (expeditionYard != null ? expeditionYard.anchor : new Vector3(24f, 0f, -50f))
                    + new Vector3(-4f, 0.22f, 3f),
                ColorFor("cavern_crawler"));

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
            BuildSavedPreview(vehicle.transform, id, color);

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

        private static void EnsureSavedPreviews(Transform root)
        {
            foreach (VehicleRuntime vehicle in root.GetComponentsInChildren<VehicleRuntime>(true))
            {
                if (vehicle == null || vehicle.transform.Find(PreviewRootName) != null) continue;
                BuildSavedPreview(vehicle.transform, vehicle.VehicleId, ColorFor(vehicle.VehicleId));
            }
        }

        private static void BuildSavedPreview(Transform vehicle, string id, Color color)
        {
            Transform old = vehicle.Find(PreviewRootName);
            if (old != null) Object.DestroyImmediate(old.gameObject);
            Transform preview = new GameObject(PreviewRootName).transform;
            preview.SetParent(vehicle, false);
            Color dark = Color.Lerp(color, Color.black, 0.45f);
            Color glow = Color.Lerp(color, Color.white, 0.35f);

            if (id == "dune_hoverbike")
            {
                PreviewPart(preview, "BikeChassis", PrimitiveType.Cube,
                    new Vector3(0f, 0.48f, 0f), new Vector3(0.55f, 0.30f, 2.18f), Vector3.zero, color);
                PreviewPart(preview, "HoverPad_L", PrimitiveType.Cylinder,
                    new Vector3(-0.58f, 0.16f, 0f), new Vector3(0.30f, 0.055f, 0.38f), Vector3.zero, glow);
                PreviewPart(preview, "HoverPad_R", PrimitiveType.Cylinder,
                    new Vector3(0.58f, 0.16f, 0f), new Vector3(0.30f, 0.055f, 0.38f), Vector3.zero, glow);
                PreviewPart(preview, "RearThruster", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.48f, -1.20f), new Vector3(0.32f, 0.16f, 0.32f),
                    new Vector3(90f, 0f, 0f), dark);
            }
            else if (id == "cavern_crawler")
            {
                PreviewPart(preview, "CrawlerHull", PrimitiveType.Cube,
                    new Vector3(0f, 0.52f, -0.12f), new Vector3(1.38f, 0.58f, 2.05f), Vector3.zero, color);
                PreviewPart(preview, "Tread_L", PrimitiveType.Cube,
                    new Vector3(-0.86f, 0.30f, -0.10f), new Vector3(0.42f, 0.48f, 2.35f), Vector3.zero, dark);
                PreviewPart(preview, "Tread_R", PrimitiveType.Cube,
                    new Vector3(0.86f, 0.30f, -0.10f), new Vector3(0.42f, 0.48f, 2.35f), Vector3.zero, dark);
                PreviewPart(preview, "Drill", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.60f, 1.35f), new Vector3(0.42f, 0.45f, 0.42f),
                    new Vector3(90f, 0f, 0f), glow);
            }
            else
            {
                PreviewPart(preview, "SkiffHull", PrimitiveType.Cube,
                    new Vector3(0f, 0.34f, 0f), new Vector3(1.25f, 0.34f, 2.35f), Vector3.zero, color);
                PreviewPart(preview, "Pontoon_L", PrimitiveType.Cube,
                    new Vector3(-0.82f, 0.20f, -0.05f), new Vector3(0.28f, 0.25f, 2.62f), Vector3.zero, dark);
                PreviewPart(preview, "Pontoon_R", PrimitiveType.Cube,
                    new Vector3(0.82f, 0.20f, -0.05f), new Vector3(0.28f, 0.25f, 2.62f), Vector3.zero, dark);
                PreviewPart(preview, "FanRing", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.72f, -1.35f), new Vector3(0.48f, 0.12f, 0.48f),
                    new Vector3(90f, 0f, 0f), glow);
            }
        }

        private static void PreviewPart(Transform parent, string name, PrimitiveType primitive,
            Vector3 localPosition, Vector3 localScale, Vector3 localEuler, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(primitive);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            part.transform.localRotation = Quaternion.Euler(localEuler);
            Collider collider = part.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);
            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = Mat(color);
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        private static Color ColorFor(string id)
        {
            if (id == "dune_hoverbike") return new Color(0.68f, 0.42f, 0.92f);
            if (id == "cavern_crawler") return new Color(0.92f, 0.62f, 0.16f);
            return new Color(0.20f, 0.78f, 0.90f);
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
