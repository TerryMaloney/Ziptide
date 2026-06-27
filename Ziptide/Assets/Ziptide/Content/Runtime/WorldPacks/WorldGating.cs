using Ziptide.Core;

namespace Ziptide.Content
{
    /// <summary>
    /// Pure story-gating logic for a <see cref="WorldPackDefinition"/> against a
    /// <see cref="PlayerProfile"/>: whether its required flags are met, and granting its world-level
    /// flags on completion. Headless (no Unity scene types), so it's EditMode-testable — the runtime
    /// (<c>JobDirector</c> for granting; the travel/offer UI for the requirement check) calls into it.
    /// Mirrors <see cref="JobRewards"/>: data + profile in, deterministic result out.
    /// </summary>
    public static class WorldGating
    {
        /// <summary>
        /// True if EVERY flag in <see cref="WorldPackDefinition.flagsRequired"/> is set on the profile.
        /// Permissive on missing data: a null pack or empty requirement list is always met. A null
        /// profile fails any non-empty requirement (can't prove it's unlocked — fail closed there).
        /// </summary>
        public static bool MeetsRequirements(WorldPackDefinition pack, PlayerProfile profile)
        {
            if (pack == null || pack.flagsRequired == null || pack.flagsRequired.Count == 0) return true;
            if (profile == null) return false;

            for (int i = 0; i < pack.flagsRequired.Count; i++)
            {
                string flag = pack.flagsRequired[i];
                if (string.IsNullOrEmpty(flag)) continue; // blank entries don't gate
                if (!profile.HasFlag(flag)) return false;
            }
            return true;
        }

        /// <summary>
        /// First requirement flag that is NOT yet set (for diagnostics / "locked because…" UI), or null
        /// if requirements are met. Cheap: returns on the first miss.
        /// </summary>
        public static string FirstMissingRequirement(WorldPackDefinition pack, PlayerProfile profile)
        {
            if (pack == null || pack.flagsRequired == null) return null;
            for (int i = 0; i < pack.flagsRequired.Count; i++)
            {
                string flag = pack.flagsRequired[i];
                if (string.IsNullOrEmpty(flag)) continue;
                if (profile == null || !profile.HasFlag(flag)) return flag;
            }
            return null;
        }

        /// <summary>
        /// Set every flag in <see cref="WorldPackDefinition.flagsGranted"/> on the profile. Null-safe and
        /// idempotent (SetFlag dedupes). Returns the count of flags that were newly set (already-set and
        /// blank entries don't count), so the caller can log/skip a no-op grant.
        /// </summary>
        public static int GrantWorldFlags(WorldPackDefinition pack, PlayerProfile profile)
        {
            if (pack == null || profile == null || pack.flagsGranted == null) return 0;

            int granted = 0;
            for (int i = 0; i < pack.flagsGranted.Count; i++)
            {
                string flag = pack.flagsGranted[i];
                if (string.IsNullOrEmpty(flag) || profile.HasFlag(flag)) continue;
                profile.SetFlag(flag);
                granted++;
            }
            return granted;
        }
    }
}
