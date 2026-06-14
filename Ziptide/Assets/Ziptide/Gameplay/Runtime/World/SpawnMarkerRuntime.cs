using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Marks a spawn location for PlayerRigPersistence to find after scene load.
    /// Place one per scene at the desired player arrival point.
    /// </summary>
    public class SpawnMarkerRuntime : MonoBehaviour
    {
        [Tooltip("Identifier for this marker (e.g. 'player').")]
        public string markerId = "player";
    }
}
