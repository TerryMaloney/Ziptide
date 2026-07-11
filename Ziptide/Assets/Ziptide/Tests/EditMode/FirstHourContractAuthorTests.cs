#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Audit;
using Ziptide.Editor.Patching;

namespace Ziptide.Tests.EditMode
{
    public class FirstHourContractAuthorTests
    {
        private byte[] _sourceBytes;
        private string _sourceJson;
        private string _sourceHash;
        private FirstHourContractImportResult _valid;

        [SetUp]
        public void SetUp()
        {
            Assert.IsTrue(File.Exists(FirstHourContractAuthor.SourceJsonPath),
                "Approved first-hour source JSON must be present in the repository checkout.");
            _sourceBytes = File.ReadAllBytes(FirstHourContractAuthor.SourceJsonPath);
            _sourceJson = System.Text.Encoding.UTF8.GetString(_sourceBytes);
            _sourceHash = FirstHourContractAuthor.ComputeSourceHash(_sourceBytes);
            _valid = FirstHourContractAuthor.ParseJson(_sourceJson, _sourceHash);
            Assert.IsTrue(_valid.success, _valid.code + ": " + _valid.message);
        }

        [Test]
        public void ApprovedSource_ImportsTwentyTwoBeatsAndFifteenTeachingLines()
        {
            Assert.AreEqual(22, _valid.data.beats.Count);
            Assert.AreEqual(15, _valid.teachingBeatCount);
            Assert.AreEqual(15, _valid.teachingLineCount);
            Assert.AreEqual(15, _valid.data.coreVerbs.Count);
            Assert.AreEqual("FH_BOOT_READY", _valid.data.beats[0].id);
            Assert.AreEqual("FH_CHANGED_SHIP_PAYOFF", _valid.data.beats[21].id);

            var taught = new HashSet<string>();
            for (int i = 0; i < _valid.data.beats.Count; i++)
            {
                FirstHourBeatDefinition beat = _valid.data.beats[i];
                Assert.AreEqual(i + 1, beat.sequence);
                if (string.IsNullOrEmpty(beat.teachesVerb)) continue;
                Assert.IsTrue(taught.Add(beat.teachesVerb), "Teaching verb repeated: " + beat.teachesVerb);
                Assert.IsNotNull(beat.rillLine);
                Assert.IsNotEmpty(beat.rillLine.id);
            }
            Assert.AreEqual(15, taught.Count);
        }

        [Test]
        public void RepeatImport_ProducesIdenticalCanonicalContent()
        {
            FirstHourContractImportResult again = FirstHourContractAuthor.ParseJson(_sourceJson, _sourceHash);
            Assert.IsTrue(again.success, again.code + ": " + again.message);
            Assert.AreEqual(
                FirstHourContractAuthor.CanonicalJson(_valid.data),
                FirstHourContractAuthor.CanonicalJson(again.data));
            Assert.AreEqual(_valid.sourceHash, again.sourceHash);
        }

        [Test]
        public void SourceHash_IsStableLowercaseSha256()
        {
            string one = FirstHourContractAuthor.ComputeSourceHash(_sourceBytes);
            string two = FirstHourContractAuthor.ComputeSourceHash(_sourceBytes);
            Assert.AreEqual(one, two);
            Assert.AreEqual(64, one.Length);
            StringAssert.DoesNotMatch("[A-F]", one);
            Assert.AreEqual(_sourceHash, _valid.sourceHash);
        }

        [Test]
        public void InvalidInputs_ReturnStableCodes()
        {
            FirstHourContractImportResult empty = FirstHourContractAuthor.ParseJson("", _sourceHash);
            Assert.IsFalse(empty.success);
            Assert.AreEqual(FirstHourContractAuthor.CodeJsonEmpty, empty.code);

            FirstHourContractImportResult badHash = FirstHourContractAuthor.ParseJson(_sourceJson, "BAD");
            Assert.IsFalse(badHash.success);
            Assert.AreEqual(FirstHourContractAuthor.CodeHashInvalid, badHash.code);

            FirstHourContractData wrongSchema = _valid.data.Clone();
            wrongSchema.schemaVersion = 2;
            FirstHourContractImportResult schema = FirstHourContractAuthor.ValidateData(wrongSchema, _sourceHash);
            Assert.IsFalse(schema.success);
            Assert.AreEqual(FirstHourContractAuthor.CodeSchema, schema.code);

            FirstHourContractData wrongSequence = _valid.data.Clone();
            wrongSequence.beats[8].sequence = 999;
            FirstHourContractImportResult sequence = FirstHourContractAuthor.ValidateData(wrongSequence, _sourceHash);
            Assert.IsFalse(sequence.success);
            Assert.AreEqual(FirstHourContractAuthor.CodeSequence, sequence.code);
        }

        [Test]
        public void ForwardPrerequisiteAndDuplicateTeachingVerb_FailClosed()
        {
            FirstHourContractData forward = _valid.data.Clone();
            forward.beats[0].prerequisites.Add("FH_CHANGED_SHIP_PAYOFF");
            FirstHourContractImportResult forwardResult = FirstHourContractAuthor.ValidateData(forward, _sourceHash);
            Assert.IsFalse(forwardResult.success);
            Assert.AreEqual(FirstHourContractAuthor.CodePrerequisite, forwardResult.code);

            FirstHourContractData duplicateVerb = _valid.data.Clone();
            duplicateVerb.beats[3].teachesVerb = duplicateVerb.beats[2].teachesVerb;
            FirstHourContractImportResult verbResult = FirstHourContractAuthor.ValidateData(duplicateVerb, _sourceHash);
            Assert.IsFalse(verbResult.success);
            Assert.AreEqual(FirstHourContractAuthor.CodeTeachingVerb, verbResult.code);
        }

        [Test]
        public void ReplaceWith_DeepCopiesImporterData()
        {
            FirstHourContractDefinition asset = ScriptableObject.CreateInstance<FirstHourContractDefinition>();
            try
            {
                asset.ReplaceWith(_valid.data, _sourceHash);
                string originalFirstId = asset.beats[0].id;
                string originalPrerequisite = asset.beats[1].prerequisites[0];

                _valid.data.beats[0].id = "MUTATED";
                _valid.data.beats[1].prerequisites[0] = "MUTATED";

                Assert.AreEqual(originalFirstId, asset.beats[0].id);
                Assert.AreEqual(originalPrerequisite, asset.beats[1].prerequisites[0]);
                Assert.AreEqual(22, asset.beats.Count);
                Assert.AreEqual(15, asset.TeachingBeatCount);
                Assert.AreEqual(15, asset.TeachingLineCount);
                Assert.AreEqual(_sourceHash, asset.sourceHash);
            }
            finally
            {
                Object.DestroyImmediate(asset);
            }
        }

        [Test]
        public void RuntimeDefinition_RoundTripsWithoutLosingNestedFields()
        {
            FirstHourContractDefinition asset = ScriptableObject.CreateInstance<FirstHourContractDefinition>();
            try
            {
                asset.ReplaceWith(_valid.data, _sourceHash);
                FirstHourContractData roundTrip = asset.ToData();
                Assert.AreEqual(
                    FirstHourContractAuthor.CanonicalJson(_valid.data),
                    FirstHourContractAuthor.CanonicalJson(roundTrip));
                Assert.AreEqual("TRAVEL_W000_TO_W001_COMPLETE", roundTrip.beats[8].completionSignal.id);
                Assert.AreEqual("TUT_PUNCH_IT", roundTrip.beats[8].rillLine.id);
            }
            finally
            {
                Object.DestroyImmediate(asset);
            }
        }

        [Test]
        public void Audit_MissingAsset_IsWarningOnlyWithStableCode()
        {
            var report = new SceneAuditReport { sceneName = "__FIRST_HOUR__" };
            FirstHourContractAuditRules.Run(report, null, _valid);

            Assert.AreEqual(0, report.blockerCount);
            Assert.AreEqual(1, report.warningCount);
            Assert.AreEqual(FirstHourContractAuditRules.MissingCode, report.findings[0].code);
        }

        [Test]
        public void Audit_InvalidSource_IsWarningOnlyWithStableCode()
        {
            var report = new SceneAuditReport { sceneName = "__FIRST_HOUR__" };
            FirstHourContractImportResult invalid = FirstHourContractImportResult.Fail(
                FirstHourContractAuthor.CodeJsonParse,
                "test invalid source");
            FirstHourContractAuditRules.Run(report, null, invalid);

            Assert.AreEqual(0, report.blockerCount);
            Assert.AreEqual(1, report.warningCount);
            Assert.AreEqual(FirstHourContractAuditRules.InvalidCode, report.findings[0].code);
        }

        [Test]
        public void Audit_DriftedAsset_IsWarningOnlyWithStableCode()
        {
            FirstHourContractDefinition asset = ScriptableObject.CreateInstance<FirstHourContractDefinition>();
            try
            {
                asset.ReplaceWith(_valid.data, new string('0', 64));
                var report = new SceneAuditReport { sceneName = "__FIRST_HOUR__" };
                FirstHourContractAuditRules.Run(report, asset, _valid);

                Assert.AreEqual(0, report.blockerCount);
                Assert.AreEqual(1, report.warningCount);
                Assert.AreEqual(FirstHourContractAuditRules.DriftCode, report.findings[0].code);
            }
            finally
            {
                Object.DestroyImmediate(asset);
            }
        }

        [Test]
        public void Audit_CurrentAsset_HasNoFindings()
        {
            FirstHourContractDefinition asset = ScriptableObject.CreateInstance<FirstHourContractDefinition>();
            try
            {
                asset.ReplaceWith(_valid.data, _sourceHash);
                var report = new SceneAuditReport { sceneName = "__FIRST_HOUR__" };
                FirstHourContractAuditRules.Run(report, asset, _valid);

                Assert.AreEqual(0, report.blockerCount);
                Assert.AreEqual(0, report.warningCount);
            }
            finally
            {
                Object.DestroyImmediate(asset);
            }
        }
    }
}
#endif
