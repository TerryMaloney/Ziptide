#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Gameplay;

namespace Ziptide.Editor
{
    /// <summary>
    /// Idempotent W000 surface author. Stable markers carry runtime components; no prefab or scene
    /// YAML is edited by hand. Safe for MenuItem or Unity -executeMethod batchmode use.
    /// </summary>
    public static class FirstHourSurfaceAuthor
    {
        public const string ComfortMarker = "__FIRST_HOUR_COMFORT_CONSOLE";
        public const string BunkMarker = "__FIRST_HOUR_BUNK_OBJECT";
        public const string HelmMarker = "__FIRST_HOUR_FIRST_HELM";
        public const string PortholeMarker = "__FIRST_HOUR_PORTHOLE";
        public const string W000ScenePath = "Assets/Ziptide/Scenes/Generated/W000_DriftIn.unity";

        [MenuItem("Ziptide/First Hour/Author W000 Surfaces")]
        public static void AuthorW000()
        {
            Scene scene = EditorSceneManager.OpenScene(W000ScenePath, OpenSceneMode.Single);
            Author(scene);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("ZIPTIDE: FIRST_HOUR_SURFACES_SAVED scene=" + scene.name);
        }

        public static void Author(Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded)
                throw new System.ArgumentException("A loaded scene is required.", nameof(scene));

            ShipCastOffRuntime castOff = FindInScene<ShipCastOffRuntime>(scene);
            Transform anchor = castOff != null ? castOff.transform : null;
            Vector3 origin = anchor != null ? anchor.position : Vector3.zero;
            Vector3 right = anchor != null ? anchor.right : Vector3.right;
            Vector3 forward = anchor != null ? anchor.forward : Vector3.forward;
            Quaternion rotation = anchor != null ? anchor.rotation : Quaternion.identity;

            GameObject comfort = EnsureMarker(
                scene,
                ComfortMarker,
                origin - right * 2.2f + forward * 1.2f,
                rotation);
            EnsureComponent<ComfortConsoleRuntime>(comfort);

            GameObject bunk = EnsureMarker(
                scene,
                BunkMarker,
                origin + right * 0.9f + forward * 0.8f + Vector3.up * 0.9f,
                rotation);
            EnsureBunkVisual(bunk);
            EnsureComponent<FirstHourBunkObjectRuntime>(bunk);

            GameObject helm = EnsureMarker(
                scene,
                HelmMarker,
                origin - right * 3.4f - forward * 1.1f,
                rotation);
            var helmRuntime = EnsureComponent<FirstDestinationHelmRuntime>(helm);
            if (castOff != null) helmRuntime.Configure(castOff);

            // THE PORTHOLE: the first hour opened inside a spaceship you could not see out of, so
            // the premise of the game was invisible for its first ten minutes. Set into the wall
            // past the bunk and turned to face the room, at standing eye height — the first thing
            // worth walking toward after you get up.
            GameObject porthole = EnsureMarker(
                scene,
                PortholeMarker,
                origin + right * 2.6f + forward * 1.6f + Vector3.up * 1.5f,
                Quaternion.LookRotation(-right, Vector3.up));
            EnsureComponent<PortholeRuntime>(porthole);

            Debug.Log("ZIPTIDE: FIRST_HOUR_SURFACES_AUTHORED scene=" + scene.name +
                      " castoff=" + (castOff != null) + " porthole=true");
        }

        public static GameObject EnsureMarker(
            Scene scene,
            string markerName,
            Vector3 position,
            Quaternion rotation)
        {
            GameObject existing = FindNamedInScene(scene, markerName);
            if (existing != null) return existing;

            var marker = new GameObject(markerName);
            SceneManager.MoveGameObjectToScene(marker, scene);
            marker.transform.position = position;
            marker.transform.rotation = rotation;
            return marker;
        }

        private static T EnsureComponent<T>(GameObject go) where T : Component
        {
            T existing = go.GetComponent<T>();
            return existing != null ? existing : go.AddComponent<T>();
        }

        private static void EnsureBunkVisual(GameObject bunk)
        {
            Transform existing = bunk.transform.Find("KeepsakeVisual");
            if (existing != null) return;

            var visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "KeepsakeVisual";
            visual.transform.SetParent(bunk.transform, false);
            visual.transform.localScale = new Vector3(0.22f, 0.12f, 0.32f);
            var collider = visual.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);
            // Fully qualified: this file sits in `Ziptide.Editor`, not `Ziptide.Editor.Patching` like
            // the rest of the folder, so the unqualified name does not resolve here.
            Patching.PatchMaterials.Paint(visual, new Color(0.72f, 0.48f, 0.16f));
        }

        private static T FindInScene<T>(Scene scene) where T : Component
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                T found = root.GetComponentInChildren<T>(true);
                if (found != null) return found;
            }
            return null;
        }

        private static GameObject FindNamedInScene(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Transform candidate in root.GetComponentsInChildren<Transform>(true))
                    if (candidate.name == name)
                        return candidate.gameObject;
            }
            return null;
        }
    }
}
#endif
