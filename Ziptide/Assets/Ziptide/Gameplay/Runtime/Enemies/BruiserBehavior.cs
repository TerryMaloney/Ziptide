using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Bruiser archetype (GAME_PLAN M3): a heavy grounder that squares up, WINDS UP (the readable
    /// telegraph — it lowers, glows, and paws the ground), then charges in a straight line. The charge
    /// is clamped by CollideMove, so it visibly SLAMS INTO WALLS and staggers (Recover = the counter
    /// window). A taser stun interrupts everything, including mid-charge. Pure FSM in
    /// <see cref="ChargeState"/> (CI-tested). Sidestepping the telegraphed line is the no-gear counter.
    /// </summary>
    public class BruiserBehavior : CreatureBehaviorBase
    {
        public float chargeSpeed = 8f;
        public float walkSpeed = 1.6f;

        private readonly ChargeState _fsm = new ChargeState();
        private Vector3 _chargeDir;
        private Renderer _hump;
        private static readonly Color BodyColor = new Color(0.35f, 0.30f, 0.28f);
        private static readonly Color TelegraphColor = new Color(0.95f, 0.45f, 0.20f);

        protected override void BuildVisuals()
        {
            MakePart("Torso", PrimitiveType.Cube, new Vector3(0f, 0.45f, 0f),
                new Vector3(0.7f, 0.6f, 1.0f), BodyColor, keepCollider: true);
            var hump = MakePart("Hump", PrimitiveType.Sphere, new Vector3(0f, 0.85f, -0.2f),
                new Vector3(0.5f, 0.35f, 0.55f), BodyColor);
            _hump = hump.GetComponent<Renderer>();
            MakePart("HeadPlate", PrimitiveType.Cube, new Vector3(0f, 0.45f, 0.55f),
                new Vector3(0.55f, 0.45f, 0.15f), new Color(0.25f, 0.22f, 0.20f));
        }

        protected override void Tick(float dt, float dist)
        {
            _fsm.Tick(dt, dist, Runtime.IsActive);

            if (_fsm.ChargeStarted && Player != null)
            {
                // Lock the line at launch — the player dodges the LINE, not a homing missile (fair).
                Vector3 d = Player.position - transform.position; d.y = 0f;
                _chargeDir = d.sqrMagnitude > 0.001f ? d.normalized : transform.forward;
            }

            switch (_fsm.Phase)
            {
                case ChargePhase.Idle:
                    if (Player != null && dist <= detectRange)
                    {
                        FaceToward(Player.position, dt, 3f);
                        Vector3 step = transform.position + transform.forward * walkSpeed * dt;
                        step.y = Runtime.HomePos.y;
                        CollideMove(Leashed(step));
                    }
                    Telegraph(0f);
                    break;

                case ChargePhase.Windup:
                    if (Player != null) FaceToward(Player.position, dt, 6f);
                    // Paw the ground: a low shudder that reads as "about to charge."
                    transform.position += new Vector3(Mathf.Sin(Time.time * 30f) * 0.008f, 0f, 0f);
                    Telegraph(_fsm.WindupProgress);
                    break;

                case ChargePhase.Charging:
                {
                    Vector3 before = transform.position;
                    Vector3 want = before + _chargeDir * chargeSpeed * dt;
                    want.y = Runtime.HomePos.y;
                    CollideMove(want); // deliberately NOT leashed — a charge can overshoot the leash…
                    // …but a wall stops it hard: if we barely moved, we slammed.
                    if ((transform.position - before).magnitude < chargeSpeed * dt * 0.3f)
                        _fsm.HitWall();
                    Telegraph(1f);
                    break;
                }

                case ChargePhase.Recover:
                    Telegraph(0f); // dim — the vulnerable window
                    break;
            }
        }

        public override void RestoreVisuals() => Telegraph(0f);

        private void Telegraph(float p)
        {
            if (_hump == null || _hump.material == null) return;
            Color c = Color.Lerp(BodyColor, TelegraphColor, p);
            if (_hump.material.HasProperty("_BaseColor")) _hump.material.SetColor("_BaseColor", c);
            else _hump.material.color = c;
        }
    }
}
