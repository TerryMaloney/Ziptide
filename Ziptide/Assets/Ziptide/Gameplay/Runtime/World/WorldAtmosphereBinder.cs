using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Puts WEATHER in a world that does not own a sky vista (SKYSCAPE_DESIGN §3, pillar 3:
    /// "something is always drifting").
    ///
    /// The atmosphere stack — horizon haze card at ~40 m, drifting motes at 8–15 m — is built and
    /// tested (`SkyAtmosphere`), but it only ever reaches a scene through `SkyPlanetRig`'s vista
    /// path, which needs a generated theme asset carrying a `skyVista`. ToxicCity has no theme
    /// (`SKY_VISTA_UNWIRED` has been warning about it), so the FIRST PLANET THE PLAYER EVER STANDS
    /// ON ships with perfectly still, perfectly clear air — failing rubric items §5.1 and §5.2 on
    /// the one world where the Prospect bar matters most.
    ///
    /// This binder is deliberately ADDITIVE. It drives only the haze and mote layers and never
    /// touches the sky dome, the celestial bodies, the palette, the fog or the light. A world that
    /// already owns its look keeps it exactly; it just gains air. That property is why this can land
    /// before a device session rather than after one — the worst case is "there is now some drift",
    /// not "the city looks different and nobody knows which change did it".
    ///
    /// Authored by the scene patcher with an explicit vista reference, so this is scene-owned data,
    /// not an automatic runtime owner.
    /// </summary>
    [DisallowMultipleComponent]
    public class WorldAtmosphereBinder : MonoBehaviour
    {
        /// <summary>How many frames to keep looking for the rig before giving up loudly.</summary>
        private const int MaxSearchFrames = 300;

        [SerializeField]
        [Tooltip("Vista whose atmosphere block drives the haze/mote layers. Only .atmosphere and " +
                 "vistaId are read — the dome, bodies and light coupling are NOT applied.")]
        private SkyVistaDefinition _vista;

        private SkyAtmosphereRig _rig;
        private bool _applied;
        private int _frames;

        /// <summary>Editor seam: the patcher assigns the vista at bake time.</summary>
        public void Configure(SkyVistaDefinition vista) => _vista = vista;

        public SkyVistaDefinition Vista => _vista;

        private void Update()
        {
            if (_applied) return;

            if (_vista == null || _vista.atmosphere == null || !_vista.atmosphere.enabled)
            {
                // Nothing authored is a legitimate answer (Interior-tier worlds); say so once and stop.
                _applied = true;
                Debug.Log("ZIPTIDE: WORLD_ATMO applied=0 cause=no_atmosphere_authored vista="
                    + (_vista != null ? _vista.vistaId : "none"));
                return;
            }

            Transform player = FindPlayerHead();
            if (player == null)
            {
                // The rig lives in _Boot and arrives after this scene's objects; waiting is normal.
                if (++_frames < MaxSearchFrames) return;
                _applied = true;
                Debug.LogWarning("ZIPTIDE: WORLD_ATMO applied=0 cause=no_player_after_"
                    + MaxSearchFrames + "_frames vista=" + _vista.vistaId);
                return;
            }

            if (_rig == null) _rig = gameObject.GetComponent<SkyAtmosphereRig>();
            if (_rig == null) _rig = gameObject.AddComponent<SkyAtmosphereRig>();

            _rig.Apply(_vista, player);
            _applied = true;
            Debug.Log("ZIPTIDE: WORLD_ATMO applied=1 vista=" + _vista.vistaId
                + " hazard=" + _vista.atmosphere.hazardTag
                + " intensity=" + _vista.atmosphere.intensity.ToString("F2")
                + " frames=" + _frames);
        }

        // The head, not the rig root: haze and motes are centred on where the eyes actually are, and
        // the mote shell's whole point is that the player's own parallax crosses it.
        private static Transform FindPlayerHead()
        {
            Camera cam = Camera.main;
            if (cam != null) return cam.transform;

            PlayerRigPersistence rig = Object.FindObjectOfType<PlayerRigPersistence>();
            return rig != null ? rig.transform : null;
        }
    }
}
