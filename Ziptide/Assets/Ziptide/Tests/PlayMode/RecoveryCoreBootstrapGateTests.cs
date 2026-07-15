using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ziptide.Core;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryCoreBootstrapGateTests
    {
        private readonly List<UnityEngine.Object> _created = new List<UnityEngine.Object>();

        private static readonly GateExpectation[] Expectations =
        {
            new GateExpectation("Ziptide/Assets/Ziptide/Core/Runtime/DebugHUD.cs", typeof(DebugHUD), "OnLoad", RecoveryFeatureId.DebugHud),
            new GateExpectation("Ziptide/Assets/Ziptide/Core/Runtime/EnsureXRCameraActive.cs", typeof(EnsureXRCameraActive), "OnLoad", RecoveryFeatureId.XrCameraEnforcer),
            new GateExpectation("Ziptide/Assets/Ziptide/Core/Runtime/RuntimeHealthMonitor.cs", typeof(RuntimeHealthMonitor), "EnsureExists", RecoveryFeatureId.RuntimeHealthMonitor),
            new GateExpectation("Ziptide/Assets/Ziptide/Core/Runtime/RuntimeInputEnabler.cs", typeof(RuntimeInputEnabler), "OnLoad", RecoveryFeatureId.RuntimeInputEnabler),
            new GateExpectation("Ziptide/Assets/Ziptide/Core/Runtime/RuntimeMaterialFixer.cs", typeof(RuntimeMaterialFixer), "OnLoad", RecoveryFeatureId.RuntimeMaterialFixer),
            new GateExpectation("Ziptide/Assets/Ziptide/Core/Runtime/VRBootDiagnostics.cs", typeof(VRBootDiagnostics), "OnLoad", RecoveryFeatureId.VrBootDiagnostics)
        };

        private readonly struct GateExpectation
        {
            public readonly string SourcePath;
            public readonly Type OwnerType;
            public readonly string MethodName;
            public readonly RecoveryFeatureId FeatureId;

            public GateExpectation(string sourcePath, Type ownerType, string methodName, RecoveryFeatureId featureId)
            {
                SourcePath = sourcePath;
                OwnerType = ownerType;
                MethodName = methodName;
                FeatureId = featureId;
            }
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);
            DestroyNamedImmediate("Ziptide_DebugHUD");
            DestroyNamedImmediate("__RuntimeHealth");
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = 0; i < _created.Count; i++)
            {
                if (_created[i] != null) UnityEngine.Object.DestroyImmediate(_created[i]);
            }
            _created.Clear();
            DestroyNamedImmediate("Ziptide_DebugHUD");
            DestroyNamedImmediate("__RuntimeHealth");
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);
            yield return null;
        }

        [Test]
        public void CoreBootstrapSources_UseTheirExactClosedFeatureIds()
        {
            string repositoryRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
            for (int i = 0; i < Expectations.Length; i++)
            {
                GateExpectation expectation = Expectations[i];
                string path = Path.Combine(repositoryRoot, expectation.SourcePath);
                Assert.IsTrue(File.Exists(path), "Missing bootstrap source " + expectation.SourcePath);
                string source = File.ReadAllText(path);
                string token = "RecoveryRuntimeGate.Allows(RecoveryFeatureId." + expectation.FeatureId + ")";
                StringAssert.Contains(token, source, expectation.SourcePath + " is not wired to its registered feature ID.");
            }
        }

        [UnityTest]
        public IEnumerator GoldenSlice_EnforcesCoreBootstrapAllowlist()
        {
            var cameraHost = new GameObject("__RECOVERY_CORE_GATE_CAMERA");
            _created.Add(cameraHost);
            cameraHost.AddComponent<Camera>();

            var rendererHost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rendererHost.name = "__RECOVERY_CORE_GATE_RENDERER";
            _created.Add(rendererHost);
            var renderer = rendererHost.GetComponent<Renderer>();
            renderer.sharedMaterial = null;

            yield return null;

            for (int i = 0; i < Expectations.Length; i++)
                Assert.DoesNotThrow(() => InvokeBootstrap(Expectations[i]));

            yield return null;

            Assert.IsTrue(cameraHost.activeInHierarchy,
                "GoldenSlice allowed the name-based XR camera enforcer to disable an unrelated camera.");
            Assert.IsNull(renderer.sharedMaterial,
                "GoldenSlice allowed RuntimeMaterialFixer to rewrite a renderer material.");
            Assert.IsNull(FindSceneObject("Ziptide_DebugHUD"),
                "GoldenSlice allowed the debug HUD bootstrap to create its overlay.");

            GameObject health = FindSceneObject("__RuntimeHealth");
            Assert.IsNotNull(health,
                "GoldenSlice must preserve RuntimeHealthMonitor as an approved support owner.");
            Assert.AreEqual(1, CountSceneObjects("__RuntimeHealth"),
                "The allowed health bootstrap created duplicate persistent hosts.");
        }

        private static void InvokeBootstrap(GateExpectation expectation)
        {
            MethodInfo method = expectation.OwnerType.GetMethod(
                expectation.MethodName,
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(method, expectation.OwnerType.FullName + "." + expectation.MethodName + " was not found.");
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
