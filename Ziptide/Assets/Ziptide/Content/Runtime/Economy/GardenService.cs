using System.Collections.Generic;
using Ziptide.Core;

namespace Ziptide.Content
{
    /// <summary>Why a tend attempt did or didn't apply.</summary>
    public enum TendStatus
    {
        Success,
        InvalidArgs,
        AlreadyHarvested,
        ToolNotApplicable, // tool id not in the plant's tendToolIds
        AlreadyTended,     // this tend tool was already applied to this plot
    }

    /// <summary>Why a plant-harvest attempt did or didn't succeed.</summary>
    public enum HarvestPlantStatus
    {
        Success,
        InvalidArgs,
        AlreadyHarvested,
        NotReady,            // still growing
        WrongToolFunction,   // tool.function != plant.harvestWith
        ToolCannotWorkPlant, // tool.worksOn restricts to other ids
    }

    /// <summary>How well-timed a harvest was (Additions Bank GARDEN #4/#5) — Fresh pays a bonus,
    /// Overripe pays less. Surfaced on the result so the scene can colour the plant / log it.</summary>
    public enum HarvestTiming { Prime, Fresh, Overripe }

    /// <summary>Outcome of harvesting a plot.</summary>
    public struct HarvestPlantResult
    {
        public HarvestPlantStatus status;
        public double yieldMultiplier; // multiplier actually applied (tend × timing)
        public int yieldEntries;       // resource lines credited
        public HarvestTiming timing;   // Fresh / Prime / Overripe at the moment of harvest
        public bool Success => status == HarvestPlantStatus.Success;

        public static HarvestPlantResult Fail(HarvestPlantStatus status)
            => new HarvestPlantResult { status = status };
    }

    /// <summary>
    /// Garden v1 — the grow loop: plant a seed (<see cref="PlantDefinition"/>) into a world plot, tend
    /// it with a series of tools (each speeds growth + boosts yield, once per tool), let it grow over
    /// real time (idle), then harvest with the right tool to credit the yield to the profile. Pure
    /// backend (no scene / MonoBehaviour); growth is time-based via <see cref="PlotState"/> and resolves
    /// with Core's <see cref="ProfileEconomy"/> / <see cref="IdleEngine"/>. EditMode-testable, no headset.
    ///
    /// Tend tuning constants below are v1 placeholders; they fold into BalanceConfig later.
    /// </summary>
    public static class GardenService
    {
        private const double TendYieldBonusPerPower = 0.25;   // each tend tool adds 0.25 * power to the yield multiplier
        private const double TendGrowthCreditSeconds = 60.0;  // each tend tool grants 60s * power of growth credit (speed)

        // ── Harvest timing (Additions Bank GARDEN #4/#5) — rewards checking your crops. Applied to
        // EVERY harvest by default; per-plant overridable via PlantDefinition. Non-lethal/all-ages:
        // an ignored crop only ever decays toward a floor, it never dies. ──
        public const double DefaultFreshWindowSeconds = 120.0;    // "just ripened" window (classification)
        // Fresh is CLASSIFICATION ONLY for now (0 bonus): harvest-when-ready is the baseline yield, so
        // paying a bonus there would re-baseline every existing yield + test. The +25% fresh bonus
        // (GARDEN #4) is deferred to a balance pass that updates those baselines together. The shipped
        // yield mechanic is the OVERRIPE DECAY below (GARDEN #5) — it changes nothing at/near ready.
        public const double FreshBonus = 0.0;
        public const double DefaultOverripeAfterSeconds = 900.0;  // 15 min after ready it starts to spoil
        public const double OverripeDecaySeconds = 900.0;         // then decays over the next 15 min
        public const double OverripeFloor = 0.5;                  // never below 50% — you never lose the crop

        /// <summary>Plant a seed: appends a fresh <see cref="PlotState"/> to the world and returns it.</summary>
        public static PlotState Plant(WorldState world, PlantDefinition plant, long nowUnix)
        {
            if (world == null || plant == null) return null;
            if (world.plots == null) world.plots = new List<PlotState>();

            var plot = new PlotState
            {
                plantId = plant.id,
                plantedAtUnix = nowUnix,
                growSeconds = plant.growSeconds,
                harvested = false,
                yieldMultiplier = 1.0,
                appliedTendToolIds = new List<string>(),
            };
            world.plots.Add(plot);
            return plot;
        }

        /// <summary>Whether <paramref name="tool"/> may tend <paramref name="plot"/> right now.</summary>
        public static TendStatus CanTend(PlotState plot, PlantDefinition plant, ToolDefinition tool)
        {
            if (plot == null || plant == null || tool == null || string.IsNullOrEmpty(tool.id)) return TendStatus.InvalidArgs;
            if (plot.harvested) return TendStatus.AlreadyHarvested;
            if (plant.tendToolIds == null || !plant.tendToolIds.Contains(tool.id)) return TendStatus.ToolNotApplicable;
            if (plot.appliedTendToolIds != null && plot.appliedTendToolIds.Contains(tool.id)) return TendStatus.AlreadyTended;
            return TendStatus.Success;
        }

        /// <summary>Apply a tend tool: speeds growth (growth credit) and boosts harvest yield, once per
        /// tool. Returns the gate reason if it can't apply.</summary>
        public static TendStatus Tend(PlotState plot, PlantDefinition plant, ToolDefinition tool, long nowUnix)
        {
            TendStatus gate = CanTend(plot, plant, tool);
            if (gate != TendStatus.Success) return gate;

            double power = tool.power > 0 ? tool.power : 0;
            plot.yieldMultiplier += TendYieldBonusPerPower * power;

            // Speed: move the planted anchor earlier so (now - plantedAtUnix) grows -> ready sooner.
            long credit = (long)(TendGrowthCreditSeconds * power);
            plot.plantedAtUnix -= credit;

            if (plot.appliedTendToolIds == null) plot.appliedTendToolIds = new List<string>();
            plot.appliedTendToolIds.Add(tool.id);
            return TendStatus.Success;
        }

        /// <summary>Whether <paramref name="plot"/> can be harvested now with <paramref name="tool"/>.</summary>
        public static HarvestPlantStatus CanHarvest(PlotState plot, PlantDefinition plant, ToolDefinition tool, long nowUnix)
        {
            if (plot == null || plant == null || tool == null) return HarvestPlantStatus.InvalidArgs;
            if (plot.harvested) return HarvestPlantStatus.AlreadyHarvested;
            if (!plot.IsReady(nowUnix)) return HarvestPlantStatus.NotReady;
            if (tool.function != plant.harvestWith) return HarvestPlantStatus.WrongToolFunction;
            if (!ToolWorksOnPlant(plant, tool)) return HarvestPlantStatus.ToolCannotWorkPlant;
            return HarvestPlantStatus.Success;
        }

        /// <summary>Harvest a ready plot: credits the plant's yield (× tend multiplier) to the profile
        /// and marks the plot harvested. Returns what happened.</summary>
        public static HarvestPlantResult Harvest(PlayerProfile profile, PlotState plot, PlantDefinition plant, ToolDefinition tool, long nowUnix)
        {
            if (profile == null) return HarvestPlantResult.Fail(HarvestPlantStatus.InvalidArgs);
            HarvestPlantStatus gate = CanHarvest(plot, plant, tool, nowUnix);
            if (gate != HarvestPlantStatus.Success) return HarvestPlantResult.Fail(gate);

            double tend = plot.yieldMultiplier > 0 ? plot.yieldMultiplier : 1.0;
            double timingMult = TimingMultiplier(plot, plant, nowUnix);
            double mult = tend * timingMult;
            int entries = 0;
            if (plant.harvestYield != null)
            {
                for (int i = 0; i < plant.harvestYield.Count; i++)
                {
                    var y = plant.harvestYield[i];
                    if (y == null || string.IsNullOrEmpty(y.resourceId) || y.amount <= 0) continue;
                    RewardRouter.Grant(profile, LedgerSource.Garden, y.resourceId, y.amount * mult,
                        reason: plant.id, relatedId: plot.plotId);
                    entries++;
                }
            }
            plot.harvested = true;
            return new HarvestPlantResult
            {
                status = HarvestPlantStatus.Success,
                yieldMultiplier = mult,
                yieldEntries = entries,
                timing = TimingOf(plot, plant, nowUnix),
            };
        }

        // ── Harvest timing (pure; GARDEN #4/#5) ──────────────────────────────────────────────────

        /// <summary>Unix time this plot becomes harvest-ready.</summary>
        public static long ReadyAtUnix(PlotState plot)
            => plot == null ? 0 : plot.plantedAtUnix + (long)plot.growSeconds;

        private static double FreshWindow(PlantDefinition plant)
            => (plant != null && plant.freshWindowSecondsOverride > 0)
                ? plant.freshWindowSecondsOverride : DefaultFreshWindowSeconds;

        private static double OverripeAfter(PlantDefinition plant)
            => (plant != null && plant.overripeAfterSecondsOverride > 0)
                ? plant.overripeAfterSecondsOverride : DefaultOverripeAfterSeconds;

        /// <summary>Classify how well-timed a harvest at <paramref name="nowUnix"/> is.</summary>
        public static HarvestTiming TimingOf(PlotState plot, PlantDefinition plant, long nowUnix)
        {
            if (plot == null) return HarvestTiming.Prime;
            double sinceReady = nowUnix - ReadyAtUnix(plot);
            if (sinceReady < 0) return HarvestTiming.Prime;          // not ready (harvest is gated anyway)
            if (sinceReady <= FreshWindow(plant)) return HarvestTiming.Fresh;
            if (sinceReady >= OverripeAfter(plant)) return HarvestTiming.Overripe;
            return HarvestTiming.Prime;
        }

        /// <summary>The yield multiplier from harvest timing: fresh pays a bonus, overripe decays toward
        /// the floor. 1.0 while prime or before ready.</summary>
        public static double TimingMultiplier(PlotState plot, PlantDefinition plant, long nowUnix)
        {
            if (plot == null) return 1.0;
            double sinceReady = nowUnix - ReadyAtUnix(plot);
            if (sinceReady < 0) return 1.0;
            if (sinceReady <= FreshWindow(plant)) return 1.0 + FreshBonus;

            double overAfter = OverripeAfter(plant);
            if (sinceReady < overAfter) return 1.0;

            double t = (sinceReady - overAfter) / OverripeDecaySeconds; // 0..1 across the decay window
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            return 1.0 - (1.0 - OverripeFloor) * t;
        }

        private static bool ToolWorksOnPlant(PlantDefinition plant, ToolDefinition tool)
        {
            List<string> worksOn = tool.worksOn;
            if (worksOn == null || worksOn.Count == 0) return true; // unrestricted
            for (int i = 0; i < worksOn.Count; i++)
            {
                string w = worksOn[i];
                if (string.IsNullOrEmpty(w)) continue;
                if (w == plant.id || (!string.IsNullOrEmpty(plant.biomeId) && w == plant.biomeId)) return true;
            }
            return false;
        }
    }
}
