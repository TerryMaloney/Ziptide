using System;
using System.Collections.Generic;
using System.Text;

namespace Ziptide.Core
{
    /// <summary>One economy transaction. THE META-LOOP LAW (docs/design/ZIPTIDE_META_LOOP.md): every
    /// resource grant/spend in every mode flows through <see cref="RewardRouter"/>, which writes one
    /// of these — resource flow is auditable, testable, and explainable to any model.</summary>
    [Serializable]
    public class LedgerEntry
    {
        public long atUnix;
        public string resourceId;
        public double delta;
        public string source;    // a LedgerSource constant — WHICH MODE moved the resource
        public string reason;    // human/AI-readable: jobId, plantId, recipeId, machineId…
        public string relatedId; // asset/definition id when relevant
        public string worldId;   // scene/world the transaction happened in
    }

    /// <summary>The closed set of systems allowed to move resources. A mode without a constant here
    /// has no business touching the economy (add the constant + HANDOFF entry first).</summary>
    public static class LedgerSource
    {
        public const string Campaign = "campaign_reward";
        public const string Garden = "garden_harvest";
        public const string Factory = "factory_output";
        public const string RecipeCost = "recipe_cost";
        public const string UpgradeCost = "upgrade_cost";
        public const string Multiplayer = "multiplayer_reward";
        public const string Debug = "debug";
    }

    /// <summary>Ring-capped transaction history on the profile (Meta-Loop B). Pure.</summary>
    public static class ResourceLedger
    {
        public const int MaxEntries = 500;

        public static void Append(PlayerProfile profile, LedgerEntry entry)
        {
            if (profile == null || entry == null) return;
            if (profile.ledger == null) profile.ledger = new List<LedgerEntry>();
            profile.ledger.Add(entry);
            while (profile.ledger.Count > MaxEntries) profile.ledger.RemoveAt(0);
        }

        /// <summary>Net movement recorded for a resource (over the retained window).</summary>
        public static double SumFor(PlayerProfile profile, string resourceId)
        {
            double sum = 0;
            if (profile?.ledger == null) return sum;
            foreach (var e in profile.ledger)
                if (e != null && e.resourceId == resourceId) sum += e.delta;
            return sum;
        }

        /// <summary>Human/AI-readable history for one resource — "explain where my spore went".</summary>
        public static string Explain(PlayerProfile profile, string resourceId, int maxLines = 20)
        {
            var sb = new StringBuilder();
            if (profile?.ledger == null) return "";
            int shown = 0;
            for (int i = profile.ledger.Count - 1; i >= 0 && shown < maxLines; i--)
            {
                var e = profile.ledger[i];
                if (e == null || e.resourceId != resourceId) continue;
                sb.Append(e.delta >= 0 ? "+" : "").Append(e.delta)
                  .Append(" via ").Append(e.source)
                  .Append(string.IsNullOrEmpty(e.reason) ? "" : " (" + e.reason + ")")
                  .Append(string.IsNullOrEmpty(e.worldId) ? "" : " @" + e.worldId)
                  .Append('\n');
                shown++;
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// THE MODE CONTRACT chokepoint (Meta-Loop F): campaign, garden, factory and multiplayer may only
    /// move resources through Grant/TrySpend — one profile mutation path, one ledger, one economy.
    /// Direct <see cref="PlayerProfile.AddResource"/> calls outside this class (and migration/tests)
    /// are a contract violation — see ZIPTIDE_META_LOOP.md §Future Model Rules.
    /// </summary>
    public static class RewardRouter
    {
        /// <summary>Grant a resource. Returns the new total (0 on invalid input).</summary>
        public static double Grant(PlayerProfile profile, string source, string resourceId, double amount,
            string reason = "", string relatedId = "", string worldId = "", long nowUnix = 0)
        {
            if (profile == null || string.IsNullOrEmpty(resourceId) || amount <= 0) return 0;
            double total = profile.AddResource(resourceId, amount);
            ResourceLedger.Append(profile, new LedgerEntry
            {
                atUnix = nowUnix, resourceId = resourceId, delta = amount,
                source = source, reason = reason, relatedId = relatedId, worldId = worldId,
            });
            return total;
        }

        /// <summary>Spend a resource — all-or-nothing per call. False (and no ledger entry) when
        /// the profile can't afford it.</summary>
        public static bool TrySpend(PlayerProfile profile, string source, string resourceId, double amount,
            string reason = "", string relatedId = "", string worldId = "", long nowUnix = 0)
        {
            if (profile == null || string.IsNullOrEmpty(resourceId) || amount <= 0) return false;
            if (profile.GetResource(resourceId) < amount) return false;
            profile.AddResource(resourceId, -amount);
            ResourceLedger.Append(profile, new LedgerEntry
            {
                atUnix = nowUnix, resourceId = resourceId, delta = -amount,
                source = source, reason = reason, relatedId = relatedId, worldId = worldId,
            });
            return true;
        }
    }
}
