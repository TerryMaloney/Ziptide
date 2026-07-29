using System.Collections.Generic;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Plays THE RESONANCE TELL (<see cref="ResonanceTellCore"/>): every light and readout within
    /// reach dies for a beat when half B leaves the wreck, then crawls back. It is the artifact's
    /// first act — evidence before explanation, planted so the join at the berth lands as a
    /// confirmation rather than a reveal.
    ///
    /// Deliberately harmless: it dims LIGHTS and emissive-ish renderer colors it captured itself,
    /// restores every one of them on completion (and in OnDestroy, so an interrupted scene change
    /// can't leave anything dark), and never touches gameplay state, input, or the vehicle's
    /// ability to drive. The player's ride keeps working — it just stops LOOKING like it.
    /// Logs ZIPTIDE: RESONANCE_TELL.
    /// </summary>
    public class ResonanceTellRuntime : MonoBehaviour
    {
        /// <summary>Radius the tell reaches, metres — the vehicle you drove here, and nothing else.</summary>
        public const float ReachMetres = 14f;

        private readonly List<Light> _lights = new List<Light>();
        private readonly List<float> _lightIntensity = new List<float>();
        private readonly List<IResonanceSensitive> _instruments = new List<IResonanceSensitive>();

        private float _t;
        private bool _running;

        /// <summary>
        /// Fire the tell around a point. Creates its own runner object so the caller (a collectible
        /// that destroys itself the instant it is grabbed) does not have to survive the effect.
        /// </summary>
        public static void PlayAt(Vector3 center)
        {
            var go = new GameObject("__ResonanceTell");
            go.transform.position = center;
            go.AddComponent<ResonanceTellRuntime>().Begin();
        }

        private void Begin()
        {
            Capture();
            _running = true;
            Debug.Log("ZIPTIDE: RESONANCE_TELL lights=" + _lights.Count
                + " instruments=" + _instruments.Count);
        }

        /// <summary>Snapshot what we are about to dim, so restoring is exact rather than guessed.</summary>
        private void Capture()
        {
            foreach (var light in FindObjectsOfType<Light>())
            {
                if (light == null || light.type == LightType.Directional) continue; // never the sun
                if (Vector3.Distance(light.transform.position, transform.position) > ReachMetres) continue;
                _lights.Add(light);
                _lightIntensity.Add(light.intensity);
            }

            // Instruments announce themselves through the Core interface, so the vehicle (which
            // lives in the Ship assembly) is reachable without Gameplay referencing it.
            foreach (var behaviour in FindObjectsOfType<MonoBehaviour>())
            {
                if (behaviour == null || !(behaviour is IResonanceSensitive sensitive)) continue;
                if (Vector3.Distance(behaviour.transform.position, transform.position) > ReachMetres) continue;
                _instruments.Add(sensitive);
            }
        }

        private void Update()
        {
            if (!_running) return;
            _t += Time.deltaTime;

            float power = ResonanceTellCore.Power(_t);
            Apply(power);

            if (!ResonanceTellCore.IsActive(_t))
            {
                Restore();
                Debug.Log("ZIPTIDE: RESONANCE_TELL phase=recovered");
                Destroy(gameObject);
            }
        }

        private void Apply(float power)
        {
            for (int i = 0; i < _lights.Count; i++)
                if (_lights[i] != null) _lights[i].intensity = _lightIntensity[i] * power;

            for (int i = 0; i < _instruments.Count; i++)
                _instruments[i]?.SetInstrumentPower(power);
        }

        private void Restore()
        {
            _running = false;
            for (int i = 0; i < _lights.Count; i++)
                if (_lights[i] != null) _lights[i].intensity = _lightIntensity[i];
            for (int i = 0; i < _instruments.Count; i++)
                _instruments[i]?.SetInstrumentPower(1f);
        }

        private void OnDestroy()
        {
            // If anything at all cut this short — scene change, a destroyed parent — the world must
            // still come back up. Nothing this component touches may outlive it.
            if (_running) Restore();
        }
    }
}
