using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The aim line (CONTROL_SCHEME.md "Aim"): a thin ray from the Muzzle to the first hit,
    /// shown only while a ranged gun is HELD BY A CONTROLLER HAND — VR's equivalent of console
    /// stick-aim. Socket selection is deliberately excluded: an XRSocketInteractor selects a
    /// holstered gun too, but a gun on the belt must never paint a laser through the world.
    /// Melee weapons expose a child named Muzzle as their physical tip, so this component disables
    /// itself when MeleeWeaponRuntime is present.
    /// </summary>
    public class GunLaserSight : MonoBehaviour
    {
        private const float MaxRange = 30f;
        private static readonly Color DefaultColor = new Color(1f, 0.35f, 0.25f, 0.8f);

        private Transform _muzzle;
        private LineRenderer _lr;
        private XRGrabInteractable _grab;
        private Color _color = DefaultColor;

        private void Awake()
        {
            DisableForMelee();
        }

        /// <summary>
        /// Alpha 0 keeps the default sight color (ItemDefinition fallback idiom). Init repeats the melee
        /// guard because ItemFactory calls it in every construction context, including EditMode where Unity
        /// does not invoke Awake for a newly added runtime component.
        /// </summary>
        public void Init(Color color)
        {
            if (DisableForMelee()) return;
            if (color.a > 0.01f) _color = color;
        }

        private bool DisableForMelee()
        {
            if (GetComponent<MeleeWeaponRuntime>() == null) return false;
            enabled = false;
            if (_lr != null) _lr.enabled = false;
            return true;
        }

        private void Start()
        {
            _muzzle = transform.Find("Muzzle");
            _grab = GetComponent<XRGrabInteractable>();
        }

        private void Update()
        {
            bool show = IsSelectedByControllerHand() && _muzzle != null;
            if (!show)
            {
                if (_lr != null) _lr.enabled = false;
                return;
            }
            if (_lr == null) Build();
            _lr.enabled = true;
            Vector3 from = _muzzle.position;
            Vector3 dir = _muzzle.forward;
            Vector3 to = Physics.Raycast(from, dir, out var hit, MaxRange) ? hit.point : from + dir * MaxRange;
            _lr.SetPosition(0, from);
            _lr.SetPosition(1, to);
        }

        /// <summary>
        /// XRGrabInteractable.isSelected is also true while an XRSocketInteractor owns the item.
        /// Require at least one live XRBaseControllerInteractor so belt sockets, restore parenting and
        /// other non-hand selectors never activate the sight.
        /// </summary>
        private bool IsSelectedByControllerHand()
        {
            if (_grab == null || !_grab.isSelected) return false;
            foreach (IXRSelectInteractor interactor in _grab.interactorsSelecting)
            {
                var controller = interactor as XRBaseControllerInteractor;
                if (controller != null && controller.isActiveAndEnabled && controller.gameObject.activeInHierarchy)
                    return true;
            }
            return false;
        }

        private void Build()
        {
            var go = new GameObject("__LaserSight");
            go.transform.SetParent(transform, false);
            _lr = go.AddComponent<LineRenderer>();
            _lr.useWorldSpace = true;
            _lr.positionCount = 2;
            _lr.startWidth = 0.004f;
            _lr.endWidth = 0.002f;
            _lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader != null)
            {
                var mat = new Material(shader);
                mat.SetColor("_BaseColor", _color);
                _lr.material = mat;
            }
        }
    }
}
