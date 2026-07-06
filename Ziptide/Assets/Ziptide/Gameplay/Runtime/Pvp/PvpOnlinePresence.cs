using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A6 v1 — ONLINE PRESENCE. The bridge between the local VR rig and <see cref="PvpNetHub"/>'s
    /// transport: each tick it broadcasts the local head + two hands (a <see cref="PlayerPoseMsg"/>),
    /// and it renders every OTHER player's pose as a floating avatar (helmet + salvage gloves, the
    /// <see cref="PlayerAvatarRig"/> look) so two headsets in the same room SEE each other move.
    ///
    /// Transport-agnostic: it only talks to <see cref="PvpNetHub.Active"/>, so it does the right thing
    /// on loopback (echoes to self → filtered by playerId → harmless) and over Photon alike. Spawned
    /// by the arena lobby's ONLINE tile; safe to leave running (near-zero cost until a remote appears).
    /// Combat sync (fire/hit authority) is the A6.2 follow-up — this ships presence first, clean.
    /// Logs ZIPTIDE: NET_PRESENCE remote=… when a peer's avatar appears/leaves.
    /// </summary>
    public class PvpOnlinePresence : MonoBehaviour
    {
        private const float SendInterval = 0.05f; // 20 Hz pose
        private const float RemoteTimeout = 3f;   // drop an avatar we stop hearing from

        private static readonly Color Leather = new Color(0.17f, 0.15f, 0.14f);
        private static readonly Color Plate = new Color(0.36f, 0.34f, 0.31f);
        private static readonly Color Glow = new Color(0.30f, 0.80f, 0.95f);
        // A second player reads clearest in a contrasting tint — warm amber vs. the local teal.
        private static readonly Color RemoteGlow = new Color(0.95f, 0.55f, 0.20f);

        private Transform _head, _lHand, _rHand;
        private float _sendTimer;

        private IPvpTransport _bound;

        private class Remote
        {
            public Transform root, head, lHand, rHand, weapon;
            public float lastSeen;
        }
        private readonly Dictionary<int, Remote> _remotes = new Dictionary<int, Remote>();

        private void OnEnable()
        {
            PvpNetHub.TransportChanged += Rebind;
            Rebind(PvpNetHub.Active);
        }

        private void OnDisable()
        {
            PvpNetHub.TransportChanged -= Rebind;
            if (_bound != null) _bound.OnPose -= OnRemotePose;
            _bound = null;
        }

        private void Rebind(IPvpTransport t)
        {
            if (_bound != null) _bound.OnPose -= OnRemotePose;
            _bound = t;
            if (_bound != null) _bound.OnPose += OnRemotePose;
        }

        private void Update()
        {
            if (!EnsureLocalRig()) return;

            _sendTimer -= Time.deltaTime;
            if (_sendTimer <= 0f)
            {
                _sendTimer = SendInterval;
                var t = PvpNetHub.Active;
                t.SendPose(new PlayerPoseMsg
                {
                    playerId = t.LocalPlayerId,
                    headPos = _head.position, headRot = _head.rotation,
                    lHandPos = _lHand != null ? _lHand.position : _head.position,
                    lHandRot = _lHand != null ? _lHand.rotation : _head.rotation,
                    rHandPos = _rHand != null ? _rHand.position : _head.position,
                    rHandRot = _rHand != null ? _rHand.rotation : _head.rotation,
                    heldWeapon = -1, // weapon-in-hand display is A6.2 polish
                });
            }

            // Age out remotes we've stopped hearing from (they left / disconnected).
            List<int> drop = null;
            foreach (var kv in _remotes)
                if (Time.time - kv.Value.lastSeen > RemoteTimeout)
                    (drop ??= new List<int>()).Add(kv.Key);
            if (drop != null)
                foreach (var id in drop)
                {
                    if (_remotes[id].root != null) Destroy(_remotes[id].root.gameObject);
                    _remotes.Remove(id);
                    Debug.Log("ZIPTIDE: NET_PRESENCE remote=" + id + " left");
                }
        }

        private bool EnsureLocalRig()
        {
            if (_head == null)
            {
                var cam = Camera.main;
                if (cam == null) return false;
                _head = cam.transform;
                foreach (var c in FindObjectsOfType<ActionBasedController>())
                {
                    string n = c.name.ToLowerInvariant();
                    if (n.Contains("left")) _lHand = c.transform;
                    else if (n.Contains("right")) _rHand = c.transform;
                }
            }
            return _head != null;
        }

        private void OnRemotePose(PlayerPoseMsg m)
        {
            if (m.playerId == PvpNetHub.Active.LocalPlayerId) return; // that's me echoing on loopback

            if (!_remotes.TryGetValue(m.playerId, out var r))
            {
                r = BuildRemote(m.playerId);
                _remotes[m.playerId] = r;
                Debug.Log("ZIPTIDE: NET_PRESENCE remote=" + m.playerId + " joined");
            }
            r.lastSeen = Time.time;
            r.head.SetPositionAndRotation(m.headPos, m.headRot);
            r.lHand.SetPositionAndRotation(m.lHandPos, m.lHandRot);
            r.rHand.SetPositionAndRotation(m.rHandPos, m.rHandRot);
        }

        // ── Remote avatar (primitive-built, PlayerAvatarRig palette) ─────────────────────────────
        private Remote BuildRemote(int id)
        {
            var root = new GameObject("__RemotePlayer_" + id);
            var head = new GameObject("Head").transform; head.SetParent(root.transform, false);
            // Helmet + visor so the peer reads as a person, not a floating cube.
            Part(head, "Skull", Vector3.zero, Vector3.zero, new Vector3(0.20f, 0.24f, 0.24f), Plate);
            var visor = Part(head, "Visor", new Vector3(0f, -0.01f, 0.12f), Vector3.zero,
                new Vector3(0.16f, 0.07f, 0.03f), RemoteGlow);
            Emissive(visor, RemoteGlow);
            Part(head, "CrestGlow", new Vector3(0f, 0.13f, 0f), Vector3.zero,
                new Vector3(0.04f, 0.03f, 0.14f), RemoteGlow);

            var lHand = BuildGlove(root.transform, "GloveL", true);
            var rHand = BuildGlove(root.transform, "GloveR", false);

            return new Remote { root = root.transform, head = head, lHand = lHand, rHand = rHand };
        }

        private Transform BuildGlove(Transform parent, string name, bool left)
        {
            var g = new GameObject(name).transform;
            g.SetParent(parent, false);
            float mirror = left ? -1f : 1f;
            Part(g, "Palm", new Vector3(0f, -0.02f, -0.03f), Vector3.zero, new Vector3(0.07f, 0.03f, 0.09f), Leather);
            Part(g, "Knuckle", new Vector3(0f, -0.005f, -0.01f), new Vector3(-8f, 0f, 0f), new Vector3(0.075f, 0.016f, 0.05f), Plate);
            Part(g, "Thumb", new Vector3(mirror * 0.04f, -0.03f, -0.02f), new Vector3(0f, mirror * 35f, mirror * -20f),
                new Vector3(0.02f, 0.02f, 0.05f), Leather);
            var band = Part(g, "WristGlow", new Vector3(0f, 0.006f, -0.09f), Vector3.zero, new Vector3(0.045f, 0.008f, 0.02f), RemoteGlow);
            Emissive(band, RemoteGlow);
            return g;
        }

        private static GameObject Part(Transform parent, string name, Vector3 pos, Vector3 euler, Vector3 size, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            var col = go.GetComponent<Collider>();
            if (col != null) Destroy(col); // v1 presence is a LOOK — no networked hitboxes yet (A6.2)
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.Euler(euler);
            go.transform.localScale = size;
            ItemFactory.ApplyURPColor(go, color);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }

        private static void Emissive(GameObject go, Color color)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null || r.sharedMaterial == null) return;
            r.sharedMaterial.EnableKeyword("_EMISSION");
            r.sharedMaterial.SetColor("_EmissionColor", color * 1.6f);
        }

        /// <summary>Ensure exactly one presence exists (the lobby ONLINE tile calls this).</summary>
        public static PvpOnlinePresence Ensure()
        {
            var existing = FindObjectOfType<PvpOnlinePresence>();
            if (existing != null) return existing;
            var go = new GameObject("__PvpOnlinePresence");
            return go.AddComponent<PvpOnlinePresence>();
        }
    }
}
