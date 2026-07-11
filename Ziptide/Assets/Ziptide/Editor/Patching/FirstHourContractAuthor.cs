#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    public sealed class FirstHourContractImportResult
    {
        public bool success;
        public string code;
        public string message;
        public string sourceHash;
        public FirstHourContractData data;
        public int teachingBeatCount;
        public int teachingLineCount;

        public static FirstHourContractImportResult Pass(
            FirstHourContractData contract,
            string hash,
            int teachingBeats,
            int teachingLines)
        {
            return new FirstHourContractImportResult
            {
                success = true,
                code = FirstHourContractAuthor.CodeOk,
                message = "First-hour contract is valid.",
                sourceHash = hash,
                data = contract,
                teachingBeatCount = teachingBeats,
                teachingLineCount = teachingLines
            };
        }

        public static FirstHourContractImportResult Fail(string code, string message)
        {
            return new FirstHourContractImportResult
            {
                success = false,
                code = code,
                message = message,
                sourceHash = string.Empty,
                data = null,
                teachingBeatCount = 0,
                teachingLineCount = 0
            };
        }
    }

    /// <summary>
    /// Compiles docs/first_hour/first_hour_beats.json into one deterministic Resources asset.
    /// Runtime code never parses JSON and no fallback beat list exists.
    /// </summary>
    public static class FirstHourContractAuthor
    {
        public const string AssetPath = "Assets/Ziptide/Resources/Tutorial/FirstHourContract.asset";
        public const string CodeOk = "OK";
        public const string CodeJsonEmpty = "FIRST_HOUR_CONTRACT_JSON_EMPTY";
        public const string CodeJsonParse = "FIRST_HOUR_CONTRACT_JSON_PARSE";
        public const string CodeHashInvalid = "FIRST_HOUR_CONTRACT_SOURCE_HASH_INVALID";
        public const string CodeSchema = "FIRST_HOUR_CONTRACT_SCHEMA_UNSUPPORTED";
        public const string CodeContractId = "FIRST_HOUR_CONTRACT_ID_INVALID";
        public const string CodeBeatCount = "FIRST_HOUR_CONTRACT_BEAT_COUNT";
        public const string CodeCoreVerbCount = "FIRST_HOUR_CONTRACT_CORE_VERB_COUNT";
        public const string CodeSequence = "FIRST_HOUR_CONTRACT_SEQUENCE_INVALID";
        public const string CodeBeatNull = "FIRST_HOUR_CONTRACT_BEAT_NULL";
        public const string CodeBeatId = "FIRST_HOUR_CONTRACT_BEAT_ID_INVALID";
        public const string CodeBeatDuplicate = "FIRST_HOUR_CONTRACT_BEAT_ID_DUPLICATE";
        public const string CodePrerequisite = "FIRST_HOUR_CONTRACT_PREREQUISITE_INVALID";
        public const string CodeSignal = "FIRST_HOUR_CONTRACT_SIGNAL_INVALID";
        public const string CodeEvidence = "FIRST_HOUR_CONTRACT_EVIDENCE_INVALID";
        public const string CodeSafety = "FIRST_HOUR_CONTRACT_SAFETY_INVALID";
        public const string CodeTeachingVerb = "FIRST_HOUR_CONTRACT_TEACHING_VERB_INVALID";
        public const string CodeTeachingCount = "FIRST_HOUR_CONTRACT_TEACHING_COUNT";
        public const string CodeTeachingLine = "FIRST_HOUR_CONTRACT_TEACHING_LINE_INVALID";
        public const string CodeTeachingLineCount = "FIRST_HOUR_CONTRACT_TEACHING_LINE_COUNT";
        public const string CodeBoundary = "FIRST_HOUR_CONTRACT_BOUNDARY_INVALID";
        public const string CodeSourceMissing = "FIRST_HOUR_CONTRACT_SOURCE_MISSING";
        public const string CodeSourceRead = "FIRST_HOUR_CONTRACT_SOURCE_READ_FAILED";

        public static string SourceJsonPath
        {
            get
            {
                return Path.GetFullPath(Path.Combine(
                    Application.dataPath,
                    "..",
                    "..",
                    "docs",
                    "first_hour",
                    "first_hour_beats.json"));
            }
        }

        [MenuItem("Ziptide/Tutorial/Author First-Hour Contract")]
        public static void AuthorFromMenu()
        {
            EnsureAuthored();
        }

        public static bool EnsureAuthored()
        {
            FirstHourContractImportResult result = ImportSource();
            if (!result.success)
            {
                Debug.LogWarning(
                    "[Ziptide] FIRST_HOUR_CONTRACT_IMPORT_FAILED code=" + result.code +
                    " message=" + result.message);
                return false;
            }

            string absoluteAssetDirectory = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Resources",
                "Tutorial"));
            Directory.CreateDirectory(absoluteAssetDirectory);
            AssetDatabase.Refresh();

            FirstHourContractDefinition asset =
                AssetDatabase.LoadAssetAtPath<FirstHourContractDefinition>(AssetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<FirstHourContractDefinition>();
                AssetDatabase.CreateAsset(asset, AssetPath);
            }

            asset.ReplaceWith(result.data, result.sourceHash);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceUpdate);

            Debug.Log(
                "ZIPTIDE: FIRST_HOUR_CONTRACT beats=" + asset.beats.Count +
                " teaching=" + asset.TeachingBeatCount +
                " hash=" + asset.sourceHash);
            return true;
        }

        public static FirstHourContractImportResult ImportSource()
        {
            string path = SourceJsonPath;
            if (!File.Exists(path))
                return FirstHourContractImportResult.Fail(
                    CodeSourceMissing,
                    "Approved source JSON is missing: " + path);

            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                string json = Encoding.UTF8.GetString(bytes);
                return ParseJson(json, ComputeSourceHash(bytes));
            }
            catch (Exception ex)
            {
                return FirstHourContractImportResult.Fail(
                    CodeSourceRead,
                    "Could not read approved source JSON: " + ex.GetType().Name);
            }
        }

        public static FirstHourContractImportResult ParseJson(string json, string sourceHash)
        {
            if (string.IsNullOrWhiteSpace(json))
                return FirstHourContractImportResult.Fail(CodeJsonEmpty, "JSON is empty.");
            if (!IsLowerHexHash(sourceHash))
                return FirstHourContractImportResult.Fail(CodeHashInvalid, "Source hash must be 64 lowercase hex characters.");

            FirstHourContractData data;
            try
            {
                data = JsonUtility.FromJson<FirstHourContractData>(json);
            }
            catch (Exception ex)
            {
                return FirstHourContractImportResult.Fail(
                    CodeJsonParse,
                    "JSON parse failed: " + ex.GetType().Name);
            }

            if (data == null)
                return FirstHourContractImportResult.Fail(CodeJsonParse, "JSON parse returned null.");

            return ValidateData(data, sourceHash);
        }

        public static FirstHourContractImportResult ValidateData(FirstHourContractData data, string sourceHash)
        {
            if (data == null)
                return FirstHourContractImportResult.Fail(CodeJsonParse, "Contract data is null.");
            if (!IsLowerHexHash(sourceHash))
                return FirstHourContractImportResult.Fail(CodeHashInvalid, "Source hash must be 64 lowercase hex characters.");
            if (data.schemaVersion != 1)
                return FirstHourContractImportResult.Fail(CodeSchema, "Expected schemaVersion 1.");
            if (!string.Equals(data.contractId, "first-hour-v1", StringComparison.Ordinal))
                return FirstHourContractImportResult.Fail(CodeContractId, "Expected contractId first-hour-v1.");
            if (data.beats == null || data.beats.Count != 22)
                return FirstHourContractImportResult.Fail(CodeBeatCount, "Expected exactly 22 beats.");
            if (data.coreVerbs == null || data.coreVerbs.Count != 15)
                return FirstHourContractImportResult.Fail(CodeCoreVerbCount, "Expected exactly 15 core verbs.");

            var allowedEvidence = new HashSet<string>(data.allowedEvidence ?? new List<string>(), StringComparer.Ordinal);
            var allowedStatuses = new HashSet<string>(data.bindingStatuses ?? new List<string>(), StringComparer.Ordinal);
            var coreVerbs = new HashSet<string>(data.coreVerbs, StringComparer.Ordinal);
            var taughtVerbs = new HashSet<string>(StringComparer.Ordinal);
            var seenBeatIds = new HashSet<string>(StringComparer.Ordinal);
            int teachingBeats = 0;
            int teachingLines = 0;

            for (int i = 0; i < data.beats.Count; i++)
            {
                FirstHourBeatDefinition beat = data.beats[i];
                if (beat == null)
                    return FirstHourContractImportResult.Fail(CodeBeatNull, "Beat at index " + i + " is null.");
                if (beat.sequence != i + 1)
                    return FirstHourContractImportResult.Fail(
                        CodeSequence,
                        "Beat " + (beat.id ?? "<missing>") + " has sequence " + beat.sequence +
                        "; expected " + (i + 1) + ".");
                if (string.IsNullOrWhiteSpace(beat.id))
                    return FirstHourContractImportResult.Fail(CodeBeatId, "Beat at sequence " + beat.sequence + " has no id.");
                if (!seenBeatIds.Add(beat.id))
                    return FirstHourContractImportResult.Fail(CodeBeatDuplicate, "Duplicate beat id: " + beat.id);

                if (beat.prerequisites == null)
                    return FirstHourContractImportResult.Fail(CodePrerequisite, "Prerequisites list is null for " + beat.id + ".");
                foreach (string prerequisite in beat.prerequisites)
                {
                    if (string.IsNullOrWhiteSpace(prerequisite) || !seenBeatIds.Contains(prerequisite))
                        return FirstHourContractImportResult.Fail(
                            CodePrerequisite,
                            "Unknown or forward prerequisite " + (prerequisite ?? "<null>") + " for " + beat.id + ".");
                }

                if (beat.completionSignal == null || string.IsNullOrWhiteSpace(beat.completionSignal.id) ||
                    string.IsNullOrWhiteSpace(beat.completionSignal.status) ||
                    !allowedStatuses.Contains(beat.completionSignal.status))
                {
                    return FirstHourContractImportResult.Fail(
                        CodeSignal,
                        "Invalid completion signal for " + beat.id + ".");
                }

                if (beat.evidence == null || beat.evidence.Count == 0)
                    return FirstHourContractImportResult.Fail(CodeEvidence, "Evidence is missing for " + beat.id + ".");
                foreach (string evidence in beat.evidence)
                {
                    if (string.IsNullOrWhiteSpace(evidence) || !allowedEvidence.Contains(evidence))
                        return FirstHourContractImportResult.Fail(
                            CodeEvidence,
                            "Unsupported evidence " + (evidence ?? "<null>") + " for " + beat.id + ".");
                }

                if (beat.allowsInputLock || beat.movesPlayerRig)
                    return FirstHourContractImportResult.Fail(
                        CodeSafety,
                        "Beat " + beat.id + " violates no-input-lock/no-rig-motion law.");

                bool teaches = !string.IsNullOrWhiteSpace(beat.teachesVerb);
                if (teaches)
                {
                    teachingBeats++;
                    if (!coreVerbs.Contains(beat.teachesVerb) || !taughtVerbs.Add(beat.teachesVerb))
                        return FirstHourContractImportResult.Fail(
                            CodeTeachingVerb,
                            "Unknown or duplicate teaching verb " + beat.teachesVerb + " on " + beat.id + ".");
                    if (beat.hesitationSeconds <= 0f)
                        return FirstHourContractImportResult.Fail(
                            CodeTeachingVerb,
                            "Teaching beat " + beat.id + " needs a positive hesitation delay.");
                    if (beat.rillLine == null || string.IsNullOrWhiteSpace(beat.rillLine.id) ||
                        string.IsNullOrWhiteSpace(beat.rillLine.status) ||
                        !allowedStatuses.Contains(beat.rillLine.status))
                    {
                        return FirstHourContractImportResult.Fail(
                            CodeTeachingLine,
                            "Teaching beat " + beat.id + " has an invalid RILL line reference.");
                    }
                    teachingLines++;
                }
                else if (beat.rillLine != null && !string.IsNullOrWhiteSpace(beat.rillLine.id))
                {
                    return FirstHourContractImportResult.Fail(
                        CodeTeachingLine,
                        "Beat " + beat.id + " has a teaching line but no teachesVerb.");
                }
            }

            if (teachingBeats != 15 || taughtVerbs.Count != coreVerbs.Count)
                return FirstHourContractImportResult.Fail(CodeTeachingCount, "Expected each of 15 core verbs exactly once.");
            if (teachingLines != 15)
                return FirstHourContractImportResult.Fail(CodeTeachingLineCount, "Expected exactly 15 teaching line references.");
            if (!string.Equals(data.beats[0].id, "FH_BOOT_READY", StringComparison.Ordinal) ||
                !string.Equals(data.beats[data.beats.Count - 1].id, "FH_CHANGED_SHIP_PAYOFF", StringComparison.Ordinal))
            {
                return FirstHourContractImportResult.Fail(
                    CodeBoundary,
                    "First and last beat ids do not match the approved contract boundaries.");
            }

            return FirstHourContractImportResult.Pass(data.Clone(), sourceHash, teachingBeats, teachingLines);
        }

        public static string CanonicalJson(FirstHourContractData data)
        {
            return data == null ? string.Empty : JsonUtility.ToJson(data, false);
        }

        public static string ComputeSourceHash(byte[] bytes)
        {
            if (bytes == null) bytes = Array.Empty<byte>();
            using (SHA256 sha = SHA256.Create())
            {
                byte[] digest = sha.ComputeHash(bytes);
                var builder = new StringBuilder(digest.Length * 2);
                foreach (byte value in digest) builder.Append(value.ToString("x2"));
                return builder.ToString();
            }
        }

        private static bool IsLowerHexHash(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 64) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool digit = c >= '0' && c <= '9';
                bool lowerHex = c >= 'a' && c <= 'f';
                if (!digit && !lowerHex) return false;
            }
            return true;
        }
    }
}
#endif
