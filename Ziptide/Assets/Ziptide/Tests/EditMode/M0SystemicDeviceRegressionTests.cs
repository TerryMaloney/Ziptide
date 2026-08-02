#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Gameplay;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Regression packet for the 2026-07-27 and 2026-07-28 Quest evidence. Numeric pose guesses are
    /// forbidden; tests assert semantic axes, owner lifecycle, and mutually-exclusive locomotion modes.
    /// </summary>
    public sealed class M0SystemicDeviceRegressionTests
    {
        [Test]
        public void BootPatcher_InstallsPlayerHeightSafetyAndBuildContracts()
        {
            string d2 = Read("Editor", "Patching", "ScenePatcherD2.cs");
            StringAssert.Contains("PatcherUtil.EnsureComponent<PlayerSafetyRuntime>(xrOriginGo);", d2);
            StringAssert.Contains("LocomotionContractEnforcer.EnsureCurrentScene();", d2);
            StringAssert.Contains("PlayerRigHeightContractEnforcer.EnsureCurrentScene();", d2);

            string belt = Read("Gameplay", "Runtime", "Inventory", "BeltRig.cs");
            StringAssert.Contains("PlayerSafetyRuntime.EnsureOnRig(rig.gameObject);", belt,
                "Runtime rig must install safety even when generated Boot content is stale.");
            StringAssert.Contains("TurnModeRuntimeAuthority.EnsureOnRig(rig.gameObject);", belt);
        }

        [Test]
        public void XrRigHeightContract_UsesFloorTrackingAtBuildAndRuntime()
        {
            string editor = Read("Editor", "Patching", "PlayerRigHeightContractEnforcer.cs");
            StringAssert.Contains("FloorTrackingOriginMode = 2", editor);
            StringAssert.Contains("offset.floatValue = 0f", editor);
            StringAssert.Contains("cameraOffset.localPosition", editor);

            string runtime = Read("Gameplay", "Runtime", "Player", "PlayerSafetyRuntime.cs");
            StringAssert.Contains("TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor)", runtime);
            StringAssert.Contains("CameraYOffset", runtime);
            StringAssert.Contains("CameraFloorOffsetObject", runtime);
            StringAssert.Contains("PLAYER_HEIGHT_RUNTIME", runtime);
            StringAssert.Contains("PLAYER_HEIGHT_REPAIRED", runtime);
        }

        [Test]
        public void SmoothTurnContract_IsOneActiveValueVector2Provider()
        {
            string source = Read("Editor", "Patching", "LocomotionContractEnforcer.cs");
            StringAssert.Contains("FindReference(\"Turn\")", source);
            StringAssert.Contains("continuous Turn must be Value/Vector2", source);
            StringAssert.Contains("smooth.enabled = true", source);
            StringAssert.Contains("snap.enabled = false", source);
            StringAssert.Contains("Golden route requires smooth enabled and snap disabled", source);
            Assert.That(TurnModeCore.DefaultSmoothTurnSpeed, Is.InRange(45f, 75f));
        }

        [Test]
        public void SmoothTurnRuntime_WaitsForUsableInputAndNeverLeavesSnapEnabled()
        {
            GameObject rig = new GameObject("TurnReadinessRig");
            try
            {
                ActionBasedContinuousTurnProvider smooth = rig.AddComponent<ActionBasedContinuousTurnProvider>();
                ActionBasedSnapTurnProvider snap = rig.AddComponent<ActionBasedSnapTurnProvider>();
                snap.enabled = true;

                bool ready = TurnModeCore.ApplySmoothOnlyWhenReady(rig.transform);

                Assert.IsFalse(ready, "An unbound turn action is not device-ready.");
                Assert.IsFalse(smooth.enabled, "XRI must not ReadValue from an unready action.");
                Assert.IsFalse(snap.enabled, "Golden route never leaves snap and smooth active together.");
            }
            finally
            {
                Object.DestroyImmediate(rig);
            }

            string runtime = Read("Gameplay", "Runtime", "Player", "PlayerSafetyRuntime.cs");
            StringAssert.Contains("action.controls.Count == 0", runtime);
            StringAssert.Contains("action.ReadValue<Vector2>()", runtime);
            StringAssert.Contains("DefaultExecutionOrder(-10000)", runtime);
            StringAssert.Contains("TURN_MODE smoothReady=", runtime);
        }

        [Test]
        public void InventoryRestore_UsesRealSocketSelection_NeverProximityOwnership()
        {
            string source = Read("Gameplay", "Runtime", "Inventory", "InventoryState.cs");
            // The point of this assertion is that restore goes through a REAL socket selection
            // rather than proximity ownership - not which overload spells it. The concrete-class
            // overload is a CS0619 hard error on Unity 6, so the call is now interface-typed.
            StringAssert.Contains("manager.SelectEnter((IXRSelectInteractor)socket, (IXRSelectInteractable)grab);",
                source);
            StringAssert.Contains("HOLSTER_DOCK_BLOCKER", source);
            StringAssert.DoesNotContain("Vector3.Distance(item.transform.position, h.transform.position) <= 0.18f", source);
            StringAssert.DoesNotContain("item.transform.SetParent(anchor, false);", source);
        }

        [Test]
        public void Holster_UsesPerItemXriAttachTargetAndSemanticDownAxis()
        {
            Assert.IsTrue(HolsterSocketInteractor.AllowsItemId("breaker_blade"));
            string source = Read("Gameplay", "Runtime", "Inventory", "HolsterSocketInteractor.cs");
            StringAssert.Contains("override Transform GetAttachTransform(IXRInteractable interactable)", source);
            StringAssert.Contains("HolsterPose_", source);
            StringAssert.Contains("WeaponPoseCore.MapLocalBasisToWorld", source);
            StringAssert.DoesNotContain("socketAnchor.rotation =", source);

            Quaternion root = WeaponPoseCore.MapLocalBasisToWorld(
                Vector3.forward, Vector3.up, Vector3.down, Vector3.forward);
            Vector3 mappedAxis = root * Vector3.forward;
            Assert.Greater(Vector3.Dot(mappedAxis.normalized, Vector3.down), 0.999f);
        }

        [Test]
        public void BreakerBlade_HandPoseUsesTipAxis_NotEulerGuess_AndSocketsDoNotAttack()
        {
            Quaternion basis = WeaponPoseCore.BuildLocalBasis(Vector3.up, Vector3.forward);
            Assert.Greater(Vector3.Dot((basis * Vector3.forward).normalized, Vector3.up), 0.999f);

            string source = Read("Gameplay", "Runtime", "Weapons", "MeleeWeaponRuntime.cs");
            StringAssert.Contains("MELEE_GRIP_SEMANTIC", source);
            StringAssert.Contains("ResolveAxisLocal", source);
            StringAssert.DoesNotContain("new Vector3(82f, 180f, 0f)", source);
            StringAssert.DoesNotContain("MELEE_GRIP_POSE", source);
            StringAssert.Contains("interactor is XRBaseControllerInteractor", source);
            StringAssert.DoesNotContain("bool held = _grab != null && _grab.isSelected;", source);
        }

        [Test]
        public void TheEscapeHatch_IsOwnedByThePersistentRig_NotByAnOptionalLocomotionComponent()
        {
            // DV-02: "after entering ToxicCity I could not leave the world." PlayerMenuRuntime existed
            // the whole time — it was installed only as a side effect of DashLocomotion's Awake, so a
            // rig without that component shipped with no way out at all.
            string rig = Read("Gameplay", "Runtime", "Player", "PlayerRigPersistence.cs");
            StringAssert.Contains("EnsurePlayerMenu();", rig);
            StringAssert.Contains("PLAYER_MENU_ENSURED", rig);
            StringAssert.Contains("AddComponent<PlayerMenuRuntime>()", rig);
        }

        [Test]
        public void SameSceneReturnToShip_IsAnUnstuckRecovery_NotANoop()
        {
            string source = Read("Gameplay", "Runtime", "Player", "PlayerMenuRuntime.cs");
            StringAssert.Contains("PlayerSafetyRuntime.RecoverNow(\"field_menu_same_scene\")", source);
            StringAssert.Contains("action=recover_spawn", source);
        }

        [Test]
        public void SpawnRuntimeDiagnostic_ExcludesOnlyExplicitNonSolidVisuals()
        {
            string source = Read("Gameplay", "Runtime", "World", "SpawnMarkerRuntime.cs");
            StringAssert.Contains("QueryTriggerInteraction.Ignore", source);
            StringAssert.Contains("col.GetComponentInParent<PlayerRigPersistence>()", source);
            StringAssert.Contains("col.GetComponentInParent<ObjectiveBeacon>()", source);
            StringAssert.Contains("SkySphere", source);
            StringAssert.Contains("SPAWN_RUNTIME_BLOCKER", source);
            StringAssert.Contains("sceneName != ZiptideConstants.SceneBoot", source);
        }

        [Test]
        public void PlayerHeightSafety_ProtectsChildrenAndRejectsImpossibleEyeHeight()
        {
            Assert.Less(PlayerSafetyRuntime.MinimumPlausibleEyeHeight, 0.45f);
            Assert.Greater(PlayerSafetyRuntime.MaximumPlausibleEyeHeight, 2.0f);
            Assert.Less(PlayerSafetyRuntime.MaximumPlausibleEyeHeight, 2.5f);
            Assert.That(PlayerSafetyRuntime.RecoveryEyeHeight,
                Is.InRange(PlayerSafetyRuntime.MinimumPlausibleEyeHeight, PlayerSafetyRuntime.MaximumPlausibleEyeHeight));
        }

        [Test]
        public void VehicleSafety_RejectsRoofJumpClassAndRepairsMountedEyeHeight()
        {
            Assert.LessOrEqual(VehicleSafetyRuntime.MaximumGroundStep, 1.5f);
            Assert.LessOrEqual(VehicleSafetyRuntime.MaximumVerticalEscape, 2.5f);
            Assert.Less(VehicleSafetyRuntime.MaximumMountedEyeHeight, 2.0f);
            string source = Read("Ship", "Runtime", "VehicleSafetyRuntime.cs");
            StringAssert.Contains("VEHICLE_VERTICAL_ESCAPE_RECOVER", source);
            StringAssert.Contains("VEHICLE_EYE_HEIGHT_REPAIRED", source);
        }

        [Test]
        public void CouplerPart_HasSingleOwnerCollisionIsolationAndEscapeRecovery()
        {
            string source = Read("Gameplay", "Runtime", "Story", "RepairPartSafetyRuntime.cs");
            StringAssert.Contains("InteractableSelectMode.Single", source);
            StringAssert.Contains("Physics.IgnoreCollision", source);
            StringAssert.Contains("REPAIR_PART_SELECT", source);
            StringAssert.Contains("REPAIR_PART_RECOVERED", source);
            Assert.IsTrue(File.Exists(Path.Combine(Application.dataPath, "Ziptide", "Gameplay", "Runtime",
                "Story", "RepairPartSafetyInstallerRuntime.cs")));
        }

        [Test]
        public void HomeHubDeparture_IsOneWayAndRestoresCachedSockets()
        {
            string source = Read("Gameplay", "Runtime", "Tutorial", "HomeHubAnchorLockRuntime.cs");
            StringAssert.Contains("stableFor >= 0.45f", source);
            StringAssert.Contains("_hub.enabled = false", source);
            StringAssert.Contains("_departureCommitted = true", source);
            StringAssert.Contains("if (_departureCommitted) return;", source);
            StringAssert.Contains("_socketStates", source);
            StringAssert.Contains("HOME_HUB_DEPARTURE sockets_restored=true", source);
        }

        [Test]
        public void ObjectiveBeacon_HidesBeforeItCanObstructInteractionOrCockpit()
        {
            Assert.LessOrEqual(ObjectiveBeacon.CloseHideDistance, 5f);
            string source = Read("Gameplay", "Runtime", "World", "ObjectiveBeacon.cs");
            StringAssert.Contains("BEACON_CLOSE_HIDE", source);
            StringAssert.Contains("_pillar.gameObject.SetActive(!shouldHide)", source);
        }

        private static string Read(params string[] parts)
        {
            string path = Path.Combine(Application.dataPath, "Ziptide");
            foreach (string part in parts) path = Path.Combine(path, part);
            Assert.IsTrue(File.Exists(path), path);
            return File.ReadAllText(path);
        }
    }
}
#endif
