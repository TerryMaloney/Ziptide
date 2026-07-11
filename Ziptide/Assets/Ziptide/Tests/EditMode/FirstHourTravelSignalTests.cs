#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class FirstHourTravelSignalTests
    {
        [Test]
        public void SuccessfulTravel_PublishesDestinationOnce()
        {
            var published = new List<string>();
            bool emitted = false;

            bool first = TravelCoordinator.TryPublishTravelCompleted(
                ZiptideConstants.SceneToxicCity,
                true,
                ref emitted,
                published.Add);
            bool duplicate = TravelCoordinator.TryPublishTravelCompleted(
                ZiptideConstants.SceneToxicCity,
                true,
                ref emitted,
                published.Add);

            Assert.IsTrue(first);
            Assert.IsFalse(duplicate);
            Assert.IsTrue(emitted);
            CollectionAssert.AreEqual(new[] { ZiptideConstants.SceneToxicCity }, published);
        }

        [Test]
        public void FailedOrEmptyTravel_PublishesNothing()
        {
            var published = new List<string>();
            bool emitted = false;

            Assert.IsFalse(TravelCoordinator.TryPublishTravelCompleted(
                ZiptideConstants.SceneToxicCity,
                false,
                ref emitted,
                published.Add));
            Assert.IsFalse(TravelCoordinator.TryPublishTravelCompleted(
                "",
                true,
                ref emitted,
                published.Add));

            Assert.IsFalse(emitted);
            Assert.AreEqual(0, published.Count);
        }

        [Test]
        public void MissingSubscriber_DoesNotThrowAndStillLatchesSuccess()
        {
            bool emitted = false;
            Assert.DoesNotThrow(() =>
            {
                Assert.IsTrue(TravelCoordinator.TryPublishTravelCompleted(
                    ZiptideConstants.SceneW000,
                    true,
                    ref emitted,
                    null));
            });
            Assert.IsTrue(emitted);
        }

        [Test]
        public void CanonicalDestinations_MapToExactFirstHourSignals()
        {
            Assert.AreEqual(
                TravelCoordinator.FirstHourOutboundSignal,
                TravelCoordinator.MapFirstHourTravelSignal(ZiptideConstants.SceneToxicCity));
            Assert.AreEqual(
                "TRAVEL_W000_TO_W001_COMPLETE",
                TravelCoordinator.MapFirstHourTravelSignal("ToxicCity"));

            Assert.AreEqual(
                TravelCoordinator.FirstHourReturnSignal,
                TravelCoordinator.MapFirstHourTravelSignal(ZiptideConstants.SceneW000));
            Assert.AreEqual(
                "TRAVEL_W001_TO_W000_COMPLETE",
                TravelCoordinator.MapFirstHourTravelSignal("W000_DriftIn"));
        }

        [Test]
        public void UnrelatedDestination_MapsNoneButNeutralPublicationRemainsValid()
        {
            Assert.IsNull(TravelCoordinator.MapFirstHourTravelSignal("W007_SableStation"));

            var published = new List<string>();
            bool emitted = false;
            Assert.IsTrue(TravelCoordinator.TryPublishTravelCompleted(
                "W007_SableStation",
                true,
                ref emitted,
                published.Add));
            CollectionAssert.AreEqual(new[] { "W007_SableStation" }, published);
        }

        [Test]
        public void RuntimeSource_PublishesOnlyAfterRestoreAndTravelOk()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "World",
                "TravelCoordinator.cs");
            Assert.IsTrue(File.Exists(path), path);

            string source = File.ReadAllText(path);
            int restore = source.IndexOf("yield return InventoryState.RestoreAfterTravel(playerRig.transform);");
            int travelOk = source.IndexOf("Debug.Log(\"ZIPTIDE: TRAVEL_OK dest=\" + sceneName);");
            int publish = source.IndexOf("TryPublishTravelCompleted(", travelOk);

            Assert.GreaterOrEqual(restore, 0);
            Assert.Greater(travelOk, restore);
            Assert.Greater(publish, travelOk);
            StringAssert.Contains("public static event Action<string> TravelCompleted", source);
            StringAssert.Contains("ZIPTIDE: FIRST_HOUR_TRAVEL dest=", source);
            Assert.AreEqual(2, Count(source, "SceneManager.LoadScene(sceneName);"),
                "FH-S03 must not add or remove scene-load calls");
            Assert.AreEqual(1, Count(source, "yield return InventoryState.RestoreAfterTravel(playerRig.transform);"));
            StringAssert.DoesNotContain("TravelCompleted?.Invoke(sceneName)",
                source.Substring(0, restore),
                "travel completion must never emit before inventory restoration");
        }

        private static int Count(string source, string token)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(token, index)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }
    }
}
#endif
