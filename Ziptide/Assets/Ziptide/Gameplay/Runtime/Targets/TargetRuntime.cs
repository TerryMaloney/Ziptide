using UnityEngine;
using UnityEngine.Events;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Target that reacts to hits: color change, optional impulse, reset after delay.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TargetRuntime : MonoBehaviour
    {
        [Tooltip("Duration before resetting color (seconds).")]
        public float resetDelay = 1.5f;

        [Tooltip("Optional impulse on hit.")]
        public float hitWobbleForce = 2f;

        [Tooltip("Invoked when Hit() is called (e.g. for job target counting).")]
        public UnityEvent OnHit = new UnityEvent();

        private Renderer _renderer;
        private Material _instanceMat;
        private Color _defaultColor;
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private float _resetAt = -1f;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null && _renderer.material != null)
            {
                _instanceMat = _renderer.material;
                _defaultColor = _instanceMat.HasProperty(BaseColorId) ? _instanceMat.GetColor(BaseColorId) : _instanceMat.color;
            }
            else
                _defaultColor = Color.red;
        }

        private void Update()
        {
            if (_resetAt > 0f && Time.time >= _resetAt)
            {
                _resetAt = -1f;
                SetColor(_defaultColor);
            }
        }

        /// <summary>
        /// Called when hit by pistol (or other hitscan). Applies feedback and schedules reset.
        /// </summary>
        public void Hit(float force, Vector3 point)
        {
            SetColor(Color.green);
            _resetAt = Time.time + resetDelay;

            var rb = GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic && hitWobbleForce > 0f)
            {
                var dir = (transform.position - point).normalized;
                rb.AddForceAtPosition(dir * hitWobbleForce, point, ForceMode.Impulse);
            }

            OnHit?.Invoke();
        }

        private void SetColor(Color c)
        {
            if (_instanceMat != null)
            {
                if (_instanceMat.HasProperty(BaseColorId))
                    _instanceMat.SetColor(BaseColorId, c);
                else
                    _instanceMat.color = c;
            }
        }
    }
}
