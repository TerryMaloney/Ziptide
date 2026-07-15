using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ziptide.Core;
using Ziptide.Gameplay;
using Ziptide.Gameplay.DevTools;
using Ziptide.Gameplay.Tutorial;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryGameplayBootstrapGateTests
    {
        private static readonly SourceExpectation[] Expectations =
        {
            new SourceExpectation("Ziptide/Assets/Ziptide/Gameplay/Runtime/Audio/AmbienceDirector.cs", RecoveryFeatureId.AmbienceDirector),
            new SourceExpectation("Ziptide/Assets/Ziptide/Gameplay/Runtime/Player/ComfortVignette.cs", RecoveryFeatureId.ComfortVignette),
            new SourceExpectation("Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ConquestMissionRuntime.cs", RecoveryFeatureId.ConquestMissionInjector),
            new SourceExpectation("Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevWarpBoard.cs", RecoveryFeatureId.DevWarpBoard),
            new SourceExpectation("Ziptide/Assets/Ziptide/Gameplay/Runtime/Enemies/EcologyDirector.cs", RecoveryFeatureId.EcologyInjector),
            new SourceExpectation("Ziptide/Assets/Ziptide/Gameplay/Runtime/Pvp/PvpProgressionRuntime.cs", RecoveryFeatureId.PvpProgression),
            new SourceExpectation("Ziptide/Assets/Ziptide/Gameplay/Runtime/Photo/QuartersCameraFeature.cs", RecoveryFeatureId.QuartersCameraInjector),
            new SourceExpectation("Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs", RecoveryFeatureId.FirstHourObservation),
            new SourceExpectation("Ziptide/Assets/Ziptide/Gameplay/Runtime/Diagnostics/SingletonValidator.cs", RecoveryFeatureId.SingletonValidator),
            new SourceExpectation("Ziptide/Assets/ZiptideNet/NetBootstrap.cs", RecoveryFeatureId.NetBootstrap)
        };

        private readonly struct SourceExpectation
        {
            public readonly string SourcePath;
            public readonly RecoveryFeatureId FeatureId;

            public SourceExpectation(string sourcePath, RecoveryFeatureId featureId)
            {
                SourcePath = sourcePath;
                FeatureId = featureId;
            }
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);
            DestroyNamedImmediate("__DevWarpBoard");
            DestroyNamedImmediate("PvpProgression");
            DestroyNamedImmediate("__FirstHourObservationAdapter");
            DestroyNamedImmediate("__AmbienceDirector");
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            DestroyNamedImmediate("__DevWarpBoard");
            DestroyNamedImmediate("PvpProgression");
            DestroyNamedImmediate("__FirstHourObservationAdapter");
            DestroyNamedImmediate("__AmbienceDirector");
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);
            yield return null;
        }

        [Test]
        public void GameplayBootstrapSources_UseTheirExactClosedFeatureIds()
        {
            string repositoryRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
            for (int i = 0; i < Expectations.Length; i++)
            {
                SourceExpectation expectation = Expectations[i];
                string path = Path.Combine(repositoryRoot, expectation.SourcePath);
                Assert.IsTrue(File.Exists(path), "Missing bootstrap source " + expectation.SourcePath);
                string source = File.ReadAllText(path);
                string token = "RecoveryRuntimeGate.Allows(RecoveryFeatureId." + expectation.FeatureId + ")";
                StringAssert.Contains(token, source, expectation.SourcePath + " is not wired to its registered feature ID.");
            }
        }

        [UnityTest]
        public IEnumerator GoldenSlice_EnforcesDirectGameplayBootstrapPolicy()
        {
            Assert.DoesNotThrow(() => InvokePrivateStatic(typeof(DevWarpBoard), "Bootstrap"));
            Assert.DoesNotThrow(() => InvokePrivateStatic(typeof(PvpProgressionRuntime), "EnsureExists"));
            Assert.DoesNotThrow(() => InvokePrivateStatic(typeof(FirstHourObservationAdapter), "EnsureExists"));
            Assert.DoesNotThrow(() => InvokePrivateStatic(typeof(AmbienceDirector), "EnsureExists"));
            yield return null;

            Assert.IsNull(FindSceneObject("__DevWarpBoard"),
                "GoldenSlice allowed the developer warp board bootstrap.");
            Assert.IsNull(FindSceneObject("PvpProgression"),
                "GoldenSlice allowed the PvP progression bootstrap.");

            Assert.IsNotNull(FindSceneObject("__FirstHourObservationAdapter"),
                "GoldenSlice must retain the first-hour observation support owner.");
            Assert.AreEqual(1, CountSceneObjects("__FirstHourObservationAdapter"),
                "First-hour observation created duplicate persistent hosts.");

            Assert.IsNotNull(FindSceneObject("__AmbienceDirector"),
                "GoldenSlice must retain the ambience support owner.");
            Assert.AreEqual(1, CountSceneObjects("__AmbienceDirector"),
                "AmbienceDirector created duplicate persistent hosts.");
        }

        private static void InvokePrivateStatic(Type type, string methodName)
        {
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(method, type.FullName + "." + methodName + " was not found.");
            try
            {
                method.Invoke(null, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        private static void DestroyNamedImmediate(string objectName)
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                var go = all[i];
                if (go != null && go.scene.IsValid() && go.name == objectName)
                    UnityEngine.Object.DestroyImmediate(go);
            }
        }

        private static int CountSceneObjects(string objectName)
        {
            int count = 0;
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                var go = all[i];
                if (go != null && go.scene.IsValid() && go.name == objectName) count++;
            }
            return count;
        }

        private static GameObject FindSceneObject(string objectName)
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                var go = all[i];
                if (go != null && go.scene.IsValid() && go.name == objectName)
                    return go;
            }
            return null;
        }
    }
}
