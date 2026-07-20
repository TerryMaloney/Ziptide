#if UNITY_EDITOR
using System;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.WorldImprovement
{
    internal sealed class ArrivalIdentityModule : IWorldImprovementModule
    {
        public string Id => "arrival_identity";
        public int CurrentVersion => 1;

        public WorldImprovementModuleResult Apply(WorldImprovementContext context,
            WorldImprovementModuleSpec spec, Transform moduleRoot)
        {
            Transform spawn = context.Spawn;
            Vector3 forward = spawn != null ? spawn.forward : Vector3.forward;
            Vector3 right = spawn != null ? spawn.right : Vector3.right;
            Vector3 origin = spawn != null ? spawn.position : Vector3.zero;
            Vector3 candidate = origin + forward * 5.5f + right * 3.0f;
            if (!context.TryGround(candidate, out Vector3 grounded)) grounded = origin;
            moduleRoot.position = grounded;
            Vector3 look = origin - grounded;
            look.y = 0f;
            if (look.sqrMagnitude > 0.01f) moduleRoot.rotation = Quaternion.LookRotation(look);

            float scale = Mathf.Lerp(0.85f, 1.25f, Mathf.Clamp01(spec.intensity * 0.5f));
            context.Part(moduleRoot, "IdentityPylon_L", PrimitiveType.Cylinder,
                new Vector3(-0.95f * scale, 1.35f * scale, 0f),
                new Vector3(0.18f * scale, 1.35f * scale, 0.18f * scale), Vector3.zero, context.Primary);
            context.Part(moduleRoot, "IdentityPylon_R", PrimitiveType.Cylinder,
                new Vector3(0.95f * scale, 1.35f * scale, 0f),
                new Vector3(0.18f * scale, 1.35f * scale, 0.18f * scale), Vector3.zero, context.Primary);
            context.Part(moduleRoot, "IdentityLintel", PrimitiveType.Cube,
                new Vector3(0f, 2.55f * scale, 0f),
                new Vector3(2.25f * scale, 0.18f * scale, 0.22f * scale), Vector3.zero, context.Accent);
            context.Part(moduleRoot, "IdentityGlow", PrimitiveType.Sphere,
                new Vector3(0f, 2.92f * scale, 0f), Vector3.one * 0.22f * scale,
                Vector3.zero, context.Glow);

            var labelObject = new GameObject("IdentityLabel");
            labelObject.transform.SetParent(moduleRoot, false);
            labelObject.transform.localPosition = new Vector3(0f, 2.20f * scale, -0.13f);
            var label = labelObject.AddComponent<TextMesh>();
            label.text = string.IsNullOrWhiteSpace(context.Manifest.displayName)
                ? context.SceneName.Replace('_', ' ') : context.Manifest.displayName;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.035f * scale;
            label.fontSize = 42;
            label.color = context.Glow;

            return new WorldImprovementModuleResult
            {
                ObjectCount = 5,
                Summary = "arrival identity landmark at " + grounded.ToString("F1"),
            };
        }
    }

    internal sealed class RouteBeaconModule : IWorldImprovementModule
    {
        public string Id => "route_beacons";
        public int CurrentVersion => 1;

        public WorldImprovementModuleResult Apply(WorldImprovementContext context,
            WorldImprovementModuleSpec spec, Transform moduleRoot)
        {
            var anchors = context.FindNavigationAnchors();
            int maxBeacons = Mathf.Clamp(spec.budget / 3, 1, 16);
            int count = 0;
            Vector3 spawn = context.Spawn != null ? context.Spawn.position : Vector3.zero;
            for (int i = 0; i < anchors.Count && count / 3 < maxBeacons; i++)
            {
                Transform anchor = anchors[i];
                Vector3 radial = anchor.position - spawn;
                radial.y = 0f;
                Vector3 lateral = radial.sqrMagnitude > 0.1f
                    ? Vector3.Cross(Vector3.up, radial.normalized) : Vector3.right;
                Vector3 candidate = anchor.position + lateral * 0.70f;
                if (!context.TryGround(candidate, out Vector3 grounded)) continue;

                var beacon = new GameObject("RouteBeacon_" + i).transform;
                beacon.SetParent(moduleRoot, false);
                beacon.position = grounded;
                float height = 0.75f + 0.15f * Mathf.Clamp(spec.intensity, 0f, 2f);
                context.Part(beacon, "Post", PrimitiveType.Cube,
                    new Vector3(0f, height * 0.5f, 0f), new Vector3(0.07f, height, 0.07f),
                    Vector3.zero, context.Primary);
                context.Part(beacon, "Cap", PrimitiveType.Cylinder,
                    new Vector3(0f, height + 0.06f, 0f), new Vector3(0.16f, 0.05f, 0.16f),
                    Vector3.zero, context.Accent);
                context.Part(beacon, "Glow", PrimitiveType.Sphere,
                    new Vector3(0f, height + 0.17f, 0f), Vector3.one * 0.10f,
                    Vector3.zero, context.Glow);
                count += 3;
            }

            return new WorldImprovementModuleResult
            {
                ObjectCount = count,
                Summary = (count / 3) + " route beacons",
            };
        }
    }

    internal sealed class AmbientMotionModule : IWorldImprovementModule
    {
        public string Id => "ambient_motion";
        public int CurrentVersion => 1;

        public WorldImprovementModuleResult Apply(WorldImprovementContext context,
            WorldImprovementModuleSpec spec, Transform moduleRoot)
        {
            Vector3 origin = context.Spawn != null ? context.Spawn.position : Vector3.zero;
            moduleRoot.position = origin;
            int particles = Mathf.Clamp(spec.budget, 4, 12);
            float radius = 3.5f + Mathf.Clamp(spec.intensity, 0f, 2f) * 2.0f;
            for (int i = 0; i < particles; i++)
            {
                float angle = i * Mathf.PI * 2f / particles;
                float y = 0.9f + (i % 3) * 0.45f;
                context.Part(moduleRoot, "AmbientMote_" + i, PrimitiveType.Sphere,
                    new Vector3(Mathf.Cos(angle) * radius, y, Mathf.Sin(angle) * radius),
                    Vector3.one * (0.055f + (i % 2) * 0.025f), Vector3.zero, context.Glow);
            }
            var runtime = moduleRoot.gameObject.AddComponent<WorldAmbientMotionRuntime>();
            runtime.Configure(0.08f + spec.intensity * 0.035f, 0.55f + spec.intensity * 0.20f,
                0.05f, context.Manifest.seed + spec.seedOffset);

            return new WorldImprovementModuleResult
            {
                ObjectCount = particles,
                Summary = particles + " deterministic ambient motes",
            };
        }
    }

    internal sealed class HorizonFrameModule : IWorldImprovementModule
    {
        public string Id => "horizon_frame";
        public int CurrentVersion => 1;

        public WorldImprovementModuleResult Apply(WorldImprovementContext context,
            WorldImprovementModuleSpec spec, Transform moduleRoot)
        {
            Vector3 origin = context.Spawn != null ? context.Spawn.position : Vector3.zero;
            int towers = Mathf.Clamp(spec.budget / 2, 4, 10);
            float radius = 28f + Mathf.Clamp(spec.intensity, 0f, 2f) * 7f;
            var random = new System.Random(context.Manifest.seed + spec.seedOffset + 991);
            int count = 0;
            for (int i = 0; i < towers; i++)
            {
                float angle = (i / (float)towers) * Mathf.PI * 2f + (float)random.NextDouble() * 0.22f;
                Vector3 candidate = origin + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
                if (!context.TryGround(candidate, out Vector3 grounded)) grounded = candidate - Vector3.up * 2f;
                float height = 5.5f + (float)random.NextDouble() * 7.5f;
                var tower = new GameObject("HorizonLandmark_" + i).transform;
                tower.SetParent(moduleRoot, false);
                tower.position = grounded;
                tower.rotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);
                context.Part(tower, "Mass", PrimitiveType.Cube,
                    new Vector3(0f, height * 0.5f, 0f),
                    new Vector3(1.2f + (i % 3) * 0.35f, height, 1.0f),
                    new Vector3(0f, 0f, (i % 2 == 0 ? 2.5f : -2.5f)), context.Primary * 0.65f);
                context.Part(tower, "Crown", PrimitiveType.Cylinder,
                    new Vector3(0f, height + 0.35f, 0f),
                    new Vector3(0.45f, 0.32f, 0.45f), Vector3.zero,
                    i % 2 == 0 ? context.Accent : context.Glow);
                count += 2;
            }
            return new WorldImprovementModuleResult
            {
                ObjectCount = count,
                Summary = towers + " horizon silhouettes",
            };
        }
    }
}
#endif
