using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The Warden (GAME_PLAN M3; CREATURE_DESIGN §Wardens): the Shell's immune system. Lawful and
    /// telegraphed — a tall sentinel that is a statue until the story's Signal rises
    /// (<see cref="SignalState.Tier"/>): tier 1 it WATCHES (turns its eye to you), tier 2 it WARNS when
    /// crowded (eye ramps orange), and only if you stand your ground through the whole warning does it
    /// PURSUE and arrest (one heavy non-lethal stun, then it disengages). Backing off always
    /// de-escalates. The Ch.6 C6_WARDEN_ALLY flag turns it calm-green forever. Pure FSM in
    /// <see cref="WardenState"/> (CI-tested). Logs ZIPTIDE: WARDEN mode=… on transitions.
    /// </summary>
    public class WardenBehavior : CreatureBehaviorBase
    {
        public float pursueSpeed = 3.2f;

        private readonly WardenState _fsm = new WardenState();
        private WardenMode _lastMode = WardenMode.Dormant;
        private Renderer _eye;
        private float _nextArrestAllowed;

        private static readonly Color DormantEye = new Color(0.25f, 0.28f, 0.33f);
        private static readonly Color WatchEye = new Color(0.35f, 0.75f, 0.95f);
        private static readonly Color WarnEye = new Color(0.95f, 0.55f, 0.15f);
        private static readonly Color PursueEye = new Color(0.95f, 0.25f, 0.15f);
        private static readonly Color AllyEye = new Color(0.35f, 0.9f, 0.55f);

        protected override void BuildVisuals()
        {
            MakePart("Pillar", PrimitiveType.Cube, new Vector3(0f, 1.1f, 0f),
                new Vector3(0.5f, 2.2f, 0.5f), new Color(0.20f, 0.21f, 0.25f), keepCollider: true);
            MakePart("Shoulders", PrimitiveType.Cube, new Vector3(0f, 2.05f, 0f),
                new Vector3(0.85f, 0.25f, 0.6f), new Color(0.16f, 0.17f, 0.21f));
            var eye = MakePart("Eye", PrimitiveType.Sphere, new Vector3(0f, 1.85f, 0.28f),
                new Vector3(0.18f, 0.10f, 0.10f), DormantEye);
            _eye = eye.GetComponent<Renderer>();
        }

        protected override void Tick(float dt, float dist)
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            int tier = SignalState.Tier(profile);
            bool ally = profile != null && profile.HasFlag(ZiptideFlags.C6_WARDEN_ALLY);

            _fsm.Tick(dt, tier, dist, ally);

            if (_fsm.Mode != _lastMode)
            {
                Debug.Log("ZIPTIDE: WARDEN mode=" + _fsm.Mode + " tier=" + tier);
                _lastMode = _fsm.Mode;
            }

            switch (_fsm.Mode)
            {
                case WardenMode.Dormant:
                    SetEye(DormantEye);
                    break; // a statue

                case WardenMode.Ally:
                    SetEye(AllyEye);
                    if (Player != null && dist <= detectRange) FaceToward(Player.position, dt, 2f);
                    break;

                case WardenMode.Watch:
                    SetEye(WatchEye);
                    if (Player != null) FaceToward(Player.position, dt, 4f);
                    break;

                case WardenMode.Warn:
                    // Hold ground; the eye ramps toward red with the warning window.
                    SetEye(Color.Lerp(WarnEye, PursueEye, _fsm.WarnProgress));
                    if (Player != null) FaceToward(Player.position, dt, 8f);
                    break;

                case WardenMode.Pursue:
                {
                    SetEye(PursueEye);
                    if (Player == null) break;
                    FaceToward(Player.position, dt, 8f);
                    Vector3 step = Vector3.MoveTowards(transform.position,
                        new Vector3(Player.position.x, Runtime.HomePos.y, Player.position.z), pursueSpeed * dt);
                    CollideMove(Leashed(step));
                    // The arrest: one heavy, lawful stun — then a long personal cooldown (it disengages;
                    // it wanted compliance, not a kill).
                    if (dist <= touchRadius + 0.4f && Time.time >= _nextArrestAllowed)
                    {
                        _nextArrestAllowed = Time.time + 8f;
                        var stun = FindObjectOfType<PlayerStunReceiver>();
                        if (stun != null) stun.ApplyStun(2.5f, 0.3f);
                        Debug.Log("ZIPTIDE: WARDEN_ARREST");
                    }
                    break;
                }
            }
        }

        public override void RestoreVisuals() => SetEye(DormantEye);

        private void SetEye(Color c)
        {
            if (_eye == null || _eye.material == null) return;
            if (_eye.material.HasProperty("_BaseColor")) _eye.material.SetColor("_BaseColor", c);
            else _eye.material.color = c;
        }
    }
}
