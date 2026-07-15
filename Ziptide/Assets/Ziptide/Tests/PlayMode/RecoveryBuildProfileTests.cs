using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryBuildProfileTests
    {
        [Test]
        public void Resolver_DefaultsExplicitlyToFullDevelopment()
        {
            Assert.AreEqual(
                RecoveryBuildProfileKind.FullDevelopment,
                RecoveryBuildProfile.ResolveKind(false));
            Assert.AreSame(
                RecoveryExposureProfiles.FullDevelopment,
                RecoveryBuildProfile.ResolveProfile(false));
        }

        [Test]
        public void Resolver_SelectsGoldenOnlyWhenDefineIsPresent()
        {
            Assert.AreEqual(
                RecoveryBuildProfileKind.GoldenSlice,
                RecoveryBuildProfile.ResolveKind(true));
            Assert.AreSame(
                RecoveryExposureProfiles.GoldenSlice,
                RecoveryBuildProfile.ResolveProfile(true));
            Assert.IsFalse(
                RecoveryBuildProfile.ResolveProfile(true).Allows(RecoveryFeatureId.RuntimeMaterialFixer));
            Assert.IsFalse(
                RecoveryBuildProfile.ResolveProfile(true).Allows(RecoveryFeatureId.PvpProgression));
            Assert.IsFalse(
                RecoveryBuildProfile.ResolveProfile(true).Allows(RecoveryFeatureId.EcologyInjector),
                "Ecology remains conditional until creature contact/grounding proof is promoted.");
            Assert.IsTrue(
                RecoveryBuildProfile.ResolveProfile(true).Allows(RecoveryFeatureId.TravelCoordinator));
        }

        [Test]
        public void EditorTestPlayer_HasNoImplicitGoldenDefine()
        {
            Assert.AreEqual(
                RecoveryBuildProfileKind.FullDevelopment,
                RecoveryBuildProfile.CompiledKind,
                "The test editor should remain FullDevelopment unless the Golden define is supplied explicitly.");
        }

        [Test]
        public void GoldenBuilder_UsesPerBuildDefineAndOnlyLockedScenes()
        {
            string root = RepositoryRoot();
            string builder = File.ReadAllText(Path.Combine(
                root, "Ziptide", "Assets", "Ziptide", "Editor", "Build", "RecoveryBuildAndroid.cs"));

            StringAssert.Contains(
                "extraScriptingDefines = new[] { RecoveryBuildProfile.GoldenDefine }",
                builder);
            StringAssert.Contains(
                "Ziptide.Build.RecoveryBuildAndroid.PatchScenesThenGoldenAPK",
                builder);
            StringAssert.DoesNotContain("SetScriptingDefineSymbols", builder);
            StringAssert.DoesNotContain("EditorBuildSettingsScene.GetActiveSceneList", builder);
            StringAssert.Contains("ZIPTIDE: BUILD_PROFILE profile=GoldenSlice", builder);
            StringAssert.Contains("recovery_golden_build_profile.json", builder);

            StringAssert.Contains("Assets/Ziptide/Scenes/_Boot.unity", builder);
            StringAssert.Contains("Assets/Ziptide/Scenes/Generated/W000_DriftIn.unity", builder);
            StringAssert.Contains("Assets/Ziptide/Scenes/ToxicCity.unity", builder);

            StringAssert.DoesNotContain("MilestoneA_GrabCube.unity", builder);
            StringAssert.DoesNotContain("PvP_Arena01.unity", builder);
            StringAssert.DoesNotContain("W002_DryCistern.unity", builder);
            StringAssert.DoesNotContain("Arena_Void.unity", builder);
        }

        [Test]
        public void QuestScripts_DefaultToGoldenAndKeepFullDevelopmentExplicit()
        {
            string root = RepositoryRoot();
            string buildScript = File.ReadAllText(Path.Combine(root, "tools", "dev_build_install.ps1"));
            string smokeScript = File.ReadAllText(Path.Combine(root, "tools", "quest_smoke.ps1"));

            StringAssert.Contains("[ValidateSet(\"GoldenSlice\", \"FullDevelopment\")]", buildScript);
            StringAssert.Contains("[string]$BuildProfile = \"GoldenSlice\"", buildScript);
            StringAssert.Contains("Ziptide.Build.RecoveryBuildAndroid.PatchScenesThenGoldenAPK", buildScript);
            StringAssert.Contains("Ziptide.Build.BuildAndroid.PatchScenesThenAPK", buildScript);

            StringAssert.Contains("[string]$BuildProfile = \"GoldenSlice\"", smokeScript);
            StringAssert.Contains("ZIPTIDE: BUILD_PROFILE profile=GoldenSlice", smokeScript);
            StringAssert.Contains("ZIPTIDE: RECOVERY_EXPOSURE buildProfile=GoldenSlice profile=GoldenSlice", smokeScript);
        }

        [Test]
        public void RuntimeGate_InitializesFromCompiledProfileNotRuntimeDebugState()
        {
            string root = RepositoryRoot();
            string gate = File.ReadAllText(Path.Combine(
                root, "Ziptide", "Assets", "Ziptide", "Core", "Runtime", "Recovery", "RecoveryRuntimeGate.cs"));

            StringAssert.Contains("RecoveryBuildProfile.CompiledProfile", gate);
            StringAssert.Contains("buildProfile=", gate);
            StringAssert.DoesNotContain("if (Debug.isDebugBuild", gate);
            StringAssert.DoesNotContain("if (!Debug.isDebugBuild", gate);
        }

        private static string RepositoryRoot() =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
    }
}
