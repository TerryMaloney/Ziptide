using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Flyer archetype (GAME_PLAN M3): rides thermals high over its zone in a lazy figure-8; when the
    /// player wanders below, it TELEGRAPHS (hangs dead-still — a hawk's hover — wings folding) then
    /// DIVES at the head, pulls up ~1m short, and climbs away. Contact = the base touch-stun. Counter:
    /// the hover-pause is your warning — step aside or taser it mid-dive (a stun drops it to a slow
    /// grounded flop). Collision-clean via CollideMove (it pulls out of a dive INTO a wall).
    /// </summary>
    public class FlyerBehavior : CreatureBehaviorBase
    {
        public float soarHeight = 5f;
        public float soarSpeed = 2.2f;
        public float diveSpeed = 9f;
        public float hoverTelegraphSeconds = 0.9f;
        public float diveCooldown = 5f;

        private enum Mode { Soar, Hover, Dive, Climb }
        private Mode _mode = Mode.Soar;

        private float _timer;
        private float _nextDiveAllowed;
        private Vector3 _diveTarget;
        private float _figure8;
        private Transform _wingL, _wingR;
        private static readonly Color BodyColor = new Color(0.30f, 0.34f, 0.42f);

        protected override void BuildVisuals()
        {
            MakePart("Body", PrimitiveType.Capsule, Vector3.zero,
                new Vector3(0.25f, 0.18f, 0.25f), BodyColor, keepCollider: true);
            _wingL = MakePart("WingL", PrimitiveType.Cube, new Vector3(-0.35f, 0.05f, 0f),
                new Vector3(0.55f, 0.03f, 0.25f), BodyColor * 0.85f).transform;
            _wingR = MakePart("WingR", PrimitiveType.Cube, new Vector3(0.35f, 0.05f, 0f),
                new Vector3(0.55f, 0.03f, 0.25f), BodyColor * 0.85f).transform;
        }

        protected override void Tick(float dt, float dist)
        {
            // Wing flap — folds tight during hover/dive (part of the telegraph read).
            float flap = _mode == Mode.Soar || _mode == Mode.Climb ? Mathf.Sin(Time.time * 6f) * 25f : 70f;
            if (_wingL != null) _wingL.localRotation = Quaternion.Euler(0f, 0f, flap);
            if (_wingR != null) _wingR.localRotation = Quaternion.Euler(0f, 0f, -flap);

            switch (_mode)
            {
                case Mode.Soar:
                {
                    _figure8 += soarSpeed * 0.25f * dt;
                    Vector3 target = Runtime.HomePos + new Vector3(
                        Mathf.Sin(_figure8) * 5f, soarHeight + Mathf.Sin(_figure8 * 2f) * 0.6f,
                        Mathf.Sin(_figure8 * 2f) * 3f);
                    CollideMove(Leashed(Vector3.MoveTowards(transform.position, target, soarSpeed * dt)));
                    FaceToward(target + (target - transform.position), dt, 3f);

                    if (Player != null && dist <= detectRange && Time.time >= _nextDiveAllowed)
                    {
                        _mode = Mode.Hover;
                        _timer = hoverTelegraphSeconds;
                    }
                    break;
                }

                case Mode.Hover:
                    // Dead-still hang: the readable "it's coming" beat.
                    if (Player != null) FaceToward(Player.position, dt, 8f);
                    _timer -= dt;
                    if (_timer <= 0f)
                    {
                        _diveTarget = Player != null ? Player.position : Runtime.HomePos;
                        _mode = Mode.Dive;
                        _nextDiveAllowed = Time.time + diveCooldown;
                    }
                    break;

                case Mode.Dive:
                {
                    Vector3 before = transform.position;
                    Vector3 want = Vector3.MoveTowards(before, _diveTarget, diveSpeed * dt);
                    CollideMove(want);
                    FaceToward(_diveTarget, dt, 10f);
                    bool pulledUp = (transform.position - _diveTarget).magnitude < 1.0f;
                    bool blocked = (transform.position - before).magnitude < diveSpeed * dt * 0.2f;
                    if (pulledUp || blocked) _mode = Mode.Climb;
                    break;
                }

                case Mode.Climb:
                {
                    Vector3 target = Runtime.HomePos + Vector3.up * soarHeight;
                    CollideMove(Vector3.MoveTowards(transform.position, target, diveSpeed * 0.6f * dt));
                    if ((transform.position - target).magnitude < 1.2f) _mode = Mode.Soar;
                    break;
                }
            }
        }

        public override void OnStunned(float seconds)
        {
            // Tased mid-air: it flutters down and restarts its climb after — the dive is cancelled.
            _mode = Mode.Climb;
        }
    }
}
