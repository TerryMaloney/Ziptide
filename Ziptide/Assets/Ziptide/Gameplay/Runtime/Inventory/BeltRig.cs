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
            var cam = GetComponentInParent<Camera>(true);
            if (cam == null) cam = Camera.main;
            if (cam == null) cam = Object.FindFirstObjectByType<Camera>();
            _cameraOrHead = cam != null ? cam.transform : null;
            EnsureVisualsAndSockets();
            UpdateBeltPosition();
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
            var holster = transform.Find(holsterName);
            if (holster == null)
            {
                var go = new GameObject(holsterName);
                go.transform.SetParent(transform, false);
                go.transform.localPosition = localOffset;
                holster = go.transform;
            }

            var trigger = holster.GetComponent<SphereCollider>();
            if (trigger == null) trigger = holster.gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 0.16f;

            HolsterSocketInteractor socket = holster.GetComponent<HolsterSocketInteractor>();
            if (socket == null) socket = holster.gameObject.AddComponent<HolsterSocketInteractor>();

            Transform attach = holster.Find("HolsterAttach");
            if (attach == null)
            {
                GameObject attachGo = new GameObject("HolsterAttach");
                attachGo.transform.SetParent(holster, false);
                attach = attachGo.transform;
            }
            attach.localPosition = new Vector3(0f, -0.04f, 0f);
            attach.localRotation = HolsterPoseCore.Resolve("", holsterName);
            socket.attachTransform = attach;

            if (holster.Find("HolsterMarker") == null)
            {
                var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
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
            var shader = Shader.Find("Universal Render Pipeline/Lit");
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

    /// <summary>Pure, shared belt-pose contract. ItemFactory hand poses never leak into holsters.</summary>
    public static class HolsterPoseCore
    {
        public static Quaternion Resolve(string itemId, string socketName)
        {
            float sideRoll = socketName != null && socketName.ToLowerInvariant().Contains("left") ? -8f : 8f;
            if (socketName != null && socketName.ToLowerInvariant().Contains("center")) sideRoll = 0f;

            // All current hip items hang with their long/forward axis down the leg. The blade gets a
            // little more outward cant so it reads as sheathed rather than as a gun barrel on the belt.
            if (itemId == "breaker_blade") return Quaternion.Euler(88f, 0f, sideRoll * 1.5f);
            return Quaternion.Euler(82f, 0f, sideRoll);
        }
    }
}
