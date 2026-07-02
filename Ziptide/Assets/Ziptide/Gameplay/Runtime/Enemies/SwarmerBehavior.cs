using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Swarmer archetype (GAME_PLAN M3): a small cluster of chitin bodies that skitters as one — loose
    /// boid offsets around a shared center that orbits the player at a short standoff and periodically
    /// DARTS in (the telegraph is the gather: the bodies pull tight before the dart). Contact = the
    /// base's non-lethal touch-stun. Counter: taser stuns the whole cluster; gravity shoves it back.
    /// Evolution: colony organisms from the chitin worlds. Collision-clean via CollideMove.
    /// </summary>
    public class SwarmerBehavior : CreatureBehaviorBase
    {
        public int bodyCount = 5;
        public float orbitDistance = 3.5f;
        public float dartInterval = 4f;
        public float dartSpeed = 6f;
        public float cruiseSpeed = 2.5f;

        private Transform[] _bodies;
        private float[] _bodyPhase;
        private float _orbitAngle;
        private float _nextDart;
        private float _dartUntil;
        private static readonly Color BodyColor = new Color(0.55f, 0.42f, 0.22f);

        protected override void BuildVisuals()
        {
            _bodies = new Transform[bodyCount];
            _bodyPhase = new float[bodyCount];
            for (int i = 0; i < bodyCount; i++)
            {
                float s = 0.14f + (i % 3) * 0.03f;
                var b = MakePart("Bug_" + i, PrimitiveType.Sphere,
                    Random.insideUnitSphere * 0.4f, new Vector3(s, s * 0.6f, s * 1.3f), BodyColor,
                    keepCollider: i == 0); // one collider so weapons can hit the cluster
                _bodies[i] = b.transform;
                _bodyPhase[i] = Random.value * 10f;
            }
            _nextDart = Time.time + dartInterval * (0.5f + Random.value);
        }

        protected override void Tick(float dt, float dist)
        {
            bool darting = Time.time < _dartUntil;
            Vector3 desired;

            if (Player != null && dist <= detectRange)
            {
                if (!darting && Time.time >= _nextDart && dist < orbitDistance * 2f)
                {
                    _dartUntil = Time.time + 0.7f;
                    _nextDart = Time.time + dartInterval;
                }

                if (darting)
                {
                    // The dart: straight at the player's feet.
                    Vector3 target = new Vector3(Player.position.x, Runtime.HomePos.y + 0.4f, Player.position.z);
                    desired = Vector3.MoveTowards(transform.position, target, dartSpeed * dt);
                }
                else
                {
                    // Orbit at standoff.
                    _orbitAngle += 40f * dt * Mathf.Deg2Rad;
                    Vector3 ring = new Vector3(Mathf.Cos(_orbitAngle), 0f, Mathf.Sin(_orbitAngle)) * orbitDistance;
                    Vector3 target = Player.position + ring;
                    target.y = Runtime.HomePos.y + 0.4f;
                    desired = Vector3.MoveTowards(transform.position, target, cruiseSpeed * dt);
                }
            }
            else
            {
                // Idle skitter around home.
                _orbitAngle += 25f * dt * Mathf.Deg2Rad;
                Vector3 target = Runtime.HomePos + new Vector3(Mathf.Cos(_orbitAngle), 0.4f, Mathf.Sin(_orbitAngle)) * 1.5f;
                desired = Vector3.MoveTowards(transform.position, target, cruiseSpeed * 0.7f * dt);
            }

            CollideMove(Leashed(desired));

            // Boid jitter: bodies swirl loose normally, GATHER TIGHT during a dart (the telegraph).
            float spread = darting ? 0.15f : 0.45f;
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (_bodies[i] == null) continue;
                float p = _bodyPhase[i] + Time.time * (darting ? 9f : 4f);
                _bodies[i].localPosition = new Vector3(
                    Mathf.Sin(p) * spread, Mathf.Sin(p * 1.7f) * spread * 0.5f, Mathf.Cos(p * 1.3f) * spread);
            }
        }
    }
}
