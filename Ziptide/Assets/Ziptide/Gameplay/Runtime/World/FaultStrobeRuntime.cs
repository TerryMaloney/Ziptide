using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The relay mast's red fault light (CityWayfindingAuthor). A fault light that does not blink
    /// is a decoration, and this one has a job: it is the errand's landmark in the sightline
    /// triple, and it is also the first thing that tells you the relay is wrong — long before
    /// anybody says so out loud.
    ///
    /// The cadence is a DOUBLE blink then a long dark, which is what industrial fault beacons do
    /// and what a steady pulse does not: steady reads as "operating", double-blink reads as
    /// "reporting a problem". No lights, no per-frame allocation, no material instance — a
    /// MaterialPropertyBlock, so there is nothing to destroy and nothing for the resource
    /// discipline gate to catch.
    /// </summary>
    public class FaultStrobeRuntime : MonoBehaviour
    {
        private const float PeriodSeconds = 1.6f;
        private const float FlashSeconds = 0.16f;
        private const float DarkFraction = 0.10f;

        private Renderer _renderer;
        private MaterialPropertyBlock _block;
        private Color _lit = new Color(1f, 0.24f, 0.18f);
        private bool _wasOn;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null && _renderer.sharedMaterial != null
                && _renderer.sharedMaterial.HasProperty("_EmissionColor"))
                _lit = _renderer.sharedMaterial.GetColor("_EmissionColor");
            _block = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (_renderer == null) return;

            float phase = Mathf.Repeat(Time.time, PeriodSeconds);
            bool on = phase < FlashSeconds
                      || (phase > FlashSeconds * 2f && phase < FlashSeconds * 3f);
            if (on == _wasOn) return;   // only touch the renderer on a transition
            _wasOn = on;

            _renderer.GetPropertyBlock(_block);
            _block.SetColor("_EmissionColor", on ? _lit : _lit * DarkFraction);
            _renderer.SetPropertyBlock(_block);
        }
    }
}
