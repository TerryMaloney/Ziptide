using UnityEngine;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Visible, slow, DODGEABLE bolt the PvP bot fires at the player. Mirrors the drone's StunBolt feel
    /// (straight-line travel, blocked by walls) but on reaching the player's head it registers a PvP hit via
    /// <see cref="PvpPlayer.ReceiveHit"/> so the match scores it. Replaces the old instant hitscan that gave
    /// the player nothing to see or dodge ("the bot is hitting me with something but I don't see anything").
    /// </summary>
    public class PvpBolt : MonoBehaviour
    {
        private Vector3 _velocity;
        private PvpPlayer _target;
        private Transform _head;
        private float _life = 4f;
        private const float HitRadius = 0.55f;

        public void Init(Vector3 velocity, PvpPlayer target, Transform head)
        {
            _velocity = velocity;
            _target = target;
            _head = head;
            BuildVisual();
        }

        private void BuildVisual()
        {
            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = "Bolt";
            ball.transform.SetParent(transform, false);
            ball.transform.localScale = Vector3.one * 0.28f;
            var col = ball.GetComponent<Collider>();
            if (col != null) Destroy(col);
            var r = ball.GetComponent<Renderer>();
            if (r != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
                var mat = new Material(shader);
                var c = new Color(1f, 0.55f, 0.15f); // hostile orange so it reads as "incoming, dodge it"
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

            // Block on walls: absorbed by solid geometry that isn't the firing bot or the player — no
            // shooting through cover after it leaves the muzzle.
            if (d > 0.0001f && Physics.Raycast(transform.position, step / d, out var hit, d, ~0, QueryTriggerInteraction.Ignore))
            {
                var col = hit.collider;
                bool isBot = col != null && col.GetComponentInParent<PvpBot>() != null;
                bool isPlayer = col != null && IsRig(col.transform);
                if (col != null && !isBot && !isPlayer) { Destroy(gameObject); return; }
            }

            transform.position += step;
            _life -= Time.deltaTime;
            if (_life <= 0f) { Destroy(gameObject); return; }

            Vector3 hp = _head != null ? _head.position
                       : (_target != null ? _target.transform.position : transform.position);
            if (Vector3.Distance(transform.position, hp) <= HitRadius)
            {
                if (_target != null)
                    _target.ReceiveHit(PvpWeapon.Taser, transform.position, _velocity.sqrMagnitude > 0.001f ? _velocity.normalized : Vector3.forward);
                Destroy(gameObject);
            }
        }

        private static bool IsRig(Transform t)
        {
            while (t != null)
            {
                if (t.name == "XR Origin" || t.GetComponent<PvpPlayer>() != null) return true;
                t = t.parent;
            }
            return false;
        }
    }
}
