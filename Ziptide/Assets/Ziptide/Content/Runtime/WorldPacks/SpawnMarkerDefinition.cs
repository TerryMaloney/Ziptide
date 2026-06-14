using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Minimal spawn point definition: id and local pose for use in a world pack.
    /// </summary>
    [Serializable]
    public class SpawnMarkerDefinition
    {
        [Tooltip("Unique id referenced by job steps (e.g. GoToMarker).")]
        public string markerId = "spawn";

        [Tooltip("Local position relative to world origin or pack anchor.")]
        public Vector3 localPosition = Vector3.zero;

        [Tooltip("Local euler angles (degrees).")]
        public Vector3 localEulerAngles = Vector3.zero;
    }
}
