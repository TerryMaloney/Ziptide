using System.Collections.Generic;
using Ziptide.Core;

namespace Ziptide.Content
{
    /// <summary>Why a build-extractor attempt did or didn't succeed.</summary>
    public enum BuildMineStatus
    {
        Success,
        InvalidArgs,    // null profile / world / machine
        InvalidNode,    // null node or no resourceId
        CannotAfford,   // build recipe cost not met
    }

    /// <summary>Outcome of placing an extractor.</summary>
    public struct BuildMineResult
    {
        public BuildMineStatus status;
        public MineState mine;   // the placed mine on success, else null
        public bool Success => status == BuildMineStatus.Success;

        public static BuildMineResult Fail(BuildMineStatus status)
            => new BuildMineResult { status = status, mine = null };
    }

    /// <summary>
    /// Mining v1 — the idle extractor loop: pay a machine's build recipe, place an extractor
    /// (<see cref="MineState"/>) on a resource node in a world, then let it accrue that resource over
    /// time (storage-capped). Pure backend (no scene / MonoBehaviour); production + collection reuse
    /// Core's <see cref="ProfileEconomy"/> / <see cref="IdleEngine"/>, so a live "tick" and an offline
    /// "welcome-back" resolve with the same math. EditMode-testable, no headset.
    ///
    /// Conveyor routing / placement geometry is gameplay (the scene layer); this is the data/economy
    /// half only. Node-reserve depletion is intentionally out of scope for v1 — idle mines are bounded
    /// by storage cap, which is what drives the "come back and collect" loop.
    /// </summary>
    public static class MiningService
    {
        /// <summary>Resolved production rate (resource/second) for an extractor of
        /// <paramref name="machine"/> working <paramref name="node"/>. v1 uses the machine throughput;
        /// per-node richness / tool tiers fold in with BalanceConfig later.</summary>
        public static double ResolveRate(MachineDefinition machine, ResourceNodeDefinition node)
            => machine != null && machine.ratePerSecond > 0 ? machine.ratePerSecond : 0;

        /// <summary>Build an extractor on a node: spends the machine's <c>buildRecipe</c> from the
        /// profile, then adds a <see cref="MineState"/> producing the node's resource to
        /// <paramref name="world"/>. Costs are spent only on success (all-or-nothing).</summary>
        public static BuildMineResult TryBuildMine(
            PlayerProfile profile, WorldState world,
            MachineDefinition machine, ResourceNodeDefinition node, long nowUnix)
        {
            if (profile == null || world == null || machine == null)
                return BuildMineResult.Fail(BuildMineStatus.InvalidArgs);
            if (node == null || string.IsNullOrEmpty(node.resourceId))
                return BuildMineResult.Fail(BuildMineStatus.InvalidNode);

            if (!RecipeService.TrySpend(profile, machine.buildRecipe))
                return BuildMineResult.Fail(BuildMineStatus.CannotAfford);

            if (world.mines == null) world.mines = new List<MineState>();
            var mine = new MineState
            {
                machineId = machine.id,
                resourceId = node.resourceId,
                ratePerSecond = ResolveRate(machine, node),
                stored = 0,
                storageCap = machine.storageCap,
                lastResolvedAtUnix = nowUnix,
            };
            world.mines.Add(mine);
            return new BuildMineResult { status = BuildMineStatus.Success, mine = mine };
        }
    }
}
