using System.Collections.Generic;
using System.Linq;

namespace Ziptide.Visuals
{
    /// <summary>One asset's dependency facts, source-agnostic (reconciliation R3). Collected by
    /// editor-side IForgeDependencySource implementations; consumed by the pure bucketing below.</summary>
    public class ForgeAssetInfo
    {
        public string id;
        public string[] refs;          // merged storyRefs + worldRuleRefs + tokenRefs + family + tags (lowercase)
        public ForgeQualityState state;
        public bool hero;              // storyTags contains "hero"
        public bool hasConsumers;      // some seam (item/theme/district) references this id
    }

    /// <summary>
    /// THE NORTH-STAR QUERY, pure and tested: "we changed X — which assets are affected, and what
    /// may happen to each?" Buckets follow the reconciliation contract:
    ///   SafeAuto   — unlocked proxies; regeneration/re-texture can proceed without review.
    ///   Review     — hero-tagged or Locked; a human looks before anything regenerates.
    ///   Breaking   — the changed ref IS a consumed asset id (renaming it breaks seams) — flagged,
    ///                never auto-acted.
    ///   Deprecated — Deprecated state or nothing consumes it; retire consumers first, never delete.
    /// Editor sources feed it; nothing here touches AssetDatabase, so EditMode tests pin it.
    /// </summary>
    public static class ForgeStaleness
    {
        public enum Bucket { SafeAuto, Review, Breaking, Deprecated }

        public static SortedDictionary<Bucket, List<string>> Affected(
            IEnumerable<ForgeAssetInfo> assets, string changedRef)
        {
            var result = new SortedDictionary<Bucket, List<string>>
            {
                { Bucket.SafeAuto, new List<string>() },
                { Bucket.Review, new List<string>() },
                { Bucket.Breaking, new List<string>() },
                { Bucket.Deprecated, new List<string>() },
            };
            if (string.IsNullOrEmpty(changedRef) || assets == null) return result;
            string needle = changedRef.ToLowerInvariant();

            foreach (var a in assets.Where(a => a != null && a.id != null).OrderBy(a => a.id))
            {
                bool idHit = a.id.ToLowerInvariant() == needle;
                bool refHit = a.refs != null && a.refs.Any(r => r != null && r.ToLowerInvariant() == needle);
                if (!idHit && !refHit) continue;

                if (idHit && a.hasConsumers) { result[Bucket.Breaking].Add(a.id); continue; }
                if (a.state == ForgeQualityState.Deprecated || !a.hasConsumers)
                { result[Bucket.Deprecated].Add(a.id); continue; }
                if (a.hero || a.state == ForgeQualityState.Locked)
                { result[Bucket.Review].Add(a.id); continue; }
                result[Bucket.SafeAuto].Add(a.id);
            }
            return result;
        }
    }
}
