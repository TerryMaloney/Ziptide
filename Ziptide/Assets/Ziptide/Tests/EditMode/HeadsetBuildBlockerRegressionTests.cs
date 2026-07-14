#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Ziptide.Tests.EditMode
{
    /// <summary>Source-contract regressions for the failures found by Terry's first local Quest build.</summary>
    public sealed class HeadsetBuildBlockerRegressionTests
    {
        [Test]
        public void CameraRuntime_DoesNotAllocateUnityObjectsInFieldInitializers()
        {
            string source = Read("Gameplay", "Runtime", "Photo", "CameraRuntime.cs");
            StringAssert.DoesNotContain("readonly MaterialPropertyBlock _screenBlock = new", source);
            StringAssert.Contains("_screenBlock = new MaterialPropertyBlock();", source);
            StringAssert.Contains("private void Awake()", source);
        }

        [Test]
        public void StyledDistricts_RemoveLegacyFacadeColliders()
        {
            string source = Read("Editor", "Patching", "BuildingBuilder.cs");
            StringAssert.Contains("RemoveLegacyFacadeRing(cityRoot, district.id);", source);
            StringAssert.Contains("child.name.StartsWith(\"Facade_\")", source);
            StringAssert.Contains("Object.DestroyImmediate(child.gameObject);", source);
        }

        [Test]
        public void W011Undercroft_IsGeneratedBeforeBuildSceneAudit()
        {
            string build = Read("Editor", "Build", "BuildAndroid.cs");
            string cavern = Read("Editor", "Patching", "ScenePatcherCavern.cs");
            StringAssert.Contains("ScenePatcherCavern.EnsureUndercroftInBuildSettings();", build);
            StringAssert.Contains("public static void EnsureUndercroftInBuildSettings()", cavern);
            StringAssert.Contains("sceneName = \"W011_Undercroft\"", cavern);
            StringAssert.Contains("EnsureInBuildSettings(scenePath);", cavern);
        }

        [Test]
        public void W011Undercroft_SpawnHasSynchronizedNonTriggerLandingColliderBeforeAudit()
        {
            string build = Read("Editor", "Build", "BuildAndroid.cs");
            string safety = Read("Editor", "Patching", "CaveSpawnSafety.cs");
            StringAssert.Contains("CaveSpawnSafety.EnsureUndercroftSpawnFloor();", build);
            StringAssert.Contains("FloorName = \"__SPAWN_FLOOR\"", safety);
            StringAssert.Contains("GameObject.CreatePrimitive(PrimitiveType.Cube)", safety);
            StringAssert.Contains("floor.layer = 0;", safety);
            StringAssert.Contains("BoxCollider collider", safety);
            StringAssert.Contains("collider.isTrigger = false;", safety);
            StringAssert.Contains("Physics.autoSyncTransforms = true;", safety);
            StringAssert.Contains("Physics.SyncTransforms();", safety);
            StringAssert.Contains("Physics.DefaultRaycastLayers", safety);
            StringAssert.Contains("QueryTriggerInteraction.Ignore", safety);
            StringAssert.Contains("EditorSceneManager.SaveScene(scene, UndercroftScenePath);", safety);
        }

        private static string Read(params string[] parts)
        {
            string path = Path.Combine(Application.dataPath, "Ziptide");
            foreach (string part in parts) path = Path.Combine(path, part);
            Assert.IsTrue(File.Exists(path), path);
            return File.ReadAllText(path);
        }
    }
}
#endif