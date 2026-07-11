using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Reads a LocomotionProfile and applies its values to the XR locomotion
    /// providers at runtime. Enables smooth OR snap turn, configures dash.
    /// </summary>
    public class LocomotionDirector : MonoBehaviour
    {
        [SerializeField] private LocomotionProfile profile;

        private void Start()
        {
            if (profile == null)
            {
                Debug.LogWarning("[Ziptide] LocomotionDirector: no profile assigned.");
                return;
            }
            ApplyProfile(profile);
            ApplyComfortSettings(ComfortSettings.Resolve(ComfortSettings.CurrentPreset));
        }

        public void ApplyProfile(LocomotionProfile p)
        {
            if (p == null) return;
            profile = p;

            var moveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>(true);
            if (moveProvider != null)
            {
                moveProvider.moveSpeed = p.moveSpeed;
                moveProvider.useGravity = p.useGravity;
            }

            var smoothTurn = GetComponentInChildren<ActionBasedContinuousTurnProvider>(true);
            var snapTurn = GetComponentInChildren<ActionBasedSnapTurnProvider>(true);

            if (smoothTurn != null && snapTurn != null)
            {
                bool smooth = p.turnMode == TurnMode.Smooth;
                smoothTurn.gameObject.SetActive(smooth);
                snapTurn.gameObject.SetActive(!smooth);

                smoothTurn.turnSpeed = p.smoothTurnSpeed;
                snapTurn.turnAmount = p.snapTurnAngle;
            }
            else if (smoothTurn != null)
            {
                smoothTurn.turnSpeed = p.smoothTurnSpeed;
                smoothTurn.gameObject.SetActive(p.turnMode == TurnMode.Smooth);
            }
            else if (snapTurn != null)
            {
                snapTurn.turnAmount = p.snapTurnAngle;
                snapTurn.gameObject.SetActive(p.turnMode == TurnMode.Snap);
            }

            var dash = GetComponentInChildren<DashLocomotion>(true);
            if (dash != null)
            {
                dash.Configure(p.dashDistance, p.dashDuration, p.dashCooldown, p.dashVerticalLift);
                dash.ConfigureSprint(p.sprintMultiplier);
                dash.ConfigureBody(p.crouchSpeedFactor, p.slideBoost, p.slideSeconds, p.autoRunDoubleTapWindow);
                dash.enabled = p.dashEnabled;
            }
        }

        /// <summary>
        /// Additive device-comfort application. The profile remains the owner of ordinary movement;
        /// these dials alter only the fields named by the locked preset table. No rig transform moves.
        /// </summary>
        public void ApplyComfortSettings(ComfortDialSet settings)
        {
            var smoothTurn = GetComponentInChildren<ActionBasedContinuousTurnProvider>(true);
            var snapTurn = GetComponentInChildren<ActionBasedSnapTurnProvider>(true);

            if (smoothTurn != null)
            {
                smoothTurn.turnSpeed = settings.smoothTurnSpeed;
                smoothTurn.gameObject.SetActive(settings.smoothTurn);
            }
            if (snapTurn != null)
            {
                snapTurn.turnAmount = settings.snapTurnAngle;
                snapTurn.gameObject.SetActive(!settings.smoothTurn);
            }

            var dash = GetComponentInChildren<DashLocomotion>(true);
            if (dash != null)
            {
                // DashLocomotion's compatibility API treats old-asset zero/one values as "missing".
                // Translate Cozy's semantic OFF to the smallest accepted, effectively neutral slide.
                float translatedBoost = settings.slideBoost <= 1f ? 1.011f : settings.slideBoost;
                float translatedSeconds = settings.slideSeconds <= 0f ? 0.051f : settings.slideSeconds;
                float crouch = profile != null ? profile.crouchSpeedFactor : 0.55f;
                float tapWindow = profile != null ? profile.autoRunDoubleTapWindow : 0.35f;
                dash.ConfigureBody(crouch, translatedBoost, translatedSeconds, tapWindow);
                dash.enabled = settings.dashEnabled;
            }
        }
    }
}
