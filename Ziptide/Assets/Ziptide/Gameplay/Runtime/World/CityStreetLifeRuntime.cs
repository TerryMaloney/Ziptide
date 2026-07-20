using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>Quest-cheap motion for prebuilt steam puffs and lamp glow transforms.</summary>
    [DisallowMultipleComponent]
    public sealed class CityStreetLifeRuntime : MonoBehaviour
    {
        private Transform[] _steam;
        private Transform[] _lamps;
        private Vector3[] _steamBase;
        private Vector3[] _steamScale;
        private Vector3[] _lampScale;

        private void Awake()
        {
            Cache();
        }

        private void Cache()
        {
            var all = GetComponentsInChildren<Transform>(true);
            var steam = new System.Collections.Generic.List<Transform>();
            var lamps = new System.Collections.Generic.List<Transform>();
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].name.StartsWith("Steam_")) steam.Add(all[i]);
                else if (all[i].name.StartsWith("LampGlow_")) lamps.Add(all[i]);
            }
            _steam = steam.ToArray();
            _lamps = lamps.ToArray();
            _steamBase = new Vector3[_steam.Length];
            _steamScale = new Vector3[_steam.Length];
            _lampScale = new Vector3[_lamps.Length];
            for (int i = 0; i < _steam.Length; i++)
            {
                _steamBase[i] = _steam[i].localPosition;
                _steamScale[i] = _steam[i].localScale;
            }
            for (int i = 0; i < _lamps.Length; i++) _lampScale[i] = _lamps[i].localScale;
        }

        private void Update()
        {
            if (_steam == null) Cache();
            for (int i = 0; i < _steam.Length; i++)
            {
                Transform t = _steam[i];
                if (t == null) continue;
                float phase = Mathf.Repeat(Time.time * (0.34f + i * 0.013f) + i * 0.31f, 1f);
                Vector3 p = _steamBase[i];
                p.y += phase * 1.15f;
                p.x += Mathf.Sin(Time.time * 0.55f + i) * 0.08f;
                t.localPosition = p;
                t.localScale = _steamScale[i] * Mathf.Lerp(0.55f, 1.35f, phase);
            }

            for (int i = 0; i < _lamps.Length; i++)
            {
                Transform t = _lamps[i];
                if (t == null) continue;
                float breathe = 0.88f + 0.12f * Mathf.Sin(Time.time * 1.7f + i * 0.8f);
                t.localScale = _lampScale[i] * breathe;
            }
        }
    }
}
