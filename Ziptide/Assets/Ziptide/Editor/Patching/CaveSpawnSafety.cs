#if UNITY_EDITOR
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Device-build safety patch for the generated W011 cave. Cavern art modules are allowed to be
    /// renderer-only, but a player spawn must always have a physical landing surface beneath it.
    /// </summary>
    public static class CaveSpawnSafety
    {
        private const string UndercroftScenePath = "Assets/Scenes/W011_Undercroft.unity";
        private const string SpawnName = "__SPAWN_PLAYER";
        private const string FloorName = "__SPAWN_FLOOR";

        public static void EnsureUndercroftSpawnFloor()
        {
            if (!File.Exists(UndercroftScenePath))
            {
                Debug.LogWarning("[Ziptide] cave spawn safety skipped; scene missing: " + UndercroftScenePath);
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(UndercroftScenePath, OpenSceneMode.Single);
            GameObject spawn = GameObject.Find(SpawnName);
            if (spawn == null)
            {
                Debug.LogWarning("[Ziptide] cave spawn safety skipped; " + SpawnName + " missing");
                return;
            }

            GameObject floor = GameObject.Find(FloorName);
            if (floor == null)
            {
                floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floor.name = FloorName;
            }

            if (floor.scene != scene)
                SceneManager.MoveGameObjectToScene(floor, scene);

            floor.SetActive(true);
            floor.layer = 0; // Default layer: included by Physics.DefaultRaycastLayers.

            // Spawn is authored 0.25m above chamber Y. A 0.2m slab centered 0.35m below the marker
            // puts its top exactly 0.25m below the marker and safely inside the audit's 6m ray.
            Vector3 spawnPos = spawn.transform.position;
            floor.transform.position = new Vector3(spawnPos.x, spawnPos.y - 0.35f, spawnPos.z);
            floor.transform.rotation = Quaternion.identity;
            floor.transform.localScale = new Vector3(4f, 0.2f, 4f);

            BoxCollider collider = floor.GetComponent<BoxCollider>();
            if (collider == null) collider = floor.AddComponent<BoxCollider>();
            collider.enabled = true;
            collider.isTrigger = false;
            collider.center = Vector3.zero;
            collider.size = Vector3.one;
            ItemFactory.ApplyURPColor(floor, new Color(0.18f, 0.17f, 0.16f));

            // Batchmode opens, patches, and immediately audits scenes without an Editor update tick.
            // Keep transform auto-sync enabled for the remainder of this build and force one sync now,
            // otherwise a newly saved collider can exist in scene YAML but be absent from raycasts.
            Physics.autoSyncTransforms = true;
            Physics.SyncTransforms();

            RaycastHit hit;
            bool detected = Physics.Raycast(spawnPos + Vector3.up * 0.2f, Vector3.down, out hit, 6f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            if (!detected)
                Debug.LogError("[Ziptide] cave spawn safety created a collider but physics could not raycast it");
            else
                Debug.Log("[Ziptide] cave spawn safety raycast hit " + hit.collider.name +
                          " distance=" + hit.distance.ToString("F3"));

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, UndercroftScenePath);
            Debug.Log("[Ziptide] cave spawn safety ensured floor under " + SpawnName);
        }
    }
}
#endif