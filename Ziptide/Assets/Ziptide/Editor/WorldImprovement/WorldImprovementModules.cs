#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Ziptide.Content;

namespace Ziptide.Editor.WorldImprovement
{
    public sealed class WorldImprovementModuleResult
    {
        public int ObjectCount;
        public string Summary;
    }

    public interface IWorldImprovementModule
    {
        string Id { get; }
        int CurrentVersion { get; }
        WorldImprovementModuleResult Apply(WorldImprovementContext context,
            WorldImprovementModuleSpec spec, Transform moduleRoot);
    }

    public sealed class WorldImprovementContext
    {
        private const string MaterialFolder = "Assets/Ziptide/Generated/WorldImprovement/Materials";
        private readonly Dictionary<Color, Material> _materials = new Dictionary<Color, Material>();

        public string SceneName { get; internal set; }
        public string ScenePath { get; internal set; }
        public Transform Root { get; internal set; }
        public Transform Spawn { get; internal set; }
        public WorldImprovementManifest Manifest { get; internal set; }
        public Color Primary { get; internal set; }
        public Color Accent { get; internal set; }
        public Color Glow { get; internal set; }

        /// <summary>
        /// Resolve a durable material asset keyed by exact RGBA. Generated scenes must never reference a
        /// transient in-memory Material: the editor, CI checkout and APK must all reopen the same object.
        /// </summary>
        public Material Material(Color color)
        {
            if (_materials.TryGetValue(color, out Material cached) && cached != null) return cached;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return null;

            EnsureAssetFolder(MaterialFolder);
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            string path = MaterialFolder + "/WIM_" + hex + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(shader) { name = "WIM_" + hex };
                AssetDatabase.CreateAsset(material, path);
            }
            else if (material.shader != shader)
            {
                material.shader = shader;
            }

            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_EmissionColor") && color.maxColorComponent > 0.55f)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.3f);
            }
            else if (material.HasProperty("_EmissionColor"))
            {
                material.DisableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.black);
            }
            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssetIfDirty(material);
            _materials[color] = material;
            return material;
        }

        public GameObject Part(Transform parent, string name, PrimitiveType primitive,
            Vector3 localPosition, Vector3 localScale, Vector3 localEuler, Color color)
        {
            var go = GameObject.CreatePrimitive(primitive);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.Euler(localEuler);
            go.transform.localScale = localScale;
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = Material(color);
                if (material != null) renderer.sharedMaterial = material;
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
            return go;
        }

        public bool TryGround(Vector3 candidate, out Vector3 grounded)
        {
            RaycastHit[] hits = Physics.RaycastAll(candidate + Vector3.up * 20f, Vector3.down, 60f,
                ~0, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider collider = hits[i].collider;
                if (collider == null || !collider.enabled || collider.isTrigger) continue;
                if (collider.transform.IsChildOf(Root)) continue;
                if (hits[i].normal.y < 0.35f) continue;
                grounded = new Vector3(candidate.x, hits[i].point.y, candidate.z);
                return true;
            }
            grounded = candidate;
            return false;
        }

        public List<Transform> FindNavigationAnchors()
        {
            var anchors = new List<Transform>();
            if (Spawn != null) anchors.Add(Spawn);
            foreach (Transform t in UnityEngine.Object.FindObjectsOfType<Transform>(true))
            {
                if (t == null || t == Spawn || t.IsChildOf(Root)) continue;
                string n = t.name;
                bool match = n.StartsWith("Marker_", StringComparison.OrdinalIgnoreCase)
                    || n.StartsWith("poi_", StringComparison.OrdinalIgnoreCase)
                    || n.StartsWith("Dest_", StringComparison.OrdinalIgnoreCase)
                    || n.IndexOf("BoardPanel", StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("TravelButton", StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("MouthTrigger", StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("Disembark", StringComparison.OrdinalIgnoreCase) >= 0;
                if (!match) continue;
                bool duplicate = false;
                for (int i = 0; i < anchors.Count; i++)
                    if ((anchors[i].position - t.position).sqrMagnitude < 4f) { duplicate = true; break; }
                if (!duplicate) anchors.Add(t);
            }
            return anchors;
        }

        private static void EnsureAssetFolder(string folder)
        {
            string normalized = folder.Replace('\\', '/').Trim('/');
            string[] parts = normalized.Split('/');
            if (parts.Length == 0 || parts[0] != "Assets")
                throw new InvalidOperationException("Generated material folder must live under Assets: " + folder);
            string current = "Assets";
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }

    public static class WorldImprovementModuleRegistry
    {
        private static readonly Dictionary<string, IWorldImprovementModule> Modules =
            new Dictionary<string, IWorldImprovementModule>(StringComparer.Ordinal)
            {
                { "arrival_identity", new ArrivalIdentityModule() },
                { "route_beacons", new RouteBeaconModule() },
                { "ambient_motion", new AmbientMotionModule() },
                { "horizon_frame", new HorizonFrameModule() },
                { "grounded_route", new GroundedRouteModule() },
                { "discovery_nodes", new DiscoveryNodeModule() },
                { "story_traces", new StoryTraceModule() },
            };

        public static IWorldImprovementModule Resolve(string id)
            => !string.IsNullOrEmpty(id) && Modules.TryGetValue(id, out IWorldImprovementModule module)
                ? module : null;

        public static string[] KnownIds()
        {
            var ids = new string[Modules.Count];
            Modules.Keys.CopyTo(ids, 0);
            Array.Sort(ids, StringComparer.Ordinal);
            return ids;
        }
    }
}
#endif
