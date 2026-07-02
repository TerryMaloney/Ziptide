using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// STATIC NET (A4 — area control): trigger lobs a crackling net that arcs under gravity; wherever
    /// it lands it opens a <see cref="SlowZoneRuntime"/> for a few seconds. Small damage on a direct
    /// combatant hit — the SLOW is the payload. Counter: it's a visible arc, sidestep it.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class StaticNetGunRuntime : MonoBehaviour
    {
        private XRGrabInteractable _grab;
        private Transform _muzzle;
        private float _nextFireTime;

        private ArenaWeaponDefinition Def
        {
            get
            {
                var item = GetComponent<ItemRuntime>();
                return item != null ? item.Definition as ArenaWeaponDefinition : null;
            }
        }

        private void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _muzzle = transform.Find("Muzzle");
        }

        private void OnEnable() { if (_grab != null) _grab.activated.AddListener(OnActivated); }
        private void OnDisable() { if (_grab != null) _grab.activated.RemoveListener(OnActivated); }

        private void OnActivated(ActivateEventArgs args)
        {
            var def = Def;
            float cooldown = def != null ? def.fireCooldown : 1.2f;
            if (Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + cooldown;

            Vector3 origin = _muzzle != null ? _muzzle.position : transform.position + transform.forward * 0.2f;
            Vector3 dir = _muzzle != null ? _muzzle.forward : transform.forward;

            var net = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            net.name = "StaticNet";
            net.transform.position = origin + dir * 0.15f;
            net.transform.localScale = Vector3.one * 0.16f;
            ItemFactory.ApplyURPColor(net, new Color(0.35f, 0.95f, 0.55f));

            var rb = net.AddComponent<Rigidbody>();
            rb.mass = 0.25f;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = (dir + Vector3.up * 0.25f).normalized * (def != null ? def.netThrowSpeed : 9f);

            foreach (var gunCol in GetComponentsInChildren<Collider>(true))
                if (gunCol != null) Physics.IgnoreCollision(net.GetComponent<Collider>(), gunCol);
            var playerBody = Object.FindObjectOfType<CharacterController>();
            if (playerBody != null) Physics.IgnoreCollision(net.GetComponent<Collider>(), playerBody);

            net.AddComponent<StaticNetProjectile>();

            var interactor = args.interactorObject as XRBaseControllerInteractor;
            if (interactor != null) interactor.SendHapticImpulse(0.5f, 0.08f);
            Debug.Log("ZIPTIDE: NET_THROWN");
        }
    }

    /// <summary>The lobbed net: first solid contact opens the slow zone (and stings a combatant it
    /// lands on directly).</summary>
    public class StaticNetProjectile : MonoBehaviour
    {
        private float _life = 5f;
        private bool _landed;

        private void Update()
        {
            _life -= Time.deltaTime;
            if (_life <= 0f && !_landed) Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_landed) return;
            _landed = true;

            var pvp = collision.gameObject.GetComponentInParent<IPvpDamageable>();
            if (pvp != null)
            {
                PvpHitSource.Report(0); // player weapon
                pvp.ReceiveHit(PvpWeapon.StaticNet, transform.position, Vector3.down);
            }

            var zone = new GameObject("SlowZone");
            zone.transform.position = transform.position;
            zone.AddComponent<SlowZoneRuntime>();
            Debug.Log("ZIPTIDE: NET_ZONE_OPEN");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// The crackling slow field the net leaves behind: anything combat-shaped inside moves at
    /// <see cref="PvpRules.StaticNetSlowFactor"/> speed — the player through PlayerStunReceiver
    /// (re-applied while inside), bots through <see cref="PvpBot.ApplySlow"/>. Dies after
    /// StaticNetZoneSeconds.
    /// </summary>
    public class SlowZoneRuntime : MonoBehaviour
    {
        private float _dieAt;
        private float _nextApplyAt;
        private Transform _playerHead;
        private GameObject _disc;

        private void Start()
        {
            _dieAt = Time.time + (float)PvpRules.StaticNetZoneSeconds;
            var rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig != null) _playerHead = rig.GetComponentInChildren<Camera>()?.transform;
            if (_playerHead == null && Camera.main != null) _playerHead = Camera.main.transform;

            float r = (float)PvpRules.StaticNetZoneRadius;
            _disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _disc.name = "SlowField";
            _disc.transform.SetParent(transform, false);
            _disc.transform.localPosition = new Vector3(0f, 0.04f, 0f);
            _disc.transform.localScale = new Vector3(r * 2f, 0.04f, r * 2f);
            var col = _disc.GetComponent<Collider>();
            if (col != null) Destroy(col);
            ItemFactory.ApplyURPColor(_disc, new Color(0.30f, 0.85f, 0.50f));
        }

        private void Update()
        {
            if (Time.time >= _dieAt) { Destroy(gameObject); return; }
            if (Time.time < _nextApplyAt) return;
            _nextApplyAt = Time.time + 0.25f; // re-apply cadence: leaving the zone ends the slow fast

            float r = (float)PvpRules.StaticNetZoneRadius;
            float slow = (float)PvpRules.StaticNetSlowFactor;

            if (_playerHead != null && FlatDist(_playerHead.position, transform.position) <= r)
            {
                var stun = FindObjectOfType<PlayerStunReceiver>();
                if (stun != null) stun.ApplyStun(0.35f, slow); // brief + re-applied = smooth, escapable
            }
            foreach (var bot in FindObjectsOfType<PvpBot>())
                if (bot != null && bot.IsAlive && FlatDist(bot.transform.position, transform.position) <= r)
                    bot.ApplySlow(0.35f, slow);
        }

        private static float FlatDist(Vector3 a, Vector3 b)
        { float dx = a.x - b.x, dz = a.z - b.z; return Mathf.Sqrt(dx * dx + dz * dz); }
    }
}
