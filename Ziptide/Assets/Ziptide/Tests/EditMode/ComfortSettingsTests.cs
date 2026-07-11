#if UNITY_EDITOR
using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public class ComfortSettingsTests
    {
        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey(ComfortSettings.PresetPrefKey);
            PlayerPrefs.DeleteKey("ziptide_comfort_vignette");
        }

        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey(ComfortSettings.PresetPrefKey);
            PlayerPrefs.DeleteKey("ziptide_comfort_vignette");
        }

        [Test]
        public void Standard_IsTheInstallDefault_AndMatchesLockedTable()
        {
            Assert.AreEqual(ComfortPreset.Standard, ComfortSettings.DefaultPreset);
            Assert.AreEqual(ComfortPreset.Standard, ComfortSettings.CurrentPreset);

            ComfortDialSet d = ComfortSettings.Resolve(ComfortPreset.Standard);
            Assert.IsFalse(d.smoothTurn);
            Assert.AreEqual(30f, d.snapTurnAngle);
            Assert.AreEqual(0f, d.smoothTurnSpeed);
            Assert.AreEqual(0.6f, d.vignetteStrength);
            Assert.AreEqual(1.35f, d.slideBoost);
            Assert.AreEqual(0.8f, d.slideSeconds);
            Assert.IsTrue(d.dashEnabled);
            Assert.AreEqual(8f, d.ziplineMaxSpeed);
            Assert.IsTrue(d.flightRollEnabled);
            Assert.AreEqual(0.4f, d.flightYawRepeatSeconds);
            Assert.IsTrue(d.useShippedFlightBoost);
            Assert.AreEqual(0f, d.flightBoostCeiling);
        }

        [Test]
        public void Cozy_MatchesLockedTable()
        {
            ComfortDialSet d = ComfortSettings.Resolve(ComfortPreset.Cozy);
            Assert.IsFalse(d.smoothTurn);
            Assert.AreEqual(45f, d.snapTurnAngle);
            Assert.AreEqual(0f, d.smoothTurnSpeed);
            Assert.AreEqual(1f, d.vignetteStrength);
            Assert.AreEqual(1f, d.slideBoost);
            Assert.AreEqual(0f, d.slideSeconds);
            Assert.IsTrue(d.dashEnabled);
            Assert.AreEqual(5.5f, d.ziplineMaxSpeed);
            Assert.IsFalse(d.flightRollEnabled);
            Assert.AreEqual(0.55f, d.flightYawRepeatSeconds);
            Assert.IsFalse(d.useShippedFlightBoost);
            Assert.AreEqual(1.8f, d.flightBoostCeiling);
        }

        [Test]
        public void Bold_MatchesLockedTable()
        {
            ComfortDialSet d = ComfortSettings.Resolve(ComfortPreset.Bold);
            Assert.IsTrue(d.smoothTurn);
            Assert.AreEqual(0f, d.snapTurnAngle);
            Assert.AreEqual(120f, d.smoothTurnSpeed);
            Assert.AreEqual(0.15f, d.vignetteStrength);
            Assert.AreEqual(1.35f, d.slideBoost);
            Assert.AreEqual(0.8f, d.slideSeconds);
            Assert.IsTrue(d.dashEnabled);
            Assert.AreEqual(8f, d.ziplineMaxSpeed);
            Assert.IsTrue(d.flightRollEnabled);
            Assert.AreEqual(0.4f, d.flightYawRepeatSeconds);
            Assert.IsTrue(d.useShippedFlightBoost);
            Assert.AreEqual(0f, d.flightBoostCeiling);
        }

        [Test]
        public void SelectedPreset_PersistsAtDeviceLevel_AndSurvivesFreshProfile()
        {
            ComfortSettings.SelectPreset(ComfortPreset.Cozy);
            Assert.AreEqual(ComfortPreset.Cozy, ComfortSettings.CurrentPreset);

            var fresh = ProfileSerializer.NewProfile();
            Assert.IsNotNull(fresh);
            Assert.AreEqual(ComfortPreset.Cozy, ComfortSettings.CurrentPreset);
        }

        [Test]
        public void InvalidStoredPreset_FallsBackToStandard()
        {
            PlayerPrefs.SetInt(ComfortSettings.PresetPrefKey, 999);
            Assert.AreEqual(ComfortPreset.Standard, ComfortSettings.CurrentPreset);
        }

        [Test]
        public void ComfortSettingsSource_NeverReferencesProfileOrGameplayOwners()
        {
            string source = Read("Core", "Runtime", "ComfortSettings.cs");
            StringAssert.DoesNotContain("using Ziptide.Gameplay", source);
            StringAssert.DoesNotContain("SaveSystem.", source);
            StringAssert.DoesNotContain("LocomotionDirector", source);
            StringAssert.DoesNotContain("ComfortVignette", source);
            StringAssert.DoesNotContain("ZiplineRuntime", source);
            StringAssert.Contains("public const ComfortPreset DefaultPreset = ComfortPreset.Standard", source);
        }

        [Test]
        public void ComfortConsole_DelegatesToExistingOwners_AndNeverMovesRig()
        {
            string source = Read("Gameplay", "Runtime", "Tutorial", "ComfortConsoleRuntime.cs");
            StringAssert.Contains("director.ApplyComfortSettings(settings)", source);
            StringAssert.Contains("vignette.SetStrength(settings.vignetteStrength)", source);
            StringAssert.Contains("zipline.maxSpeed = settings.ziplineMaxSpeed", source);
            StringAssert.Contains("ZIPTIDE: COMFORT_PRESET preset=", source);
            StringAssert.DoesNotContain("SaveSystem.", source);
            StringAssert.DoesNotContain("transform.position +=", source);
            StringAssert.DoesNotContain("_rig.position", source);
            StringAssert.DoesNotContain("CharacterController.Move", source);
        }

        private static string Read(params string[] parts)
        {
            string path = Path.Combine(Application.dataPath, "Ziptide");
            for (int i = 0; i < parts.Length; i++) path = Path.Combine(path, parts[i]);
            Assert.IsTrue(File.Exists(path), path);
            return File.ReadAllText(path);
        }
    }

    public class ComfortCoverageTests
    {
        [Test]
        public void SingleVignette_CoversRigMotionAndExternalWorldMotion()
        {
            string source = Read("Gameplay", "Runtime", "Player", "ComfortVignette.cs");
            StringAssert.Contains("Vector3 pos = _rig.position;", source);
            StringAssert.Contains("float yaw = _rig.eulerAngles.y;", source);
            StringAssert.Contains("public void ReportExternalMotion(float speed01, float turn01)", source);
            Assert.AreEqual(1, Count(source, "ComfortCore.TargetAperture("));
        }

        [Test]
        public void ComfortApplication_ChangesProvidersButNeverTranslatesTheRig()
        {
            string source = Read("Gameplay", "Runtime", "Locomotion", "LocomotionDirector.cs");
            StringAssert.Contains("public void ApplyComfortSettings(ComfortDialSet settings)", source);
            StringAssert.Contains("smoothTurn.gameObject.SetActive(settings.smoothTurn)", source);
            StringAssert.Contains("snapTurn.gameObject.SetActive(!settings.smoothTurn)", source);
            StringAssert.DoesNotContain("transform.position", source);
            StringAssert.DoesNotContain("CharacterController.Move", source);
            StringAssert.DoesNotContain("SetParent", source);
        }

        private static string Read(params string[] parts)
        {
            string path = Path.Combine(Application.dataPath, "Ziptide");
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
