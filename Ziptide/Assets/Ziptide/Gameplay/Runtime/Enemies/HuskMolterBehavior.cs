using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Husk-molter (CREATURE_DESIGN novel #8): when STUNNED it sheds a grey decoy husk and skitters out
    /// the back — you swing at the statue while the real one repositions. Evolution: a crawler with a
    /// disposable exoskeleton. Counter: hit the MOVING one (the husk never moves and crumbles on its
    /// own); a gravity grab before it molts skips the trick. One molt per stun; brief cooldown so it
    /// can't chain-escape. Otherwise stalks like a grounder.
    /// </summary>
    public class HuskMolterBehavior : CreatureBehaviorBase
    {
        public float stalkSpeed = 2.4f;
        public float moltCooldown = 6f;
        public float huskLifeSeconds = 5f;

        private float _nextMoltAllowed;
        private static readonly Color BodyColor = new Color(0.35f, 0.45f, 0.30f);
        private static readonly Color HuskColor = new Color(0.42f, 0.42f, 0.40f);

        protected override void BuildVisuals()
        {
            MakePart("Shell", PrimitiveType.Capsule, Vector3.zero,
                new Vector3(0.32f, 0.22f, 0.32f), BodyColor, keepCollider: true);
            MakePart("Ridge", PrimitiveType.Cube, new Vector3(0f, 0.2f, 0f),
                new Vector3(0.08f, 0.15f, 0.4f), BodyColor * 0.75f);
        }

        protected override void Tick(float dt, float dist)
        {
            if (Player != null && dist <= detectRange)
            {
                Vector3 target = new Vector3(Player.position.x, Runtime.HomePos.y + 0.25f, Player.position.z);
                CollideMove(Leashed(Vector3.MoveTowards(transform.position, target, stalkSpeed * dt)));
                FaceToward(Player.position, dt, 6f);
            }
            else
            {
                CollideMove(Vector3.MoveTowards(transform.position, Runtime.HomePos + Vector3.up * 0.25f, stalkSpeed * 0.5f * dt));
            }
        }

        public override void OnStunned(float seconds)
        {
            if (Time.time < _nextMoltAllowed) return; // the stun sticks this time — the counter window
            _nextMoltAllowed = Time.time + moltCooldown;

            // Shed the decoy where it stood. Forged body: the husk is a FROZEN grey clone of the
            // actual creature (the trick only works if the shed skin looks like you). Unforged:
            // the primitive capsule decoy, exactly as before.
            var husk = Ziptide.Visuals.ForgeBodyTell.TryCloneStatue(gameObject, HuskColor);
            if (husk == null)
            {
                husk = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                var col = husk.GetComponent<Collider>();
                if (col != null) Destroy(col);
                husk.transform.localScale = new Vector3(0.32f, 0.22f, 0.32f);
                var r = husk.GetComponent<Renderer>();
                if (r != null)
                {
                    var shader = Shader.Find("Universal Render Pipeline/Lit");
                    if (shader == null) shader = Shader.Find("Standard");
                    if (shader != null)
                    {
                        var mat = new Material(shader);
                        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", HuskColor);
                        else mat.color = HuskColor;
                        r.sharedMaterial = mat;
                        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }
            }
            husk.name = "__Husk";
            husk.transform.position = transform.position;
            husk.transform.rotation = transform.rotation;
            Destroy(husk, huskLifeSeconds); // husks crumble on their own

            // …and skitter out the back, wall-clamped (never through solids).
            Vector3 away = Player != null
                ? (transform.position - Player.position)
                : -transform.forward;
            away.y = 0f;
            Vector3 escape = transform.position + (away.sqrMagnitude > 0.01f ? away.normalized : Vector3.back) * 2.2f;
            escape.y = Runtime.HomePos.y + 0.25f;
            CollideMove(Leashed(escape));
            Debug.Log("ZIPTIDE: HUSK_MOLT");
        }
    }
}
