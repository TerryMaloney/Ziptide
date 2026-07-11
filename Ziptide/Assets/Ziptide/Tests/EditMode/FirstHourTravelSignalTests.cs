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
            string source = ReadTravelSource();
            int restore = source.IndexOf("yield return InventoryState.RestoreAfterTravel(playerRig.transform);");
            int travelOk = source.IndexOf("Debug.Log(\"ZIPTIDE: TRAVEL_OK dest=\" + sceneName);");
            int publish = source.IndexOf("TryPublishTravelCompleted(", travelOk);

            Assert.GreaterOrEqual(restore, 0);
            Assert.Greater(travelOk, restore);
            Assert.Greater(publish, travelOk);
            StringAssert.Contains("public static event Action<string> TravelCompleted", source);
            StringAssert.Contains("ZIPTIDE: FIRST_HOUR_TRAVEL dest=", source);
            Assert.AreEqual(1, Count(source, "SceneManager.LoadScene(sceneName);"),
                "the no-coordinator fallback remains the only synchronous load");
            Assert.AreEqual(1, Count(source,
                "SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);"),
                "TravelCoroutine owns exactly one async destination load");
            Assert.AreEqual(1, Count(source, "yield return InventoryState.RestoreAfterTravel(playerRig.transform);"));
            StringAssert.DoesNotContain("TravelCompleted?.Invoke(sceneName)",
                source.Substring(0, restore),
                "travel completion must never emit before inventory restoration");
        }

        [Test]
        public void RuntimeSource_HoldsAsyncActivationUntilReadyAndHasTimeoutEscape()
        {
            string source = ReadTravelSource();
            int leadWait = source.IndexOf("if (lead > 0f) yield return new WaitForSeconds(lead);");
            int asyncLoad = source.IndexOf("SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);");
            int hold = source.IndexOf("loadOperation.allowSceneActivation = false;", asyncLoad);
            int threshold = source.IndexOf("loadOperation.progress < 0.9f", hold);
            int timeout = source.IndexOf("const float loadTimeout = 20f;", hold);
            int timeoutLog = source.IndexOf("ZIPTIDE: TRAVEL_TIMEOUT dest=", hold);
            int activate = source.IndexOf("loadOperation.allowSceneActivation = true;", hold);
            int doneWait = source.IndexOf("while (!loadOperation.isDone) yield return null;", activate);
            int postLoadFrame = source.IndexOf("// 3. Wait one frame for the new scene to initialise.", doneWait);

            Assert.GreaterOrEqual(leadWait, 0);
            Assert.Greater(asyncLoad, leadWait, "async loading starts only after the existing crest-cover wait");
            Assert.Greater(hold, asyncLoad);
            Assert.Greater(threshold, hold);
            Assert.Greater(timeout, hold);
            Assert.Greater(timeoutLog, threshold);
            Assert.Greater(activate, timeoutLog);
            Assert.Greater(doneWait, activate);
            Assert.Greater(postLoadFrame, doneWait);
            StringAssert.Contains("loadElapsed += Time.unscaledDeltaTime;", source);
            StringAssert.Contains("reason=async_load_not_started", source);
        }

        private static string ReadTravelSource()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "World",
                "TravelCoordinator.cs");
            Assert.IsTrue(File.Exists(path), path);
            return File.ReadAllText(path);
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
