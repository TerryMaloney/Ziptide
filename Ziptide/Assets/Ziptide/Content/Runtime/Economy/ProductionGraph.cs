using System;
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Content
{
    /// <summary>
    /// META-LOOP D — the PURE abstract production simulation. v1 model: a powered
    /// Processor/BioRefiner/Assembler with a recipe pulls the recipe's costs from the PROFILE
    /// (RewardRouter, RecipeCost) when idle, works durationTicks, then grants the outputs
    /// (RewardRouter, Factory). Conveyors/splitters/bins exist as layout vocabulary and validation
    /// (a working machine must trace to an InputBin and an OutputStation) — their per-item motion is
    /// representative visuals only, never truth. Deterministic; catch-up is capped (anti-exploit,
    /// same law as ProfileEconomy.ResolveWorld).
    /// </summary>
    public static class ProductionGraph
    {
        public const int MaxCatchUpTicks = 2000;   // offline cap — ~2.7h at 5s/tick
        public const double SecondsPerTick = 5.0;

        /// <summary>Structural problems (empty = valid). CODE-token style like WorldSpecValidator.</summary>
        public static List<string> Validate(List<MachineNodeState> nodes,
            Func<string, RecipeDefinition> recipeById, HashSet<string> registeredResources)
        {
            var issues = new List<string>();
            if (nodes == null) return issues;
            var byId = new Dictionary<string, MachineNodeState>();
            foreach (var n in nodes)
            {
                if (n == null || string.IsNullOrEmpty(n.nodeId)) { issues.Add("NODE_ID_EMPTY"); continue; }
                if (byId.ContainsKey(n.nodeId)) issues.Add("NODE_ID_DUP:" + n.nodeId);
                byId[n.nodeId] = n;
            }
            foreach (var n in byId.Values)
            {
                foreach (var up in n.inputNodeIds)
                    if (!byId.ContainsKey(up)) issues.Add("NODE_INPUT_UNKNOWN:" + n.nodeId + "->" + up);
                if (!IsWorker(n.type)) continue;
                if (string.IsNullOrEmpty(n.recipeId)) { issues.Add("MACHINE_NO_RECIPE:" + n.nodeId); continue; }
                var r = recipeById != null ? recipeById(n.recipeId) : null;
                if (r == null) { issues.Add("RECIPE_UNKNOWN:" + n.recipeId); continue; }
                if (r.requiredMachineType != MachineType.None && r.requiredMachineType != n.type)
                    issues.Add("RECIPE_WRONG_MACHINE:" + n.nodeId + ":" + n.recipeId);
                if (registeredResources != null)
                {
                    foreach (var c in r.costs)
                        if (c != null && !string.IsNullOrEmpty(c.resourceId) && !registeredResources.Contains(c.resourceId))
                            issues.Add("RECIPE_INPUT_UNREGISTERED:" + c.resourceId);
                    if (!string.IsNullOrEmpty(r.producesId) && !registeredResources.Contains(r.producesId))
                        issues.Add("RECIPE_OUTPUT_UNREGISTERED:" + r.producesId);
                }
                if (!TracesTo(n, byId, MachineType.InputBin)) issues.Add("MACHINE_NO_INPUT_PATH:" + n.nodeId);
                if (!FeedsAn(n, byId, MachineType.OutputStation)) issues.Add("MACHINE_NO_OUTPUT_PATH:" + n.nodeId);
            }
            return issues;
        }

        /// <summary>Advance the graph by whole ticks. Deterministic; ledger-routed; returns batches
        /// completed. unlock gate: a recipe with an unset unlockFlag is skipped silently.</summary>
        public static int Tick(List<MachineNodeState> nodes, PlayerProfile profile, string worldId,
            Func<string, RecipeDefinition> recipeById, int ticks)
        {
            if (nodes == null || profile == null || recipeById == null || ticks <= 0) return 0;
            if (ticks > MaxCatchUpTicks) ticks = MaxCatchUpTicks;

            int completed = 0;
            for (int t = 0; t < ticks; t++)
            {
                foreach (var n in nodes)
                {
                    if (n == null || !IsWorker(n.type) || !n.powered || string.IsNullOrEmpty(n.recipeId)) continue;
                    var r = recipeById(n.recipeId);
                    if (r == null) continue;
                    if (!string.IsNullOrEmpty(r.unlockFlag) && !profile.HasFlag(r.unlockFlag)) continue;
                    int duration = r.durationTicks > 0 ? r.durationTicks
                        : Mathf.Max(1, (int)(r.craftSeconds / SecondsPerTick));

                    if (n.progressTicks <= 0)
                    {
                        // Idle: try to start a batch by paying the inputs (all-or-nothing).
                        if (!RecipeService.CanAfford(profile, r)) continue;
                        RecipeService.TrySpend(profile, r); // routes through RewardRouter.RecipeCost
                        n.progressTicks = duration;
                    }

                    n.progressTicks--;
                    if (n.progressTicks <= 0 && !string.IsNullOrEmpty(r.producesId))
                    {
                        RewardRouter.Grant(profile, LedgerSource.Factory, r.producesId, r.producesAmount,
                            reason: r.id, relatedId: n.nodeId, worldId: worldId);
                        completed++;
                    }
                }
            }
            return completed;
        }

        /// <summary>Offline catch-up: elapsed seconds → capped ticks → Tick. One call on world entry
        /// (TickResolver seam — pairs with ProfileEconomy.ResolveWorld for mines/plots).</summary>
        public static int CatchUp(List<MachineNodeState> nodes, PlayerProfile profile, string worldId,
            Func<string, RecipeDefinition> recipeById, long elapsedSeconds)
        {
            if (elapsedSeconds <= 0) return 0;
            int ticks = (int)Math.Min(MaxCatchUpTicks, elapsedSeconds / (long)SecondsPerTick);
            return Tick(nodes, profile, worldId, recipeById, ticks);
        }

        private static bool IsWorker(MachineType t)
            => t == MachineType.Processor || t == MachineType.BioRefiner || t == MachineType.Assembler;

        private static bool TracesTo(MachineNodeState n, Dictionary<string, MachineNodeState> byId, MachineType goal)
        {
            var seen = new HashSet<string>();
            var stack = new Stack<MachineNodeState>();
            stack.Push(n);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();
                if (cur.type == goal) return true;
                if (!seen.Add(cur.nodeId)) continue;
                foreach (var up in cur.inputNodeIds)
                    if (byId.TryGetValue(up, out var upNode)) stack.Push(upNode);
            }
            return false;
        }

        private static bool FeedsAn(MachineNodeState n, Dictionary<string, MachineNodeState> byId, MachineType goal)
        {
            var seen = new HashSet<string>();
            var stack = new Stack<MachineNodeState>();
            stack.Push(n);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();
                if (cur.type == goal) return true;
                if (!seen.Add(cur.nodeId)) continue;
                foreach (var other in byId.Values)
                    if (other.inputNodeIds.Contains(cur.nodeId)) stack.Push(other);
            }
            return false;
        }
    }
}
