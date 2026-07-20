#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.WorldImprovement
{
    /// <summary>
    /// Adds collider-free grounded route traces between arrival and existing authored anchors. Every
    /// marker is independently support-probed; a missing sample terminates that trace rather than drawing
    /// guidance over void, water or an unbuilt connection.
    /// </summary>
    internal sealed class GroundedRouteModule : IWorldImprovementModule
    {
        public string Id => "grounded_route";
        public int CurrentVersion => 1;

        public WorldImprovementModuleResult Apply(WorldImprovementContext context,
            WorldImprovementModuleSpec spec, Transform moduleRoot)
        {
            Vector3 origin = context.Spawn != null ? context.Spawn.position : Vector3.zero;
            List<Transform> anchors = context.FindNavigationAnchors();
            int budget = Mathf.Max(1, spec.budget);
            int count = 0;

            for (int a = 0; a < anchors.Count && count < budget; a++)
            {
                Transform anchor = anchors[a];
                if (anchor == null || (anchor.position - origin).sqrMagnitude < 4f) continue;
                Vector3 delta = anchor.position - origin;
                delta.y = 0f;
                float distance = delta.magnitude;
                if (distance < 2f) continue;

                int samples = Mathf.Clamp(Mathf.CeilToInt(distance / 2.4f), 2, 18);
                float previousY = origin.y;
                for (int i = 1; i < samples && count < budget; i++)
                {
                    float t = i / (float)samples;
                    Vector3 candidate = Vector3.Lerp(origin, anchor.position, t);
                    if (!context.TryGround(candidate, out Vector3 grounded)) break;
                    if (i > 1 && Mathf.Abs(grounded.y - previousY) > 1.35f) break;
                    previousY = grounded.y;

                    GameObject marker = context.Part(moduleRoot, "RouteTrace_" + a + "_" + i,
                        PrimitiveType.Cylinder, grounded + Vector3.up * 0.025f,
                        new Vector3(0.13f, 0.018f, 0.13f), Vector3.zero,
                        i % 3 == 0 ? context.Accent : context.Glow);
                    marker.transform.rotation = Quaternion.identity;
                    count++;
                }
            }

            // A sparse world may expose no named anchor. Preserve a useful, deterministic arrival cue
            // without inventing a destination or leaving a required module empty.
            if (count == 0)
            {
                int fallback = Mathf.Min(6, budget);
                for (int i = 0; i < fallback; i++)
                {
                    float angle = i * Mathf.PI * 2f / fallback;
                    Vector3 candidate = origin + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 2.2f;
                    if (!context.TryGround(candidate, out Vector3 grounded)) continue;
                    context.Part(moduleRoot, "ArrivalTrace_" + i, PrimitiveType.Cylinder,
                        grounded + Vector3.up * 0.025f, new Vector3(0.13f, 0.018f, 0.13f),
                        Vector3.zero, context.Glow);
                    count++;
                }
            }

            return new WorldImprovementModuleResult
            {
                ObjectCount = count,
                Summary = count + " continuously grounded route traces",
            };
        }
    }

    /// <summary>
    /// Places bounded one-shot XR discovery interactions near existing navigation anchors. These are
    /// deliberately self-contained: no economy, save, inventory or story-flag authority is duplicated.
    /// </summary>
    internal sealed class DiscoveryNodeModule : IWorldImprovementModule
    {
        public string Id => "discovery_nodes";
        public int CurrentVersion => 1;

        public WorldImprovementModuleResult Apply(WorldImprovementContext context,
            WorldImprovementModuleSpec spec, Transform moduleRoot)
        {
            Vector3 origin = context.Spawn != null ? context.Spawn.position : Vector3.zero;
            List<Transform> anchors = context.FindNavigationAnchors();
            int nodeBudget = Mathf.Clamp(spec.budget / 4, 1, 4);
            int count = 0;
            int nodeIndex = 0;

            for (int i = 0; i < anchors.Count && nodeIndex < nodeBudget; i++)
            {
                Transform anchor = anchors[i];
                if (anchor == null) continue;
                Vector3 away = anchor.position - origin;
                away.y = 0f;
                Vector3 lateral = away.sqrMagnitude > 0.1f
                    ? Vector3.Cross(Vector3.up, away.normalized) : Vector3.right;
                Vector3 candidate = anchor.position + lateral * (1.15f + 0.15f * (nodeIndex % 2));
                if (!context.TryGround(candidate, out Vector3 grounded)) continue;
                count += BuildNode(context, moduleRoot, grounded, nodeIndex);
                nodeIndex++;
            }

            if (nodeIndex == 0)
            {
                Vector3 forward = context.Spawn != null ? context.Spawn.forward : Vector3.forward;
                Vector3 right = context.Spawn != null ? context.Spawn.right : Vector3.right;
                Vector3[] offsets =
                {
                    forward * 4.2f + right * 2.2f,
                    forward * 5.0f - right * 2.4f,
                };
                for (int i = 0; i < offsets.Length && nodeIndex < nodeBudget; i++)
                {
                    if (!context.TryGround(origin + offsets[i], out Vector3 grounded)) continue;
                    count += BuildNode(context, moduleRoot, grounded, nodeIndex);
                    nodeIndex++;
                }
            }

            return new WorldImprovementModuleResult
            {
                ObjectCount = count,
                Summary = nodeIndex + " reachable discovery nodes",
            };
        }

        private static int BuildNode(WorldImprovementContext context, Transform parent,
            Vector3 grounded, int index)
        {
            Transform node = new GameObject("DiscoveryNode_" + index).transform;
            node.SetParent(parent, false);
            node.position = grounded;

            context.Part(node, "Pedestal", PrimitiveType.Cylinder,
                new Vector3(0f, 0.36f, 0f), new Vector3(0.38f, 0.36f, 0.38f),
                Vector3.zero, context.Primary);
            context.Part(node, "PedestalRing", PrimitiveType.Cylinder,
                new Vector3(0f, 0.73f, 0f), new Vector3(0.46f, 0.055f, 0.46f),
                Vector3.zero, context.Accent);

            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            core.name = "DiscoveryCore";
            core.transform.SetParent(node, false);
            core.transform.localPosition = new Vector3(0f, 1.03f, 0f);
            core.transform.localScale = Vector3.one * 0.34f;
            Renderer renderer = core.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = context.Material(context.Glow);
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
            Collider collider = core.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
                collider.isTrigger = false;
            }
            WorldDiscoveryNodeRuntime runtime = core.AddComponent<WorldDiscoveryNodeRuntime>();
            runtime.Configure(context.SceneName + "-discovery-" + index, context.Glow, context.Accent);

            GameObject labelObject = new GameObject("DiscoveryStatus");
            labelObject.transform.SetParent(core.transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 1.30f, 0f);
            labelObject.transform.localScale = Vector3.one * 2.94f;
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = "TOUCH TO LOG";
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.025f;
            label.fontSize = 42;
            label.color = context.Glow;

            return 4;
        }
    }

    /// <summary>
    /// Adds small deterministic environmental traces that imply prior activity without claiming story,
    /// reward or quest authority. This gives every world a reusable narrative-density floor while biome-
    /// specific Lore Forge content remains manifest-driven and replaceable later.
    /// </summary>
    internal sealed class StoryTraceModule : IWorldImprovementModule
    {
        public string Id => "story_traces";
        public int CurrentVersion => 1;

        public WorldImprovementModuleResult Apply(WorldImprovementContext context,
            WorldImprovementModuleSpec spec, Transform moduleRoot)
        {
            Vector3 origin = context.Spawn != null ? context.Spawn.position : Vector3.zero;
            int clusters = Mathf.Clamp(spec.budget / 4, 1, 4);
            var random = new System.Random(context.Manifest.seed + spec.seedOffset + 31337);
            int count = 0;

            for (int i = 0; i < clusters; i++)
            {
                float angle = (i / (float)clusters) * Mathf.PI * 2f
                    + (float)random.NextDouble() * 0.35f;
                float radius = 7f + (float)random.NextDouble() * 5f;
                Vector3 candidate = origin + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
                if (!context.TryGround(candidate, out Vector3 grounded)) continue;

                Transform trace = new GameObject("StoryTrace_" + i).transform;
                trace.SetParent(moduleRoot, false);
                trace.position = grounded;
                trace.rotation = Quaternion.Euler(0f, (float)random.NextDouble() * 360f, 0f);

                context.Part(trace, "AbandonedCase", PrimitiveType.Cube,
                    new Vector3(-0.28f, 0.18f, 0f), new Vector3(0.55f, 0.32f, 0.38f),
                    new Vector3(0f, 12f, -4f), context.Primary);
                context.Part(trace, "FieldCanister", PrimitiveType.Cylinder,
                    new Vector3(0.36f, 0.24f, 0.10f), new Vector3(0.16f, 0.24f, 0.16f),
                    new Vector3(0f, 0f, 8f), context.Accent);
                context.Part(trace, "BrokenAntenna", PrimitiveType.Cube,
                    new Vector3(0.05f, 0.52f, -0.18f), new Vector3(0.045f, 0.62f, 0.045f),
                    new Vector3(0f, 0f, 28f), context.Primary * 0.72f);
                context.Part(trace, "ResidualSignal", PrimitiveType.Sphere,
                    new Vector3(0.30f, 0.62f, -0.10f), Vector3.one * 0.10f,
                    Vector3.zero, context.Glow);
                count += 4;
            }

            if (count == 0 && context.TryGround(origin + Vector3.forward * 6f, out Vector3 fallback))
            {
                Transform trace = new GameObject("StoryTrace_Fallback").transform;
                trace.SetParent(moduleRoot, false);
                trace.position = fallback;
                context.Part(trace, "AbandonedCase", PrimitiveType.Cube,
                    new Vector3(0f, 0.18f, 0f), new Vector3(0.55f, 0.32f, 0.38f),
                    Vector3.zero, context.Primary);
                context.Part(trace, "FieldCanister", PrimitiveType.Cylinder,
                    new Vector3(0.42f, 0.24f, 0f), new Vector3(0.16f, 0.24f, 0.16f),
                    Vector3.zero, context.Accent);
                context.Part(trace, "BrokenAntenna", PrimitiveType.Cube,
                    new Vector3(-0.28f, 0.50f, 0f), new Vector3(0.045f, 0.60f, 0.045f),
                    new Vector3(0f, 0f, 24f), context.Primary * 0.72f);
                context.Part(trace, "ResidualSignal", PrimitiveType.Sphere,
                    new Vector3(-0.08f, 0.62f, 0f), Vector3.one * 0.10f,
                    Vector3.zero, context.Glow);
                count = 4;
            }

            return new WorldImprovementModuleResult
            {
                ObjectCount = count,
                Summary = (count / 4) + " environmental story traces",
            };
        }
    }
}
#endif
