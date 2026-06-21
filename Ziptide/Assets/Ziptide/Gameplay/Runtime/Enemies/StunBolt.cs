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
            Vector3 step = _velocity * Time.deltaTime;
            float d = step.magnitude;

            // Block on walls: if the bolt's step crosses solid geometry (not the firing drone or the
            // player), it's absorbed — no shooting through cover even after it's left the muzzle.
            if (d > 0.0001f && Physics.Raycast(transform.position, step / d, out var hit, d, ~0, QueryTriggerInteraction.Ignore))
            {
                var col = hit.collider;
                bool isDrone = col != null && col.GetComponentInParent<DroneRuntime>() != null;
                bool isPlayer = col != null && IsRig(col.transform);
                if (col != null && !isDrone && !isPlayer) { Destroy(gameObject); return; }
            }

            transform.position += step;
            _life -= Time.deltaTime;
            if (_life <= 0f) { Destroy(gameObject); return; }

            if (_receiver == null) _receiver = FindObjectOfType<PlayerStunReceiver>();
            if (_receiver != null && Vector3.Distance(transform.position, _receiver.HitPoint) <= HitRadius)
            {
                _receiver.ApplyStun(_stunSeconds, _slowFactor);
                Destroy(gameObject);
            }
        }

        private static bool IsRig(Transform t)
        {
            while (t != null)
            {
                if (t.name == "XR Origin" || t.GetComponent<PlayerStunReceiver>() != null) return true;
                t = t.parent;
            }
            return false;
        }
    }
}
