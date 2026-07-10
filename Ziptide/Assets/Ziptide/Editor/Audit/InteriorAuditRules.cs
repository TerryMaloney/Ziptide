#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// HARDWIRING 1.3e ③ — the interior gate (EXCELLENCE_MAP gap #4, per non-boot scene). Runs on
    /// every baked "Interior" root (an <see cref="InteriorCullRuntime"/>); pre-1.3e bakes (no
    /// portal data at all) are exempt — the runbook drives their re-bake, not a red build.
    ///  - INTERIOR_DISCONNECTED (blocker): the serialized plan no longer forms one walkable
    ///    component — a hand-edit or patcher collision broke the access rule.
    ///  - INTERIOR_BARE_ROOM (blocker): a Room_&lt;i&gt; portal group with fewer than 2 children
    ///    (its light + at least one furnishing) — RoomFurnishCore guarantees this at build time,
    ///    so a bare room in a BAKED scene is a regression.
    ///  - INTERIOR_PORTAL_HALF_ARMED (warning): rects without Room_ groups or groups without
    ///    rects — portal culling silently off; re-bake the world.
    ///  - INTERIOR_FURNITURE_IN_CORRIDOR (warning): a furnishing's collider footprint overlaps a
    ///    corridor rect beyond tolerance — a doorway may be blocked (AABB check, so warn not block).
    ///  - INTERIOR_OVER_BUDGET (warning): one interior carries a suspicious renderer count; the
    ///    district blocker (BuildingAuditRules) still owns the hard cap — this localizes the blame.
    /// </summary>
    public static class InteriorAuditRules
    {
        private const int RendererSoftCap = 260;    // per interior; the district cap is the law
        private const float CorridorTolerance = 0.05f;

        public static void Run(SceneAuditReport report)
        {
            foreach (var cull in Object.FindObjectsOfType<InteriorCullRuntime>(true))
            {
                bool hasRects = cull.roomRects != null && cull.roomRects.Length > 0;
                Transform root = cull.transform;

                var roomNodes = new List<Transform>();
                for (int i = 0; ; i++)
                {
                    var node = root.Find("Room_" + i);
                    if (node == null) break;
                    roomNodes.Add(node);
                }

                if (!hasRects && roomNodes.Count == 0) continue; // pre-1.3e bake — exempt

                string where = Path(root);
                if (!hasRects || roomNodes.Count == 0)
                {
                    report.Warning("INTERIOR_PORTAL_HALF_ARMED",
                        where + " has " + (hasRects ? "plan rects but no Room_ groups"
                                                    : "Room_ groups but no plan rects") +
                        " — per-room portal culling is silently off. Re-bake this world.", where);
                }

                if (hasRects)
                {
                    var plan = new InteriorPlan
                    {
                        Rooms = new List<Rect>(cull.roomRects),
                        Corridors = cull.corridorRects != null
                            ? new List<Rect>(cull.corridorRects) : new List<Rect>(),
                    };
                    if (!RoomPartitioner.IsFullyConnected(plan))
                        report.Blocker("INTERIOR_DISCONNECTED",
                            where + ": the serialized room plan is no longer one walkable " +
                            "component — the access rule is broken. Re-bake; if it persists, a " +
                            "patcher is editing the plan after InteriorBuilder.", where);

                    // Furniture vs corridor mouths (footprint-space AABB approximation).
                    for (int i = 0; i < roomNodes.Count; i++)
                        foreach (var box in roomNodes[i].GetComponentsInChildren<BoxCollider>(true))
                        {
                            Vector3 c = root.InverseTransformPoint(box.transform.TransformPoint(box.center));
                            Vector3 half = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);
                            var foot = new Rect(c.x - half.x, c.z - half.z, half.x * 2f, half.z * 2f);
                            foreach (var corridor in plan.Corridors)
                            {
                                var shrunk = new Rect(corridor.x + CorridorTolerance,
                                    corridor.y + CorridorTolerance,
                                    corridor.width - 2f * CorridorTolerance,
                                    corridor.height - 2f * CorridorTolerance);
                                if (shrunk.width > 0f && shrunk.height > 0f && foot.Overlaps(shrunk))
                                {
                                    report.Warning("INTERIOR_FURNITURE_IN_CORRIDOR",
                                        where + "/Room_" + i + ": '" + box.name +
                                        "' overlaps a corridor — a doorway may be blocked. " +
                                        "RoomFurnishCore can't produce this; something moved.", where);
                                    break;
                                }
                            }
                        }
                }

                for (int i = 0; i < roomNodes.Count; i++)
                    if (roomNodes[i].childCount < 2)
                        report.Blocker("INTERIOR_BARE_ROOM",
                            where + "/Room_" + i + " has " + roomNodes[i].childCount +
                            " children — every room ships its light AND at least one furnishing " +
                            "(the bare-room guarantee). Re-bake this world.", where);

                int renderers = root.GetComponentsInChildren<Renderer>(true).Length;
                if (renderers > RendererSoftCap)
                    report.Warning("INTERIOR_OVER_BUDGET",
                        where + " carries " + renderers + " renderers (soft cap " + RendererSoftCap +
                        "). The district budget still gates the build; trim the furnish catalog " +
                        "or the room count if this district goes red.", where);
            }
        }

        private static string Path(Transform t)
        {
            string p = t.name;
            while (t.parent != null) { t = t.parent; p = t.name + "/" + p; }
            return p;
        }
    }
}
#endif
