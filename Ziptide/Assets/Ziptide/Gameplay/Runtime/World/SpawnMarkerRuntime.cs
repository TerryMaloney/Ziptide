using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Marks a spawn location for PlayerRigPersistence. Runtime diagnostics mirror the build audit:
    /// triggers, the floor and the persistent player rig are excluded, so buriedAtTorso is no longer
    /// a false all-layer CheckSphere result. Content-world obstruction/no-floor states are blockers;
    /// _Boot is an intentional menu/bootstrap scene with no world floor.
    /// </summary>
    public class SpawnMarkerRuntime : MonoBehaviour
    {
        [Tooltip("Identifier for this marker (e.g. 'player').")]
        public string markerId = "player";

        private void Start()
        {
            if (markerId != "player") return;

            bool below = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down,
                out RaycastHit hit, 10f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Collider floor = below ? hit.collider : null;
            float feetY = transform.position.y + 0.2f;
            bool buried = false;
            string obstruction = "NONE";

            Collider[] overlaps = Physics.OverlapSphere(transform.position + Vector3.up * 0.9f, 0.3f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < overlaps.Length; i++)
            {
                Collider col = overlaps[i];
                if (col == null || col == floor) continue;
                if (col.bounds.max.y <= feetY) continue;
                if (col.GetComponentInParent<PlayerRigPersistence>() != null) continue;
                buried = true;
                obstruction = col.name;
                break;
            }

            string sceneName = gameObject.scene.name;
            string line = "ZIPTIDE: SPAWN_AT scene=" + sceneName
                + " pos=" + transform.position.ToString("F2")
                + " groundBelow=" + (below ? hit.distance.ToString("F2") + "m@" + hit.collider.name : "NONE")
                + " buriedAtTorso=" + buried + " obstruction=" + obstruction;

            bool contentWorldNoFloor = !below && sceneName != ZiptideConstants.SceneBoot;
            if (buried || contentWorldNoFloor)
                Debug.LogError("ZIPTIDE: SPAWN_RUNTIME_BLOCKER " + line);
            else
                Debug.Log(line);
        }
    }
}
