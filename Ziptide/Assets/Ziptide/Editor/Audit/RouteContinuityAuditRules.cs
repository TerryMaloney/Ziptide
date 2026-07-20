#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// PG-3: samples authored route surfaces in world space. Presence of a Connection object is not enough;
    /// its traversable centerline and each player spawn must resolve to enabled solid support.
    /// </summary>
    public static class RouteContinuityAuditRules
    {
        public const string RouteGap = "ROUTE_SURFACE_GAP_SAMPLED";
        public const string SpawnUnsupported = "ROUTE_SPAWN_UNSUPPORTED";
        public const float DefaultSampleStep = 0.45f;

        public static void Run(SceneAuditReport report)
        {
            Physics.SyncTransforms();
            foreach (Transform root in Object.FindObjectsOfType<Transform>())
            {
                if (root == null || root.name != "Connections") continue;
                for (int i = 0; i < root.childCount; i++)
                {
                    Transform route = root.GetChild(i);
                    Collider collider = route != null ? route.GetComponent<Collider>() : null;
                    Renderer renderer = route != null ? route.GetComponent<Renderer>() : null;
                    if (collider == null || renderer == null || !collider.enabled || collider.isTrigger) continue;

                    Bounds bounds = collider.bounds;
                    bool alongX = bounds.size.x >= bounds.size.z;
                    float half = (alongX ? bounds.extents.x : bounds.extents.z) - 0.08f;
                    Vector3 axis = alongX ? Vector3.right : Vector3.forward;
                    Vector3 center = new Vector3(bounds.center.x, bounds.max.y + 0.30f, bounds.center.z);
                    Vector3 start = center - axis * Mathf.Max(0f, half);
                    Vector3 end = center + axis * Mathf.Max(0f, half);
                    int unsupported = CountUnsupportedSamples(start, end, DefaultSampleStep, 0.05f, 1.20f);
                    if (unsupported > 0)
                        report.Blocker(RouteGap,
                            GetPath(route) + " has " + unsupported
                            + " unsupported centerline sample(s); the visible connection contains a fall-through gap.",
                            GetPath(route));
                }
            }

            foreach (SpawnMarkerRuntime spawn in Object.FindObjectsOfType<SpawnMarkerRuntime>(true))
            {
                if (spawn == null) continue;
                Vector3 origin = spawn.transform.position + Vector3.up * 0.25f;
                if (!HasSolidSupport(origin, 1.60f))
                    report.Blocker(SpawnUnsupported,
                        GetPath(spawn.transform) + " has no sampled solid support below the player spawn.",
                        GetPath(spawn.transform));
            }
        }

        public static int CountUnsupportedSamples(Vector3 start, Vector3 end,
            float sampleStep, float originLift, float rayDistance)
        {
            float distance = Vector3.Distance(start, end);
            int intervals = Mathf.Max(1, Mathf.CeilToInt(distance / Mathf.Max(0.05f, sampleStep)));
            int unsupported = 0;
            for (int i = 0; i <= intervals; i++)
            {
                float t = i / (float)intervals;
                Vector3 point = Vector3.Lerp(start, end, t) + Vector3.up * originLift;
                if (!HasSolidSupport(point, rayDistance)) unsupported++;
            }
            return unsupported;
        }

        public static bool HasSolidSupport(Vector3 origin, float rayDistance)
        {
            RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down,
                Mathf.Max(0.10f, rayDistance), ~0, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider collider = hits[i].collider;
                if (collider == null || !collider.enabled || collider.isTrigger) continue;
                if (hits[i].normal.y < 0.35f) continue;
                return true;
            }
            return false;
        }

        private static string GetPath(Transform value)
        {
            if (value == null) return string.Empty;
            var parts = new List<string>();
            while (value != null)
            {
                parts.Insert(0, value.name);
                value = value.parent;
            }
            return string.Join("/", parts);
        }
    }
}
#endif
