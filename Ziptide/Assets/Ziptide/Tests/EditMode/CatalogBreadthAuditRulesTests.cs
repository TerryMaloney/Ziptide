#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ziptide.Content;
using Ziptide.Editor.Audit;
using Ziptide.Editor.Patching;

namespace Ziptide.Tests.EditMode
{
    public class CatalogBreadthAuditRulesTests
    {
        [Test]
        public void AuthorTables_MeetCurrentBreadthAndIdentityLaws()
        {
            GardenAuthor.PlantSpec[] plants = GardenAuthor.PlantSpecs();
            Assert.GreaterOrEqual(plants.Length, CatalogBreadthAuditRules.MinimumPlantSpecs);
            Assert.AreEqual(plants.Length,
                plants.Select(spec => spec.Id).Distinct(System.StringComparer.Ordinal).Count(),
                "plant ids must be unique");
            Assert.IsTrue(plants.All(spec => !string.IsNullOrWhiteSpace(spec.Id)));

            VehicleAuthor.VehicleSpec[] vehicles = VehicleAuthor.VehicleSpecs();
            Assert.GreaterOrEqual(vehicles.Length, CatalogBreadthAuditRules.MinimumVehicleSpecs);
            Assert.AreEqual(vehicles.Length,
                vehicles.Select(spec => spec.Id).Distinct(System.StringComparer.Ordinal).Count(),
                "vehicle ids must be unique");
            Assert.GreaterOrEqual(
                vehicles.Select(spec => spec.Archetype).Distinct().Count(),
                CatalogBreadthAuditRules.MinimumVehicleArchetypes);
        }

        [Test]
        public void MissingIds_ReturnsStableSortedSetDifference()
        {
            string[] missing = CatalogBreadthAuditRules.MissingIds(
                new[] { "c", "a", "b", "b", "", null },
                new[] { "b", "unrelated" });

            CollectionAssert.AreEqual(new[] { "a", "c" }, missing);
        }

        [Test]
        public void CurrentFleet_ReportsTheThreeUnauthoredArchetypes()
        {
            VehicleArchetype[] missing = CatalogBreadthAuditRules.MissingVehicleArchetypes(
                VehicleAuthor.VehicleSpecs());

            CollectionAssert.AreEqual(
                new[]
                {
                    VehicleArchetype.Rover,
                    VehicleArchetype.GravSled,
                    VehicleArchetype.Walker,
                },
                missing,
                "adding one of these rides should remove only that exact debt entry");
        }

        [Test]
        public void ProjectAudit_HasNoStructuralBlockersInOrdinaryCiMode()
        {
            var report = new SceneAuditReport { sceneName = "__CATALOG_BREADTH_TEST__" };
            CatalogBreadthAuditRules.Run(report, requireAuthoredAssets: false);

            Assert.AreEqual(0, report.blockerCount,
                "structural catalog errors:\n" + string.Join("\n", report.findings.Select(f => f.ToString())));

            foreach (AuditFinding finding in report.findings)
                Assert.AreEqual(AuditSeverity.Warning, finding.severity,
                    "ordinary CI may report breadth debt, but no structural blocker is allowed");
        }

        [Test]
        public void ProjectAudit_ReportsDebtWithNamedActionableCodes()
        {
            var report = new SceneAuditReport { sceneName = "__CATALOG_BREADTH_TEST__" };
            CatalogBreadthAuditRules.Run(report, requireAuthoredAssets: false);
            var codes = new HashSet<string>(
                report.findings.Select(f => f.code),
                System.StringComparer.Ordinal);

            Assert.IsTrue(codes.Contains("PLANT_CATALOG_UNSURFACED"),
                "until all species are reachable, the audit must name the exact plant surfacing debt");
            Assert.IsTrue(codes.Contains("VEHICLE_ARCHETYPES_UNREPRESENTED"),
                "the three missing vehicle families must remain visible debt");
            Assert.IsTrue(codes.Contains("VEHICLE_GARAGE_SURFACE_MISSING"),
                "the garage/catalog surface must remain visible until it actually exists");
        }

        [Test]
        public void BuildGate_OrderIsStableAndAfterCreatureGate()
        {
            var gate = new CatalogBreadthBuildGate();
            Assert.AreEqual(835, gate.callbackOrder);
            Assert.Greater(gate.callbackOrder, new CreatureBehaviorBuildGate().callbackOrder);
        }
    }
}
#endif
