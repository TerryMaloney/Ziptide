using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Slow, visible, non-lethal projectile fired by a combat drone. Travels in a straight line and,
    /// if it reaches the player's head, applies a brief stun via <see cref="PlayerStunReceiver"/>.
    /// Deterministic manual integration — does NOT damage drones or react to world geometry.
    /// </summary>
    public class StunBolt : MonoBehaviour
    {
        private Vector3 _velocity;
        private float _stunSeconds;
        private float _slowFactor;
        private float _life = 4f;
        private const float HitRadius = 0.6f;
        private PlayerStunReceiver _receiver;

        public void Init(Vector3 velocity, float stunSeconds, float slowFactor)
        {
            _velocity = velocity;
            _stunSeconds = stunSeconds;
            _slowFactor = slowFactor;
            _receiver = FindObjectOfType<PlayerStunReceiver>();
            BuildVisual();
        }

        private void BuildVisual()
        {
            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = "Bolt";
            ball.transform.SetParent(transform, false);
            ball.transform.localScale = Vector3.one * 0.25f;
            var col = ball.GetComponent<Collider>();
            if (col != null) Destroy(col);
            var r = ball.GetComponent<Renderer>();
            if (r != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
                var mat = new Material(shader);
                var c = new Color(0.3f, 0.9f, 1f);
                mat.color = c;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
                r.material = mat;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        private void Update()
        {
            transform.position += _velocity * Time.deltaTime;
            _life -= Time.deltaTime;
            if (_life <= 0f) { Destroy(gameObject); return; }

            if (_receiver == null) _receiver = FindObjectOfType<PlayerStunReceiver>();
            if (_receiver != null && Vector3.Distance(transform.position, _receiver.HitPoint) <= HitRadius)
            {
                _receiver.ApplyStun(_stunSeconds, _slowFactor);
                Destroy(gameObject);
            }
        }
    }
}
