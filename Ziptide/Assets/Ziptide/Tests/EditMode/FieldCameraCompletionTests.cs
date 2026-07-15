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
        public void CompositionHelpers_AreBoundedAndDeterministic()
        {
            Assert.IsTrue(PhotoSubjectDetector.HorizonInFrame(Vector3.forward, 60f));
            Assert.IsFalse(PhotoSubjectDetector.HorizonInFrame(
                Quaternion.Euler(40f, 0f, 0f) * Vector3.forward, 60f));
            Assert.AreEqual(1f, PhotoSubjectDetector.Centering01(new Vector3(0.5f, 0.5f, 1f)), 0.001f);
            Assert.AreEqual(0f, PhotoSubjectDetector.Centering01(new Vector3(1f, 0.5f, 1f)), 0.001f);
            Assert.IsTrue(PhotoSubjectDetector.IsCelestialKey("SkyBody_GasGiant"));
            Assert.IsTrue(PhotoSubjectDetector.IsLandmarkKey("Hero_TideTower"));
            Assert.IsTrue(PhotoSubjectDetector.IsCreatureKey("ForgeVisual_witness_mite"));
            Assert.IsFalse(PhotoSubjectDetector.IsCreatureKey("StreetLamp"));
        }

        [Test]
        public void FileNamingAndDeletion_AreTraversalSafe()
        {
            string file = PhotoCaptureCamera.BuildFileName("W005 / Cavern", 1234, 7);
            Assert.AreEqual(Path.GetFileName(file), file);
            StringAssert.EndsWith("_1234_007.png", file);
            StringAssert.DoesNotContain("/", file);
            StringAssert.DoesNotContain("\\", file);

            string root = Path.Combine(Application.temporaryCachePath, "photo_test_" + System.Guid.NewGuid());
            string folder = PhotoCaptureCamera.PhotoFolderPath(root);
            Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder, file), new byte[] { 1, 2, 3 });
            Assert.IsFalse(PhotoCaptureCamera.SafeDeletePhoto(root, "../escape.png"));
            Assert.IsTrue(PhotoCaptureCamera.SafeDeletePhoto(root, file));
            Directory.Delete(root, true);
        }

        [Test]
        public void Wall_HasSixUniqueRatingCodedSlots()
        {
            Assert.AreEqual(6, QuartersPhotoWall.MaxDisplayedPhotos);
            Vector3[] positions = Enumerable.Range(0, 6).Select(QuartersPhotoWall.SlotLocalPosition).ToArray();
            Assert.AreEqual(6, positions.Distinct().Count());
            Assert.AreNotEqual(QuartersPhotoWall.FrameColorForRating(0), QuartersPhotoWall.FrameColorForRating(1));
            Assert.AreNotEqual(QuartersPhotoWall.FrameColorForRating(1), QuartersPhotoWall.FrameColorForRating(2));
        }

        [Test]
        public void CaptureOwner_PinsBudgetStorageAndCleanup()
        {
            string source = Read("Gameplay", "Runtime", "Photo", "PhotoCaptureCamera.cs");
            StringAssert.Contains("CaptureWidth = 512", source);
            StringAssert.Contains("CaptureHeight = 384", source);
            StringAssert.Contains("MaxViewfinderHz = 10f", source);
            StringAssert.Contains("PhotoComposition.Evaluate", source);
            StringAssert.Contains("PhotoAlbum.Add", source);
            StringAssert.Contains("save.Save();", source);
            StringAssert.Contains("_target.Release();", source);
            StringAssert.Contains("Destroy(_target);", source);
            StringAssert.Contains("Destroy(_readback);", source);
            StringAssert.Contains("Destroy(_viewfinderMaterial);", source);
            StringAssert.DoesNotContain("DontDestroyOnLoad", source);
        }

        [Test]
        public void RuntimeAndQuarters_WireOneCaptureOwnerDockAndWall()
        {
            string camera = Read("Gameplay", "Runtime", "Photo", "CameraRuntime.cs");
            Assert.AreEqual(1, Count(camera, "AddComponent<PhotoCaptureCamera>()"));
            Assert.AreEqual(1, Count(camera, "_captureCamera.TryCapture"));
            StringAssert.Contains("SendHapticImpulse", camera);

            string feature = Read("Gameplay", "Runtime", "Photo", "QuartersCameraFeature.cs");
            StringAssert.Contains("\"handheld_camera\"", feature);
            Assert.AreEqual(1, Count(feature, "AddComponent<QuartersPhotoWall>()"));
            StringAssert.Contains("SceneManager.sceneLoaded += OnSceneLoaded", feature);
            StringAssert.DoesNotContain("DontDestroyOnLoad", feature);

            string wall = Read("Gameplay", "Runtime", "World", "QuartersPhotoWall.cs");
            StringAssert.Contains("photos.Count - 1 - slot", wall);
            StringAssert.Contains("Destroy(material)", wall);
            StringAssert.Contains("Destroy(texture)", wall);
        }

        [Test]
        public void ExistingItemHolsterAndBuildPipelinesRemainTheOnlyOwners()
        {
            string factory = Read("Gameplay", "Runtime", "Items", "ItemFactory.cs");
            StringAssert.Contains("def is CameraDefinition", factory);
            StringAssert.Contains("go.AddComponent<CameraRuntime>()", factory);
            string holster = Read("Gameplay", "Runtime", "Inventory", "HolsterSocketInteractor.cs");
            StringAssert.Contains("\"handheld_camera\"", holster);
            string build = Read("Editor", "Build", "BuildAndroid.cs");
            Assert.AreEqual(1, Count(build, "CameraAuthor.EnsureAuthored();"));
            StringAssert.Contains("RunRequired(\"CameraAuthor.EnsureAuthored\"", build,
                "the one camera author hook must remain fail-closed");
        }

        private static string Read(params string[] parts)
        {
            string path = Path.Combine(Application.dataPath, "Ziptide");
            foreach (string part in parts) path = Path.Combine(path, part);
            Assert.IsTrue(File.Exists(path), path);
            return File.ReadAllText(path);
        }

        private static int Count(string source, string token)
        {
            int count = 0, index = 0;
            while ((index = source.IndexOf(token, index, System.StringComparison.Ordinal)) >= 0)
            { count++; index += token.Length; }
            return count;
        }
    }
}
#endif
