using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryRuntimeCensusTests
    {
        private RecoveryTestRig _fixture;
        private readonly List<GameObject> _created = new List<GameObject>();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            DestroyNamedImmediate("__RECOVERY_CENSUS_CANVAS");
            DestroyNamedImmediate("__RECOVERY_CENSUS_TEXT");
            DestroyNamedImmediate("__RECOVERY_CENSUS_AUDIO");
            DestroyNamedImmediate("__RECOVERY_CENSUS_PRIMITIVE");
            DestroyNamedImmediate("__RECOVERY_CENSUS_EXTRA_XRI");
            DestroyNamedImmediate("__RECOVERY_CENSUS_CREDITS");
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = 0; i < _created.Count; i++)
                if (_created[i] != null) Object.DestroyImmediate(_created[i]);
            _created.Clear();
            _fixture?.Dispose();
            _fixture = null;
            yield return null;
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);
        }

        [UnityTest]
        public IEnumerator ScopedFixture_CapturesStableCompositionAndWritesArtifacts()
        {
            _fixture = new RecoveryTestRig(new Vector3(0.18f, 1.65f, 0.12f));

            GameObject canvasHost = Child("__RECOVERY_CENSUS_CANVAS");
            canvasHost.AddComponent<Canvas>();

            GameObject textHost = Child("__RECOVERY_CENSUS_TEXT");
            textHost.AddComponent<TextMesh>().text = "CENSUS";

            GameObject audioHost = Child("__RECOVERY_CENSUS_AUDIO");
            audioHost.AddComponent<AudioSource>();

            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            primitive.name = "__RECOVERY_CENSUS_PRIMITIVE";
            primitive.transform.SetParent(_fixture.Root.transform, false);
            _created.Add(primitive);
            yield return null;

            RecoveryRuntimeCensusSnapshot snapshot = RecoveryRuntimeCensus.Capture(
                "R1_4_CONTROLLED_FIXTURE",
                _fixture.Root.transform);

            Assert.AreEqual("GoldenSlice", snapshot.activeProfile);
            Assert.AreEqual(1, Count(snapshot.managers, "XRInteractionManager", true));
            Assert.AreEqual(1, Count(snapshot.managers, "InputActionManager", true));
            Assert.AreEqual(1, snapshot.cameras.Count);
            Assert.AreEqual("TAGGED_MAIN_CAMERA", snapshot.cameras[0].role);
            Assert.GreaterOrEqual(Count(snapshot.ui, "Canvas", false), 1);
            Assert.GreaterOrEqual(Count(snapshot.ui, "TextMesh", false), 1);
            Assert.GreaterOrEqual(snapshot.audioSources.Count, 1);
            Assert.GreaterOrEqual(snapshot.materials.Count, 1);
            Assert.IsFalse(HasFinding(snapshot, "DUPLICATE_XRI_MANAGER"));
            Assert.IsFalse(HasFinding(snapshot, "FORBIDDEN_AUTOMATIC_OWNER_ACTIVE"));

            RecoveryCensusArtifactPaths artifacts = RecoveryRuntimeCensus.WriteArtifacts(
                snapshot,
                "r1_4_controlled_fixture");
            Assert.IsTrue(File.Exists(artifacts.JsonPath), "Census JSON artifact was not written.");
            Assert.IsTrue(File.Exists(artifacts.MarkdownPath), "Census Markdown artifact was not written.");
            StringAssert.Contains("R1_4_CONTROLLED_FIXTURE", File.ReadAllText(artifacts.JsonPath));
            StringAssert.Contains("Findings", File.ReadAllText(artifacts.MarkdownPath));
        }

        [UnityTest]
        public IEnumerator DuplicateInteractionManagers_ProduceAStableBlockerFinding()
        {
            _fixture = new RecoveryTestRig(new Vector3(0.2f, 1.65f, 0.1f));
            GameObject extra = Child("__RECOVERY_CENSUS_EXTRA_XRI");
            extra.AddComponent<XRInteractionManager>();
            yield return null;

            RecoveryRuntimeCensusSnapshot snapshot = RecoveryRuntimeCensus.Capture(
                "R1_4_DUPLICATE_MANAGER_PROBE",
                _fixture.Root.transform);

            Assert.AreEqual(2, Count(snapshot.managers, "XRInteractionManager", true));
            Assert.IsTrue(HasFinding(snapshot, "DUPLICATE_XRI_MANAGER"),
                "The census did not turn a real duplicate XR manager into a blocker finding.");
        }

        [Test]
        public void ForcedForbiddenCreditsSurface_ProducesAStableBlockerFinding()
        {
            _fixture = new RecoveryTestRig(new Vector3(0.2f, 1.65f, 0.1f));
            GameObject host = Child("__RECOVERY_CENSUS_CREDITS");
            CreditsHud hud = host.AddComponent<CreditsHud>();
            Assert.IsFalse(hud.enabled, "Golden policy did not initially disable the legacy HUD.");

            // Simulate a future bypass that re-enables the component after Awake. Capture occurs
            // synchronously before Update can fail it closed again.
            hud.enabled = true;
            RecoveryRuntimeCensusSnapshot snapshot = RecoveryRuntimeCensus.Capture(
                "R1_4_FORBIDDEN_SURFACE_PROBE",
                _fixture.Root.transform);
            hud.enabled = false;

            Assert.IsTrue(HasFinding(snapshot, "FORBIDDEN_PLAYER_SURFACE_ACTIVE"),
                "The census did not report a forcibly re-enabled forbidden player surface.");
        }

        private GameObject Child(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(_fixture.Root.transform, false);
            _created.Add(go);
            return go;
        }

        private static int Count(
            List<RecoveryCensusComponentRecord> records,
            string category,
            bool activeOnly)
        {
            int count = 0;
            for (int i = 0; i < records.Count; i++)
            {
                RecoveryCensusComponentRecord record = records[i];
                if (record.category != category) continue;
                if (activeOnly && (!record.active || !record.enabled)) continue;
                count++;
            }
            return count;
        }

        private static bool HasFinding(RecoveryRuntimeCensusSnapshot snapshot, string code)
        {
            for (int i = 0; i < snapshot.findings.Count; i++)
                if (snapshot.findings[i].code == code) return true;
            return false;
        }

        private static void DestroyNamedImmediate(string name)
        {
            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (go != null && go.scene.IsValid() && go.name == name)
                    Object.DestroyImmediate(go);
            }
        }
    }
}
