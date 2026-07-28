using System;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Marks a spawn location for PlayerRigPersistence. Runtime diagnostics mirror the build audit:
    /// triggers, the floor, the persistent player rig and explicit non-solid wayfinding/sky visuals are
    /// excluded. Content-world obstruction/no-floor states remain blockers; _Boot intentionally has no
    /// world floor.
    /// </summary>
    public class SpawnMarkerRuntime : MonoBehaviour
    {
        [Tooltip("Identifier for this marker (e.g. 'player').")]
        public string markerId = "player";

        private void Start()
        {
            if (markerId != "player") return;

            bool below = TryFindGround(out RaycastHit hit);
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
                if (IsNonSolidWorldVisual(col)) continue;
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

        private bool TryFindGround(out RaycastHit ground)
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 2.5f,
                Vector3.down, 15f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider col = hits[i].collider;
                if (col == null || IsNonSolidWorldVisual(col)) continue;
                if (col.GetComponentInParent<PlayerRigPersistence>() != null) continue;
                if (Vector3.Dot(hits[i].normal, Vector3.up) < 0.55f) continue;
                if (hits[i].point.y > transform.position.y + 0.5f) continue;
                ground = hits[i];
                return true;
            }
            ground = default;
            return false;
        }

        private static bool IsNonSolidWorldVisual(Collider col)
        {
            if (col == null) return true;
            if (col.GetComponentInParent<ObjectiveBeacon>() != null) return true;
            if (col.name.IndexOf("SkySphere", StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }
    }
}
