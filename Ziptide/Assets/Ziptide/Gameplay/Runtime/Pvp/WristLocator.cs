using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Left-wrist locator: hold the right hand near the left wrist for a few seconds to ping the
    /// opponent's direction, then a cooldown. Timing is the pure <see cref="LocatorState"/>; this reads
    /// the two controllers' proximity and shows a brief direction arrow. Anti-camp tool.
    /// </summary>
    public class WristLocator : MonoBehaviour
    {
        public float wristTouchDistance = 0.2f;
        public float pingSeconds = 3f;

        private readonly LocatorState _state = new LocatorState();
        private Transform _left;
        private Transform _right;
        private Transform _cam;
        private Transform _target;
        private GameObject _ping;
        private float _pingHideAt;

        private void Start()
        {
            var rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig != null) _cam = rig.GetComponentInChildren<Camera>()?.transform;
            FindHands();
            var bot = FindObjectOfType<PvpBot>();
            if (bot != null) _target = bot.transform;
        }

        private void FindHands()
        {
            var interactors = FindObjectsOfType<XRBaseControllerInteractor>();
            // Heuristic: leftmost vs rightmost relative to the camera; falls back to first two.
            if (interactors == null || interactors.Length < 2) return;
            Transform a = interactors[0].transform, b = interactors[1].transform;
            if (_cam != null)
            {
                float da = Vector3.Dot(a.position - _cam.position, _cam.right);
                _left = da < 0 ? a : b;
                _right = da < 0 ? b : a;
            }
            else { _left = a; _right = b; }
        }

        private void Update()
        {
            if (_left == null || _right == null) { FindHands(); if (_left == null) return; }

            bool held = Vector3.Distance(_right.position, _left.position) <= wristTouchDistance;
            bool fired = _state.Tick(Time.time, Time.deltaTime, held);
            if (fired) ShowPing();

            if (_ping != null && Time.time >= _pingHideAt)
            {
                Destroy(_ping);
                _ping = null;
            }
        }

        private void ShowPing()
        {
            if (_target == null || _cam == null) return;
            if (_ping != null) Destroy(_ping);

            _ping = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _ping.name = "__LocatorPing";
            var col = _ping.GetComponent<Collider>();
            if (col != null) Destroy(col);
            _ping.transform.SetParent(_cam, false);

            Vector3 dir = (_target.position - _cam.position).normalized;
            _ping.transform.localPosition = _cam.InverseTransformDirection(dir) * 0.6f;
            _ping.transform.rotation = Quaternion.LookRotation(dir);
            _ping.transform.localScale = new Vector3(0.03f, 0.03f, 0.5f);
            ItemFactory.ApplyURPColor(_ping, new Color(0.3f, 0.9f, 1f));
            var r = _ping.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            _pingHideAt = Time.time + pingSeconds;
            Debug.Log("ZIPTIDE: PVP_LOCATOR_PING");
        }
    }
}
