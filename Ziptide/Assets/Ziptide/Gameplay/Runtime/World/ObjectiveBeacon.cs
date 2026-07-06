using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The "go here" light pillar (Test Day 1: "it says use the kiosk to start but idk where the
    /// kiosk is — add some sort of intuitive direction help"). A thin vertical beam of unlit color
    /// rising from the target, readable over buildings from anywhere in a district, with a slow
    /// breathing pulse so it reads as a signal rather than architecture. Attach to ANY objective
    /// host; no colliders, one draw call, scene-scoped.
    /// </summary>
    public class ObjectiveBeacon : MonoBehaviour
    {
        private Transform _pillar;
        private Material _mat;
        private Color _color;

        public static ObjectiveBeacon Attach(GameObject host, Color color, float height = 14f)
        {
            if (host == null) return null;
            var existing = host.GetComponentInChildren<ObjectiveBeacon>();
            if (existing != null) return existing;

            var go = new GameObject("__ObjectiveBeacon");
            go.transform.SetParent(host.transform, false);
            var beacon = go.AddComponent<ObjectiveBeacon>();
            beacon.Build(color, height);
            Debug.Log("ZIPTIDE: BEACON_TARGET host=" + host.name + " scene=" + host.scene.name);
            return beacon;
        }

        private void Build(Color color, float height)
        {
            _color = color;
            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = "Beam";
            Destroy(pillar.GetComponent<Collider>());
            pillar.transform.SetParent(transform, false);
            pillar.transform.localPosition = new Vector3(0f, height * 0.5f, 0f);
            pillar.transform.localScale = new Vector3(0.22f, height * 0.5f, 0.22f);
            _pillar = pillar.transform;

            var shader = Shader.Find("Universal Render Pipeline/Unlit");
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
            if (_pillar == null || _mat == null) return;
            float pulse = 0.75f + 0.25f * Mathf.Sin(Time.time * 2.2f);
            _mat.SetColor("_BaseColor", _color * pulse);
            var s = _pillar.localScale;
            s.x = s.z = 0.22f * (0.9f + 0.1f * pulse);
            _pillar.localScale = s;
        }

        private void OnDestroy()
        {
            if (_mat != null) Destroy(_mat);
        }
    }
}
