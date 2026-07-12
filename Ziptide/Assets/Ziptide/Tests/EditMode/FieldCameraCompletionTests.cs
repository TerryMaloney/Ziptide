#if UNITY_EDITOR
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class FieldCameraCompletionTests
    {
        [Test]
        public void HorizonDetection_RespectsVerticalFov()
        {
            Assert.IsTrue(PhotoSubjectDetector.HorizonInFrame(Vector3.forward, 60f));
            Assert.IsTrue(PhotoSubjectDetector.HorizonInFrame(
                Quaternion.Euler(25f, 0f, 0f) * Vector3.forward,
                60f));
            Assert.IsFalse(PhotoSubjectDetector.HorizonInFrame(
                Quaternion.Euler(40f, 0f, 0f) * Vector3.forward,
                60f));
            Assert.IsFalse(PhotoSubjectDetector.HorizonInFrame(Vector3.zero, 60f));
        }

        [Test]
        public void Centering_IsOneAtCenterAndZeroAtEdgesOrBehind()
        {
            Assert.AreEqual(1f, PhotoSubjectDetector.Centering01(new Vector3(0.5f, 0.5f, 1f)), 0.0001f);
            Assert.AreEqual(0.5f, PhotoSubjectDetector.Centering01(new Vector3(0.75f, 0.5f, 1f)), 0.0001f);
            Assert.AreEqual(0f, PhotoSubjectDetector.Centering01(new Vector3(1f, 0.5f, 1f)), 0.0001f);
            Assert.AreEqual(0f, PhotoSubjectDetector.Centering01(new Vector3(0.5f, 0.5f, -1f)), 0.0001f);
        }

        [Test]
        public void AuthoredNamingVocabulary_ClassifiesSubjects()
        {
            Assert.IsTrue(PhotoSubjectDetector.IsCelestialKey("SkyBody_GasGiant"));
            Assert.IsTrue(PhotoSubjectDetector.IsCelestialKey("Moon_02"));
            Assert.IsTrue(PhotoSubjectDetector.IsLandmarkKey("Hero_TideTower"));
            Assert.IsTrue(PhotoSubjectDetector.IsLandmarkKey("poi_story_anchor"));
            Assert.IsTrue(PhotoSubjectDetector.IsCreatureKey("ForgeVisual_witness_mite"));
            Assert.IsTrue(PhotoSubjectDetector.IsCreatureKey("Warden_Body"));
            Assert.IsFalse(PhotoSubjectDetector.IsCreatureKey("StreetLamp"));
        }

        [Test]
        public void CaptureFileNames_AreStableSafePngBasenames()
        {
            Assert.AreEqual("toxic_city", PhotoCaptureCamera.SanitizeFilePart("Toxic City"));
            Assert.AreEqual("world", PhotoCaptureCamera.SanitizeFilePart("   "));
            string file = PhotoCaptureCamera.BuildFileName("W005 / Cavern", 1234, 7);
            Assert.AreEqual(Path.GetFileName(file), file);
            StringAssert.StartsWith("photo_w005_", file);
            StringAssert.EndsWith("_1234_007.png", file);
            StringAssert.DoesNotContain("/", file);
            StringAssert.DoesNotContain("\\", file);
        }

        [Test]
        public void Delete_IsBasenameOnlyAndRemovesExistingPhoto()
        {
            string root = Path.Combine(Application.temporaryCachePath, "field_camera_test_" + System.Guid.NewGuid());
            string folder = PhotoCaptureCamera.PhotoFolderPath(root);
            Directory.CreateDirectory(folder);
            string file = "photo_test_1_000.png";
            File.WriteAllBytes(Path.Combine(folder, file), new byte[] { 1, 2, 3 });

            Assert.IsFalse(PhotoCaptureCamera.SafeDeletePhoto(root, "../escape.png"));
            Assert.IsTrue(PhotoCaptureCamera.SafeDeletePhoto(root, file));
            Assert.IsFalse(File.Exists(Path.Combine(folder, file)));
            Directory.Delete(root, true);
        }

        [Test]
        public void PhotoWallLayout_IsSixUniqueNewestFirstSlots()
        {
            Assert.AreEqual(6, QuartersPhotoWall.MaxDisplayedPhotos);
            Vector3[] positions = Enumerable.Range(0, 6)
                .Select(QuartersPhotoWall.SlotLocalPosition)
                .ToArray();
            Assert.AreEqual(6, positions.Distinct().Count());
            Assert.AreEqual(positions[0].y, positions[2].y, 0.0001f);
            Assert.Less(positions[3].y, positions[0].y);
            Assert.AreNotEqual(
                QuartersPhotoWall.FrameColorForRating(0),
                QuartersPhotoWall.FrameColorForRating(1));
            Assert.AreNotEqual(
                QuartersPhotoWall.FrameColorForRating(1),
                QuartersPhotoWall.FrameColorForRating(2));
        }

        [Test]
        public void CaptureOwner_PinsBudgetStorageScoringAndCleanup()
        {
            string source = Read("Gameplay", "Runtime", "Photo", "PhotoCaptureCamera.cs");
            StringAssert.Contains("CaptureWidth = 512", source);
            StringAssert.Contains("CaptureHeight = 384", source);
            StringAssert.Contains("MaxViewfinderHz = 10f", source);
            StringAssert.Contains("PhotoSubjectDetector.Detect(_camera)", source);
            StringAssert.Contains("PhotoComposition.Evaluate", source);
            StringAssert.Contains("PhotoAlbum.Add", source);
            StringAssert.Contains("SafeDeletePhoto", source);
            StringAssert.Contains("save.Save();", source);
            StringAssert.Contains("_target.Release();", source);
            StringAssert.Contains("Destroy(_target);", source);
            StringAssert.Contains("Destroy(_readback);", source);
            StringAssert.Contains("Destroy(_viewfinderMaterial);", source);
            StringAssert.DoesNotContain("DontDestroyOnLoad", source);
            StringAssert.DoesNotContain("Camera.main", source);
        }

        [Test]
        public void CameraRuntime_DelegatesExactlyOneRealCaptureOwner()
        {
            string source = Read("Gameplay", "Runtime", "Photo", "CameraRuntime.cs");
            Assert.AreEqual(1, Count(source, "AddComponent<PhotoCaptureCamera>()"));
            Assert.AreEqual(1, Count(source, "_captureCamera.TryCapture"));
            StringAssert.Contains("hand.SendHapticImpulse", source);
            StringAssert.DoesNotContain("File.WriteAllBytes", source,
                "CameraRuntime owns feel; PhotoCaptureCamera owns storage/rendering");
        }

        [Test]
        public void QuartersFeature_WiresOneCameraAndOneBoundedWall()
        {
            string feature = Read("Gameplay", "Runtime", "Photo", "QuartersCameraFeature.cs");
            StringAssert.Contains("ItemFactory.Create(\n                \"handheld_camera\"", feature);
            Assert.AreEqual(1, Count(feature, "AddComponent<QuartersPhotoWall>()"));
            StringAssert.Contains("SceneManager.sceneLoaded += OnSceneLoaded", feature);
            StringAssert.DoesNotContain("DontDestroyOnLoad", feature);

            string wall = Read("Gameplay", "Runtime", "World", "QuartersPhotoWall.cs");
            StringAssert.Contains("photos.Count - 1 - slot", wall);
            StringAssert.Contains("MaxDisplayedPhotos = 6", wall);
            StringAssert.Contains("Destroy(_materials[i])", wall);
            StringAssert.Contains("Destroy(_textures[i])", wall);
            StringAssert.DoesNotContain("Resources.LoadAll", wall);
        }

        [Test]
        public void Camera_RemainsInExistingItemAndHolsterPipelines()
        {
            string itemFactory = Read("Gameplay", "Runtime", "Items", "ItemFactory.cs");
            StringAssert.Contains("def is CameraDefinition", itemFactory);
            StringAssert.Contains("CreateCamera(camDef, position)", itemFactory);
            StringAssert.Contains("go.AddComponent<CameraRuntime>()", itemFactory);

            string holster = Read("Gameplay", "Runtime", "Inventory", "HolsterSocketInteractor.cs");
            StringAssert.Contains("\"handheld_camera\"", holster);

            string build = Read("Editor", "Build", "BuildAndroid.cs");
            Assert.AreEqual(1, Count(build, "CameraAuthor.EnsureAuthored();"));
        }

        private static string Read(params string[] parts)
        {
            string path = Application.dataPath;
            path = Path.Combine(path, "Ziptide");
            foreach (string part in parts) path = Path.Combine(path, part);
            Assert.IsTrue(File.Exists(path), path);
            return File.ReadAllText(path);
        }

        private static int Count(string source, string token)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(token, index, System.StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }
    }
}
#endif
