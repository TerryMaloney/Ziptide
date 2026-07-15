using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>Additive Quarters furnishing: one real camera dock and one bounded photo wall.</summary>
    public sealed class QuartersCameraFeature : MonoBehaviour
    {
        public const string FeatureRootName = "FieldCameraFeature";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstallSceneHook()
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.QuartersCameraInjector)) return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.QuartersCameraInjector)) return;
            foreach (QuartersRoom room in Object.FindObjectsOfType<QuartersRoom>()) EnsureOn(room);
        }

        public static QuartersCameraFeature EnsureOn(QuartersRoom room)
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.QuartersCameraInjector)) return null;
            if (room == null) return null;
            QuartersCameraFeature existing = room.GetComponent<QuartersCameraFeature>();
            return existing != null ? existing : room.gameObject.AddComponent<QuartersCameraFeature>();
        }

        private void Start() => BuildFeature();

        public void BuildFeature()
        {
            Transform old = transform.Find(FeatureRootName); if (old != null) Destroy(old.gameObject);
            Transform root = new GameObject(FeatureRootName).transform; root.SetParent(transform, false);

            Transform dock = new GameObject("FieldCameraDock").transform;
            dock.SetParent(root, false); dock.localPosition = new Vector3(-1.55f, 0f, -1.45f);
            GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pedestal.name = "Pedestal"; pedestal.transform.SetParent(dock, false);
            pedestal.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            pedestal.transform.localScale = new Vector3(0.72f, 0.9f, 0.58f);
            ItemFactory.ApplyURPColor(pedestal, new Color(0.18f, 0.22f, 0.25f));

            GameObject item = ItemFactory.Create("handheld_camera", dock.TransformPoint(new Vector3(0f, 1.05f, 0f)));
            if (item != null) { item.name = "DockedFieldCamera"; item.transform.SetParent(dock, true); }
            else Debug.LogWarning("ZIPTIDE: FIELD_CAMERA_DOCK_EMPTY");

            Transform wall = new GameObject("PhotoWallHost").transform;
            wall.SetParent(root, false); wall.localPosition = new Vector3(2.22f, 0f, 0f);
            wall.localRotation = Quaternion.Euler(0f, -90f, 0f);
            wall.gameObject.AddComponent<QuartersPhotoWall>();
            Debug.Log("ZIPTIDE: FIELD_CAMERA_QUARTERS_READY");
        }
    }
}
