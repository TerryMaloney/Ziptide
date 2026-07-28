using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Distant "go here" light pillar. It is wayfinding, never cockpit/interactable geometry: once the
    /// tracked head reaches the objective the pillar hides, preventing vehicle and machine beacons from
    /// becoming tall glowing obstructions at arm's length.
    /// </summary>
    public class ObjectiveBeacon : MonoBehaviour
    {
        public const float CloseHideDistance = 5f;

        private Transform _pillar;
        private Material _mat;
        private Color _color;
        private bool _closeHidden;

        public static ObjectiveBeacon Attach(GameObject host, Color color, float height = 14f)
        {
            if (host == null) return null;
            ObjectiveBeacon existing = host.GetComponentInChildren<ObjectiveBeacon>();
            if (existing != null) return existing;

            GameObject go = new GameObject("__ObjectiveBeacon");
            go.transform.SetParent(host.transform, false);
            ObjectiveBeacon beacon = go.AddComponent<ObjectiveBeacon>();
            beacon.Build(color, height);
            Debug.Log("ZIPTIDE: BEACON_TARGET host=" + host.name + " scene=" + host.scene.name);
            return beacon;
        }

        private void Build(Color color, float height)
        {
            _color = color;
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = "Beam";
            Destroy(pillar.GetComponent<Collider>());
            pillar.transform.SetParent(transform, false);
            pillar.transform.localPosition = new Vector3(0f, height * 0.5f, 0f);
            pillar.transform.localScale = new Vector3(0.22f, height * 0.5f, 0.22f);
            _pillar = pillar.transform;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            _mat = shader != null ? new Material(shader) : null;
            if (_mat != null)
            {
                _mat.SetColor("_BaseColor", color);
                pillar.GetComponent<Renderer>().sharedMaterial = _mat;
            }
            pillar.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void Update()
        {
            if (_pillar == null) return;
            Camera cam = Camera.main;
            bool shouldHide = cam != null
                && (cam.transform.position - transform.position).sqrMagnitude
                    <= CloseHideDistance * CloseHideDistance;
            if (shouldHide != _closeHidden)
            {
                _closeHidden = shouldHide;
                _pillar.gameObject.SetActive(!shouldHide);
                Debug.Log("ZIPTIDE: BEACON_CLOSE_HIDE host=" + transform.parent.name
                    + " hidden=" + shouldHide.ToString().ToLowerInvariant());
            }
            if (_closeHidden || _mat == null) return;

            float pulse = 0.75f + 0.25f * Mathf.Sin(Time.time * 2.2f);
            _mat.SetColor("_BaseColor", _color * pulse);
            Vector3 s = _pillar.localScale;
            s.x = s.z = 0.22f * (0.9f + 0.1f * pulse);
            _pillar.localScale = s;
        }

        private void OnDestroy()
        {
            if (_mat != null) Destroy(_mat);
        }
    }
}
