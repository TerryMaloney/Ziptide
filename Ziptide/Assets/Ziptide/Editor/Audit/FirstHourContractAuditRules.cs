#if UNITY_EDITOR
using System;
using UnityEditor;
using Ziptide.Content;
using Ziptide.Editor.Patching;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// WARN-only first-hour asset integrity. Missing or invalid data disables tutorial orchestration;
    /// it must never become a world-build blocker or invent a fallback beat list.
    /// </summary>
    public static class FirstHourContractAuditRules
    {
        public const string MissingCode = "FIRST_HOUR_CONTRACT_ASSET_MISSING";
        public const string DriftCode = "FIRST_HOUR_CONTRACT_ASSET_DRIFT";
        public const string InvalidCode = "FIRST_HOUR_CONTRACT_ASSET_INVALID";

        public static void Run(SceneAuditReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            FirstHourContractDefinition asset =
                AssetDatabase.LoadAssetAtPath<FirstHourContractDefinition>(FirstHourContractAuthor.AssetPath);
            FirstHourContractImportResult source = FirstHourContractAuthor.ImportSource();
            Run(report, asset, source);
        }

        /// <summary>Dependency seam for deterministic EditMode tests.</summary>
        public static void Run(
            SceneAuditReport report,
            FirstHourContractDefinition asset,
            FirstHourContractImportResult source)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            if (source == null || !source.success)
            {
                report.Warning(
                    InvalidCode,
                    "Approved first-hour JSON is invalid or unreadable. Tutorial orchestration must remain disabled. " +
                    "sourceCode=" + (source != null ? source.code : "NULL_RESULT") +
                    " sourceMessage=" + (source != null ? source.message : "No import result."),
                    FirstHourContractAuthor.SourceJsonPath);
                return;
            }

            if (asset == null)
            {
                report.Warning(
                    MissingCode,
                    "Generated first-hour Resources asset is missing. Run the author or a canonical build; " +
                    "base gameplay remains available but tutorial orchestration must stay disabled.",
                    FirstHourContractAuthor.AssetPath);
                return;
            }

            FirstHourContractImportResult assetValidation =
                FirstHourContractAuthor.ValidateData(asset.ToData(), asset.sourceHash);
            if (!assetValidation.success)
            {
                report.Warning(
                    InvalidCode,
                    "Generated first-hour asset is structurally invalid. Tutorial orchestration must remain disabled. " +
                    "assetCode=" + assetValidation.code + " assetMessage=" + assetValidation.message,
                    FirstHourContractAuthor.AssetPath);
                return;
            }

            bool hashDrift = !string.Equals(asset.sourceHash, source.sourceHash, StringComparison.Ordinal);
            bool contentDrift = !string.Equals(
                FirstHourContractAuthor.CanonicalJson(asset.ToData()),
                FirstHourContractAuthor.CanonicalJson(source.data),
                StringComparison.Ordinal);

            if (hashDrift || contentDrift)
            {
                report.Warning(
                    DriftCode,
                    "Generated first-hour asset does not match the approved JSON. " +
                    "hashDrift=" + hashDrift + " contentDrift=" + contentDrift +
                    ". Re-run FirstHourContractAuthor before relying on tutorial orchestration.",
                    FirstHourContractAuthor.AssetPath);
            }
        }
    }
}
#endif
