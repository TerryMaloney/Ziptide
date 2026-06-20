using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>What a scan blip represents — drives blip color/icon on the wrist radar.</summary>
    public enum ScanKind { Enemy, Objective, Loot, Node }

    /// <summary>
    /// Anything the Wrist Scanner can detect: the PvP opponent now, and (campaign) hidden nodes, loot,
    /// objectives later. One interface so the same premium device is reused everywhere instead of a
    /// one-off PvP gadget.
    /// </summary>
    public interface IScannable
    {
        Transform ScanTransform { get; }
        ScanKind ScanKind { get; }
        bool ScanActive { get; }
    }
}
