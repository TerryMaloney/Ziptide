#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Ziptide.Content;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// FORGE III F3.7 deterministic world placement. Signs are non-interactive visual language:
    /// ForgeModuleLook owns the body, one shared opaque glyph material per destination class owns the
    /// Shell script, and no collider/light/input/gameplay state is introduced.
    /// </summary>
    public static class SignAuthor
    {
        public const string RootName = "ShellSignage";
        public const string GlyphChildName = "ShellGlyph";
        public const int MaxSigns = 6;
        public const float ApproachOffset = 3.2f;
        public const float HeightAboveTerrain = 1.65f;

        public struct SignPlan
        {
            public string name;
            public string recipeId;
            public SignDestinationClass destinationClass;
            public Vector3 position;
            public Vector3 forward;
            public int seed;
        }

        public static int Place(
            Transform parent,
            CityLayoutDefinition kit,
            IReadOnlyList<Vector2> route)
        {
            if (parent == null || kit == null) return 0;
            SignRecipeLibrary.EnsureAllAuthored();
            List<SignPlan> plans = Plan(kit, route);
            return PlacePlans(parent, plans, ResolveMaterial);
        }

        /// <summary>Stable priority and approach placement from existing POI/route data.</summary>
        public static List<SignPlan> Plan(
            CityLayoutDefinition kit,
            IReadOnlyList<Vector2> route)
        {
            var plans = new List<SignPlan>();
            if (kit == null || kit.pois == null) return plans;

            var pois = new List<PoiDef>();
            foreach (PoiDef poi in kit.pois)
                if (poi != null) pois.Add(poi);
            pois.Sort(ComparePois);

            for (int i = 0; i < pois.Count && plans.Count < MaxSigns; i++)
            {
                PoiDef poi = pois[i];
                Vector2 poiPoint = new Vector2(poi.position.x, poi.position.z);
                Vector2 approach = NearestRoutePoint(route, poiPoint);
                Vector2 direction = approach - poiPoint;
                if (direction.sqrMagnitude < 0.0001f) direction = Vector2.up;
                direction.Normalize();

                Vector2 signPoint = poiPoint + direction * ApproachOffset;
                float ground = kit.experience != null
                    ? WorldExperienceBuilder.HeightAt(kit, signPoint.x, signPoint.y)
                    : poi.position.y;
                SignDestinationClass destination = DestinationClassFor(poi.type);

                plans.Add(new SignPlan
                {
                    name = "ShellSign_" + plans.Count + "_" + SafeName(poi),
                    recipeId = RecipeFor(poi.type),
                    destinationClass = destination,
                    position = new Vector3(signPoint.x, ground + HeightAboveTerrain, signPoint.y),
                    forward = new Vector3(direction.x, 0f, direction.y),
                    seed = StableSeed(kit.seed, poi.id, poi.type, plans.Count)
                });
            }

            return plans;
        }

        /// <summary>Idempotent structure builder separated for EditMode verification.</summary>
        public static int PlacePlans(
            Transform parent,
            IReadOnlyList<SignPlan> plans,
            Func<SignDestinationClass, Material> materialResolver)
        {
            if (parent == null) return 0;

            Transform existing = parent.Find(RootName);
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);

            var root = new GameObject(RootName).transform;
            root.SetParent(parent, false);
            root.gameObject.isStatic = true;

            int count = Mathf.Min(MaxSigns, plans != null ? plans.Count : 0);
            for (int i = 0; i < count; i++)
            {
                SignPlan plan = plans[i];
                Material material = materialResolver != null
                    ? materialResolver(plan.destinationClass)
                    : null;
                BuildSign(root, plan, material);
            }
            return count;
        }

        public static GameObject BuildSign(
            Transform parent,
            SignPlan plan,
            Material glyphMaterial)
        {
            var holder = new GameObject(string.IsNullOrEmpty(plan.name) ? "ShellSign" : plan.name);
            holder.transform.SetParent(parent, false);
            holder.transform.position = plan.position;
            Vector3 forward = plan.forward.sqrMagnitude > 0.0001f
                ? plan.forward.normalized
                : Vector3.forward;
            holder.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            holder.isStatic = true;

            Vector3 bodyScale = BodyScale(plan.recipeId);
            var fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fallback.name = "FallbackBody";
            fallback.transform.SetParent(holder.transform, false);
            fallback.transform.localScale = bodyScale;
            fallback.isStatic = true;
            Collider bodyCollider = fallback.GetComponent<Collider>();
            if (bodyCollider != null) UnityEngine.Object.DestroyImmediate(bodyCollider);
            Renderer bodyRenderer = fallback.GetComponent<Renderer>();
            if (bodyRenderer != null)
            {
                bodyRenderer.shadowCastingMode = ShadowCastingMode.Off;
                bodyRenderer.receiveShadows = false;
                bodyRenderer.lightProbeUsage = LightProbeUsage.Off;
                bodyRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }

            var look = holder.AddComponent<ForgeModuleLook>();
            look.recipeId = plan.recipeId;
            look.keepChildren = new[] { GlyphChildName };
            look.tintJitter = 0.025f;

            var glyph = GameObject.CreatePrimitive(PrimitiveType.Quad);
            glyph.name = GlyphChildName;
            glyph.transform.SetParent(holder.transform, false);
            glyph.transform.localPosition = new Vector3(0f, 0f, bodyScale.z * 0.52f + 0.004f);
            glyph.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            Vector2 glyphScale = GlyphScale(plan.recipeId);
            glyph.transform.localScale = new Vector3(glyphScale.x, glyphScale.y, 1f);
            glyph.isStatic = true;
            Collider glyphCollider = glyph.GetComponent<Collider>();
            if (glyphCollider != null) UnityEngine.Object.DestroyImmediate(glyphCollider);
            Renderer glyphRenderer = glyph.GetComponent<Renderer>();
            if (glyphRenderer != null)
            {
                glyphRenderer.sharedMaterial = glyphMaterial;
                glyphRenderer.shadowCastingMode = ShadowCastingMode.Off;
                glyphRenderer.receiveShadows = false;
                glyphRenderer.lightProbeUsage = LightProbeUsage.Off;
                glyphRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }

            return holder;
        }

        public static SignDestinationClass DestinationClassFor(PoiType type)
        {
            switch (type)
            {
                case PoiType.TravelBerth:
                    return SignDestinationClass.Travel;
                case PoiType.RuinCache:
                case PoiType.CaveSecret:
                    return SignDestinationClass.Vendor;
                default:
                    return SignDestinationClass.Job;
            }
        }

        public static string RecipeFor(PoiType type)
        {
            switch (DestinationClassFor(type))
            {
                case SignDestinationClass.Travel:
                    return SignRecipeLibrary.HangingRecipeId;
                case SignDestinationClass.Vendor:
                    return SignRecipeLibrary.ChevronRecipeId;
                default:
                    return SignRecipeLibrary.WallPlateRecipeId;
            }
        }

        public static int StableSeed(int worldSeed, string poiId, PoiType type, int index)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string value = poiId ?? string.Empty;
                for (int i = 0; i < value.Length; i++)
                {
                    hash ^= value[i];
                    hash *= 16777619u;
                }
                hash ^= (uint)worldSeed;
                hash *= 16777619u;
                hash ^= (uint)type;
                hash *= 16777619u;
                hash ^= (uint)index;
                return (int)(hash & 0x7FFFFFFF);
            }
        }

        private static Material ResolveMaterial(SignDestinationClass destination)
        {
            return AssetDatabase.LoadAssetAtPath<Material>(
                SignRecipeLibrary.GlyphMaterialPath(destination));
        }

        private static int ComparePois(PoiDef a, PoiDef b)
        {
            int priority = Priority(a.type).CompareTo(Priority(b.type));
            if (priority != 0) return priority;
            int id = string.CompareOrdinal(a.id ?? string.Empty, b.id ?? string.Empty);
            if (id != 0) return id;
            int x = a.position.x.CompareTo(b.position.x);
            return x != 0 ? x : a.position.z.CompareTo(b.position.z);
        }

        private static int Priority(PoiType type)
        {
            if (type == PoiType.TravelBerth) return 0;
            if (type == PoiType.StoryAnchor) return 1;
            if (DestinationClassFor(type) == SignDestinationClass.Job) return 2;
            return 3;
        }

        private static Vector2 NearestRoutePoint(IReadOnlyList<Vector2> route, Vector2 point)
        {
            if (route == null || route.Count == 0) return point + Vector2.up;
            Vector2 nearest = route[0];
            float best = (nearest - point).sqrMagnitude;
            for (int i = 1; i < route.Count; i++)
            {
                float distance = (route[i] - point).sqrMagnitude;
                if (distance >= best) continue;
                best = distance;
                nearest = route[i];
            }
            return nearest;
        }

        private static string SafeName(PoiDef poi)
        {
            if (poi != null && !string.IsNullOrEmpty(poi.id)) return poi.id;
            return poi != null ? poi.type.ToString() : "Unknown";
        }

        private static Vector3 BodyScale(string recipeId)
        {
            if (recipeId == SignRecipeLibrary.HangingRecipeId)
                return new Vector3(0.98f, 0.42f, 0.08f);
            if (recipeId == SignRecipeLibrary.ChevronRecipeId)
                return new Vector3(0.34f, 0.34f, 0.08f);
            return new Vector3(0.74f, 0.34f, 0.07f);
        }

        private static Vector2 GlyphScale(string recipeId)
        {
            if (recipeId == SignRecipeLibrary.HangingRecipeId) return new Vector2(0.72f, 0.25f);
            if (recipeId == SignRecipeLibrary.ChevronRecipeId) return new Vector2(0.21f, 0.21f);
            return new Vector2(0.55f, 0.21f);
        }
    }
}
#endif
