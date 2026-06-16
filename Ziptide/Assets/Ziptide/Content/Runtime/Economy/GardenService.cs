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

    /// <summary>Outcome of harvesting a plot.</summary>
    public struct HarvestPlantResult
    {
        public HarvestPlantStatus status;
        public double yieldMultiplier; // multiplier actually applied
        public int yieldEntries;       // resource lines credited
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

            double mult = plot.yieldMultiplier > 0 ? plot.yieldMultiplier : 1.0;
            int entries = 0;
            if (plant.harvestYield != null)
            {
                for (int i = 0; i < plant.harvestYield.Count; i++)
                {
                    var y = plant.harvestYield[i];
                    if (y == null || string.IsNullOrEmpty(y.resourceId) || y.amount <= 0) continue;
                    profile.AddResource(y.resourceId, y.amount * mult);
                    entries++;
                }
            }
            plot.harvested = true;
            return new HarvestPlantResult { status = HarvestPlantStatus.Success, yieldMultiplier = mult, yieldEntries = entries };
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
