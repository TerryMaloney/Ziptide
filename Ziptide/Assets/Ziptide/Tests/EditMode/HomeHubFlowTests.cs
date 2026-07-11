#if UNITY_EDITOR
using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

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
        public void HomeHubRuntime_DelegatesPersistenceAndTravelWithoutOwningEither()
        {
            string source = Read("Gameplay", "Runtime", "Tutorial", "HomeHubRuntime.cs");

            StringAssert.Contains("SaveSystem.Instance.StartNewProfile()", source);
            StringAssert.Contains("SaveSystem.Instance.Load();", source);
            StringAssert.Contains("if (_travel != null) _travel(_targetScene);", source);
            StringAssert.Contains("ZIPTIDE: HOME_HUB_READY continue=", source);
            StringAssert.Contains("ZIPTIDE: HOME_HUB_CHOICE choice=", source);
            StringAssert.DoesNotContain("ProfileSerializer.NewProfile", source);
            StringAssert.DoesNotContain("SaveFileStore", source);
            StringAssert.DoesNotContain("TravelCoordinator", source);
            StringAssert.DoesNotContain("SceneManager", source);
        }

        private static string Read(params string[] parts)
        {
            string path = Application.dataPath;
            path = Path.Combine(path, "Ziptide");
            for (int i = 0; i < parts.Length; i++) path = Path.Combine(path, parts[i]);
            Assert.IsTrue(File.Exists(path), path);
            return File.ReadAllText(path);
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
