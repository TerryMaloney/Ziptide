using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// FIELD CAMERA — additive Quarters furnishing without a ship/scene edit. A scene-load hook finds
    /// host-agnostic QuartersRoom instances, adds this feature once, then this component builds one
    /// real grabbable camera dock and one six-photo wall.
    /// </summary>
    public sealed class QuartersCameraFeature : MonoBehaviour
    {
        public const string FeatureRootName = "FieldCameraFeature";
        public const string DockName = "FieldCameraDock";
        public const string WallHostName = "PhotoWallHost";

        private static readonly Color DockColor = new Color(0.18f, 0.22f, 0.25f);
        private static readonly Color LabelColor = new Color(0.35f, 0.85f, 0.90f);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstallSceneHook()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            QuartersRoom[] rooms = Object.FindObjectsOfType<QuartersRoom>();
            for (int i = 0; i < rooms.Length; i++) EnsureOn(rooms[i]);
        }

        public static QuartersCameraFeature EnsureOn(QuartersRoom room)
        {
            if (room == null) return null;
            QuartersCameraFeature existing = room.GetComponent<QuartersCameraFeature>();
            return existing != null ? existing : room.gameObject.AddComponent<QuartersCameraFeature>();
        }

        private void Start()
        {
            BuildFeature();
        }

        public void BuildFeature()
        {
            Transform old = transform.Find(FeatureRootName);
            if (old != null) Destroy(old.gameObject);

            var rootObject = new GameObject(FeatureRootName);
            Transform root = rootObject.transform;
            root.SetParent(transform, false);

            BuildCameraDock(root);
            BuildPhotoWall(root);
        }

        private static void BuildCameraDock(Transform root)
        {
            var dock = new GameObject(DockName);
            dock.transform.SetParent(root, false);
            dock.transform.localPosition = new Vector3(-1.55f, 0f, -1.45f);
            dock.transform.localRotation = Quaternion.Euler(0f, 25f, 0f);

            GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pedestal.name = "Pedestal";
            pedestal.transform.SetParent(dock.transform, false);
            pedestal.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            pedestal.transform.localScale = new Vector3(0.72f, 0.90f, 0.58f);
            ItemFactory.ApplyURPColor(pedestal, DockColor);

            GameObject cradle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cradle.name = "Cradle";
            cradle.transform.SetParent(dock.transform, false);
            cradle.transform.localPosition = new Vector3(0f, 0.96f, 0f);
            cradle.transform.localScale = new Vector3(0.46f, 0.10f, 0.36f);
            ItemFactory.ApplyURPColor(cradle, LabelColor * 0.55f);
            Collider cradleCollider = cradle.GetComponent<Collider>();
            if (cradleCollider != null) cradleCollider.enabled = false;

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(dock.transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 1.28f, -0.31f);
            labelObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = "FIELD CAMERA\nTRIGGER: CAPTURE";
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.fontSize = 48;
            label.characterSize = 0.026f;
            label.color = LabelColor;

            GameObject cameraItem = ItemFactory.Create(
                "handheld_camera",
                dock.transform.TransformPoint(new Vector3(0f, 1.10f, 0f)));
            if (cameraItem == null)
            {
                Debug.LogWarning("ZIPTIDE: FIELD_CAMERA_DOCK_EMPTY reason=item_definition_unavailable");
                return;
            }

            cameraItem.name = "DockedFieldCamera";
            cameraItem.transform.SetParent(dock.transform, true);
            cameraItem.transform.rotation = dock.transform.rotation;
            Debug.Log("ZIPTIDE: FIELD_CAMERA_DOCK_READY");
        }

        private static void BuildPhotoWall(Transform root)
        {
            var host = new GameObject(WallHostName);
            host.transform.SetParent(root, false);
            // Right wall: local +Z of the host faces inward into the cabin.
            host.transform.localPosition = new Vector3(2.22f, 0f, 0f);
            host.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            host.AddComponent<QuartersPhotoWall>();
        }
    }
}
