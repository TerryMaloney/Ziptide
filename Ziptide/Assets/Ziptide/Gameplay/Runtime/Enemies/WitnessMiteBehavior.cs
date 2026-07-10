using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Witness-mite (CREATURE_DESIGN novel #1): FREEZES SOLID while you look at it — it only moves while
    /// unobserved. Evolution: a contained-universe organism that learned the Observers' gaze "fixes"
    /// matter (the meta made mechanic). Counter: keep it in view and it's a statue; taser/gravity work
    /// while frozen. Pure gaze check in <see cref="GazeMath"/> (CI-tested). It stalks toward you in the
    /// gaps between glances — turning your head IS the encounter.
    /// </summary>
    public class WitnessMiteBehavior : CreatureBehaviorBase
    {
        public float stalkSpeed = 3.5f;
        [Tooltip("Cosine of the half-angle that counts as 'looking at it' (0.55 ≈ 57°).")]
        public float cosHalfFov = 0.55f;
        public float observeRange = 18f;

        private Transform _shell;
        private static readonly Color MoveColor = new Color(0.65f, 0.30f, 0.45f);
        private static readonly Color FrozenColor = new Color(0.45f, 0.45f, 0.50f); // stone
        private Renderer _bodyRenderer;

        protected override void BuildVisuals()
        {
            var body = MakePart("Mite", PrimitiveType.Sphere, Vector3.zero,
                new Vector3(0.3f, 0.22f, 0.38f), MoveColor, keepCollider: true);
            _shell = body.transform;
            _bodyRenderer = body.GetComponent<Renderer>();
            MakePart("Spine1", PrimitiveType.Cube, new Vector3(0f, 0.15f, -0.05f),
                new Vector3(0.05f, 0.18f, 0.05f), MoveColor * 0.8f);
            MakePart("Spine2", PrimitiveType.Cube, new Vector3(0.08f, 0.13f, 0.08f),
                new Vector3(0.05f, 0.14f, 0.05f), MoveColor * 0.8f);
        }

        protected override void Tick(float dt, float dist)
        {
            bool observed = false;
            if (Player != null && dist <= observeRange)
                observed = GazeMath.IsObserved(Player.forward, transform.position - Player.position,
                                               cosHalfFov, observeRange);

            if (observed)
            {
                // Fixed by the gaze — dead still, stone-grey. (Also its vulnerable window.)
                SetFrozen(true);
                return;
            }

            SetFrozen(false);
            if (Player != null && dist <= detectRange)
            {
                // Unwatched: skitter straight at you, fast enough to feel it gained ground.
                Vector3 target = new Vector3(Player.position.x, Runtime.HomePos.y + 0.2f, Player.position.z);
                CollideMove(Leashed(Vector3.MoveTowards(transform.position, target, stalkSpeed * dt)));
                FaceToward(Player.position, dt, 10f);
                if (_shell != null) // scuttle wobble only while moving
                    _shell.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 20f) * 8f);
            }
        }

        public override void RestoreVisuals() => SetFrozen(false);

        /// <summary>The stone-freeze tell. Forged body: tint/clear through the ForgeBodyTell bridge
        /// (clearing restores the genome's OWN palette). Unforged: recolor the primitive as before.</summary>
        private void SetFrozen(bool frozen)
        {
            bool bridged = frozen
                ? Ziptide.Visuals.ForgeBodyTell.TrySetBodyTint(gameObject, FrozenColor)
                : Ziptide.Visuals.ForgeBodyTell.TryClearBodyTint(gameObject);
            if (!bridged) SetColor(frozen ? FrozenColor : MoveColor);
        }

        private void SetColor(Color c)
        {
            if (_bodyRenderer == null || _bodyRenderer.material == null) return;
            if (_bodyRenderer.material.HasProperty("_BaseColor")) _bodyRenderer.material.SetColor("_BaseColor", c);
            else _bodyRenderer.material.color = c;
        }
    }
}
