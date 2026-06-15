using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Hovering drone target. When shot (TargetRuntime hit) or shocked (taser) it DIES:
    /// stops hovering, crashes to the ground under gravity, and stays down — it does not
    /// recover or disappear.
    /// </summary>
    public class DroneRuntime : MonoBehaviour, IShockable
    {
        private enum State { Active, Dead }

        [SerializeField] private float bobAmplitude = 0.15f;
        [SerializeField] private float bobSpeed = 1.5f;
        [SerializeField] private Color activeColor = new Color(0.8f, 0.2f, 0.1f);
        [SerializeField] private Color deadColor = new Color(0.15f, 0.15f, 0.15f);

        private State _state = State.Active;
        private Vector3 _homePos;
        private Renderer _renderer;
        private Material _mat;
        private TargetRuntime _target;

        public static event System.Action<DroneRuntime> OnDroneDisabled;

        private void Awake()
        {
            _homePos = transform.position;
            _renderer = GetComponentInChildren<Renderer>();
            if (_renderer != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                if (shader != null)
                {
                    _mat = new Material(shader);
                    SetColor(activeColor);
                    _renderer.material = _mat;
                    _renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }

            // Die from the pistol/hitscan too: a TargetRuntime hit kills the drone.
            _target = GetComponent<TargetRuntime>();
            if (_target != null)
                _target.OnHit.AddListener(Kill);
        }

        private void OnDestroy()
        {
            if (_target != null) _target.OnHit.RemoveListener(Kill);
        }

        private void Update()
        {
            if (_state != State.Active) return;
            float y = _homePos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            transform.position = new Vector3(_homePos.x, y, _homePos.z);
            transform.Rotate(Vector3.up, 30f * Time.deltaTime, Space.World);
        }

        public void Shock(float seconds) => Kill();

        /// <summary>Kill the drone: stop hovering, crash to the ground under gravity, stay down.</summary>
        public void Kill()
        {
            if (_state == State.Dead) return;
            _state = State.Dead;
            SetColor(deadColor);
            Debug.Log("ZIPTIDE: DRONE_DOWN name=" + gameObject.name);
            OnDroneDisabled?.Invoke(this);

            if (GetComponent<Collider>() == null && GetComponentInChildren<Collider>() == null)
                gameObject.AddComponent<BoxCollider>();

            var rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse); // tumble as it falls
            rb.WakeUp();
        }

        private void SetColor(Color c)
        {
            if (_mat == null) return;
            if (_mat.HasProperty("_BaseColor")) _mat.SetColor("_BaseColor", c);
            else if (_mat.HasProperty("_Color")) _mat.SetColor("_Color", c);
        }
    }
}
