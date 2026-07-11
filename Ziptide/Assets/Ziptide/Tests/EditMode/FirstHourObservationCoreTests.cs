#if UNITY_EDITOR
using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay.Tutorial;

namespace Ziptide.Tests.EditMode
{
    public class FirstHourObservationCoreTests
    {
        private static readonly FirstHourObservationVector Forward =
            new FirstHourObservationVector(0d, 0d, 1d);

        [Test]
        public void Gaze_OutsideConeResetsContinuousDwell()
        {
            var core = new FirstHourObservationCore(18d, 0.75d);

            Assert.IsFalse(core.SampleLook(Forward, Forward, 0.5d));
            Assert.Greater(core.LookProgress01, 0d);

            Assert.IsFalse(core.SampleLook(
                Forward,
                new FirstHourObservationVector(1d, 0d, 0d),
                0.01d));
            Assert.AreEqual(0d, core.LookDwellSeconds);

            Assert.IsFalse(core.SampleLook(Forward, Forward, 0.74d));
            Assert.IsTrue(core.SampleLook(Forward, Forward, 0.01d));
            Assert.IsTrue(core.LookComplete);
        }

        [Test]
        public void Gaze_MissingOrInvalidTargetVectorIsNoOp()
        {
            var core = new FirstHourObservationCore();

            Assert.IsFalse(core.SampleLook(Forward, default, 100d));
            Assert.AreEqual(0d, core.LookDwellSeconds);
            Assert.IsFalse(core.SampleLook(
                Forward,
                new FirstHourObservationVector(double.NaN, 0d, 1d),
                100d));
            Assert.AreEqual(0d, core.LookDwellSeconds);
        }

        [Test]
        public void Movement_CompletesAtOneMeterNetPlanarDistance()
        {
            var core = new FirstHourObservationCore();
            core.ResetMovement();

            Assert.IsFalse(core.SampleMovement(Point(0d, 0d, 0d)));
            Assert.IsFalse(core.SampleMovement(Point(0.99d, 10d, 0d)));
            Assert.IsTrue(core.SampleMovement(Point(1.0d, -10d, 0d)));
            Assert.AreEqual(1d, core.MovementNetDistance, 0.000001d);
            Assert.IsTrue(core.MovementComplete);
        }

        [Test]
        public void Movement_CompletesAtCumulativeDistanceEvenAfterTurningBack()
        {
            var core = new FirstHourObservationCore();
            core.ResetMovement();

            Assert.IsFalse(core.SampleMovement(Point(0d, 0d, 0d)));
            Assert.IsFalse(core.SampleMovement(Point(0.5d, 0d, 0d)));
            Assert.IsFalse(core.SampleMovement(Point(0d, 0d, 0d)));
            Assert.IsTrue(core.SampleMovement(Point(0.5d, 0d, 0d)));

            Assert.AreEqual(1.5d, core.MovementCumulativeDistance, 0.000001d);
            Assert.AreEqual(0.5d, core.MovementNetDistance, 0.000001d);
        }

        [Test]
        public void Movement_StraightPathIsSamplingRateIndependent()
        {
            var coarse = new FirstHourObservationCore();
            coarse.SampleMovement(Point(0d, 0d, 0d));
            coarse.SampleMovement(Point(0.5d, 0d, 0d));
            coarse.SampleMovement(Point(1d, 0d, 0d));

            var fine = new FirstHourObservationCore();
            fine.SampleMovement(Point(0d, 0d, 0d));
            for (int i = 1; i <= 20; i++)
                fine.SampleMovement(Point(i / 20d, 0d, 0d));

            Assert.IsTrue(coarse.MovementComplete);
            Assert.IsTrue(fine.MovementComplete);
            Assert.AreEqual(coarse.MovementNetDistance, fine.MovementNetDistance, 0.000001d);
            Assert.AreEqual(coarse.MovementCumulativeDistance, fine.MovementCumulativeDistance, 0.000001d);
        }

        [Test]
        public void Movement_IgnoresVerticalDisplacement()
        {
            var core = new FirstHourObservationCore();
            core.SampleMovement(Point(0d, 0d, 0d));
            core.SampleMovement(Point(0d, 100d, 0d));

            Assert.IsFalse(core.MovementComplete);
            Assert.AreEqual(0d, core.MovementCumulativeDistance);
            Assert.AreEqual(0d, core.MovementNetDistance);
        }

        [Test]
        public void Arrival_RequiresBothFreeDwellAndNaturalDirectionChange()
        {
            var timeOnly = new FirstHourObservationCore();
            Assert.IsFalse(timeOnly.SampleArrival(Forward, 3.1d));
            Assert.AreEqual(0d, timeOnly.ArrivalMaxDirectionChangeDegrees, 0.000001d);

            var angleOnly = new FirstHourObservationCore();
            Assert.IsFalse(angleOnly.SampleArrival(Forward, 0.1d));
            Assert.IsFalse(angleOnly.SampleArrival(YawForward(30d), 0.1d));
            Assert.GreaterOrEqual(angleOnly.ArrivalMaxDirectionChangeDegrees, 25d);
            Assert.Less(angleOnly.ArrivalElapsedSeconds, 3d);

            Assert.IsTrue(angleOnly.SampleArrival(YawForward(30d), 2.8d));
            Assert.IsTrue(angleOnly.ArrivalComplete);
        }

        [Test]
        public void Arrival_HasNoForcedTargetAndInvalidTrackingDoesNotAdvance()
        {
            var core = new FirstHourObservationCore();

            Assert.IsFalse(core.SampleArrival(default, 100d));
            Assert.AreEqual(0d, core.ArrivalElapsedSeconds);

            Assert.IsFalse(core.SampleArrival(Forward, 1.5d));
            Assert.IsTrue(core.SampleArrival(YawForward(-30d), 1.5d));
            Assert.IsTrue(core.ArrivalComplete);
        }

        [Test]
        public void CompletedObservationsLatchUntilExplicitReset()
        {
            var core = new FirstHourObservationCore();
            Assert.IsTrue(core.SampleLook(Forward, Forward, 1d));
            Assert.IsTrue(core.SampleLook(Forward, new FirstHourObservationVector(-1d, 0d, 0d), 0d));
            Assert.IsTrue(core.LookComplete);

            core.ResetLook();
            Assert.IsFalse(core.LookComplete);
            Assert.AreEqual(0d, core.LookProgress01);
        }

        [Test]
        public void Adapter_MapsOnlyTheThreeOwnedBeats()
        {
            string signal;
            Assert.IsTrue(FirstHourObservationAdapter.TryGetSignalId(
                FirstHourObservationAdapter.LookBeatId, out signal));
            Assert.AreEqual(FirstHourObservationAdapter.LookSignalId, signal);

            Assert.IsTrue(FirstHourObservationAdapter.TryGetSignalId(
                FirstHourObservationAdapter.MoveBeatId, out signal));
            Assert.AreEqual(FirstHourObservationAdapter.MoveSignalId, signal);

            Assert.IsTrue(FirstHourObservationAdapter.TryGetSignalId(
                FirstHourObservationAdapter.ArrivalBeatId, out signal));
            Assert.AreEqual(FirstHourObservationAdapter.ArrivalSignalId, signal);

            Assert.IsFalse(FirstHourObservationAdapter.TryGetSignalId("FH_NOT_OWNED", out signal));
            Assert.IsNull(signal);
        }

        [Test]
        public void AdapterSource_HasNoRigTravelProfileOrInputOwnership()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "Tutorial",
                "FirstHourObservationAdapter.cs");
            Assert.IsTrue(File.Exists(path), path);

            string source = File.ReadAllText(path);
            StringAssert.DoesNotContain("TravelCoordinator", source);
            StringAssert.DoesNotContain("SaveSystem", source);
            StringAssert.DoesNotContain("PlayerProfile", source);
            StringAssert.DoesNotContain("Teleport", source);
            StringAssert.DoesNotContain("transform.position =", source);
            StringAssert.DoesNotContain("camera.transform.position =", source);
            StringAssert.DoesNotContain("InputAction", source);
            StringAssert.DoesNotContain("CharacterController", source);
        }

        [Test]
        public void CoreSource_HasNoUnityDependencyOrClock()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "Tutorial",
                "FirstHourObservationCore.cs");
            Assert.IsTrue(File.Exists(path), path);

            string source = File.ReadAllText(path);
            StringAssert.DoesNotContain("UnityEngine", source);
            StringAssert.DoesNotContain("Time.", source);
            StringAssert.DoesNotContain("DateTime", source);
            StringAssert.DoesNotContain("Debug.Log", source);
        }

        private static FirstHourObservationVector Point(double x, double y, double z)
        {
            return new FirstHourObservationVector(x, y, z);
        }

        private static FirstHourObservationVector YawForward(double degrees)
        {
            double radians = degrees * Math.PI / 180d;
            return new FirstHourObservationVector(Math.Sin(radians), 0d, Math.Cos(radians));
        }
    }
}
#endif
