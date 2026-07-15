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
            StringAssert.Contains("RunRequired(\"ScenePatcherCavern.EnsureUndercroftInBuildSettings\"", build,
                "the undercroft generator must remain a required build hook");
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
            StringAssert.Contains("RunRequired(\"CaveSpawnSafety.EnsureUndercroftSpawnFloor\"", build,
                "the spawn-floor repair must remain a required build hook");
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

        [Test]
        public void WorldAudit_SynchronizesPhysicsAfterEverySceneOpen()
        {
            string sync = Read("Editor", "Audit", "AuditPhysicsSync.cs");
            StringAssert.Contains("[InitializeOnLoad]", sync);
            StringAssert.Contains("EditorSceneManager.sceneOpened += OnSceneOpened;", sync);
            StringAssert.Contains("Physics.autoSyncTransforms = true;", sync);
            StringAssert.Contains("Physics.SyncTransforms();", sync);
        }

        [Test]
        public void D2_PreservesAuthoredWorldSpawnInsteadOfMovingItToOrigin()
        {
            string d2 = Read("Editor", "Patching", "ScenePatcherD2.cs");
            StringAssert.Contains("GameObject go = GameObject.Find(SpawnMarkerName);", d2);
            StringAssert.Contains("if (go == null)", d2);
            StringAssert.DoesNotContain("var go = PatcherUtil.EnsureRootObject(SpawnMarkerName, spawnPos);", d2);
        }

        [Test]
        public void CavernFloorPad_ColliderIsThin_NeverAPhantomDome()
        {
            Ziptide.Editor.Art.CavernKitLibrary.EnsureRegistered();
            Assert.IsTrue(Ziptide.Editor.Art.ArtModuleRegistry.TryBuild("cavernModule:rock/FloorPad", out var pad),
                "FloorPad module must build from the registry");
            try
            {
                pad.transform.position = new Vector3(500f, -400f, 500f);
                pad.transform.localScale = new Vector3(6f, 1f, 6f);
                Physics.SyncTransforms();

                foreach (var col in pad.GetComponentsInChildren<Collider>(true))
                {
                    Assert.IsFalse(col is CapsuleCollider,
                        col.name + ": a squashed capsule degenerates to a chamber-radius sphere");
                    Assert.LessOrEqual(col.bounds.max.y, pad.transform.position.y + 0.3f,
                        col.name + " bulges above the walk surface — the phantom dome is back");
                }

                Vector3 spawn = pad.transform.position + Vector3.up * 0.25f;
                foreach (var hit in Physics.OverlapSphere(spawn + Vector3.up * 0.9f, 0.3f))
                    Assert.Fail("'" + hit.name + "' overlaps the spawn torso — SPAWN_OVERLAP_SOLID");
                Assert.IsTrue(Physics.Raycast(spawn + Vector3.up * 0.2f, Vector3.down, out _, 6f),
                    "no floor under the cave spawn — SPAWN_NO_FLOOR");
            }
            finally
            {
                Object.DestroyImmediate(pad);
            }
        }

        [Test]
        public void CavernPatcher_HealsPreFixFloorPadCapsules()
        {
            string cavern = Read("Editor", "Patching", "ScenePatcherCavern.cs");
            StringAssert.Contains("HealFloorPadColliders();", cavern);
            StringAssert.Contains("parent.name.StartsWith(\"CavePad_\")", cavern);
            string kit = Read("Editor", "Art", "CavernKitLibrary.cs");
            StringAssert.Contains("disc.AddComponent<BoxCollider>();", kit);
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
