using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>
    /// Contract for loading pod content by id. Implementations live OUTSIDE Core (e.g. Ziptide.Content)
    /// so the source of pod data — a ScriptableObject registry, a JSON pack, Addressables, etc. — is
    /// swappable without touching gameplay. Gameplay depends on this interface, never on a concrete loader.
    /// </summary>
    public interface IPodLoader
    {
        /// <summary>Ids of every pod this loader can provide. Never null.</summary>
        IReadOnlyList<string> AvailablePodIds { get; }

        /// <summary>Returns the pod with the given id, or null if it is unknown.</summary>
        PodNarrative Load(string podId);

        /// <summary>True if a pod with the given id can be loaded.</summary>
        bool Has(string podId);
    }
}
