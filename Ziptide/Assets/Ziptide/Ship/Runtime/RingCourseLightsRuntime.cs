using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Ship
{
    /// <summary>
    /// Applies <see cref="RingLampChaseCore"/> to the authored ring course: finds Ring_&lt;i&gt;
    /// children under the lane root, reads the flight's next-ring index, and paints every
    /// segment through MaterialPropertyBlocks (no material instantiation on Quest). Full
    /// repaint only when the next ring changes; per-frame animation touches only the ONE
    /// chasing ring. Serialized by ScenePatcherSpaceLane. Logs RING_LIGHTS on transitions.
    /// </summary>
    public class RingCourseLightsRuntime : MonoBehaviour
    {
        [Tooltip("The flight runtime whose course progress drives the lamps.")]
        [SerializeField] private ShipFlightRuntime flight;
        [Tooltip("Root the patcher parented the Ring_<i> objects under (the lane content).")]
        [SerializeField] private Transform lane;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private readonly List<Renderer[]> _rings = new List<Renderer[]>();
        private MaterialPropertyBlock _block;
        private int _lastNext = int.MinValue;
        private bool _saidDeadRing;

        private void Start()
        {
            _block = new MaterialPropertyBlock();
            if (lane == null) lane = transform;
            for (int i = 0; ; i++)
            {
                var ring = lane.Find("Ring_" + i);
                if (ring == null) break;

                // ONLY THE SEQUENCER LAMPS ANIMATE (docs/design/THE_CATCH.md §8). This used to grab
                // every renderer in the ring and repaint the whole structure green, which is most of
                // why a catch ring read as a toy hoop: a machine does not change colour, its
                // indicator lights do. A ring with no Lamps child — CATCH 3, whose sequencer is dead
                // — therefore stays dark forever, which is how the player learns the Catch is
                // failing without anybody saying it.
                var lamps = ring.Find("Lamps");
                _rings.Add(lamps != null
                    ? lamps.GetComponentsInChildren<Renderer>()
                    : System.Array.Empty<Renderer>());
            }
            if (_rings.Count == 0)
                Debug.Log("ZIPTIDE: RING_LIGHTS none (no Ring_<i> children under " + lane.name + ")");
        }

        private void LateUpdate()
        {
            if (_rings.Count == 0) return;
            int next = flight != null ? flight.NextRingIndex : 0;

            if (next != _lastNext)
            {
                for (int i = 0; i < _rings.Count; i++)
                    PaintRing(i, RingLampChaseCore.Classify(i, next), Time.time);
                _lastNext = next;
                Debug.Log("ZIPTIDE: RING_LIGHTS next=" + next + "/" + _rings.Count);

                // The dead gate gets a line the first time it is the one ahead of you: the player
                // sees an unlit ring and hears why it matters, which is how the Catch tells you it
                // is failing (docs/design/THE_CATCH.md §6).
                if (!_saidDeadRing && next >= 0 && next < _rings.Count && _rings[next].Length == 0)
                {
                    _saidDeadRing = true;
                    var rill = FindObjectOfType<Ziptide.Gameplay.RillCompanion>();
                    if (rill != null) rill.SayById("CATCH_DEAD_RING");
                }
            }

            if (next >= 0 && next < _rings.Count)
                PaintRing(next, RingLampState.Next, Time.time);
        }

        private void PaintRing(int ringIndex, RingLampState state, float time)
        {
            var segments = _rings[ringIndex];
            for (int s = 0; s < segments.Length; s++)
            {
                var renderer = segments[s];
                if (renderer == null) continue;
                Color color = RingLampChaseCore.SegmentColor(state, s, segments.Length, time);
                renderer.GetPropertyBlock(_block);
                _block.SetColor(BaseColorId, color);
                _block.SetColor(ColorId, color);
                renderer.SetPropertyBlock(_block);
            }
        }
    }
}
