using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// FIELD CAMERA — the held device (commit 2): builds the diegetic camera detail (a lens ring at
    /// the front + a viewfinder screen on the back that faces the player) and wires the trigger to
    /// <see cref="Capture"/>. On a shot it clicks (haptic), flashes the screen, and logs
    /// PHOTO_SHUTTER — the capture SEAM. The real render + live viewfinder + saved photo land in the
    /// next commit by filling in <see cref="Capture"/> (a `PhotoCaptureCamera` on the rig will do the
    /// render; `PhotoComposition`/`PhotoAlbum` already exist and are tested). Modeled on the gun
    /// runtimes' `_grab.activated` idiom; the forward child is "Lens" (NOT "Muzzle") so no laser sight.
    /// </summary>
    public class CameraRuntime : MonoBehaviour
    {
        private XRGrabInteractable _grab;
        private Transform _lens;
        private Renderer _screen;
        private float _nextShutter;
        private float _flashUntil = -1f;
        private Color _idleTint = new Color(0.10f, 0.12f, 0.14f, 1f);

        private CameraDefinition Def
        {
            get { var rt = GetComponent<ItemRuntime>(); return rt != null ? rt.Definition as CameraDefinition : null; }
        }

        /// <summary>The lens forward — where the shot points. Public so the capture camera (next
        /// commit) can align to it.</summary>
        public Transform Lens => _lens;

        private void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _lens = transform.Find("Lens");
            var def = Def;
            if (def != null) _idleTint = def.viewfinderTint;
            BuildBody();
        }

        private void OnEnable() { if (_grab != null) _grab.activated.AddListener(OnActivated); }
        private void OnDisable() { if (_grab != null) _grab.activated.RemoveListener(OnActivated); }

        private void OnActivated(ActivateEventArgs args)
            => Capture(args.interactorObject as XRBaseControllerInteractor);

        /// <summary>Take a shot. Commit 2: click + flash + PHOTO_SHUTTER (the wired seam). The next
        /// commit renders the frame, scores it via PhotoComposition, and saves it via PhotoAlbum.</summary>
        public void Capture(XRBaseControllerInteractor hand)
        {
            if (Time.time < _nextShutter) return;
            var def = Def;
            _nextShutter = Time.time + (def != null && def.shutterCooldown > 0f ? def.shutterCooldown : 0.6f);
            _flashUntil = Time.time + 0.12f;
            if (hand != null) hand.SendHapticImpulse(0.5f, 0.05f); // the shutter click
            Debug.Log("ZIPTIDE: PHOTO_SHUTTER dir=" +
                      (_lens != null ? _lens.forward.ToString("F2") : "?"));
        }

        private void BuildBody()
        {
            // Lens ring at the front (points along the body's forward). If ItemFactory didn't place a
            // "Lens" child, make one so the device always has a forward reference.
            if (_lens == null)
            {
                var lensGo = new GameObject("Lens");
                lensGo.transform.SetParent(transform, false);
                lensGo.transform.localPosition = new Vector3(0f, 0f, 0.03f);
                _lens = lensGo.transform;
            }
            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.name = "LensBarrel";
            var bc = barrel.GetComponent<Collider>();
            if (bc != null) Destroy(bc);
            barrel.transform.SetParent(_lens, false);
            barrel.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // cylinder axis → forward
            barrel.transform.localScale = new Vector3(0.035f, 0.02f, 0.035f);
            ItemFactory.ApplyURPColor(barrel, new Color(0.05f, 0.05f, 0.06f));

            // Viewfinder screen on the BACK of the body, facing the player when held to the eye.
            var screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screen.name = "Viewfinder";
            var sc = screen.GetComponent<Collider>();
            if (sc != null) Destroy(sc);
            screen.transform.SetParent(transform, false);
            screen.transform.localPosition = new Vector3(0f, 0f, -0.028f);
            screen.transform.localScale = new Vector3(0.06f, 0.045f, 0.004f);
            ItemFactory.ApplyURPColor(screen, _idleTint);
            _screen = screen.GetComponent<Renderer>();

            foreach (var r in GetComponentsInChildren<Renderer>())
                if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void Update()
        {
            if (_screen == null) return;
            // Shutter flash: pop to white on capture, ease back to the idle tint.
            if (_flashUntil > 0f)
            {
                float t = (_flashUntil - Time.time) / 0.12f;
                if (t <= 0f) { ItemFactory.ApplyURPColor(_screen.gameObject, _idleTint); _flashUntil = -1f; }
                else ItemFactory.ApplyURPColor(_screen.gameObject, Color.Lerp(_idleTint, Color.white, t));
            }
        }
    }
}
