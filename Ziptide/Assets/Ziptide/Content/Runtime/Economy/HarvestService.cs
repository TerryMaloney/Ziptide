using System.Collections.Generic;

namespace Ziptide.Content
{
    /// <summary>Why a harvest attempt did or didn't produce resources.</summary>
    public enum HarvestStatus
    {
        Success,
        InvalidNode,        // null node / no resourceId
        InvalidTool,        // null tool
        WrongToolFunction,  // tool.function != node.requiredFunction
        ToolTierTooLow,     // tool.tier < node.requiredToolTier
        ToolCannotWorkNode, // tool.worksOn restricts to other ids
        NodeExhausted,      // finite node with no reserve left
    }

    /// <summary>Outcome of a single harvest attempt.</summary>
    public struct HarvestResult
    {
        public HarvestStatus status;
        public string resourceId;
        public double amount;   // amount granted (after tool power + reserve clamp)
        public bool Success => status == HarvestStatus.Success;

        public static HarvestResult Fail(HarvestStatus status, string resourceId = null)
            => new HarvestResult { status = status, resourceId = resourceId, amount = 0 };
    }

    /// <summary>
    /// Harvest v1 — the simplest economy loop: work a resource node with a suitable tool to produce its
    /// resource. Pure (no Unity scene objects / MonoBehaviours), so the gate + math are fully
    /// EditMode/CI-testable with no headset. Stateful reserve depletion + profile crediting live on
    /// <see cref="ResourceNode"/>; this service is the stateless decision layer.
    /// </summary>
    public static class HarvestService
    {
        /// <summary>Whether <paramref name="tool"/> may work <paramref name="node"/> (function + tier +
        /// worksOn gate). Returns the failing reason, or <see cref="HarvestStatus.Success"/>.</summary>
        public static HarvestStatus CheckTool(ResourceNodeDefinition node, ToolDefinition tool)
        {
            if (node == null || string.IsNullOrEmpty(node.resourceId)) return HarvestStatus.InvalidNode;
            if (tool == null) return HarvestStatus.InvalidTool;
            if (tool.function != node.requiredFunction) return HarvestStatus.WrongToolFunction;
            if (tool.tier < node.requiredToolTier) return HarvestStatus.ToolTierTooLow;
            if (!ToolWorksOnNode(node, tool)) return HarvestStatus.ToolCannotWorkNode;
            return HarvestStatus.Success;
        }

        /// <summary>Pure preview of a single harvest of a node holding <paramref name="remaining"/>
        /// reserve with <paramref name="tool"/>. No mutation. amount = yieldPerHarvest * tool.power,
        /// clamped to the remaining reserve for finite nodes.</summary>
        public static HarvestResult Evaluate(ResourceNodeDefinition node, double remaining, ToolDefinition tool)
        {
            HarvestStatus gate = CheckTool(node, tool);
            if (gate != HarvestStatus.Success)
                return HarvestResult.Fail(gate, node != null ? node.resourceId : null);

            bool finite = node.reserve > 0;
            if (finite && remaining <= 0)
                return HarvestResult.Fail(HarvestStatus.NodeExhausted, node.resourceId);

            double power = tool.power > 0 ? tool.power : 0;
            double amount = node.yieldPerHarvest * power;
            if (amount < 0) amount = 0;
            if (finite && amount > remaining) amount = remaining;

            return new HarvestResult { status = HarvestStatus.Success, resourceId = node.resourceId, amount = amount };
        }

        private static bool ToolWorksOnNode(ResourceNodeDefinition node, ToolDefinition tool)
        {
            List<string> worksOn = tool.worksOn;
            if (worksOn == null || worksOn.Count == 0) return true; // unrestricted tool

            for (int i = 0; i < worksOn.Count; i++)
            {
                string w = worksOn[i];
                if (string.IsNullOrEmpty(w)) continue;
                if (w == node.resourceId || w == node.id ||
                    (!string.IsNullOrEmpty(node.biomeId) && w == node.biomeId))
                    return true;
            }
            return false;
        }
    }
}
