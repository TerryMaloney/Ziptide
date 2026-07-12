using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Content.Photo;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// FIELD CAMERA — handheld device shell, live viewfinder, and shutter owner. The heavy render,
    /// file, score, and bounded-album work is delegated to the item-owned PhotoCaptureCamera.
    /// </summary>
    public class CameraRuntime : MonoBehaviour
    {
        private XRGrabInteractable _grab;
        private Transform _lens;
        private Renderer _screen;
        private PhotoCaptureCamera _captureCamera;
        private float _nextShutter;
        private float _flashUntil = -1f;
        private readonly MaterialPropertyBlock _screenBlock = new MaterialPropertyBlock();
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private CameraDefinition Def
        {
            get
            {
                var rt = GetComponent<ItemRuntime>();
                return rt != null ? rt.Definition as CameraDefinition : null;
            }
        }

        public Transform Lens => _lens;
        public PhotoCaptureCamera CaptureOwner => _captureCamera;

        private void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _lens = transform.Find("Lens");
            BuildBody();
            _captureCamera = gameObject.GetComponent<PhotoCaptureCamera>();
            if (_captureCamera == null) _captureCamera = gameObject.AddComponent<PhotoCaptureCamera>();
            _captureCamera.Initialize(_lens, _screen, Def);
        }

        private void OnEnable()
        {
            if (_grab != null) _grab.activated.AddListener(OnActivated);
        }

        private void OnDisable()
        {
            if (_grab != null) _grab.activated.RemoveListener(OnActivated);
        }

        private void OnActivated(ActivateEventArgs args)
        {
            Capture(args.interactorObject as XRBaseControllerInteractor);
        }

        /// <summary>Shutter cadence + haptic + screen flash around the real PNG capture.</summary>
        public bool Capture(XRBaseControllerInteractor hand)
        {
            if (Time.time < _nextShutter) return false;
            CameraDefinition def = Def;
            _nextShutter = Time.time +
                           (def != null && def.shutterCooldown > 0f ? def.shutterCooldown : 0.6f);

            if (_captureCamera == null ||
                !_captureCamera.TryCapture(out CapturedPhoto captured, out PhotoScore score))
            {
                Debug.LogWarning("ZIPTIDE: PHOTO_SHUTTER_FAIL dir=" +
                                 (_lens != null ? _lens.forward.ToString("F2") : "?"));
                return false;
            }

            _flashUntil = Time.time + 0.12f;
            if (hand != null) hand.SendHapticImpulse(0.5f, 0.05f);
            Debug.Log("ZIPTIDE: PHOTO_SHUTTER rating=" + score.Rating +
                      " score=" + score.Total +
                      " file=" + (captured != null ? captured.file : "?"));
            return true;
        }

        private void BuildBody()
        {
            if (_lens == null)
            {
                var lensGo = new GameObject("Lens");
                lensGo.transform.SetParent(transform, false);
                lensGo.transform.localPosition = new Vector3(0f, 0f, 0.03f);
                _lens = lensGo.transform;
            }

            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.name = "LensBarrel";
            Collider barrelCollider = barrel.GetComponent<Collider>();
            if (barrelCollider != null) Destroy(barrelCollider);
            barrel.transform.SetParent(_lens, false);
            barrel.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            barrel.transform.localScale = new Vector3(0.035f, 0.02f, 0.035f);
            ItemFactory.ApplyURPColor(barrel, new Color(0.05f, 0.05f, 0.06f));

            var screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screen.name = "Viewfinder";
            Collider screenCollider = screen.GetComponent<Collider>();
            if (screenCollider != null) Destroy(screenCollider);
            screen.transform.SetParent(transform, false);
            screen.transform.localPosition = new Vector3(0f, 0f, -0.028f);
            screen.transform.localScale = new Vector3(0.06f, 0.045f, 0.004f);
            ItemFactory.ApplyURPColor(
                screen,
                Def != null ? Def.viewfinderTint : new Color(0.10f, 0.12f, 0.14f, 1f));
            _screen = screen.GetComponent<Renderer>();

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                if (renderer != null)
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void Update()
        {
            if (_screen == null) return;
            Color tint = _flashUntil > Time.time ? Color.white : Color.white;
            _screen.GetPropertyBlock(_screenBlock);
            if (_screen.sharedMaterial != null && _screen.sharedMaterial.HasProperty(BaseColorId))
                _screenBlock.SetColor(BaseColorId, tint);
            if (_screen.sharedMaterial != null && _screen.sharedMaterial.HasProperty(ColorId))
                _screenBlock.SetColor(ColorId, tint);
            _screen.SetPropertyBlock(_screenBlock);
            if (_flashUntil > 0f && Time.time >= _flashUntil) _flashUntil = -1f;
        }
    }
}
