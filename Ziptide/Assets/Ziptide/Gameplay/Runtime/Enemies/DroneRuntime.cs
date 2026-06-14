using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Simple hovering drone: bobs in place, can be shocked via IShockable, counts as target.
    /// States: Active (patrol bob), Shocked (drops, disabled), Recover (returns).
    /// </summary>
    public class DroneRuntime : MonoBehaviour, IShockable
    {
        private enum State { Active, Shocked, Recovering }

        [SerializeField] private float bobAmplitude = 0.15f;
        [SerializeField] private float bobSpeed = 1.5f;
        [SerializeField] private Color activeColor = new Color(0.8f, 0.2f, 0.1f);
        [SerializeField] private Color shockedColor = new Color(0.1f, 0.8f, 1f);

        private State _state = State.Active;
        private Vector3 _homePos;
        private float _shockTimer;
        private float _recoverTimer;
        private Renderer _renderer;
        private Material _mat;

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
        }

        private void Update()
        {
            switch (_state)
            {
                case State.Active:
                    float y = _homePos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
                    transform.position = new Vector3(_homePos.x, y, _homePos.z);
                    transform.Rotate(Vector3.up, 30f * Time.deltaTime, Space.World);
                    break;

                case State.Shocked:
                    _shockTimer -= Time.deltaTime;
                    transform.position += Vector3.down * 0.3f * Time.deltaTime;
                    if (_shockTimer <= 0f)
                    {
                        _state = State.Recovering;
                        _recoverTimer = 1.5f;
                    }
                    break;

                case State.Recovering:
                    _recoverTimer -= Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, _homePos, 1.5f * Time.deltaTime);
                    if (_recoverTimer <= 0f || Vector3.Distance(transform.position, _homePos) < 0.05f)
                    {
                        transform.position = _homePos;
                        _state = State.Active;
                        SetColor(activeColor);
                    }
                    break;
            }
        }

        public void Shock(float seconds)
        {
            if (_state == State.Shocked) return;
            _state = State.Shocked;
            _shockTimer = seconds;
            SetColor(shockedColor);
            Debug.Log("ZIPTIDE: DRONE_SHOCKED name=" + gameObject.name);
            OnDroneDisabled?.Invoke(this);
        }

        private void SetColor(Color c)
        {
            if (_mat == null) return;
            if (_mat.HasProperty("_BaseColor")) _mat.SetColor("_BaseColor", c);
            else if (_mat.HasProperty("_Color")) _mat.SetColor("_Color", c);
        }
    }
}
