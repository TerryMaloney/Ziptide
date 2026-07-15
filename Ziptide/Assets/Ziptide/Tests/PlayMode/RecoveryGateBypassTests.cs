using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ziptide.Core;
using Ziptide.Gameplay;
using Ziptide.Gameplay.DevTools;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryGateBypassTests
    {
        private readonly List<GameObject> _created = new List<GameObject>();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);
            DestroyNamedImmediate("DevWarpBoard");
            DestroyNamedImmediate("DevMenuCanvas");
            DestroyNamedImmediate("__DevWarpRunner");
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = 0; i < _created.Count; i++)
                if (_created[i] != null) UnityEngine.Object.DestroyImmediate(_created[i]);
            _created.Clear();
            DestroyNamedImmediate("DevWarpBoard");
            DestroyNamedImmediate("DevMenuCanvas");
            DestroyNamedImmediate("__DevWarpRunner");
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);
            yield return null;
        }

        [Test]
        public void RuntimeBootstrapDiscovery_CoversActualSourceAttributes()
        {
            string repositoryRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
            string[] scanRoots =
            {
                "Ziptide/Assets/Ziptide/Core/Runtime",
                "Ziptide/Assets/Ziptide/Gameplay/Runtime",
                "Ziptide/Assets/ZiptideNet"
            };
            const string gateInfrastructure =
                "Ziptide/Assets/Ziptide/Core/Runtime/Recovery/RecoveryRuntimeGate.cs";

            var catalogPaths = new HashSet<string>(StringComparer.Ordinal);
            foreach (var registration in RecoveryAutomaticOwnerCatalog.All)
                catalogPaths.Add(registration.SourceRelativePath.Replace('\\', '/'));

            var discovered = new List<string>();
            var marker = new Regex(@"(?m)^\s*\[RuntimeInitializeOnLoadMethod(?:\s*\(|\s*\])");
            for (int r = 0; r < scanRoots.Length; r++)
            {
                string absoluteRoot = Path.Combine(repositoryRoot, scanRoots[r]);
                Assert.IsTrue(Directory.Exists(absoluteRoot), "Missing runtime scan root " + scanRoots[r]);
                foreach (string file in Directory.GetFiles(absoluteRoot, "*.cs", SearchOption.AllDirectories))
                {
                    string source = File.ReadAllText(file);
                    if (!marker.IsMatch(source)) continue;

                    string relative = file.Substring(repositoryRoot.Length)
                        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        .Replace('\\', '/');
                    if (relative == gateInfrastructure) continue;

                    discovered.Add(relative);
                    Assert.IsTrue(catalogPaths.Contains(relative),
                        "Real runtime bootstrap is absent from the closed exposure catalog: " + relative);
                }
            }

            Assert.Contains("Ziptide/Assets/ZiptideNet/NetBootstrap.cs", discovered,
                "The source-driven scan did not inspect the conditional Photon bootstrap.");
        }

        [UnityTest]
        public IEnumerator GoldenSlice_BlocksSceneAuthoredAndPublicBypassPaths()
        {
            var board = AddBlocked<DevWarpBoard>("Bypass_DevWarpBoard");
            var legacyMenu = AddBlocked<DevMenu>("Bypass_DevMenu");
            var conquest = AddBlocked<ConquestMissionRuntime>("Bypass_Conquest");
            var ecology = AddBlocked<EcologyDirector>("Bypass_Ecology");
            var pvp = AddBlocked<PvpProgressionRuntime>("Bypass_Pvp");
            var quarters = AddBlocked<QuartersCameraFeature>("Bypass_Quarters");
            yield return null;

            Assert.IsFalse(board.enabled, "Scene-authored DevWarpBoard remained active in GoldenSlice.");
            Assert.IsFalse(legacyMenu.enabled, "Scene-authored legacy DevMenu remained active in GoldenSlice.");
            Assert.IsFalse(conquest.enabled, "Scene-authored ConquestMissionRuntime remained active in GoldenSlice.");
            Assert.IsFalse(ecology.enabled, "Scene-authored EcologyDirector remained active in GoldenSlice.");
            Assert.IsFalse(pvp.enabled, "Scene-authored PvpProgressionRuntime remained active in GoldenSlice.");
            Assert.IsFalse(quarters.enabled, "Scene-authored QuartersCameraFeature remained active in GoldenSlice.");

            board.Show();
            board.Toggle();
            legacyMenu.Show();
            legacyMenu.Toggle();
            quarters.BuildFeature();
            DevWarp.WarpToMarker("Spawn_Player");
            yield return null;

            Assert.IsNull(FindSceneObject("DevWarpBoard"), "Blocked board public API created a menu.");
            Assert.IsNull(FindSceneObject("DevMenuCanvas"), "Blocked legacy menu public API created a canvas.");
            Assert.IsNull(quarters.transform.Find(QuartersCameraFeature.FeatureRootName),
                "Blocked Quarters public builder created its feature root.");
            Assert.IsNull(FindSceneObject("__DevWarpRunner"),
                "Blocked developer warp API created a persistent runner.");
            Assert.IsFalse(DevWarp.Enabled, "Developer warp API reports enabled in GoldenSlice.");
        }

        private T AddBlocked<T>(string name) where T : MonoBehaviour
        {
            var go = new GameObject(name);
            _created.Add(go);
            return go.AddComponent<T>();
        }

        private static void DestroyNamedImmediate(string objectName)
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (go != null && go.scene.IsValid() && go.name == objectName)
                    UnityEngine.Object.DestroyImmediate(go);
            }
        }

        private static GameObject FindSceneObject(string objectName)
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (go != null && go.scene.IsValid() && go.name == objectName)
                    return go;
            }
            return null;
        }
    }
}
