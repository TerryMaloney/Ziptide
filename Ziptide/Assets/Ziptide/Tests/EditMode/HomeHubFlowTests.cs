#if UNITY_EDITOR
using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Ziptide.Core;
using Ziptide.Gameplay;
using Ziptide.Editor;

namespace Ziptide.Tests.EditMode
{
    public class HomeHubFlowTests
    {
        [Test]
        public void NewGame_CommitsTravelExactlyOnce()
        {
            var flow = new HomeHubFlowState(false);

            Assert.IsTrue(flow.TryChoose(HomeHubChoice.NewGame, out bool firstTravel));
            Assert.IsTrue(firstTravel);
            Assert.IsTrue(flow.TravelCommitted);

            Assert.IsFalse(flow.TryChoose(HomeHubChoice.NewGame, out bool secondTravel));
            Assert.IsFalse(secondTravel);
            Assert.IsFalse(flow.TryChoose(HomeHubChoice.Settings, out _));
        }

        [Test]
        public void Continue_IsUnavailableWithoutValidSave()
        {
            var noSave = new HomeHubFlowState(false);
            Assert.IsFalse(noSave.CanContinue);
            Assert.IsFalse(noSave.TryChoose(HomeHubChoice.Continue, out bool travel));
            Assert.IsFalse(travel);
            Assert.IsFalse(noSave.TravelCommitted);

            var withSave = new HomeHubFlowState(true);
            Assert.IsTrue(withSave.CanContinue);
            Assert.IsTrue(withSave.TryChoose(HomeHubChoice.Continue, out travel));
            Assert.IsTrue(travel);
        }

        [Test]
        public void Settings_DoesNotTravelOrConsumeTheChoiceGate()
        {
            var flow = new HomeHubFlowState(false);

            Assert.IsTrue(flow.TryChoose(HomeHubChoice.Settings, out bool travel));
            Assert.IsFalse(travel);
            Assert.IsFalse(flow.TravelCommitted);

            Assert.IsTrue(flow.TryChoose(HomeHubChoice.Settings, out travel));
            Assert.IsFalse(travel);
            Assert.IsTrue(flow.TryChoose(HomeHubChoice.NewGame, out travel));
            Assert.IsTrue(travel);
        }

        [Test]
        public void BootLoader_UsesHomeHubAndRetainsSingleTravelCoordinatorPath()
        {
            string source = Read("Gameplay", "Runtime", "World", "BootLoader.cs");

            StringAssert.Contains("new GameObject(\"__HOME_HUB_RUNTIME\").AddComponent<HomeHubRuntime>()", source);
            StringAssert.Contains("home.Configure(target, destination =>", source);
            StringAssert.Contains("TravelCoordinator.TravelTo(destination, skipGate: true);", source);
            Assert.AreEqual(1, Count(source, "TravelCoordinator.TravelTo("));
            StringAssert.DoesNotContain("SceneManager", source);
            StringAssert.DoesNotContain("LoadScene", source);
        }

        [Test]
        public void SaveSystem_ExposesExplicitValidContinueAndNewProfileCommands()
        {
            string source = Read("Gameplay", "Runtime", "Persistence", "SaveSystem.cs");

            StringAssert.Contains("public static bool HasExistingProfile", source);
            StringAssert.Contains("ProfileSerializer.TryDeserialize(text, out _)", source);
            StringAssert.Contains("public PlayerProfile StartNewProfile()", source);
            StringAssert.Contains("Profile = ProfileSerializer.NewProfile();", source);
            StringAssert.Contains("Save();", source);
            StringAssert.DoesNotContain("PlayerPrefs.Delete", source);
            StringAssert.DoesNotContain("File.Delete", source);
            Assert.AreEqual(1, Count(source, "SaveFileStore.WriteAtomic("));
        }

        [Test]
        public void HomeHubRuntime_DelegatesPersistenceTravelAndBootSettings()
        {
            string source = Read("Gameplay", "Runtime", "Tutorial", "HomeHubRuntime.cs");

            StringAssert.Contains("SaveSystem.Instance.StartNewProfile()", source);
            StringAssert.Contains("SaveSystem.Instance.Load();", source);
            StringAssert.Contains("if (_travel != null) _travel(_targetScene);", source);
            StringAssert.Contains("new GameObject(\"__HOME_HUB_COMFORT_SETTINGS\")", source);
            StringAssert.Contains("_settingsConsole.Configure(false)", source);
            StringAssert.Contains("ZIPTIDE: HOME_HUB_READY continue=", source);
            StringAssert.Contains("ZIPTIDE: HOME_HUB_CHOICE choice=", source);
            StringAssert.DoesNotContain("ProfileSerializer.NewProfile", source);
            StringAssert.DoesNotContain("SaveFileStore", source);
            StringAssert.DoesNotContain("TravelCoordinator", source);
            StringAssert.DoesNotContain("SceneManager", source);
        }

        [Test]
        public void ShipCastOff_SelectionIsAdditiveAndAcceptsOnlyW001()
        {
            var go = new GameObject("castoff-test");
            try
            {
                var castOff = go.AddComponent<ShipCastOffRuntime>();
                string seen = null;
                castOff.DestinationSelected += destination => seen = destination;

                LogAssert.Expect(
                    LogType.Warning,
                    new Regex("ZIPTIDE: FLIGHT_DESTINATION_REJECTED target=PvP_Arena01"));
                Assert.IsFalse(castOff.SelectFirstDestination("PvP_Arena01"));
                Assert.IsNull(seen);
                Assert.IsTrue(castOff.SelectFirstDestination(ZiptideConstants.SceneToxicCity));
                Assert.AreEqual(ZiptideConstants.SceneToxicCity, castOff.SelectedDestination);
                Assert.AreEqual(ZiptideConstants.SceneToxicCity, seen);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void BunkObserver_PublishesNamedObjectOnce()
        {
            var go = new GameObject("bunk-test");
            string seen = null;
            int calls = 0;
            Action<string> handler = id => { seen = id; calls++; };
            FirstHourBunkObjectRuntime.NamedBunkObjectGrabbed += handler;
            try
            {
                var observer = go.AddComponent<FirstHourBunkObjectRuntime>();
                observer.NotifyGrabbed();
                observer.NotifyGrabbed();

                Assert.AreEqual(1, calls);
                Assert.AreEqual(FirstHourBunkObjectRuntime.ObjectId, seen);
            }
            finally
            {
                FirstHourBunkObjectRuntime.NamedBunkObjectGrabbed -= handler;
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void SurfaceAuthor_IsIdempotentAndUsesStableMarkers()
        {
            // Unity's EditMode runner owns an unsaved scene; creating a second untitled scene additively
            // is illegal. Author into the active test scene, then remove only the three stable markers.
            Scene scene = SceneManager.GetActiveScene();
            Assert.IsTrue(scene.IsValid() && scene.isLoaded);
            Assert.IsNull(FindNamed(scene, FirstHourSurfaceAuthor.ComfortMarker));
            Assert.IsNull(FindNamed(scene, FirstHourSurfaceAuthor.BunkMarker));
            Assert.IsNull(FindNamed(scene, FirstHourSurfaceAuthor.HelmMarker));

            try
            {
                FirstHourSurfaceAuthor.Author(scene);
                FirstHourSurfaceAuthor.Author(scene);

                Assert.AreEqual(1, CountNamed(scene, FirstHourSurfaceAuthor.ComfortMarker));
                Assert.AreEqual(1, CountNamed(scene, FirstHourSurfaceAuthor.BunkMarker));
                Assert.AreEqual(1, CountNamed(scene, FirstHourSurfaceAuthor.HelmMarker));

                Assert.IsNotNull(FindNamed(scene, FirstHourSurfaceAuthor.ComfortMarker)
                    .GetComponent<ComfortConsoleRuntime>());
                Assert.IsNotNull(FindNamed(scene, FirstHourSurfaceAuthor.BunkMarker)
                    .GetComponent<FirstHourBunkObjectRuntime>());
                Assert.IsNotNull(FindNamed(scene, FirstHourSurfaceAuthor.HelmMarker)
                    .GetComponent<FirstDestinationHelmRuntime>());
            }
            finally
            {
                DestroyNamed(scene, FirstHourSurfaceAuthor.ComfortMarker);
                DestroyNamed(scene, FirstHourSurfaceAuthor.BunkMarker);
                DestroyNamed(scene, FirstHourSurfaceAuthor.HelmMarker);
            }
        }

        [Test]
        public void HelmAndAuthorSourcesDoNotCreateASecondTravelOrLaunchPath()
        {
            string helm = Read("Gameplay", "Runtime", "Tutorial", "FirstDestinationHelmRuntime.cs");
            string castOff = Read("Gameplay", "Runtime", "Story", "ShipCastOffRuntime.cs");
            string author = Read("Editor", "Patching", "FirstHourSurfaceAuthor.cs");

            StringAssert.Contains("_castOff.SelectFirstDestination(ZiptideConstants.SceneToxicCity)", helm);
            StringAssert.DoesNotContain("TravelCoordinator.", helm);
            StringAssert.DoesNotContain("SceneManager.", helm);

            StringAssert.Contains("public bool SelectFirstDestination(string destinationScene)", castOff);
            Assert.AreEqual(1, Count(castOff, "TravelCoordinator.TravelTo("));
            Assert.AreEqual(1, Count(castOff, "private void TryLaunch()"));
            Assert.AreEqual(1, Count(castOff, "private IEnumerator LaunchSequence()"));

            StringAssert.Contains("__FIRST_HOUR_COMFORT_CONSOLE", author);
            StringAssert.Contains("__FIRST_HOUR_BUNK_OBJECT", author);
            StringAssert.Contains("__FIRST_HOUR_FIRST_HELM", author);
            StringAssert.Contains("Assets/Ziptide/Scenes/Generated/W000_DriftIn.unity", author);
            StringAssert.Contains("public static GameObject EnsureMarker(", author);
        }

        private static string Read(params string[] parts)
        {
            string path = Application.dataPath;
            path = Path.Combine(path, "Ziptide");
            for (int i = 0; i < parts.Length; i++) path = Path.Combine(path, parts[i]);
            Assert.IsTrue(File.Exists(path), path);
            return File.ReadAllText(path);
        }

        private static GameObject FindNamed(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
                foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                    if (t.name == name) return t.gameObject;
            return null;
        }

        private static void DestroyNamed(Scene scene, string name)
        {
            GameObject go = FindNamed(scene, name);
            if (go != null) UnityEngine.Object.DestroyImmediate(go);
        }

        private static int CountNamed(Scene scene, string name)
        {
            int count = 0;
            foreach (GameObject root in scene.GetRootGameObjects())
                foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                    if (t.name == name) count++;
            return count;
        }

        private static int Count(string source, string token)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }
    }
}
#endif
