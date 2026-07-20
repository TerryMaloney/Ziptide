#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Replaces ToxicCity's legacy single-cube edge facades with a deterministic, budgeted
    /// base/middle/cap grammar. The original collider footprint and total height remain authoritative;
    /// all added windows, awnings, curbs and rooftop silhouettes are presentation-only.
    /// </summary>
    public static class ToxicCityStageA
    {
        public const int TargetObjectsPerDistrict = 220;
        public const int HardObjectsPerDistrict = 280;
        public const int HardMaterialsPerDistrict = 12;
        private const string MarkerName = "__CITY_STAGE_A";
        private const float WindowDepth = 0.08f;

        public readonly struct Summary
        {
            public readonly int Districts;
            public readonly int Facades;
            public readonly int Objects;
            public readonly int Renderers;
            public readonly int Materials;

            public Summary(int districts, int facades, int objects, int renderers, int materials)
            {
                Districts = districts;
                Facades = facades;
                Objects = objects;
                Renderers = renderers;
                Materials = materials;
            }
        }

        public static Summary Enrich(Transform cityRoot, CityLayoutDefinition kit)
        {
            if (cityRoot == null || kit == null) return default;
            if (cityRoot.Find(MarkerName) != null) return Measure(cityRoot);

            var factory = new CityStageAPrimitiveFactory();
            int districtCount = 0;
            int facadeCount = 0;
            int totalObjects = 0;
            int totalRenderers = 0;

            for (int districtIndex = 0; districtIndex < kit.districts.Count; districtIndex++)
            {
                DistrictDef def = kit.districts[districtIndex];
                if (def == null || string.IsNullOrEmpty(def.id)) continue;
                Transform district = cityRoot.Find("District_" + def.id);
                if (district == null) continue;

                var facades = new List<Transform>();
                for (int i = 0; i < district.childCount; i++)
                {
                    Transform child = district.GetChild(i);
                    if (child != null && child.name.StartsWith("Facade_")) facades.Add(child);
                }
                if (facades.Count == 0) continue;

                GlobalPalette palette = def.useOverride && def.paletteOverride != null
                    ? def.paletteOverride
                    : kit.palette;
                int districtSeed = kit.seed ^ StableHash(def.id) ^ (districtIndex * 7919);
                AddCurbs(district, def.bounds, kit.walkwayHeight, palette, factory);

                for (int i = 0; i < facades.Count; i++)
                {
                    Transform facade = facades[i];
                    bool isXEdge = facade.name.StartsWith("Facade_X");
                    Vector3 center = facade.localPosition;
                    Vector3 scale = facade.localScale;
                    string name = facade.name;
                    Object.DestroyImmediate(facade.gameObject);
                    BuildFacade(district, name, center, scale, isXEdge,
                        districtSeed ^ (i * 104729), palette, factory);
                    facadeCount++;
                }

                int objects = CountObjects(district);
                int renderers = CountRenderers(district);
                int materials = CountMaterials(district);
                districtCount++;
                totalObjects += objects;
                totalRenderers += renderers;
                Debug.Log("ZIPTIDE: CITY_STAGE_A district=" + def.id
                    + " facades=" + facades.Count
                    + " objects=" + objects
                    + " renderers=" + renderers
                    + " materials=" + materials);
            }

            var marker = new GameObject(MarkerName);
            marker.transform.SetParent(cityRoot, false);
            Debug.Log("ZIPTIDE: CITY_STAGE_A_COMPLETE districts=" + districtCount
                + " facades=" + facadeCount
                + " objects=" + totalObjects
                + " renderers=" + totalRenderers
                + " sharedMaterials=" + factory.MaterialCount);
            return new Summary(districtCount, facadeCount, totalObjects, totalRenderers, factory.MaterialCount);
        }

        private static void BuildFacade(Transform district, string name, Vector3 legacyCenter,
            Vector3 legacyScale, bool isXEdge, int seed, GlobalPalette palette,
            CityStageAPrimitiveFactory factory)
        {
            float totalHeight = Mathf.Max(3.4f, legacyScale.y);
            float baseHeight = Mathf.Clamp(totalHeight * 0.30f, 2.1f, 2.9f);
            float capHeight = Mathf.Clamp(totalHeight * 0.12f, 0.55f, 1.05f);
            float middleHeight = Mathf.Max(0.8f, totalHeight - baseHeight - capHeight);
            float bottom = legacyCenter.y - totalHeight * 0.5f;
            float crownScale = CityBuildingPresentationCore.CrownScale(seed);
            Vector3 footprint = new Vector3(Mathf.Max(0.8f, legacyScale.x), 1f,
                Mathf.Max(0.8f, legacyScale.z));

            var root = new GameObject(name);
            root.transform.SetParent(district, false);
            root.transform.localPosition = new Vector3(legacyCenter.x, bottom, legacyCenter.z);

            Color baseColor = Color.Lerp(palette.building2, Color.black, 0.22f);
            Color middleColor = CityBuildingPresentationCore.UsesAlternateWall(seed, 1, isXEdge ? 0 : 1)
                ? palette.building2 : palette.building1;
            Color capColor = Color.Lerp(palette.building2, palette.rail, 0.22f);

            factory.Cube(root.transform, "Base", new Vector3(0f, baseHeight * 0.5f, 0f),
                new Vector3(footprint.x, baseHeight, footprint.z), baseColor, true);
            factory.Cube(root.transform, "Middle",
                new Vector3(0f, baseHeight + middleHeight * 0.5f, 0f),
                new Vector3(footprint.x * 0.96f, middleHeight, footprint.z * 0.96f),
                middleColor, true);
            factory.Cube(root.transform, "Cap",
                new Vector3(0f, totalHeight - capHeight * 0.5f, 0f),
                new Vector3(footprint.x * crownScale, capHeight, footprint.z * crownScale),
                capColor, true);
            factory.Cube(root.transform, "Cornice",
                new Vector3(0f, totalHeight - capHeight - 0.05f, 0f),
                new Vector3(footprint.x * 1.02f, 0.12f, footprint.z * 1.02f), palette.rail);

            AddWindows(root.transform, footprint, baseHeight, middleHeight, isXEdge, seed, palette, factory);
            AddStorefront(root.transform, footprint, baseHeight, isXEdge, seed, palette, factory);
            AddRoof(root.transform, footprint, totalHeight, seed, palette, factory);
        }

        private static void AddWindows(Transform root, Vector3 footprint, float baseHeight,
            float middleHeight, bool isXEdge, int seed, GlobalPalette palette,
            CityStageAPrimitiveFactory factory)
        {
            int rows = Mathf.Clamp(Mathf.RoundToInt(middleHeight / 2.35f), 1, 4);
            float width = isXEdge ? footprint.x : footprint.z;
            float spacing = width * 0.27f;
            float face = isXEdge ? -(footprint.z * 0.5f + WindowDepth)
                : -(footprint.x * 0.5f + WindowDepth);

            for (int row = 0; row < rows; row++)
            {
                float y = baseHeight + ((row + 0.55f) / rows) * middleHeight;
                for (int column = 0; column < 2; column++)
                {
                    float across = column == 0 ? -spacing : spacing;
                    CityWindowLight mode = CityBuildingPresentationCore.WindowFor(
                        seed, row, isXEdge ? 0 : 1, column, 0.62f);
                    Vector3 position = isXEdge ? new Vector3(across, y, face)
                        : new Vector3(face, y, across);
                    Vector3 size = isXEdge
                        ? new Vector3(Mathf.Min(0.95f, width * 0.22f), 0.78f, WindowDepth)
                        : new Vector3(WindowDepth, 0.78f, Mathf.Min(0.95f, width * 0.22f));
                    factory.Cube(root, "Window_" + row + "_" + column + "_" + mode,
                        position, size, factory.WindowColor(mode, palette), false,
                        mode != CityWindowLight.Dark);
                }
            }
        }

        private static void AddStorefront(Transform root, Vector3 footprint, float baseHeight,
            bool isXEdge, int seed, GlobalPalette palette, CityStageAPrimitiveFactory factory)
        {
            float face = isXEdge ? -(footprint.z * 0.5f + 0.05f)
                : -(footprint.x * 0.5f + 0.05f);
            float offset = CityBuildingPresentationCore.SignedOffset(seed, 5)
                * (isXEdge ? footprint.x : footprint.z) * 0.18f;
            Vector3 doorPosition = isXEdge ? new Vector3(offset, 1.05f, face)
                : new Vector3(face, 1.05f, offset);
            Vector3 doorSize = isXEdge ? new Vector3(0.82f, 2.05f, 0.10f)
                : new Vector3(0.10f, 2.05f, 0.82f);
            factory.Cube(root, "DoorVisual", doorPosition, doorSize,
                Color.Lerp(palette.facadeWindow, palette.metal, 0.35f));

            Vector3 awningPosition = doorPosition + Vector3.up * (baseHeight * 0.48f);
            Vector3 awningSize = isXEdge ? new Vector3(1.65f, 0.16f, 0.62f)
                : new Vector3(0.62f, 0.16f, 1.65f);
            if (isXEdge) awningPosition.z -= 0.26f; else awningPosition.x -= 0.26f;
            factory.Cube(root, "Awning", awningPosition, awningSize,
                Color.Lerp(palette.accent, palette.rail, 0.25f));
        }

        private static void AddRoof(Transform root, Vector3 footprint, float totalHeight,
            int seed, GlobalPalette palette, CityStageAPrimitiveFactory factory)
        {
            int count = CityBuildingPresentationCore.RoofClutterCount(seed, 1, 3);
            for (int i = 0; i < count; i++)
            {
                float x = CityBuildingPresentationCore.SignedOffset(seed, 20 + i) * footprint.x * 0.24f;
                float z = CityBuildingPresentationCore.SignedOffset(seed, 40 + i) * footprint.z * 0.24f;
                float h = 0.30f + 0.18f * (i + 1);
                factory.Cube(root, "RoofUnit_" + i,
                    new Vector3(x, totalHeight + h * 0.5f, z),
                    new Vector3(0.48f + i * 0.08f, h, 0.42f + i * 0.06f),
                    i % 2 == 0 ? palette.metal : palette.rail);
            }
            if (CityBuildingPresentationCore.HasAntenna(seed))
            {
                factory.Cube(root, "Antenna",
                    new Vector3(
                        CityBuildingPresentationCore.SignedOffset(seed, 71) * footprint.x * 0.18f,
                        totalHeight + 1.05f,
                        CityBuildingPresentationCore.SignedOffset(seed, 73) * footprint.z * 0.18f),
                    new Vector3(0.06f, 2.1f, 0.06f), palette.accent, false, true);
            }
        }

        private static void AddCurbs(Transform district, Vector2 bounds, float walkwayHeight,
            GlobalPalette palette, CityStageAPrimitiveFactory factory)
        {
            var root = new GameObject("__CITY_STAGE_A_CURBS");
            root.transform.SetParent(district, false);
            float x = bounds.x * 0.5f, z = bounds.y * 0.5f;
            const float width = 0.18f, height = 0.10f;
            Color color = Color.Lerp(palette.concrete, palette.catwalk, 0.35f);
            factory.Cube(root.transform, "Curb_N", new Vector3(0f, walkwayHeight + height * 0.5f, z - 0.12f),
                new Vector3(bounds.x, height, width), color);
            factory.Cube(root.transform, "Curb_S", new Vector3(0f, walkwayHeight + height * 0.5f, -z + 0.12f),
                new Vector3(bounds.x, height, width), color);
            factory.Cube(root.transform, "Curb_E", new Vector3(x - 0.12f, walkwayHeight + height * 0.5f, 0f),
                new Vector3(width, height, bounds.y), color);
            factory.Cube(root.transform, "Curb_W", new Vector3(-x + 0.12f, walkwayHeight + height * 0.5f, 0f),
                new Vector3(width, height, bounds.y), color);
        }

        private static Summary Measure(Transform root)
        {
            int districts = 0, facades = 0, objects = 0, renderers = 0;
            var materials = new HashSet<int>();
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name.StartsWith("District_")) districts++;
                if (t.name.StartsWith("Facade_")) facades++;
                if (IsStageA(t)) objects++;
            }
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || !IsStageA(renderer.transform)) continue;
                renderers++;
                if (renderer.sharedMaterial != null) materials.Add(renderer.sharedMaterial.GetInstanceID());
            }
            return new Summary(districts, facades, objects, renderers, materials.Count);
        }

        public static int CountObjects(Transform district)
        {
            int count = 0;
            foreach (Transform t in district.GetComponentsInChildren<Transform>(true))
                if (IsStageA(t)) count++;
            return count;
        }

        public static int CountRenderers(Transform district)
        {
            int count = 0;
            foreach (Renderer renderer in district.GetComponentsInChildren<Renderer>(true))
                if (renderer != null && IsStageA(renderer.transform)) count++;
            return count;
        }

        public static int CountMaterials(Transform district)
        {
            var ids = new HashSet<int>();
            foreach (Renderer renderer in district.GetComponentsInChildren<Renderer>(true))
                if (renderer != null && IsStageA(renderer.transform) && renderer.sharedMaterial != null)
                    ids.Add(renderer.sharedMaterial.GetInstanceID());
            return ids.Count;
        }

        private static bool IsStageA(Transform t)
        {
            while (t != null)
            {
                if (t.name.StartsWith("Facade_") || t.name == "__CITY_STAGE_A_CURBS") return true;
                t = t.parent;
            }
            return false;
        }

        private static int StableHash(string value)
        {
            unchecked
            {
                int hash = 17;
                for (int i = 0; value != null && i < value.Length; i++) hash = hash * 31 + value[i];
                return hash;
            }
        }
    }
}
#endif
