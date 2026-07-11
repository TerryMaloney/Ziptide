#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// EXCELLENCE_MAP gap #10. Separates structural catalog parity (blockable) from actual player
    /// surfacing breadth (warning debt). It never authors or mutates assets/content.
    /// </summary>
    public static class CatalogBreadthAuditRules
    {
        public const int MinimumPlantSpecs = 20;
        public const int MinimumVehicleSpecs = 3;
        public const int MinimumVehicleArchetypes = 3;

        public const string PlantAssetFolder = "Assets/Ziptide/Resources/Garden";
        public const string VehicleAssetFolder = "Assets/Ziptide/Resources/Vehicles";
        public const string WorldPoiBuilderRelativePath = "Ziptide/Editor/Patching/WorldPoiBuilder.cs";
        public const string VehicleGarageRelativePath = "Ziptide/Ship/Runtime/VehicleGarageRuntime.cs";

        public static void Run(SceneAuditReport report, bool requireAuthoredAssets = false)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            GardenAuthor.PlantSpec[] plants = GardenAuthor.PlantSpecs();
            VehicleAuthor.VehicleSpec[] vehicles = VehicleAuthor.VehicleSpecs();

            ValidatePlantSpecs(report, plants);
            ValidateVehicleSpecs(report, vehicles);
            ValidatePlantAssets(report, plants, requireAuthoredAssets);
            ValidateVehicleAssets(report, vehicles, requireAuthoredAssets);
            ValidatePlantSurfacing(report, plants);
            ValidateVehicleSurfacing(report, vehicles);
        }

        public static string[] MissingIds(IEnumerable<string> allIds, IEnumerable<string> surfacedIds)
        {
            var surfaced = new HashSet<string>(
                surfacedIds ?? Array.Empty<string>(),
                StringComparer.Ordinal);
            return (allIds ?? Array.Empty<string>())
                .Where(id => !string.IsNullOrWhiteSpace(id) && !surfaced.Contains(id))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToArray();
        }

        public static VehicleArchetype[] MissingVehicleArchetypes(
            IEnumerable<VehicleAuthor.VehicleSpec> specs)
        {
            var represented = new HashSet<VehicleArchetype>();
            if (specs != null)
                foreach (var spec in specs)
                    represented.Add(spec.Archetype);

            return Enum.GetValues(typeof(VehicleArchetype))
                .Cast<VehicleArchetype>()
                .Where(value => !represented.Contains(value))
                .OrderBy(value => (int)value)
                .ToArray();
        }

        private static void ValidatePlantSpecs(
            SceneAuditReport report,
            IReadOnlyList<GardenAuthor.PlantSpec> specs)
        {
            if (specs == null || specs.Count < MinimumPlantSpecs)
            {
                report.Blocker("PLANT_CATALOG_TOO_NARROW",
                    "GardenAuthor exposes " + (specs != null ? specs.Count : 0) +
                    " plant specs; minimum is " + MinimumPlantSpecs + ".");
                return;
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var spec in specs)
            {
                if (string.IsNullOrWhiteSpace(spec.Id))
                {
                    report.Blocker("PLANT_SPEC_ID_EMPTY", "GardenAuthor contains a plant with an empty id.");
                    continue;
                }
                if (!ids.Add(spec.Id))
                    report.Blocker("PLANT_SPEC_ID_DUPLICATE", "Duplicate plant spec id '" + spec.Id + "'.");
                if (string.IsNullOrWhiteSpace(spec.Display))
                    report.Blocker("PLANT_SPEC_INVALID", spec.Id + " has no display name.");
                if (spec.GrowSeconds <= 0)
                    report.Blocker("PLANT_SPEC_INVALID", spec.Id + " has non-positive grow time.");
                if (spec.Yield == null || spec.Yield.Length == 0)
                    report.Blocker("PLANT_SPEC_INVALID", spec.Id + " has no harvest yield.");
                else
                    foreach (var entry in spec.Yield)
                        if (string.IsNullOrWhiteSpace(entry.resourceId) || entry.amount <= 0)
                            report.Blocker("PLANT_SPEC_INVALID",
                                spec.Id + " has invalid yield '" + entry.resourceId + "' amount=" + entry.amount + ".");
            }
        }

        private static void ValidateVehicleSpecs(
            SceneAuditReport report,
            IReadOnlyList<VehicleAuthor.VehicleSpec> specs)
        {
            if (specs == null || specs.Count < MinimumVehicleSpecs)
            {
                report.Blocker("VEHICLE_CATALOG_TOO_NARROW",
                    "VehicleAuthor exposes " + (specs != null ? specs.Count : 0) +
                    " vehicle specs; minimum is " + MinimumVehicleSpecs + ".");
                return;
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            var archetypes = new HashSet<VehicleArchetype>();
            foreach (var spec in specs)
            {
                if (string.IsNullOrWhiteSpace(spec.Id))
                {
                    report.Blocker("VEHICLE_SPEC_ID_EMPTY", "VehicleAuthor contains a vehicle with an empty id.");
                    continue;
                }
                if (!ids.Add(spec.Id))
                    report.Blocker("VEHICLE_SPEC_ID_DUPLICATE", "Duplicate vehicle spec id '" + spec.Id + "'.");
                archetypes.Add(spec.Archetype);
                if (string.IsNullOrWhiteSpace(spec.Display) || string.IsNullOrWhiteSpace(spec.Biome) ||
                    spec.Cruise <= 0f || spec.Boost < 1f || spec.Boost > 3f ||
                    spec.Hover < 0f || spec.Roam <= 0f)
                {
                    report.Blocker("VEHICLE_SPEC_INVALID",
                        spec.Id + " has invalid display/biome/comfort tuning values.");
                }
            }

            if (archetypes.Count < MinimumVehicleArchetypes)
                report.Blocker("VEHICLE_ARCHETYPE_SPAN_LOW",
                    "Starter fleet spans " + archetypes.Count + " archetypes; minimum is " +
                    MinimumVehicleArchetypes + ".");
        }

        private static void ValidatePlantAssets(
            SceneAuditReport report,
            IReadOnlyList<GardenAuthor.PlantSpec> specs,
            bool requireAuthoredAssets)
        {
            var specsById = specs
                .Where(spec => !string.IsNullOrWhiteSpace(spec.Id))
                .ToDictionary(spec => spec.Id, spec => spec, StringComparer.Ordinal);
            var assetsById = new Dictionary<string, PlantDefinition>(StringComparer.Ordinal);

            foreach (string guid in AssetDatabase.FindAssets("t:PlantDefinition", new[] { PlantAssetFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<PlantDefinition>(path);
                if (asset == null) continue;

                if (string.IsNullOrWhiteSpace(asset.id))
                {
                    report.Blocker("PLANT_ASSET_ID_EMPTY", "PlantDefinition has no id.", path);
                    continue;
                }
                if (!assetsById.TryAdd(asset.id, asset))
                    report.Blocker("PLANT_ASSET_ID_DUPLICATE", "Duplicate PlantDefinition id '" + asset.id + "'.", path);
                if (!string.Equals(Path.GetFileNameWithoutExtension(path), asset.id, StringComparison.Ordinal))
                    report.Blocker("PLANT_ASSET_FILENAME_DRIFT",
                        "Plant asset filename does not match id '" + asset.id + "'.", path);
                if (!specsById.TryGetValue(asset.id, out var spec))
                {
                    report.Blocker("PLANT_ASSET_ORPHAN",
                        "PlantDefinition id '" + asset.id + "' has no GardenAuthor spec.", path);
                    continue;
                }
                if (!PlantAssetIsUsable(asset))
                    report.Blocker("PLANT_ASSET_INVALID", asset.id + " has unusable growth/yield data.", path);
                if (!PlantAssetMatchesSpec(asset, spec))
                    report.Warning("PLANT_ASSET_SPEC_DRIFT",
                        asset.id + " differs from the latest create-only GardenAuthor spec. " +
                        "Do not overwrite blindly; reconcile or reseed deliberately.", path);
            }

            string[] missing = MissingIds(specsById.Keys, assetsById.Keys);
            if (missing.Length > 0)
            {
                string message = missing.Length + " plant assets are absent from the checked-out project: " +
                                 string.Join(", ", missing) +
                                 ". BuildAndroid authors missing assets before APK preprocessing.";
                if (requireAuthoredAssets) report.Blocker("PLANT_ASSETS_MISSING", message, PlantAssetFolder);
                else report.Warning("PLANT_ASSETS_NOT_COMMITTED", message, PlantAssetFolder);
            }
        }

        private static void ValidateVehicleAssets(
            SceneAuditReport report,
            IReadOnlyList<VehicleAuthor.VehicleSpec> specs,
            bool requireAuthoredAssets)
        {
            var specsById = specs
                .Where(spec => !string.IsNullOrWhiteSpace(spec.Id))
                .ToDictionary(spec => spec.Id, spec => spec, StringComparer.Ordinal);
            var assetsById = new Dictionary<string, VehicleDefinition>(StringComparer.Ordinal);

            foreach (string guid in AssetDatabase.FindAssets("t:VehicleDefinition", new[] { VehicleAssetFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<VehicleDefinition>(path);
                if (asset == null) continue;

                if (string.IsNullOrWhiteSpace(asset.id))
                {
                    report.Blocker("VEHICLE_ASSET_ID_EMPTY", "VehicleDefinition has no id.", path);
                    continue;
                }
                if (!assetsById.TryAdd(asset.id, asset))
                    report.Blocker("VEHICLE_ASSET_ID_DUPLICATE",
                        "Duplicate VehicleDefinition id '" + asset.id + "'.", path);
                if (!string.Equals(Path.GetFileNameWithoutExtension(path), asset.id, StringComparison.Ordinal))
                    report.Blocker("VEHICLE_ASSET_FILENAME_DRIFT",
                        "Vehicle asset filename does not match id '" + asset.id + "'.", path);
                if (!specsById.TryGetValue(asset.id, out var spec))
                {
                    report.Blocker("VEHICLE_ASSET_ORPHAN",
                        "VehicleDefinition id '" + asset.id + "' has no VehicleAuthor spec.", path);
                    continue;
                }
                if (!VehicleAssetIsUsable(asset))
                    report.Blocker("VEHICLE_ASSET_INVALID", asset.id + " has invalid comfort/roam data.", path);
                if (!VehicleAssetMatchesSpec(asset, spec))
                    report.Warning("VEHICLE_ASSET_SPEC_DRIFT",
                        asset.id + " differs from the latest create-only VehicleAuthor spec. " +
                        "Reconcile deliberately instead of silently rewriting live data.", path);
            }

            string[] missing = MissingIds(specsById.Keys, assetsById.Keys);
            if (missing.Length > 0)
            {
                string message = missing.Length + " vehicle assets are absent from the checked-out project: " +
                                 string.Join(", ", missing) +
                                 ". BuildAndroid authors missing assets before APK preprocessing.";
                if (requireAuthoredAssets) report.Blocker("VEHICLE_ASSETS_MISSING", message, VehicleAssetFolder);
                else report.Warning("VEHICLE_ASSETS_NOT_COMMITTED", message, VehicleAssetFolder);
            }
        }

        private static void ValidatePlantSurfacing(
            SceneAuditReport report,
            IReadOnlyList<GardenAuthor.PlantSpec> specs)
        {
            var surfaced = new HashSet<string>(StringComparer.Ordinal);
            foreach (string guid in AssetDatabase.FindAssets("t:WorldPackDefinition"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset == null) continue;
                var serialized = new SerializedObject(asset);
                SerializedProperty gardens = serialized.FindProperty("gardens");
                if (gardens == null || !gardens.isArray) continue;

                for (int i = 0; i < gardens.arraySize; i++)
                {
                    SerializedProperty entry = gardens.GetArrayElementAtIndex(i);
                    SerializedProperty plantId = entry.FindPropertyRelative("plantId");
                    if (plantId != null && !string.IsNullOrWhiteSpace(plantId.stringValue))
                        surfaced.Add(plantId.stringValue);
                }
            }

            string[] missing = MissingIds(specs.Select(spec => spec.Id), surfaced);
            if (missing.Length > 0)
                report.Warning("PLANT_CATALOG_UNSURFACED",
                    missing.Length + "/" + specs.Count +
                    " authored plant species are not referenced by any WorldPack garden: " +
                    string.Join(", ", missing) +
                    ". Add world-pack seeds or an explicit seed-dispenser/almanac seam.");
        }

        private static void ValidateVehicleSurfacing(
            SceneAuditReport report,
            IReadOnlyList<VehicleAuthor.VehicleSpec> specs)
        {
            string sourcePath = ResolveAssetRelativeSource(WorldPoiBuilderRelativePath);
            if (!File.Exists(sourcePath))
            {
                report.Blocker("VEHICLE_WORLD_MAPPING_SOURCE_MISSING",
                    "WorldPoiBuilder source is missing: " + sourcePath,
                    WorldPoiBuilderRelativePath);
            }
            else
            {
                string source = File.ReadAllText(sourcePath);
                foreach (var spec in specs)
                    if (!source.Contains("\"" + spec.Id + "\""))
                        report.Blocker("VEHICLE_NOT_SURFACED_BY_BIOME",
                            "Starter ride '" + spec.Id +
                            "' is absent from WorldPoiBuilder.RideForBiome.",
                            WorldPoiBuilderRelativePath);
            }

            VehicleArchetype[] missingArchetypes = MissingVehicleArchetypes(specs);
            if (missingArchetypes.Length > 0)
                report.Warning("VEHICLE_ARCHETYPES_UNREPRESENTED",
                    missingArchetypes.Length + " vehicle archetypes have no authored ride: " +
                    string.Join(", ", missingArchetypes.Select(value => value.ToString())) + ".");

            string garagePath = ResolveAssetRelativeSource(VehicleGarageRelativePath);
            if (!File.Exists(garagePath))
                report.Warning("VEHICLE_GARAGE_SURFACE_MISSING",
                    "No canonical vehicle garage/catalog surface exists yet. Expected future seam: " +
                    VehicleGarageRelativePath,
                    VehicleGarageRelativePath);
        }

        private static bool PlantAssetIsUsable(PlantDefinition asset)
        {
            if (asset == null || asset.growSeconds <= 0 || asset.harvestYield == null ||
                asset.harvestYield.Count == 0) return false;
            foreach (var entry in asset.harvestYield)
                if (entry == null || string.IsNullOrWhiteSpace(entry.resourceId) || entry.amount <= 0)
                    return false;
            return true;
        }

        private static bool PlantAssetMatchesSpec(
            PlantDefinition asset,
            GardenAuthor.PlantSpec spec)
        {
            if (!string.Equals(asset.displayName, spec.Display, StringComparison.Ordinal) ||
                !string.Equals(asset.biomeId ?? string.Empty, spec.Biome ?? string.Empty, StringComparison.Ordinal) ||
                Math.Abs(asset.growSeconds - spec.GrowSeconds) > 0.001 ||
                Math.Abs(asset.freshWindowSecondsOverride - spec.FreshOverride) > 0.001 ||
                Math.Abs(asset.overripeAfterSecondsOverride - spec.OverripeOverride) > 0.001)
                return false;

            var actualYield = new Dictionary<string, double>(StringComparer.Ordinal);
            foreach (var entry in asset.harvestYield)
                if (entry != null && !string.IsNullOrWhiteSpace(entry.resourceId))
                    actualYield[entry.resourceId] = entry.amount;
            if (spec.Yield == null || actualYield.Count != spec.Yield.Length) return false;
            foreach (var expected in spec.Yield)
                if (!actualYield.TryGetValue(expected.resourceId, out double amount) ||
                    Math.Abs(amount - expected.amount) > 0.001)
                    return false;

            var actualTools = new HashSet<string>(asset.tendToolIds ?? new List<string>(), StringComparer.Ordinal);
            var expectedTools = new HashSet<string>(spec.TendTools ?? Array.Empty<string>(), StringComparer.Ordinal);
            return actualTools.SetEquals(expectedTools);
        }

        private static bool VehicleAssetIsUsable(VehicleDefinition asset)
        {
            return asset != null && asset.cruiseSpeed > 0f &&
                   asset.boostMultiplier >= 1f && asset.boostMultiplier <= 3f &&
                   asset.hoverHeight >= 0f && asset.roamRadius > 0f;
        }

        private static bool VehicleAssetMatchesSpec(
            VehicleDefinition asset,
            VehicleAuthor.VehicleSpec spec)
        {
            return string.Equals(asset.displayName, spec.Display, StringComparison.Ordinal) &&
                   string.Equals(asset.biomeId ?? string.Empty, spec.Biome ?? string.Empty, StringComparison.Ordinal) &&
                   asset.archetype == spec.Archetype &&
                   Mathf.Abs(asset.cruiseSpeed - spec.Cruise) < 0.001f &&
                   Mathf.Abs(asset.boostMultiplier - spec.Boost) < 0.001f &&
                   Mathf.Abs(asset.hoverHeight - spec.Hover) < 0.001f &&
                   Mathf.Abs(asset.roamRadius - spec.Roam) < 0.001f;
        }

        public static string ResolveAssetRelativeSource(string relativePath)
        {
            return Path.Combine(
                Application.dataPath,
                (relativePath ?? string.Empty).Replace('/', Path.DirectorySeparatorChar));
        }
    }

    /// <summary>
    /// Build-time structural gate. BuildAndroid authors missing definitions before BuildPipeline invokes
    /// this callback; breadth debt remains warning-only while missing/orphan/invalid data blocks the APK.
    /// </summary>
    public sealed class CatalogBreadthBuildGate : IPreprocessBuildWithReport
    {
        public int callbackOrder => 835;

        public void OnPreprocessBuild(BuildReport buildReport)
        {
            var report = new SceneAuditReport { sceneName = "__CATALOG_BREADTH__" };
            CatalogBreadthAuditRules.Run(report, requireAuthoredAssets: true);

            foreach (var finding in report.findings)
            {
                string message = "ZIPTIDE: CATALOG_BREADTH_AUDIT code=" + finding.code +
                                 " path=" + finding.objectPath + " message=" + finding.message;
                if (finding.severity == AuditSeverity.Blocker) Debug.LogError(message);
                else Debug.LogWarning(message);
            }

            if (report.blockerCount > 0)
                throw new BuildFailedException(
                    "Plant/vehicle catalog breadth failed with " + report.blockerCount + " blocker(s).");
        }
    }
}
#endif
