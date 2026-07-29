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

        private void Start()
        {
            _block = new MaterialPropertyBlock();
            if (lane == null) lane = transform;
            for (int i = 0; ; i++)
            {
                var ring = lane.Find("Ring_" + i);
                if (ring == null) break;
                _rings.Add(ring.GetComponentsInChildren<Renderer>());
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
