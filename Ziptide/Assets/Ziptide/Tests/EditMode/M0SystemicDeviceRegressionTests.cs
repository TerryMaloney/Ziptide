#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Regression packet for the 2026-07-27 Quest evidence. These checks target shared owners and build
    /// hooks so the same failure cannot be copied into later worlds or side modes.
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
        }

        [Test]
        public void XrRigHeightContract_UsesFloorTrackingWithZeroSyntheticOffset()
        {
            string source = Read("Editor", "Patching", "PlayerRigHeightContractEnforcer.cs");
            StringAssert.Contains("FloorTrackingOriginMode = 2", source);
            StringAssert.Contains("offset.floatValue = 0f", source);
            StringAssert.Contains("expected Floor tracking and cameraYOffset=0", source);
        }

        [Test]
        public void SmoothTurnContract_RequiresContinuousTurnAction_NotSnapAction()
        {
            string source = Read("Editor", "Patching", "LocomotionContractEnforcer.cs");
            StringAssert.Contains("FindReference(\"Turn\")", source);
            StringAssert.Contains("continuous provider must use action 'Turn'", source);
            StringAssert.Contains("snap provider must use action 'Snap Turn'", source);
            StringAssert.Contains("smooth and snap providers share one InputActionReference", source);
        }

        [Test]
        public void InventoryRestore_UsesRealSocketSelection_NeverProximityOwnership()
        {
            string source = Read("Gameplay", "Runtime", "Inventory", "InventoryState.cs");
            StringAssert.Contains("manager.SelectEnter(socket, grab);", source);
            StringAssert.Contains("HOLSTER_DOCK_BLOCKER", source);
            StringAssert.DoesNotContain("Vector3.Distance(item.transform.position, h.transform.position) <= 0.18f", source);
            StringAssert.DoesNotContain("item.transform.SetParent(anchor, false);", source);
        }

        [Test]
        public void BreakerBlade_IsPortableAndHolstersPointDown()
        {
            Assert.IsTrue(HolsterSocketInteractor.AllowsItemId("breaker_blade"));
            Quaternion pose = HolsterPoseCore.Resolve("breaker_blade", "HolsterRight");
            Vector3 forward = pose * Vector3.forward;
            Assert.Less(forward.y, -0.85f, "Holstered blade must point down the leg, not forward.");
        }

        [Test]
        public void BreakerBlade_HandPoseForcesForwardYaw_AndSocketsDoNotAttack()
        {
            string source = Read("Gameplay", "Runtime", "Weapons", "MeleeWeaponRuntime.cs");
            StringAssert.Contains("new Vector3(82f, 180f, 0f)", source);
            StringAssert.Contains("Yaw is a device contract, not stale asset data", source);
            StringAssert.DoesNotContain("pose.y = def.gripLocalEuler.y;", source);
            StringAssert.Contains("interactor is XRBaseControllerInteractor", source);
            StringAssert.DoesNotContain("bool held = _grab != null && _grab.isSelected;", source);
        }

        [Test]
        public void SameSceneReturnToShip_IsAnUnstuckRecovery_NotANoop()
        {
            string source = Read("Gameplay", "Runtime", "Player", "PlayerMenuRuntime.cs");
            StringAssert.Contains("PlayerSafetyRuntime.RecoverNow(\"field_menu_same_scene\")", source);
            StringAssert.Contains("action=recover_spawn", source);
        }

        [Test]
        public void SpawnRuntimeDiagnostic_MatchesAuditFilteringAndEscalatesRealBlockers()
        {
            string source = Read("Gameplay", "Runtime", "World", "SpawnMarkerRuntime.cs");
            StringAssert.Contains("QueryTriggerInteraction.Ignore", source);
            StringAssert.Contains("col.GetComponentInParent<PlayerRigPersistence>()", source);
            StringAssert.Contains("SPAWN_RUNTIME_BLOCKER", source);
        }

        [Test]
        public void PlayerHeightSafety_ProtectsChildrenAndRejectsImpossibleEyeHeight()
        {
            Assert.Less(PlayerSafetyRuntime.MinimumPlausibleEyeHeight, 0.60f);
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
        public void HomeHubAnchor_IsLockedAfterTrackingSettle()
        {
            string source = Read("Gameplay", "Runtime", "Tutorial", "HomeHubAnchorLockRuntime.cs");
            StringAssert.Contains("stableFor >= 0.45f", source);
            StringAssert.Contains("_hub.enabled = false", source);
            StringAssert.Contains("HOME_HUB_ANCHOR locked_world=true", source);
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
