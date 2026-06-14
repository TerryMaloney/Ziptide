using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Ziptide.Editor.Audit
{
    public enum AuditSeverity { Warning, Blocker }

    [System.Serializable]
    public class AuditFinding
    {
        public AuditSeverity severity;
        public string code;
        public string message;
        public string objectPath;

        public AuditFinding(AuditSeverity severity, string code, string message, string objectPath = "")
        {
            this.severity = severity;
            this.code = code;
            this.message = message;
            this.objectPath = objectPath;
        }

        public override string ToString()
        {
            string tag = severity == AuditSeverity.Blocker ? "[BLOCKER]" : "[WARNING]";
            string loc = !string.IsNullOrEmpty(objectPath) ? " @ " + objectPath : "";
            return tag + " " + code + ": " + message + loc;
        }
    }

    /// <summary>
    /// Per-scene audit result. Accumulate findings, then query totals.
    /// </summary>
    public class SceneAuditReport
    {
        public string sceneName;
        public List<AuditFinding> findings = new List<AuditFinding>();

        public int blockerCount
        {
            get
            {
                int n = 0;
                foreach (var f in findings) if (f.severity == AuditSeverity.Blocker) n++;
                return n;
            }
        }

        public int warningCount
        {
            get
            {
                int n = 0;
                foreach (var f in findings) if (f.severity == AuditSeverity.Warning) n++;
                return n;
            }
        }

        public void Add(AuditSeverity severity, string code, string message, string objectPath = "")
        {
            findings.Add(new AuditFinding(severity, code, message, objectPath));
        }

        public void Blocker(string code, string message, string objectPath = "")
            => Add(AuditSeverity.Blocker, code, message, objectPath);

        public void Warning(string code, string message, string objectPath = "")
            => Add(AuditSeverity.Warning, code, message, objectPath);
    }

    /// <summary>
    /// Aggregated multi-scene audit result. Handles markdown + JSON serialisation.
    /// </summary>
    public class WorldAuditReport
    {
        public List<SceneAuditReport> scenes = new List<SceneAuditReport>();

        public int totalBlockers
        {
            get { int n = 0; foreach (var s in scenes) n += s.blockerCount; return n; }
        }

        public int totalWarnings
        {
            get { int n = 0; foreach (var s in scenes) n += s.warningCount; return n; }
        }

        public string ToMarkdown()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# World Audit Report");
            sb.AppendLine();
            sb.AppendLine("**Total Blockers:** " + totalBlockers + "  ");
            sb.AppendLine("**Total Warnings:** " + totalWarnings);
            sb.AppendLine();
            foreach (var scene in scenes)
            {
                sb.AppendLine("## Scene: " + scene.sceneName);
                sb.AppendLine("Blockers: " + scene.blockerCount + "  Warnings: " + scene.warningCount);
                sb.AppendLine();
                if (scene.findings.Count == 0)
                {
                    sb.AppendLine("_No issues found._");
                }
                else
                {
                    foreach (var f in scene.findings)
                        sb.AppendLine("- " + f.ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public string ToJson()
        {
            var sb = new StringBuilder();
            sb.Append("{\"totalBlockers\":" + totalBlockers
                + ",\"totalWarnings\":" + totalWarnings
                + ",\"scenes\":[");
            for (int i = 0; i < scenes.Count; i++)
            {
                var scene = scenes[i];
                if (i > 0) sb.Append(",");
                sb.Append("{\"sceneName\":\"" + EscapeJson(scene.sceneName) + "\""
                    + ",\"blockerCount\":" + scene.blockerCount
                    + ",\"warningCount\":" + scene.warningCount
                    + ",\"findings\":[");
                for (int j = 0; j < scene.findings.Count; j++)
                {
                    var f = scene.findings[j];
                    if (j > 0) sb.Append(",");
                    sb.Append("{\"severity\":\"" + f.severity + "\""
                        + ",\"code\":\"" + EscapeJson(f.code) + "\""
                        + ",\"message\":\"" + EscapeJson(f.message) + "\""
                        + ",\"objectPath\":\"" + EscapeJson(f.objectPath) + "\"}");
                }
                sb.Append("]}");
            }
            sb.Append("]}");
            return sb.ToString();
        }

        private static string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
        }
    }
}
