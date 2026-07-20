using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Small one-shot XR discovery interaction used by manifest-driven world-improvement modules.
    /// It owns no progression, inventory or economy state: selecting it gives immediate readable feedback,
    /// a bounded haptic confirmation and a diagnostic event. A future game can replace this runtime while
    /// retaining the portable compiler/module contract.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(XRSimpleInteractable))]
    public sealed class WorldDiscoveryNodeRuntime : MonoBehaviour
    {
        [SerializeField] private string nodeId;
        [SerializeField] private Color idleColor = new Color(0.25f, 0.75f, 0.9f, 1f);
        [SerializeField] private Color discoveredColor = new Color(0.95f, 0.78f, 0.28f, 1f);

        private XRSimpleInteractable _interactable;
        private Renderer _renderer;
        private TextMesh _status;
        private bool _discovered;
        private readonly MaterialPropertyBlock _properties = new MaterialPropertyBlock();

        public string NodeId => nodeId;
        public bool IsDiscovered => _discovered;

        public void Configure(string id, Color idle, Color discovered)
        {
            nodeId = string.IsNullOrWhiteSpace(id) ? "discovery" : id;
            idleColor = idle;
            discoveredColor = discovered;
        }

        private void Awake()
        {
            _interactable = GetComponent<XRSimpleInteractable>();
            _renderer = GetComponent<Renderer>();
            _status = GetComponentInChildren<TextMesh>(true);
            if (_interactable.interactionManager == null)
                _interactable.interactionManager = FindObjectOfType<XRInteractionManager>();
            _interactable.selectEntered.AddListener(OnSelected);
            ApplyColor(idleColor);
            SetStatus("TOUCH TO LOG");
        }

        private void OnDestroy()
        {
            if (_interactable != null)
                _interactable.selectEntered.RemoveListener(OnSelected);
        }

        private void OnSelected(SelectEnterEventArgs args)
        {
            if (_discovered) return;
            _discovered = true;
            ApplyColor(discoveredColor);
            SetStatus("SIGNAL LOGGED");

            if (args != null && args.interactorObject is XRBaseControllerInteractor controller)
                controller.SendHapticImpulse(0.32f, 0.07f);

            Debug.Log("ZIPTIDE: WORLD_DISCOVERY scene=" + gameObject.scene.name
                + " node=" + (string.IsNullOrEmpty(nodeId) ? gameObject.name : nodeId));
        }

        private void SetStatus(string value)
        {
            if (_status != null) _status.text = value;
        }

        private void ApplyColor(Color color)
        {
            if (_renderer == null) return;
            _renderer.GetPropertyBlock(_properties);
            _properties.SetColor("_BaseColor", color);
            _properties.SetColor("_Color", color);
            _renderer.SetPropertyBlock(_properties);
        }
    }
}
