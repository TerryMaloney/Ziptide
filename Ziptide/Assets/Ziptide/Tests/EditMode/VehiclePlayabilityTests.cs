#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Regression gates for the Quest 3S vehicle findings. Pure math tests verify the mounted yaw
    /// frame and steering deadzone; source-contract pins keep giant mount tiles and snap-only yaw from
    /// returning before the next device pass.
    /// </summary>
    public class VehiclePlayabilityTests
    {
        [Test]
        public void ShapeSteer_DeadzoneAndClampAreDeterministic()
        {
            Assert.AreEqual(0f, VehicleRuntime.ShapeSteer(0.10f), 0.0001f);
            Assert.AreEqual(0f, VehicleRuntime.ShapeSteer(-0.17f), 0.0001f);
            Assert.AreEqual(0.5f, VehicleRuntime.ShapeSteer(0.5f), 0.0001f);
            Assert.AreEqual(-1f, VehicleRuntime.ShapeSteer(-2f), 0.0001f);
        }

        [Test]
        public void CarryRigYaw_AppliesOnlyTheVehicleYawDelta()
        {
            Quaternion initial = Quaternion.Euler(0f, 15f, 0f);
            Quaternion carried = VehicleRuntime.CarryRigYaw(initial, 20f, 65f);
            Vector3 forward = carried * Vector3.forward;
            Vector3 expected = Quaternion.Euler(0f, 60f, 0f) * Vector3.forward;

            Assert.AreEqual(1f, Vector3.Dot(forward.normalized, expected.normalized), 0.001f);
        }

        [Test]
        public void Runtime_UsesCompactMountGripSmoothSteeringAndXStepOff()
        {
            string source = ReadVehicleSource();

            StringAssert.Contains("MountGrip", source);
            StringAssert.Contains("ZiptideRideDismount", source);
            StringAssert.Contains("<XRController>{LeftHand}/primaryButton", source);
            StringAssert.Contains("CarryRigYaw", source);
            StringAssert.Contains("ShapeSteer(right.x)", source);
            StringAssert.Contains("_playerMenu.IsOpen", source);
            StringAssert.Contains("ControlGrip_L", source);
            StringAssert.Contains("ControlGrip_R", source);
            StringAssert.DoesNotContain("Tile_MOUNT", source);
            StringAssert.DoesNotContain("Tile_DISMOUNT", source);
            StringAssert.DoesNotContain("FlightModel.SnapYaw", source);
        }

        [Test]
        public void Hoverbike_KeepsTheCentralEyeChannelClearByConstruction()
        {
            string source = ReadVehicleSource();

            StringAssert.Contains("central x ±0.24 m channel", source);
            StringAssert.Contains("V(side * .42f, .42f, .70f)", source);
            StringAssert.Contains("V(.05f, .28f, .05f)", source);
            StringAssert.DoesNotContain("V(side * .25f, .64f, .78f)", source);
            StringAssert.DoesNotContain("V(.07f, .68f, .07f)", source);
        }

        private static string ReadVehicleSource()
        {
            string full = Path.Combine(Application.dataPath, "Ziptide", "Ship", "Runtime", "VehicleRuntime.cs");
            Assert.IsTrue(File.Exists(full), full);
            return File.ReadAllText(full);
        }
    }
}
#endif
