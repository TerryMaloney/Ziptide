using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Light-grazer (CREATURE_DESIGN novel #10): GROWS IN DARKNESS, SHRINKS IN LIGHT — a big one is just
    /// a small one that sat in the dark. Evolution: photophobic Bloom-feeder from the lightless worlds
    /// (W002's cistern). Graybox light source = the player's ATTENTION: facing it up close "shines your
    /// lamp on it" and it shrinks toward harmless; left alone in the dark it swells and drifts toward
    /// you (bigger = stronger touch-slow). Counter: face it down — literally. (The Prism Beam tool
    /// replaces the gaze as the shrink source at M5 when that gear lands; documented simplification.)
    /// </summary>
    public class LightGrazerBehavior : CreatureBehaviorBase
    {
        public float growPerSecond = 0.10f;
        public float shrinkPerSecond = 0.45f;
        public float minScale = 0.5f;
        public float maxScale = 2.6f;
        public float driftSpeed = 1.1f;
        [Tooltip("Cosine half-angle for the 'lit by your attention' cone.")]
        public float cosHalfFov = 0.7f;
        public float litRange = 9f;

        private float _growth = 1f;
        private Transform _mass;
        private Renderer _massRenderer;
        private static readonly Color DarkColor = new Color(0.16f, 0.30f, 0.24f);
        private static readonly Color LitColor = new Color(0.55f, 0.85f, 0.65f);

        protected override void BuildVisuals()
        {
            var mass = MakePart("Mass", PrimitiveType.Sphere, Vector3.zero,
                Vector3.one * 0.5f, DarkColor, keepCollider: true);
            _mass = mass.transform;
            _massRenderer = mass.GetComponent<Renderer>();
        }

        protected override void Tick(float dt, float dist)
        {
            bool lit = Player != null && GazeMath.IsObserved(
                Player.forward, transform.position - Player.position, cosHalfFov, litRange);

            _growth = Mathf.Clamp(_growth + (lit ? -shrinkPerSecond : growPerSecond) * dt, minScale, maxScale);
            if (_mass != null)
            {
                float wobble = 1f + Mathf.Sin(Time.time * 2.2f) * 0.04f;
                _mass.localScale = Vector3.one * 0.5f * _growth * wobble;
            }
            if (_massRenderer != null && _massRenderer.material != null)
            {
                Color c = Color.Lerp(DarkColor, LitColor, lit ? 0.8f : 0f);
                if (_massRenderer.material.HasProperty("_BaseColor")) _massRenderer.material.SetColor("_BaseColor", c);
                else _massRenderer.material.color = c;
            }

            // Bigger = bolder: touch-slow scales with growth, and it only advances while dark.
            touchStunSeconds = 0.4f + _growth * 0.4f;
            touchRadius = 0.3f + _growth * 0.25f;

            if (!lit && Player != null && dist <= detectRange)
            {
                Vector3 target = new Vector3(Player.position.x, Runtime.HomePos.y + 0.4f, Player.position.z);
                CollideMove(Leashed(Vector3.MoveTowards(transform.position, target, driftSpeed * _growth * 0.7f * dt)));
            }
            else if (lit)
            {
                // Recoils from the light, back toward its dark home.
                CollideMove(Vector3.MoveTowards(transform.position, Runtime.HomePos, driftSpeed * dt));
            }
        }

        public override void RestoreVisuals() { }
    }
}
