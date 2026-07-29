using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE LEVEL STATE CONTRACT — the player arrives in every world in the same condition.
    ///
    /// A level is allowed to change the player: crouch them, slow them, suspend their locomotion,
    /// sit them in a vehicle, hang them off a zipline, hold them still at boot. What no level is
    /// allowed to do is let one of those changes FOLLOW the player out. The rig is
    /// DontDestroyOnLoad, so anything a world sets on it survives the world.
    ///
    /// Before this existed, each system was individually responsible for undoing itself, and two of
    /// them simply did not:
    ///   • travelling while crouched carried a shortened CharacterController and a dropped camera
    ///     into the next world;
    ///   • a drone's movement slow followed the player across a gate — which is the unexplained
    ///     "walk speed is wrong after the arena" line that has sat on the device checklist unsolved.
    ///
    /// One owner now restores the baseline on every world load, and — crucially — LOGS what it had
    /// to repair. A silent reset would hide the bug; a loud one names the system that leaked.
    ///
    /// It restores state. It does not own locomotion, comfort, input or the rig's identity, and it
    /// never moves the player.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LevelStateContract : MonoBehaviour
    {
        /// <summary>Grace period so a world's own Start() can legitimately set things up first.</summary>
        private const float SettleSeconds = 0.35f;

        private float _enforceAt = -1f;
        private string _pendingScene;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _pendingScene = scene.name;
            _enforceAt = Time.unscaledTime + SettleSeconds;
        }

        private void Update()
        {
            if (_enforceAt < 0f || Time.unscaledTime < _enforceAt) return;
            _enforceAt = -1f;
            Enforce(_pendingScene);
        }

        /// <summary>
        /// Restore the canonical arrival state. Public so a PlayMode test can assert the contract
        /// directly rather than racing a scene load.
        /// </summary>
        public void Enforce(string sceneName)
        {
            // ⚠ _Boot is EXEMPT, and this exemption is load-bearing. _Boot has no floor, so its boot
            // hold suspends movement deliberately while the Home Hub owns the player. Force-enabling
            // locomotion there would let a player walk off into nothing — turning a safety feature
            // into the exact fall this project already shipped once.
            if (sceneName == ZiptideConstants.SceneBoot)
            {
                Debug.Log("ZIPTIDE: LEVEL_STATE_SKIPPED scene=" + sceneName + " reason=boot_hold_owns_the_player");
                return;
            }

            var repaired = new StringBuilder();

            // 1. Standing. A crouch is a posture inside a world, never a condition you travel in.
            var dash = GetComponent<DashLocomotion>();
            if (dash != null)
            {
                dash.ForceStand();
                var controller = GetComponent<CharacterController>();
                if (controller != null && controller.height < 1.2f)
                    repaired.Append("controllerHeight ");
            }

            // 2. Unstunned. A slow belongs to the fight that caused it.
            var stun = GetComponent<PlayerStunReceiver>();
            if (stun != null) stun.ClearStun();

            // 3. Able to move. Any suspension owner that failed to release is repaired here, and
            //    named, because a player who cannot move is the single worst failure this game has.
            repaired.Append(ResumeLocomotion());

            // 4. Unparented. The locked law: the rig is delta-translated, never parented to a hull,
            //    a vehicle or a zipline. A parent that survived travel would drag the player.
            if (transform.parent != null)
            {
                repaired.Append("rigParent(" + transform.parent.name + ") ");
                transform.SetParent(null, worldPositionStays: true);
            }

            // 5. Normal time. Nothing in this game is allowed to leave the clock scaled.
            if (!Mathf.Approximately(Time.timeScale, 1f))
            {
                repaired.Append("timeScale(" + Time.timeScale.ToString("F2") + ") ");
                Time.timeScale = 1f;
            }

            if (repaired.Length == 0)
            {
                Debug.Log("ZIPTIDE: LEVEL_STATE_OK scene=" + sceneName);
                return;
            }

            // A repair is a FINDING, not a success. Something failed to clean up after itself.
            Debug.LogWarning("ZIPTIDE: LEVEL_STATE_REPAIRED scene=" + sceneName
                + " repaired=" + repaired.ToString().TrimEnd());
        }

        /// <summary>
        /// Re-enable every locomotion provider on the rig. Deliberately unconditional: a suspension
        /// owner that leaked is exactly the case this exists to catch, and there is no world in which
        /// arriving somewhere new with movement disabled is correct.
        /// </summary>
        private string ResumeLocomotion()
        {
            int repaired = 0;
            repaired += Resume(GetComponentsInChildren<ActionBasedContinuousMoveProvider>(true));
            repaired += Resume(GetComponentsInChildren<ActionBasedContinuousTurnProvider>(true));
            repaired += Resume(GetComponentsInChildren<ActionBasedSnapTurnProvider>(true));
            return repaired > 0 ? "locomotion(" + repaired + ") " : string.Empty;
        }

        private static int Resume(Behaviour[] behaviours)
        {
            if (behaviours == null) return 0;
            int repaired = 0;
            for (int i = 0; i < behaviours.Length; i++)
            {
                Behaviour behaviour = behaviours[i];
                if (behaviour == null || behaviour.enabled) continue;
                behaviour.enabled = true;
                repaired++;
            }
            return repaired;
        }
    }
}
