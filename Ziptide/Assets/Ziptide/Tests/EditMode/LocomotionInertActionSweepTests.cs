using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Pins VR_RIG_GOTCHAS #9. The defect these guard is not cosmetic: a zero-binding embedded XRI
    /// action throws NullReferenceException inside InputActionState.ApplyProcessors on the provider's
    /// first read, which on device is "turning is dead and the log is full of exceptions".
    ///
    /// The rule is asymmetric on purpose — clearing too little leaves the crash, clearing too much
    /// deletes real input — so both directions are asserted here.
    /// </summary>
    public class LocomotionInertActionSweepTests
    {
        private readonly List<GameObject> _spawned = new List<GameObject>();
        private readonly List<Object> _assets = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject go in _spawned)
                if (go != null) Object.DestroyImmediate(go);
            _spawned.Clear();
            foreach (Object asset in _assets)
                if (asset != null) Object.DestroyImmediate(asset);
            _assets.Clear();
        }

        private T Spawn<T>(string name) where T : Component
        {
            var go = new GameObject(name);
            _spawned.Add(go);
            return go.AddComponent<T>();
        }

        [Test]
        public void AnEmptyEmbeddedAction_IsInert()
        {
            Assert.IsTrue(LocomotionInertActionSweep.IsInertAction(new InputAction("unused")),
                "no map and no bindings means no control can ever resolve");
        }

        [Test]
        public void ABoundEmbeddedAction_IsNotInert()
        {
            var bound = new InputAction("turn", InputActionType.Value, "<Gamepad>/rightStick");
            Assert.IsFalse(LocomotionInertActionSweep.IsInertAction(bound),
                "a bound action is real input and must survive the sweep");
        }

        [Test]
        public void AReferencedProperty_IsNeverInert()
        {
            // The dangerous direction. An InputActionReference points at an asset that owns its own
            // binding state; nulling it would delete the player's actual controls.
            var asset = ScriptableObject.CreateInstance<InputActionAsset>();
            _assets.Add(asset);
            InputActionMap map = asset.AddActionMap("Locomotion");
            InputAction action = map.AddAction("Turn", InputActionType.Value);
            InputActionReference reference = InputActionReference.Create(action);
            _assets.Add(reference);

            Assert.IsFalse(LocomotionInertActionSweep.IsInertProperty(new InputActionProperty(reference)),
                "a referenced property must never be swept, even with no bindings authored yet");
        }

        [Test]
        public void TheSweep_NullsTheUnusedHand_AndLeavesTheRealOneAlone()
        {
            var turn = Spawn<ActionBasedContinuousTurnProvider>("SmoothTurn");
            turn.leftHandTurnAction = new InputActionProperty(new InputAction("unused"));
            turn.rightHandTurnAction = new InputActionProperty(
                new InputAction("turn", InputActionType.Value, "<Gamepad>/rightStick"));

            Assert.AreEqual(1, LocomotionInertActionSweep.Clear(turn));
            Assert.IsNull(turn.leftHandTurnAction.action,
                "a null action is the durable spelling of 'this hand does not turn' — "
                + "a merely disabled one is revived by XRI's next OnEnable");
            Assert.IsNotNull(turn.rightHandTurnAction.action, "right stick still turns");
        }

        [Test]
        public void TheSweep_CoversMove_Turn_AndSnap()
        {
            // The class, not the instance: every ActionBased locomotion provider ZIPTIDE authors has
            // an unused hand, so missing one provider type reopens the same crash on a different stick.
            var move = Spawn<ActionBasedContinuousMoveProvider>("Move");
            var turn = Spawn<ActionBasedContinuousTurnProvider>("SmoothTurn");
            var snap = Spawn<ActionBasedSnapTurnProvider>("SnapTurn");
            move.rightHandMoveAction = new InputActionProperty(new InputAction("unused"));
            turn.leftHandTurnAction = new InputActionProperty(new InputAction("unused"));
            snap.leftHandSnapTurnAction = new InputActionProperty(new InputAction("unused"));

            var readers = new List<Behaviour> { move, turn, snap };
            Assert.AreEqual(3, LocomotionInertActionSweep.Clear(readers));
            Assert.IsNull(move.rightHandMoveAction.action);
            Assert.IsNull(turn.leftHandTurnAction.action);
            Assert.IsNull(snap.leftHandSnapTurnAction.action);
        }

        [Test]
        public void TheSweep_IsIdempotent()
        {
            // It runs on install and again on every scene load, so a second pass must be a no-op
            // rather than churn the properties of an already-clean rig.
            var turn = Spawn<ActionBasedContinuousTurnProvider>("SmoothTurn");
            turn.leftHandTurnAction = new InputActionProperty(new InputAction("unused"));

            Assert.AreEqual(1, LocomotionInertActionSweep.Clear(turn));
            Assert.AreEqual(0, LocomotionInertActionSweep.Clear(turn));
        }

        [Test]
        public void TheSweep_IgnoresBehavioursThatAreNotLocomotionProviders()
        {
            var stranger = Spawn<LocomotionSystem>("NotAProvider");
            Assert.AreEqual(0, LocomotionInertActionSweep.Clear(stranger));
            Assert.AreEqual(0, LocomotionInertActionSweep.Clear((IList<Behaviour>)null));
            Assert.AreEqual(0, LocomotionInertActionSweep.Clear((Behaviour)null));
        }
    }
}
