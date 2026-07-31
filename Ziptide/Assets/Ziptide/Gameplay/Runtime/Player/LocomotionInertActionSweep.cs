using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The ONE implementation of "this hand does not turn" (VR_RIG_GOTCHAS #9).
    ///
    /// ZIPTIDE's control law is left stick moves, right stick turns, so the left-hand turn and snap
    /// properties on the XRI providers are authored as embedded direct actions with zero bindings.
    /// Keeping such an action merely *disabled* is not durable: XRI's OnEnable runs
    /// EnableAllDirectActions, which re-enables it, and the next ReadInput enters the Input System
    /// with no binding state and throws NullReferenceException inside InputActionState.ApplyProcessors.
    /// Assigning default(InputActionProperty) is durable — the XRI setter disables the outgoing
    /// action, later OnEnable has nothing to revive, and ReadInput null-skips a null action.
    ///
    /// This lives apart from its callers on purpose. The fix previously existed only inside
    /// InputMutationRepairDriver's post-travel repair, so every path that never travelled — cold boot,
    /// and any PlayMode test that only loads a scene — still met the crash. A repair that is correct
    /// but reachable from one trigger is a repair the rest of the game does not have.
    /// </summary>
    public static class LocomotionInertActionSweep
    {
        /// <summary>
        /// True for an embedded direct action that can never resolve a control: no owning map and no
        /// bindings. A map action or a bound action is real input and is never touched.
        /// </summary>
        public static bool IsInertAction(InputAction action)
        {
            if (action == null || action.actionMap != null) return false;
            try { return action.bindings.Count == 0; }
            catch { return false; }
        }

        /// <summary>
        /// True for a property this sweep may null out. A property carrying an InputActionReference is
        /// never inert: the asset owns its binding state and clearing it would delete real input.
        /// </summary>
        public static bool IsInertProperty(InputActionProperty property)
        {
            if (property.reference != null) return false;
            return IsInertAction(property.action);
        }

        /// <summary>
        /// Clear every inert direct property on one locomotion provider. Returns how many were cleared.
        /// Anything that is not a move / continuous-turn / snap-turn provider is ignored.
        /// </summary>
        public static int Clear(Behaviour reader)
        {
            int cleared = 0;
            switch (reader)
            {
                case ActionBasedContinuousMoveProvider move:
                    if (IsInertProperty(move.leftHandMoveAction))
                    { move.leftHandMoveAction = default; cleared++; }
                    if (IsInertProperty(move.rightHandMoveAction))
                    { move.rightHandMoveAction = default; cleared++; }
                    break;
                case ActionBasedContinuousTurnProvider turn:
                    if (IsInertProperty(turn.leftHandTurnAction))
                    { turn.leftHandTurnAction = default; cleared++; }
                    if (IsInertProperty(turn.rightHandTurnAction))
                    { turn.rightHandTurnAction = default; cleared++; }
                    break;
                case ActionBasedSnapTurnProvider snap:
                    if (IsInertProperty(snap.leftHandSnapTurnAction))
                    { snap.leftHandSnapTurnAction = default; cleared++; }
                    if (IsInertProperty(snap.rightHandSnapTurnAction))
                    { snap.rightHandSnapTurnAction = default; cleared++; }
                    break;
            }
            return cleared;
        }

        /// <summary>Clear inert properties across an explicit reader set (the post-travel repair path).</summary>
        public static int Clear(IList<Behaviour> readers)
        {
            if (readers == null) return 0;
            int cleared = 0;
            for (int i = 0; i < readers.Count; i++)
                cleared += Clear(readers[i]);
            return cleared;
        }

        /// <summary>
        /// Clear inert properties on every locomotion provider currently loaded, including inactive
        /// ones — an inactive provider is one OnEnable away from reading. This is the cold-boot and
        /// scene-load path; it must stay cheap enough to run on each scene load, which it is: three
        /// FindObjectsOfType calls at a boundary the game already spends frames on.
        /// </summary>
        public static int SweepLoadedProviders()
        {
            int cleared = 0;
            foreach (var move in Object.FindObjectsOfType<ActionBasedContinuousMoveProvider>(true))
                cleared += Clear(move);
            foreach (var turn in Object.FindObjectsOfType<ActionBasedContinuousTurnProvider>(true))
                cleared += Clear(turn);
            foreach (var snap in Object.FindObjectsOfType<ActionBasedSnapTurnProvider>(true))
                cleared += Clear(snap);
            return cleared;
        }
    }
}
