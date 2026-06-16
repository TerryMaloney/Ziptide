using Ziptide.Core;

namespace Ziptide.Content
{
    /// <summary>
    /// A runtime, depleting instance of a <see cref="ResourceNodeDefinition"/> (Definition +
    /// Runtime.Init pattern — IL2CPP-safe, no reflection). Plain C#, not a MonoBehaviour, so the
    /// harvest loop is unit-testable without a scene; a gameplay/XR component can wrap one to expose
    /// it in-world.
    /// </summary>
    public class ResourceNode
    {
        public ResourceNodeDefinition Definition { get; private set; }

        /// <summary>Reserve left for finite nodes. Unused for inexhaustible nodes (def.reserve &lt;= 0).</summary>
        public double Remaining { get; private set; }

        public bool IsFinite => Definition != null && Definition.reserve > 0;
        public bool IsExhausted => IsFinite && Remaining <= 0;

        public void Init(ResourceNodeDefinition def)
        {
            Definition = def;
            Remaining = (def != null && def.reserve > 0) ? def.reserve : 0;
        }

        /// <summary>Work this node once with <paramref name="tool"/>: validates the tool, depletes the
        /// reserve (finite nodes), and credits the yield to <paramref name="profile"/>'s inventory.
        /// Returns what happened (see <see cref="HarvestResult"/>).</summary>
        public HarvestResult Harvest(ToolDefinition tool, PlayerProfile profile)
        {
            HarvestResult result = HarvestService.Evaluate(Definition, Remaining, tool);
            if (!result.Success) return result;

            if (IsFinite) Remaining -= result.amount;
            if (result.amount > 0 && profile != null)
                profile.AddResource(result.resourceId, result.amount);
            return result;
        }
    }
}
