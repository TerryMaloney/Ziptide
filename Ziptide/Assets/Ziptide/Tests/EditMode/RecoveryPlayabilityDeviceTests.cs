#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Source-contract pins for the 2026-07-26 Quest 3S device findings. These do not pretend to be
    /// device proof; they prevent the exact repaired ownership mistakes from silently returning before
    /// the next Golden/headset gate.
    /// </summary>
    public class RecoveryPlayabilityDeviceTests
    {
        [Test]
        public void PlayerMenu_OwnsYAndReturnsThroughCanonicalTravelWithoutLoadingBoot()
        {
            string source = Read("Gameplay", "Runtime", "Player", "PlayerMenuRuntime.cs");

            StringAssert.Contains("<XRController>{LeftHand}/secondaryButton", source);
            StringAssert.Contains("TravelCoordinator.TravelTo(ZiptideConstants.SceneW000)", source);
            StringAssert.Contains("RETURN TO SHIP", source);
            StringAssert.Contains("RebindInteractables();", source);
            StringAssert.Contains("PLAYER_MENU_BOUND", source);
            StringAssert.DoesNotContain("TravelTo(ZiptideConstants.SceneBoot", source);
            StringAssert.DoesNotContain("SceneManager.LoadScene", source);
            StringAssert.DoesNotContain("Time.timeScale =", source);
        }

        [Test]
        public void GunLaser_RequiresAControllerHandRatherThanAnySelector()
        {
            string source = Read("Gameplay", "Runtime", "Weapons", "GunLaserSight.cs");

            StringAssert.Contains("IsSelectedByControllerHand()", source);
            StringAssert.Contains("interactor as XRBaseControllerInteractor", source);
            StringAssert.Contains("controller.isActiveAndEnabled", source);
            StringAssert.DoesNotContain("bool show = _grab != null && _grab.isSelected", source);
        }

        [Test]
        public void Crouch_ClickIsTurnGuardedAndCameraHeightIsBlended()
        {
            string source = Read("Gameplay", "Runtime", "Locomotion", "DashLocomotion.cs");

            StringAssert.Contains("CrouchTurnDeadzone", source);
            StringAssert.Contains("crouch_ignored reason=turning", source);
            StringAssert.Contains("turnMagnitude <= CrouchTurnDeadzone", source);
            StringAssert.Contains("Mathf.SmoothDamp", source);
            StringAssert.Contains("player-menu=Y", source);
        }

        [Test]
        public void BreakerBlade_HasADeviceSpecificVerticalPoseWithoutChangingGunTilt()
        {
            string melee = Read("Gameplay", "Runtime", "Weapons", "MeleeWeaponRuntime.cs");
            string factory = Read("Gameplay", "Runtime", "Items", "ItemFactory.cs");

            StringAssert.Contains("BreakerBladeDeviceGripEuler = new Vector3(90f, 0f, 0f)", melee);
            StringAssert.Contains("weapon=breaker_blade euler=", melee);
            StringAssert.Contains("private static readonly Vector3 GunGripTilt", factory);
        }

        [Test]
        public void ToxicExposure_UsesVisibleSurfaceAsPrimaryTruthAndLogsFallback()
        {
            string source = Read("Gameplay", "Runtime", "World", "ToxicRiverRuntime.cs");

            StringAssert.Contains("transform.Find(\"ToxicSurface\")", source);
            StringAssert.Contains("boundary=", source);
            StringAssert.Contains("visible_surface", source);
            StringAssert.Contains("authored_fallback", source);
            StringAssert.Contains("return horizontal && atSurface", source);
        }

        [Test]
        public void Taser_StopsVelocityBeforeBecomingKinematic()
        {
            string source = Read("Gameplay", "Runtime", "Weapons", "TaserDartProjectile.cs");
            int velocity = source.IndexOf("rb.velocity = Vector3.zero;");
            int angular = source.IndexOf("rb.angularVelocity = Vector3.zero;");
            int kinematic = source.IndexOf("rb.isKinematic = true;");

            Assert.GreaterOrEqual(velocity, 0);
            Assert.Greater(angular, velocity);
            Assert.Greater(kinematic, angular,
                "Do not write Rigidbody velocity after switching to kinematic; Quest logs warn every time.");
        }

        [Test]
        public void HomeHub_RetuneRemainsInsideTheExistingHandReachGate()
        {
            float along = Ziptide.Gameplay.HomeHubRuntime.AnchorDistance
                - Ziptide.Gameplay.HomeHubRuntime.TileForwardOffset;
            float furthestTile = new Vector3(0.72f, -0.22f, along).magnitude;

            Assert.GreaterOrEqual(along, 0.50f, "The tile row must not sit in the player's face.");
            Assert.LessOrEqual(furthestTile, 0.95f,
                "The no-ray direct-hand fallback must remain valid for the three-tile layout.");
        }

        private static string Read(params string[] path)
        {
            string full = Path.Combine(Application.dataPath, "Ziptide");
            for (int i = 0; i < path.Length; i++) full = Path.Combine(full, path[i]);
            Assert.IsTrue(File.Exists(full), full);
            return File.ReadAllText(full);
        }
    }
}
#endif
