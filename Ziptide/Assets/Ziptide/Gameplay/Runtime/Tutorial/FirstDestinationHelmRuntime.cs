using System;
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
            BuildSurface();
        }

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

            var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pedestal.name = "FirstHelmPedestal";
            pedestal.transform.SetParent(transform, false);
            pedestal.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            pedestal.transform.localScale = new Vector3(0.75f, 1.3f, 0.45f);
            ItemFactory.ApplyURPColor(pedestal, new Color(0.12f, 0.18f, 0.24f));

            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "Tile_W001_TOXIC_CITY";
            tile.transform.SetParent(pedestal.transform, false);
            tile.transform.localPosition = new Vector3(0f, 0.25f, -0.58f);
            tile.transform.localScale = new Vector3(0.76f, 0.28f, 0.12f);
            ItemFactory.ApplyURPColor(tile, new Color(0.18f, 0.65f, 0.82f));

            var interactable = tile.AddComponent<XRSimpleInteractable>();
            var manager = FindObjectOfType<XRInteractionManager>();
            if (manager != null) interactable.interactionManager = manager;
            interactable.selectEntered.AddListener(_ => SelectW001());

            AddLabel(tile.transform, "W001\nTOXIC CITY", new Vector3(0f, 0f, -0.56f), 0.024f);
        }

        private static void AddLabel(Transform parent, string text, Vector3 localPosition, float size)
        {
            var go = new GameObject("Label_W001");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = size;
            tm.fontSize = 64;
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
