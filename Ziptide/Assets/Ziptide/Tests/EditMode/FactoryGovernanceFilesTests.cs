using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Ziptide.Tests.EditMode
{
    public sealed class FactoryGovernanceFilesTests
    {
        private static string RepoRoot => Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));

        [Test]
        public void PipelineSpine_KeepsAllStagesCurrentPositionAndAmendmentLaw()
        {
            string pipeline = Read("docs/PIPELINE.md");
            StringAssert.Contains("Version 1.0", pipeline);
            StringAssert.Contains("THE AMENDMENT LAW", pipeline);
            StringAssert.Contains("## Current position, in one line", pipeline);
            for (int i = 0; i <= 11; i++)
                StringAssert.Contains("## STAGE " + i + " ", pipeline,
                    "Pipeline stage " + i + " disappeared from the canonical spine.");

            string checklist = Read("docs/FACTORY_GOVERNANCE_CHECKLIST.md");
            StringAssert.Contains("**Stage:** 9", checklist);
            StringAssert.Contains("**Type:** law", checklist);
            StringAssert.Contains("same-commit `docs/PIPELINE.md` amendment", checklist);
        }

        [Test]
        public void FinishedGameBenchmark_KeepsAllTenShipBackwardCategories()
        {
            string benchmark = Read("docs/design/FINISHED_GAME_BENCHMARK_RB.md");
            string checklist = Read("docs/FACTORY_GOVERNANCE_CHECKLIST.md");
            for (int i = 1; i <= 10; i++)
            {
                string id = "B" + i;
                StringAssert.Contains("| **" + id + "** |", benchmark,
                    id + " disappeared from the canonical ten-category benchmark.");
                StringAssert.Contains("| " + id + " |", checklist,
                    id + " disappeared from the recurring factory checklist.");
            }

            StringAssert.Contains("docs/design/FINISHED_GAME_BENCHMARK.md", checklist);
            StringAssert.Contains("docs/MISS_LEDGER.md", checklist);
            StringAssert.Contains("world_improvement_compile.json", checklist);
        }

        [Test]
        public void MissLedger_EveryOpenEntryCarriesTheFiveClassLawFields()
        {
            string ledger = Read("docs/MISS_LEDGER.md");
            int openStart = ledger.IndexOf("## OPEN", System.StringComparison.Ordinal);
            int closedStart = ledger.IndexOf("## CLOSED", System.StringComparison.Ordinal);
            Assert.That(openStart, Is.GreaterThanOrEqualTo(0));
            Assert.That(closedStart, Is.GreaterThan(openStart));
            string open = ledger.Substring(openStart, closedStart - openStart);

            MatchCollection entries = Regex.Matches(open,
                @"(?ms)^\s*(\d+)\.\s+(.*?)(?=^\s*\d+\.|\z)");
            Assert.That(entries.Count, Is.GreaterThanOrEqualTo(10));
            foreach (Match entry in entries)
            {
                string number = entry.Groups[1].Value;
                string block = entry.Groups[2].Value;
                StringAssert.Contains("**WHAT", block, "ledger #" + number + " lacks WHAT");
                StringAssert.Contains("**FOUND BY:**", block, "ledger #" + number + " lacks FOUND BY");
                StringAssert.Contains("**WHY MISSED:**", block, "ledger #" + number + " lacks WHY MISSED");
                StringAssert.Contains("**CLASS:**", block, "ledger #" + number + " lacks CLASS");
                StringAssert.Contains("**SYSTEM CHANGE:**", block, "ledger #" + number + " lacks SYSTEM CHANGE");
            }
        }

        [Test]
        public void ExcellenceMap_StillCarriesWorldFactoryAndShipShellSurfaces()
        {
            string map = Read("docs/EXCELLENCE_MAP.md");
            StringAssert.Contains("Repeatable world improvement", map);
            StringAssert.Contains("SHIP & STORE", map);
            StringAssert.Contains("THE GATE-COVERAGE MATRIX", map);
        }

        private static string Read(string relative)
        {
            string path = Path.Combine(RepoRoot, relative.Replace('/', Path.DirectorySeparatorChar));
            Assert.That(File.Exists(path), Is.True, "Required factory-governance file missing: " + relative);
            return File.ReadAllText(path);
        }
    }
}
