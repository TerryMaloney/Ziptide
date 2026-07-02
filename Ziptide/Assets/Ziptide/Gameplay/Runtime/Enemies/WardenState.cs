namespace Ziptide.Gameplay
{
    public enum WardenMode { Dormant, Watch, Warn, Pursue, Ally }

    /// <summary>
    /// Pure, headless Warden escalation (no Unity → CI-testable). Wardens are the Shell's immune
    /// system: LAWFUL, never an ambush (CREATURE_DESIGN §Wardens) — they escalate with the story's
    /// Signal tier and only ever one step at a time: Dormant (tier 0) → Watch (tier 1: they turn to
    /// face you) → Warn (tier 2+, you're close: hold ground, visible warning) → Pursue (you stayed
    /// crowded through the whole warning window). Backing off de-escalates. The Ch.6 ally branch
    /// (C6_WARDEN_ALLY) overrides everything. <see cref="WardenBehavior"/> wraps it with motion.
    /// </summary>
    public class WardenState
    {
        public float WatchRange = 12f;
        public float WarnRange = 4.5f;
        /// <summary>Seconds the player must stay inside WarnRange before Warn escalates to Pursue.</summary>
        public float WarnSeconds = 2.5f;

        public WardenMode Mode { get; private set; } = WardenMode.Dormant;
        /// <summary>0..1 progress of the warning window (drives the visible escalation).</summary>
        public float WarnProgress { get; private set; }

        private float _crowded;

        public void Tick(float dt, int signalTier, float dist, bool isAlly)
        {
            if (isAlly) { Mode = WardenMode.Ally; WarnProgress = 0f; _crowded = 0f; return; }

            if (signalTier <= 0)
            {
                Mode = WardenMode.Dormant;
                WarnProgress = 0f;
                _crowded = 0f;
                return;
            }

            bool inWatch = dist <= WatchRange;
            bool crowdedNow = dist <= WarnRange && signalTier >= 2;

            if (crowdedNow)
            {
                _crowded += dt;
                WarnProgress = WarnSeconds > 0f ? System.Math.Min(1f, _crowded / WarnSeconds) : 1f;
                Mode = _crowded >= WarnSeconds ? WardenMode.Pursue : WardenMode.Warn;
            }
            else
            {
                // Backing off always de-escalates — lawful, not vindictive.
                _crowded = 0f;
                WarnProgress = 0f;
                Mode = inWatch ? WardenMode.Watch : WardenMode.Dormant;
            }
        }
    }
}
