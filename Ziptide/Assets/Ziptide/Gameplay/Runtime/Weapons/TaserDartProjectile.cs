using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Sticky dart: on collision, parents to target, shocks IShockable, hits TargetRuntime, and returns
    /// one restrained hit-confirm pulse to the firing hand when a gameplay target actually accepted it.
    /// </summary>
    public class TaserDartProjectile : MonoBehaviour
    {
        private float _stunSeconds;
        private float _hitImpulse;
        private float _lifetime;
        private AudioClip _impactClip;
        private bool _stuck;
        private WeaponFeelRuntime _feel;
        private XRBaseControllerInteractor _firingHand;

        public void Init(float stunSeconds, float hitImpulse, float lifetime, AudioClip impactClip,
            WeaponFeelRuntime feel = null, XRBaseControllerInteractor firingHand = null)
        {
            _stunSeconds = stunSeconds;
            _hitImpulse = hitImpulse;
            _lifetime = lifetime;
            _impactClip = impactClip;
            _feel = feel;
            _firingHand = firingHand;
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_stuck) return;
            _stuck = true;

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Unity warns when velocity is written after the body becomes kinematic. Stop the
                // dynamic body first, then lock it for the sticky-parent state.
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            transform.SetParent(collision.transform, true);
            SpawnSpark();
            bool confirmedTarget = false;

            // A drone handles its own taser shock + location-based go-down from the stick point.
            // Routing here avoids double-triggering its TargetRuntime/IShockable paths.
            var drone = collision.gameObject.GetComponentInParent<DroneRuntime>();
            if (drone != null)
            {
                drone.RegisterHit(transform.position, true);
                confirmedTarget = true;
            }
            else
            {
                var pvp = collision.gameObject.GetComponentInParent<IPvpDamageable>();
                if (pvp != null)
                {
                    PvpHitSource.Report(0);
                    pvp.ReceiveHit(PvpWeapon.Taser, transform.position, transform.forward);
                    confirmedTarget = true;
                }
                else
                {
                    var shockable = collision.gameObject.GetComponentInParent<IShockable>();
                    if (shockable != null)
                    {
                        shockable.Shock(_stunSeconds);
                        confirmedTarget = true;
                    }

                    var target = collision.gameObject.GetComponentInParent<TargetRuntime>();
                    if (target != null)
                    {
                        target.Hit(_hitImpulse, transform.position);
                        confirmedTarget = true;
                    }
                }
            }

            if (confirmedTarget && _feel != null)
                _feel.ConfirmHit(_firingHand);

            if (_impactClip != null)
                AudioSource.PlayClipAtPoint(_impactClip, transform.position, 0.5f);

            Destroy(gameObject, 2f);
        }

        private void SpawnSpark()
        {
            var spark = GamePool.Get("taser_spark", BuildSpark, transform.position);
            GamePool.ReleaseAfter("taser_spark", spark, 0.15f);
        }

        private static GameObject BuildSpark()
        {
            var spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spark.name = "Spark";
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
                    Color color = new Color(0.3f, 0.9f, 1f, 1f);
                    mat.color = color;
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
                    r.material = mat;
                }
            }
            return spark;
        }
    }
}
