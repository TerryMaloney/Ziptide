using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Sticky dart: on collision, parents to target, shocks IShockable, hits TargetRuntime.
    /// </summary>
    public class TaserDartProjectile : MonoBehaviour
    {
        private float _stunSeconds;
        private float _hitImpulse;
        private float _lifetime;
        private AudioClip _impactClip;
        private bool _stuck;

        public void Init(float stunSeconds, float hitImpulse, float lifetime, AudioClip impactClip)
        {
            _stunSeconds = stunSeconds;
            _hitImpulse = hitImpulse;
            _lifetime = lifetime;
            _impactClip = impactClip;
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_stuck) return;
            _stuck = true;

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }

            transform.SetParent(collision.transform, true);

            SpawnSpark();

            var shockable = collision.gameObject.GetComponentInParent<IShockable>();
            if (shockable != null)
                shockable.Shock(_stunSeconds);

            var target = collision.gameObject.GetComponentInParent<TargetRuntime>();
            if (target != null)
                target.Hit(_hitImpulse, transform.position);

            if (_impactClip != null)
            {
                AudioSource.PlayClipAtPoint(_impactClip, transform.position, 0.5f);
            }

            Destroy(gameObject, 2f);
        }

        private void SpawnSpark()
        {
            var spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spark.name = "Spark";
            spark.transform.position = transform.position;
            spark.transform.localScale = Vector3.one * 0.04f;
            var col = spark.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            var r = spark.GetComponent<Renderer>();
            if (r != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader != null)
                {
                    var mat = new Material(shader);
                    mat.color = new Color(0.3f, 0.9f, 1f, 1f);
                    if (mat.HasProperty("_BaseColor"))
                        mat.SetColor("_BaseColor", new Color(0.3f, 0.9f, 1f, 1f));
                    r.material = mat;
                }
            }

            Destroy(spark, 0.15f);
        }
    }
}
