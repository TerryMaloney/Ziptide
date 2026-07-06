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

        private void Start()
        {
            if (markerId != "player") return;

            // Ground-truth report for spawn bugs (Test Day 1: "stuck halfway under the level" in
            // an arena). One log line answers the three suspects from device logcat alone:
            // is there ground below, how far down, and is the marker buried inside solid geometry?
            bool below = Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down,
                out var hit, 10f);
            bool buried = Physics.CheckSphere(transform.position + Vector3.up * 0.9f, 0.25f);
            Debug.Log("ZIPTIDE: SPAWN_AT scene=" + gameObject.scene.name
                + " pos=" + transform.position.ToString("F2")
                + " groundBelow=" + (below ? hit.distance.ToString("F2") + "m@" + hit.collider.name : "NONE")
                + " buriedAtTorso=" + buried);
        }
    }
}
