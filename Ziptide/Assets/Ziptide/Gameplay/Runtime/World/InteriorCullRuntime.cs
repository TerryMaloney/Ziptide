using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 1.3d / BUILDING_INTERIORS — interior proximity culling: an interior's renderers
    /// only draw when the player is close enough to possibly see them. The Quest-honest v1 of the
    /// portal/PVS finding (VR_TECHNIQUE_RESEARCH §2: per-frame occlusion is the CPU cost to avoid —
    /// so this checks on a slow cadence, not per frame): from across a district, walk-in tenements
    /// cost ZERO draw calls.
    ///
    /// Attached to each "Interior" root by InteriorBuilder at patch time; everything else happens at
    /// runtime in Start() (patch-time listener wiring never survives — the SalvageCache lesson).
    /// Colliders are NEVER culled — walls stay solid even when invisible, so physics/audio raycasts
    /// and a player glitching inside a far building all behave.
    /// </summary>
    public class InteriorCullRuntime : MonoBehaviour
    {
        [Tooltip("Extra visibility margin (m) beyond the interior's own bounds.")]
        public float margin = 10f;

        [Tooltip("1.3e — footprint-space room rects (filled by InteriorBuilder at patch time). " +
                 "When present, per-ROOM portal culling runs on top of the proximity cull: " +
                 "standing inside a room, only it and its corridor neighbors draw.")]
        public Rect[] roomRects;
        [Tooltip("1.3e — footprint-space corridor rects (the sight-lines between rooms).")]
        public Rect[] corridorRects;

        private const float CheckInterval = 0.5f; // slow cadence — the whole point vs per-frame

        private Renderer[] _renderers;
        private Transform _rig;
        private Vector3 _center;
        private float _radius;
        private float _nextCheck;
        private bool _visible = true;

        // 1.3e per-room portal state (only armed when roomRects were serialized).
        private InteriorPlan _plan;
        private Renderer[][] _roomRenderers;
        private readonly List<int> _visibleRooms = new List<int>();
        private int _lastRoom = -2; // -2 = force first apply; -1 = corridor/outside

        private void Start()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            if (_renderers == null || _renderers.Length == 0) { enabled = false; return; }

            // World-space bounds of the whole interior → one center + radius test.
            var b = _renderers[0].bounds;
            for (int i = 1; i < _renderers.Length; i++) b.Encapsulate(_renderers[i].bounds);
            _center = b.center;
            _radius = b.extents.magnitude + margin;

            // Stagger the first check so many interiors don't all test on the same frame.
            _nextCheck = Time.time + (GetInstanceID() & 7) * 0.07f;

            // 1.3e: rebuild the plan from the serialized rects and find the Room_<i> portal
            // groups InteriorFurnisher created — per-room culling arms only when both exist.
            if (roomRects != null && roomRects.Length > 0)
            {
                _plan = new InteriorPlan
                {
                    Rooms = new List<Rect>(roomRects),
                    Corridors = corridorRects != null
                        ? new List<Rect>(corridorRects) : new List<Rect>(),
                };
                _roomRenderers = new Renderer[roomRects.Length][];
                bool any = false;
                for (int i = 0; i < roomRects.Length; i++)
                {
                    var node = transform.Find("Room_" + i);
                    _roomRenderers[i] = node != null
                        ? node.GetComponentsInChildren<Renderer>(true) : new Renderer[0];
                    any |= _roomRenderers[i].Length > 0;
                }
                if (!any) _roomRenderers = null; // old bake without portal groups — proximity only
            }
        }

        private void Update()
        {
            if (Time.time < _nextCheck) return;
            _nextCheck = Time.time + CheckInterval;

            if (_rig == null)
            {
                var rig = Object.FindObjectOfType<PlayerRigPersistence>();
                if (rig == null) return;
                _rig = rig.transform;
            }

            bool near = (_rig.position - _center).sqrMagnitude <= _radius * _radius;
            if (near != _visible)
            {
                _visible = near;
                for (int i = 0; i < _renderers.Length; i++)
                    if (_renderers[i] != null) _renderers[i].enabled = near;
                _lastRoom = -2; // proximity stomped per-room state — force a portal re-apply
            }

            // 1.3e per-room portal pass: walls/shell stay drawn (direct children), but each
            // Room_<i> group only draws when the plan says you could see it from where you stand.
            if (near && _roomRenderers != null)
            {
                Vector3 local = transform.InverseTransformPoint(_rig.position);
                int room = InteriorVisibilityCore.RoomIndexAt(_plan, new Vector2(local.x, local.z));
                if (room == _lastRoom) return;
                _lastRoom = room;
                InteriorVisibilityCore.VisibleRooms(_plan, room, _visibleRooms);
                for (int i = 0; i < _roomRenderers.Length; i++)
                {
                    bool vis = _visibleRooms.Contains(i);
                    var group = _roomRenderers[i];
                    for (int k = 0; k < group.Length; k++)
                        if (group[k] != null) group[k].enabled = vis;
                }
            }
        }
    }
}
