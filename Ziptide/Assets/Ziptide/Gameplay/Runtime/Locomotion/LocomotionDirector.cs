using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

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
                dash.enabled = p.dashEnabled;
            }
        }
    }
}
