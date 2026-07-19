using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Ziptide.Build;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>Proof for the device-discovered ToxicCity egress failures from the 2026-07-19 retry.</summary>
    public sealed class QuestRetryRouteRegressionTests
    {
        private const string LayoutPath = "Assets/Ziptide/Content/City/ToxicCityLayout.asset";
        private const string ExitPackPath =
            "Assets/Ziptide/Content/Worlds/Packs/ToxicCityExit_WorldPack.asset";

        [Test]
        public void ToxicCityExit_ReturnsToTheOnlyGoldenHomeScene()
        {
            var exit = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(ExitPackPath);
            Assert.That(exit, Is.Not.Null);
            Assert.That(RecoveryBuildAndroid.GoldenExitSceneName, Is.EqualTo(ZiptideConstants.SceneW000));
            Assert.That(exit.sceneName, Is.EqualTo(RecoveryBuildAndroid.GoldenExitSceneName),
                "The ray-selected Leave door must not point at MilestoneA_GrabCube or another scene absent from the Golden APK.");
        }

        [Test]
        public void GoldenShipyardBridge_OverlapsBothWalkableSlabsAtWalkwayHeight()
        {
            var layout = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(LayoutPath);
            Assert.That(layout, Is.Not.Null);
            Assert.That(layout.shipyard, Is.Not.Null);

            var district = layout.districts.FirstOrDefault(d => d != null && d.id == "Shipyard");
            Assert.That(district, Is.Not.Null);

            float districtRearEdge = district.anchor.z - district.bounds.y * 0.5f;
            float berthFrontEdge = layout.shipyard.berthCenter.z + layout.shipyard.berthSize.y * 0.5f;
            Assert.That(districtRearEdge - berthFrontEdge, Is.GreaterThan(0f),
                "This assertion records the source-layout seam reproduced on Quest.");

            Bounds bridge = RecoveryBuildAndroid.GoldenShipyardBridgeBounds;
            Assert.That(bridge.max.z, Is.GreaterThan(districtRearEdge),
                "Bridge must overlap the Shipyard district rather than merely touch its collider edge.");
            Assert.That(bridge.min.z, Is.LessThan(berthFrontEdge),
                "Bridge must overlap the berth rather than leave another precision seam.");
            Assert.That(bridge.size.x, Is.GreaterThanOrEqualTo(layout.shipyard.berthSize.x),
                "The safe apron must span the full berth width, not only one approach line.");
            Assert.That(bridge.max.y, Is.EqualTo(layout.walkwayHeight).Within(0.001f),
                "Bridge top must be flush with both walkable surfaces.");
        }
    }
}
