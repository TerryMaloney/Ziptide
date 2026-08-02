using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Minimal first-destination helm. It exposes only W001/ToxicCity and delegates the selected
    /// destination into ShipCastOffRuntime; it never launches or calls TravelCoordinator itself.
    /// </summary>
    public sealed class FirstDestinationHelmRuntime : MonoBehaviour
    {
        public static event Action<string> FirstDestinationSelected;

        private ShipCastOffRuntime _castOff;
        private bool _built;

        public void Configure(ShipCastOffRuntime castOff)
        {
            _castOff = castOff;
        }

        private void Start()
        {
            if (_castOff == null) _castOff = FindObjectOfType<ShipCastOffRuntime>();
            StartCoroutine(BuildWhenTheConsoleExists());
        }

        /// <summary>
        /// DOCK TO THE LAUNCH CONTROL, don't float beside the hull.
        ///
        /// The 2026-08-01 device pass called this "a random toxic city button on the side of the
        /// ship", and it was: FirstHourSurfaceAuthor anchors this marker off the hull ROOT, 3.4 m to
        /// port at hull-centre height, while the thing it drives — PUNCH IT — lives up on the cockpit
        /// deck. So the player pressed a destination tile seven times (FIRST_HELM_SELECTED x7 in the
        /// log) and nothing ever launched, because the launch button was somewhere else entirely.
        ///
        /// ShipCastOffRuntime builds its console in Start as well, so the order is not ours to
        /// assume; wait a bounded number of frames for its anchor, then dock beside it. If there is
        /// no console at all (test ships, non-boardable berths) build in place as before.
        /// </summary>
        private IEnumerator BuildWhenTheConsoleExists()
        {
            for (int frame = 0; frame < ConsoleWaitFrames; frame++)
            {
                if (_castOff == null) _castOff = FindObjectOfType<ShipCastOffRuntime>();
                if (_castOff != null && _castOff.ConsoleAnchor != null)
                {
                    Transform anchor = _castOff.ConsoleAnchor;
                    transform.SetParent(anchor, false);
                    transform.localPosition = HelmDockOffset;
                    transform.localRotation = Quaternion.identity;
                    Debug.Log("ZIPTIDE: FIRST_HELM_DOCKED anchor=console");
                    BuildSurface();
                    yield break;
                }
                yield return null;
            }

            Debug.LogWarning("ZIPTIDE: FIRST_HELM_DOCKED anchor=none reason=no_castoff_console");
            BuildSurface();
        }

        private const int ConsoleWaitFrames = 8;

        /// <summary>
        /// One row BELOW the launch tile on the same console face: pick a destination, then punch it.
        /// Derived from ShipCastOffRuntime.ButtonLocalPos so the two rows cannot drift apart.
        /// </summary>
        private static Vector3 HelmDockOffset =>
            ShipCastOffRuntime.ButtonLocalPos - new Vector3(0f, 0.34f, 0f);

        public bool SelectW001()
        {
            if (_castOff == null) _castOff = FindObjectOfType<ShipCastOffRuntime>();
            if (_castOff == null)
            {
                Debug.LogWarning("ZIPTIDE: FIRST_HELM_MISSING_CASTOFF");
                return false;
            }

            if (!_castOff.SelectFirstDestination(ZiptideConstants.SceneToxicCity)) return false;

            const string semanticDestination = "W001_ToxicCity";
            Debug.Log("ZIPTIDE: FIRST_HELM_SELECTED dest=" + semanticDestination);
            PublishSafely(FirstDestinationSelected, semanticDestination, "helm");
            return true;
        }

        private void BuildSurface()
        {
            if (_built) return;
            _built = true;

            // Everything hangs off THIS transform, which is unscaled. The old build parented the
            // label to a (0.76, 0.28, 0.12) tile inside a (0.75, 1.3, 0.45) pedestal, so the glyphs
            // came out stretched ~1.6:1 — half of the "glitchy looking text" on device.
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "Tile_W001_TOXIC_CITY";
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = Vector3.zero;
            tile.transform.localScale = new Vector3(0.36f, 0.12f, 0.06f);
            ItemFactory.ApplyURPColor(tile, new Color(0.18f, 0.65f, 0.82f));

            var interactable = tile.AddComponent<XRSimpleInteractable>();
            var manager = FindObjectOfType<XRInteractionManager>();
            if (manager != null) interactable.interactionManager = manager;
            interactable.selectEntered.AddListener(_ => SelectW001());

            // Same signage numbers as the launch tile above it (fontSize 48 / characterSize 0.014,
            // ~6.7 cm a line), sitting proud of the plate so glyphs never render inside it.
            AddLabel(transform, "W001\nTOXIC CITY", new Vector3(0f, 0.155f, -0.05f), 0.014f);
        }

        private static void AddLabel(Transform parent, string text, Vector3 localPosition, float size)
        {
            var go = new GameObject("Label_W001");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = size;
            tm.fontSize = 48;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = new Color(0.95f, 0.98f, 1f);
        }

        private static void PublishSafely(Action<string> subscribers, string value, string phase)
        {
            if (subscribers == null) return;
            foreach (Action<string> subscriber in subscribers.GetInvocationList())
            {
                try { subscriber(value); }
                catch (Exception ex)
                {
                    Debug.LogWarning("ZIPTIDE: FIRST_HOUR_SURFACE_SUBSCRIBER_FAIL phase=" + phase +
                                     " reason=" + ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Thin semantic observer for one named grabbable bunk keepsake. XRGrabInteractable remains the
    /// grab owner; this component only publishes the first successful select once.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FirstHourBunkObjectRuntime : MonoBehaviour
    {
        public const string ObjectId = "bunk_keepsake";
        public static event Action<string> NamedBunkObjectGrabbed;

        private bool _published;

        private void Start()
        {
            if (GetComponent<Collider>() == null) gameObject.AddComponent<BoxCollider>();

            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            var grab = GetComponent<XRGrabInteractable>();
            if (grab == null) grab = gameObject.AddComponent<XRGrabInteractable>();
            var manager = FindObjectOfType<XRInteractionManager>();
            if (manager != null) grab.interactionManager = manager;
            grab.selectEntered.AddListener(_ => NotifyGrabbed());

            if (GetComponentInChildren<TextMesh>() == null)
            {
                var label = new GameObject("Label_BUNK_KEEPSAKE");
                label.transform.SetParent(transform, false);
                label.transform.localPosition = Vector3.up * 0.22f;
                var tm = label.AddComponent<TextMesh>();
                tm.text = "BUNK KEEPSAKE";
                tm.characterSize = 0.025f;
                tm.fontSize = 48;
                tm.anchor = TextAnchor.MiddleCenter;
                tm.alignment = TextAlignment.Center;
                tm.color = new Color(0.9f, 0.75f, 0.35f);
            }
        }

        public void NotifyGrabbed()
        {
            if (_published) return;
            _published = true;
            Debug.Log("ZIPTIDE: FIRST_HOUR_BUNK_GRAB id=" + ObjectId);
            PublishSafely(NamedBunkObjectGrabbed, ObjectId);
        }

        private static void PublishSafely(Action<string> subscribers, string value)
        {
            if (subscribers == null) return;
            foreach (Action<string> subscriber in subscribers.GetInvocationList())
            {
                try { subscriber(value); }
                catch (Exception ex)
                {
                    Debug.LogWarning("ZIPTIDE: FIRST_HOUR_SURFACE_SUBSCRIBER_FAIL phase=bunk reason=" +
                                     ex.Message);
                }
            }
        }
    }
}
