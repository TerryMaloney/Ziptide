using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Grabbable melee hammer for PvP: a fast swing whose head touches a <see cref="BreakableWall"/>
    /// escalates it. Auto-returns to its home spot after <see cref="PvpRules.HammerAutoReturnSeconds"/>
    /// left off-hand, so it never gets lost. Self-builds its visual + grab so the patcher just adds it.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class HammerTool : MonoBehaviour
    {
        public float swingSpeed = 1.5f;
        public float headRadius = 0.28f;
        public float breakDebounce = 0.4f;

        private XRGrabInteractable _grab;
        private Transform _head;
        private Vector3 _lastHeadPos;
        private Vector3 _home;
        private Quaternion _homeRot;
        private float _releasedAt = -1f;
        private float _lastBreakAt;

        private void Awake()
        {
            BuildVisual();
            _grab = GetComponent<XRGrabInteractable>();
            var rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            _home = transform.position;
            _homeRot = transform.rotation;
            if (_head != null) _lastHeadPos = _head.position;
        }

        private void Start()
        {
            if (_grab != null && _grab.interactionManager == null)
            {
                var mgr = FindObjectOfType<XRInteractionManager>();
                if (mgr != null) _grab.interactionManager = mgr;
            }
        }

        private void OnEnable()
        {
            if (_grab == null) _grab = GetComponent<XRGrabInteractable>();
            if (_grab != null)
            {
                _grab.selectEntered.AddListener(OnGrabbed);
                _grab.selectExited.AddListener(OnReleased);
            }
        }

        private void OnDisable()
        {
            if (_grab != null)
            {
                _grab.selectEntered.RemoveListener(OnGrabbed);
                _grab.selectExited.RemoveListener(OnReleased);
            }
        }

        private void OnGrabbed(SelectEnterEventArgs _) { _releasedAt = -1f; }
        private void OnReleased(SelectExitEventArgs _) { _releasedAt = Time.time; }

        private void BuildVisual()
        {
            // Root collider (grab volume).
            var box = gameObject.GetComponent<BoxCollider>();
            if (box == null) box = gameObject.AddComponent<BoxCollider>();
            box.size = new Vector3(0.12f, 0.5f, 0.12f);
            box.center = new Vector3(0f, 0.2f, 0f);

            var handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            handle.name = "Handle";
            handle.transform.SetParent(transform, false);
            handle.transform.localPosition = new Vector3(0f, 0.15f, 0f);
            handle.transform.localScale = new Vector3(0.05f, 0.4f, 0.05f);
            DestroyCollider(handle);
            ItemFactory.ApplyURPColor(handle, new Color(0.35f, 0.25f, 0.15f));

            var head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "Head";
            head.transform.SetParent(transform, false);
            head.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            head.transform.localScale = new Vector3(0.2f, 0.12f, 0.14f);
            DestroyCollider(head);
            ItemFactory.ApplyURPColor(head, new Color(0.4f, 0.42f, 0.46f));
            _head = head.transform;
        }

        private static void DestroyCollider(GameObject go)
        {
            var c = go.GetComponent<Collider>();
            if (c != null) Destroy(c);
        }

        private void Update()
        {
            bool held = _grab != null && _grab.isSelected;

            if (_head != null)
            {
                float speed = (_head.position - _lastHeadPos).magnitude / Mathf.Max(1e-4f, Time.deltaTime);
                _lastHeadPos = _head.position;
                if (held && speed > swingSpeed && Time.time - _lastBreakAt > breakDebounce)
                {
                    var hits = Physics.OverlapSphere(_head.position, headRadius);
                    foreach (var h in hits)
                    {
                        var wall = h.GetComponentInParent<BreakableWall>();
                        // Pass the hammer head's world position so the wall damages the brick we actually
                        // struck (localized break at the hit point), not the whole panel.
                        if (wall != null) { wall.HitFromHammer(_head.position); _lastBreakAt = Time.time; break; }
                    }
                }
            }

            // Auto-return after too long off-hand.
            if (!held && _releasedAt >= 0f && Time.time - _releasedAt > (float)PvpRules.HammerAutoReturnSeconds)
            {
                transform.position = _home;
                transform.rotation = _homeRot;
                var rb = GetComponent<Rigidbody>();
                if (rb != null) { rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
                _releasedAt = -1f;
                Debug.Log("ZIPTIDE: PVP_HAMMER_AUTORETURN");
            }
        }
    }
}
