using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Positions belt anchor at hip height relative to XR Origin camera.
    /// Owns left, center, and right holster socket transforms and their visual markers.
    /// </summary>
    public class BeltRig : MonoBehaviour
    {
        [Tooltip("Height offset from head to hip level.")]
        public float hipHeightOffset = -0.65f;

        [Tooltip("Right hip local offset.")]
        public Vector3 rightHipLocalOffset = new Vector3(0.30f, 0f, -0.02f);

        [Tooltip("Left hip local offset.")]
        public Vector3 leftHipLocalOffset = new Vector3(-0.30f, 0f, -0.02f);

        [Tooltip("Center hip local offset (front).")]
        public Vector3 centerHipLocalOffset = new Vector3(0f, 0f, 0.09f);

        private Transform _cameraOrHead;
        private Transform _holsterRight;
        private Transform _holsterLeft;
        private Transform _holsterCenter;
        private Material _holsterMat;

        private void Start()
        {
            PlayerRigPersistence rig = GetComponentInParent<PlayerRigPersistence>();
            if (rig != null)
            {
                // BeltRig is guaranteed by PlayerRigPersistence and lives on the same persistent root.
                // Use that guaranteed seam to install the device safety owners even if a stale generated
                // Boot scene omitted them. Turn activation itself waits for usable XR input controls.
                PlayerSafetyRuntime.EnsureOnRig(rig.gameObject);
                TurnModeRuntimeAuthority.EnsureOnRig(rig.gameObject);
            }

            Camera cam = GetComponentInParent<Camera>(true);
            if (cam == null) cam = Camera.main;
            if (cam == null) cam = Object.FindFirstObjectByType<Camera>();
            _cameraOrHead = cam != null ? cam.transform : null;
            EnsureVisualsAndSockets();
            UpdateBeltPosition();
            Debug.Log("ZIPTIDE: BELT_READY sockets=3 camera="
                + (_cameraOrHead != null ? _cameraOrHead.name : "NONE"));
        }

        private void LateUpdate()
        {
            UpdateBeltPosition();
        }

        private void UpdateBeltPosition()
        {
            if (_cameraOrHead == null) return;
            transform.position = _cameraOrHead.position + Vector3.up * hipHeightOffset;
            transform.rotation = Quaternion.Euler(0f, _cameraOrHead.eulerAngles.y, 0f);

            if (_holsterRight != null) _holsterRight.localPosition = rightHipLocalOffset;
            if (_holsterLeft != null) _holsterLeft.localPosition = leftHipLocalOffset;
            if (_holsterCenter != null) _holsterCenter.localPosition = centerHipLocalOffset;
        }

        private void EnsureVisualsAndSockets()
        {
            EnsureMaterials();
            _holsterRight = EnsureHolster("HolsterRight", rightHipLocalOffset);
            _holsterLeft = EnsureHolster("HolsterLeft", leftHipLocalOffset);
            _holsterCenter = EnsureHolster("HolsterCenter", centerHipLocalOffset);
        }

        private Transform EnsureHolster(string holsterName, Vector3 localOffset)
        {
            Transform holster = transform.Find(holsterName);
            if (holster == null)
            {
                GameObject go = new GameObject(holsterName);
                go.transform.SetParent(transform, false);
                go.transform.localPosition = localOffset;
                holster = go.transform;
            }

            SphereCollider trigger = holster.GetComponent<SphereCollider>();
            if (trigger == null) trigger = holster.gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 0.20f;

            HolsterSocketInteractor socket = holster.GetComponent<HolsterSocketInteractor>();
            if (socket == null) socket = holster.gameObject.AddComponent<HolsterSocketInteractor>();
            socket.socketActive = true;

            Transform attach = holster.Find("HolsterAttach");
            if (attach == null)
            {
                GameObject attachGo = new GameObject("HolsterAttach");
                attachGo.transform.SetParent(holster, false);
                attach = attachGo.transform;
            }
            attach.localPosition = new Vector3(0f, -0.04f, 0f);
            attach.localRotation = Quaternion.identity;
            socket.attachTransform = attach;

            if (holster.Find("HolsterMarker") == null)
            {
                GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = "HolsterMarker";
                marker.transform.SetParent(holster, false);
                marker.transform.localScale = Vector3.one * 0.06f;
                marker.transform.localPosition = Vector3.zero;
                marker.transform.localRotation = Quaternion.identity;
                marker.GetComponent<Collider>().enabled = false;
                ApplyMarkerRenderer(marker.GetComponent<Renderer>(), _holsterMat);
            }

            return holster;
        }

        private void EnsureMaterials()
        {
            if (_holsterMat != null) return;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            _holsterMat = new Material(shader) { name = "HolsterMarker_Mat" };
            SetBaseColor(_holsterMat, new Color(0.1f, 0.8f, 0.2f, 1f));
        }

        private static void ApplyMarkerRenderer(Renderer r, Material mat)
        {
            if (r == null || mat == null) return;
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;
        }

        private static void SetBaseColor(Material mat, Color c)
        {
            if (mat == null) return;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", c);
        }
    }

    /// <summary>
    /// Shared semantic belt target. It defines where a weapon's muzzle/tip should point in world space;
    /// item root axes are solved separately from the actual Muzzle socket by HolsterSocketInteractor.
    /// </summary>
    public static class HolsterPoseCore
    {
        public static Vector3 ResolveDesiredAxis(Transform socket, string socketName)
        {
            Vector3 outward = Vector3.zero;
            string lower = socketName != null ? socketName.ToLowerInvariant() : string.Empty;
            if (socket != null && lower.Contains("left")) outward = -socket.right;
            else if (socket != null && lower.Contains("right")) outward = socket.right;

            Vector3 back = socket != null ? -socket.forward : Vector3.back;
            return (Vector3.down + outward * 0.16f + back * 0.06f).normalized;
        }

        public static Vector3 ResolveDesiredUp(Transform socket, Vector3 desiredAxis)
        {
            Vector3 preferred = socket != null ? socket.forward : Vector3.forward;
            Vector3 projected = Vector3.ProjectOnPlane(preferred, desiredAxis);
            return projected.sqrMagnitude > 1e-6f ? projected.normalized : Vector3.forward;
        }

        // Compatibility helper for existing callers: this is the socket target basis, not an item-root
        // Euler pose. New code should use ResolveDesiredAxis/ResolveDesiredUp plus the item's Muzzle axis.
        public static Quaternion Resolve(string itemId, string socketName)
        {
            Vector3 outward = socketName != null && socketName.ToLowerInvariant().Contains("left")
                ? Vector3.left : socketName != null && socketName.ToLowerInvariant().Contains("right")
                    ? Vector3.right : Vector3.zero;
            Vector3 axis = (Vector3.down + outward * 0.16f + Vector3.back * 0.06f).normalized;
            Vector3 up = Vector3.ProjectOnPlane(Vector3.forward, axis).normalized;
            return Quaternion.LookRotation(axis, up);
        }
    }
}
