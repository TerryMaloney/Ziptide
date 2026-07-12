#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// FORGE III F3.5 world-wiring half. Places a maximum of three fixed looping mote markers along
    /// the authored traversal route. The markers carry no visuals or physics themselves; at runtime
    /// AmbientMoteVolume reserves one bounded VfxFactory system each.
    /// </summary>
    public static class AmbientMoteAuthor
    {
        public const string RootName = "AmbientMotes";
        public const int MaxEmitters = 3;
        public const float HeightAboveTerrain = 1.45f;

        public static void Place(
            Transform parent,
            CityLayoutDefinition kit,
            IReadOnlyList<Vector2> route)
        {
            if (parent == null) return;

            string recipeId = RecipeFor(
                kit != null && kit.experience != null
                    ? kit.experience.biome
                    : default);
            int[] indices = SelectRouteIndices(route != null ? route.Count : 0);
            var positions = new List<Vector3>(indices.Length);

            for (int i = 0; i < indices.Length; i++)
            {
                Vector2 point = route[indices[i]];
                float ground = kit != null
                    ? WorldExperienceBuilder.HeightAt(kit, point.x, point.y)
                    : 0f;
                positions.Add(new Vector3(
                    point.x,
                    ground + HeightAboveTerrain,
                    point.y));
            }

            PlaceMarkers(parent, recipeId, positions);
            Debug.Log("ZIPTIDE: AMBIENT_MOTES_AUTHORED recipe=" + recipeId +
                      " count=" + positions.Count);
        }

        /// <summary>Biome vocabulary. Both canon mote ids have an explicit scatter row.</summary>
        public static string RecipeFor(BiomePreset biome)
        {
            switch (biome)
            {
                case BiomePreset.TideFlats:
                case BiomePreset.CavernFloor:
                    return "motes_spore";
                case BiomePreset.Dunes:
                case BiomePreset.Mesas:
                case BiomePreset.Canyon:
                default:
                    return "motes_amber";
            }
        }

        /// <summary>
        /// Stable route sampling: start, middle and far end, with duplicates removed for short routes.
        /// </summary>
        public static int[] SelectRouteIndices(int routeCount)
        {
            if (routeCount <= 0) return new int[0];

            var indices = new List<int>(MaxEmitters) { 0 };
            if (routeCount > 2) AddUnique(indices, routeCount / 2);
            if (routeCount > 1) AddUnique(indices, routeCount - 1);
            return indices.ToArray();
        }

        /// <summary>
        /// Idempotent marker builder separated from terrain/layout math so EditMode tests can verify
        /// the exact generated structure without constructing or mutating real world assets.
        /// </summary>
        public static void PlaceMarkers(
            Transform parent,
            string recipeId,
            IReadOnlyList<Vector3> positions)
        {
            if (parent == null) return;

            Transform existing = parent.Find(RootName);
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            var root = new GameObject(RootName).transform;
            root.SetParent(parent, false);

            int count = Mathf.Min(
                MaxEmitters,
                positions != null ? positions.Count : 0);
            for (int i = 0; i < count; i++)
            {
                var marker = new GameObject("AmbientMote_" + i + "_" + recipeId);
                marker.transform.SetParent(root, false);
                marker.transform.position = positions[i];
                marker.isStatic = false;

                var volume = marker.AddComponent<AmbientMoteVolume>();
                volume.Configure(recipeId);
            }
        }

        private static void AddUnique(List<int> indices, int value)
        {
            if (!indices.Contains(value)) indices.Add(value);
        }
    }
}
#endif
