using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryExposureProfileTests
    {
        [Serializable]
        private sealed class OwnerDocument
        {
            public string id;
            public string source;
            public string symbol;
        }

        [Serializable]
        private sealed class OwnerDocumentRoot
        {
            public OwnerDocument[] owners;
        }

        [TearDown]
        public void RestoreFullDevelopmentProfile()
        {
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);
        }

        [Test]
        public void AutomaticOwnerCatalog_CoversR0EvidenceAndEveryFeatureId()
        {
            var registrations = RecoveryAutomaticOwnerCatalog.All;
            var enumValues = (RecoveryFeatureId[])Enum.GetValues(typeof(RecoveryFeatureId));
            Assert.AreEqual(enumValues.Length, registrations.Count,
                "Every closed feature ID must have exactly one automatic-owner registration.");

            var ownerIds = new HashSet<string>(StringComparer.Ordinal);
            var featureIds = new HashSet<RecoveryFeatureId>();
            var registrationsByOwner = new Dictionary<string, RecoveryAutomaticOwnerRegistration>(StringComparer.Ordinal);
            string repositoryRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));

            for (int i = 0; i < registrations.Count; i++)
            {
                var registration = registrations[i];
                Assert.IsTrue(ownerIds.Add(registration.OwnerId), "Duplicate automatic owner id " + registration.OwnerId);
                Assert.IsTrue(featureIds.Add(registration.FeatureId), "Duplicate feature id " + registration.FeatureId);
                Assert.IsTrue(File.Exists(Path.Combine(repositoryRoot, registration.SourceRelativePath)),
                    "Catalog source path is missing: " + registration.SourceRelativePath);
                registrationsByOwner.Add(registration.OwnerId, registration);
            }

            string documentPath = Path.Combine(repositoryRoot, "docs", "recovery", "automatic_runtime_owners.json");
            Assert.IsTrue(File.Exists(documentPath), "R0 automatic-owner evidence file is missing.");
            var document = JsonUtility.FromJson<OwnerDocumentRoot>(File.ReadAllText(documentPath));
            Assert.IsNotNull(document);
            Assert.IsNotNull(document.owners);
            Assert.AreEqual(document.owners.Length, registrations.Count,
                "R0 evidence and C# exposure catalog have different owner counts.");

            for (int i = 0; i < document.owners.Length; i++)
            {
                var documented = document.owners[i];
                Assert.IsTrue(registrationsByOwner.TryGetValue(documented.id, out var registration),
                    "Documented automatic owner has no C# feature registration: " + documented.id);
                Assert.AreEqual(documented.source, registration.SourceRelativePath, documented.id + " source drifted");
                Assert.AreEqual(documented.symbol, registration.Symbol, documented.id + " symbol drifted");
            }
        }

        [Test]
        public void FullDevelopment_AllowsEveryClosedFeatureId()
        {
            var values = (RecoveryFeatureId[])Enum.GetValues(typeof(RecoveryFeatureId));
            for (int i = 0; i < values.Length; i++)
                Assert.IsTrue(RecoveryExposureProfiles.FullDevelopment.Allows(values[i]), values[i].ToString());
        }

        [Test]
        public void GoldenSlice_IncludesRequiredSupportAndExcludesPrototypeMutators()
        {
            var profile = RecoveryExposureProfiles.GoldenSlice;
            var registrations = RecoveryAutomaticOwnerCatalog.All;
            for (int i = 0; i < registrations.Count; i++)
            {
                var registration = registrations[i];
                if (registration.Classification == RecoveryOwnerClassification.AlwaysRequired)
                    Assert.IsTrue(profile.Allows(registration.FeatureId), registration.OwnerId);
            }

            Assert.IsTrue(profile.Allows(RecoveryFeatureId.RuntimeHealthMonitor));
            Assert.IsTrue(profile.Allows(RecoveryFeatureId.AmbienceDirector));
            Assert.IsTrue(profile.Allows(RecoveryFeatureId.ComfortVignette));
            Assert.IsTrue(profile.Allows(RecoveryFeatureId.EcologyInjector));
            Assert.IsTrue(profile.Allows(RecoveryFeatureId.FirstHourObservation));
            Assert.IsTrue(profile.Allows(RecoveryFeatureId.SingletonValidator));

            Assert.IsFalse(profile.Allows(RecoveryFeatureId.DebugHud));
            Assert.IsFalse(profile.Allows(RecoveryFeatureId.XrCameraEnforcer));
            Assert.IsFalse(profile.Allows(RecoveryFeatureId.RuntimeInputEnabler));
            Assert.IsFalse(profile.Allows(RecoveryFeatureId.RuntimeMaterialFixer));
            Assert.IsFalse(profile.Allows(RecoveryFeatureId.VrBootDiagnostics));
            Assert.IsFalse(profile.Allows(RecoveryFeatureId.DevWarpBoard));
            Assert.IsFalse(profile.Allows(RecoveryFeatureId.ConquestMissionInjector));
            Assert.IsFalse(profile.Allows(RecoveryFeatureId.PvpProgression));
            Assert.IsFalse(profile.Allows(RecoveryFeatureId.QuartersCameraInjector));
        }

        [Test]
        public void Diagnostic_AddsOnlyExplicitApprovedDiagnostics()
        {
            var profile = RecoveryExposureProfiles.Diagnostic(
                RecoveryFeatureId.DebugHud,
                RecoveryFeatureId.DevWarpBoard);

            Assert.IsTrue(profile.Allows(RecoveryFeatureId.DebugHud));
            Assert.IsTrue(profile.Allows(RecoveryFeatureId.DevWarpBoard));
            Assert.IsFalse(profile.Allows(RecoveryFeatureId.VrBootDiagnostics),
                "Diagnostics must be opt-in individually.");
            Assert.IsFalse(profile.Allows(RecoveryFeatureId.RuntimeMaterialFixer),
                "Global fallback mutators are not diagnostic surfaces.");
            Assert.Throws<ArgumentException>(() =>
                RecoveryExposureProfiles.Diagnostic(RecoveryFeatureId.RuntimeMaterialFixer));
        }

        [Test]
        public void RuntimeGate_UsesOnlyTheExplicitActiveProfile()
        {
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);
            Assert.AreEqual("GoldenSlice", RecoveryRuntimeGate.ActiveProfileName);
            Assert.IsTrue(RecoveryRuntimeGate.Allows(RecoveryFeatureId.TravelCoordinator));
            Assert.IsFalse(RecoveryRuntimeGate.Allows(RecoveryFeatureId.ConquestMissionInjector));

            var diagnostic = RecoveryExposureProfiles.Diagnostic(RecoveryFeatureId.VrBootDiagnostics);
            RecoveryRuntimeGate.SetActiveProfile(diagnostic);
            Assert.AreEqual("Diagnostic", RecoveryRuntimeGate.ActiveProfileName);
            Assert.IsTrue(RecoveryRuntimeGate.Allows(RecoveryFeatureId.VrBootDiagnostics));
            Assert.IsFalse(RecoveryRuntimeGate.Allows(RecoveryFeatureId.DebugHud));
        }
    }
}
