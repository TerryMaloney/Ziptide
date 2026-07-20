using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;

namespace Ziptide.Tests.EditMode
{
    public sealed class HeroShipHullBuilderTests
    {
        [Test]
        public void Build_ProducesRefitCompatibleBoundedHeroSilhouette()
        {
            GameObject ship = null;
            try
            {
                ship = new GameObject("Ship_Static_Placeholder");
                ShipHullBuilder.Build(ship.transform, new Vector3(5f, 3f, 12f), new GlobalPalette());

                string[] refitAnchors =
                {
                    "Fuselage_Aft", "Fuselage_Mid", "Fuselage_Bow", "Nose_Tip",
                    "DorsalSpine", "Wing_L", "Wing_R", "WingTip_L", "WingTip_R",
                    "TailFin", "Exhaust_L", "Exhaust_R", "CargoPod"
                };
                foreach (string anchor in refitAnchors)
                    Assert.That(ship.transform.Find(anchor), Is.Not.Null,
                        anchor + " is required by ShipRefit and must stay a direct child.");

                Assert.That(ship.transform.Find("BoardingDoor_Port"), Is.Not.Null);
                Assert.That(ship.transform.Find("BoardingStep_Port"), Is.Not.Null);
                Assert.That(ship.transform.Find("NacelleRing_F_L"), Is.Not.Null);
                Assert.That(ship.transform.Find("NacelleRing_F_R"), Is.Not.Null);
                Assert.That(ship.transform.Find("RadarDish"), Is.Not.Null);

                Renderer[] renderers = ship.GetComponentsInChildren<Renderer>(true);
                Assert.That(renderers.Length, Is.GreaterThanOrEqualTo(ShipHullBuilder.MinimumHeroRenderers));

                var materials = new HashSet<int>();
                foreach (Renderer renderer in renderers)
                    if (renderer.sharedMaterial != null) materials.Add(renderer.sharedMaterial.GetInstanceID());
                Assert.That(materials.Count, Is.LessThanOrEqualTo(12),
                    "Hero ship must use a closed shared-material vocabulary.");

                int nonCubeMeshes = ship.GetComponentsInChildren<MeshFilter>(true)
                    .Count(m => m.sharedMesh != null &&
                        (m.sharedMesh.name.Contains("Sphere") ||
                         m.sharedMesh.name.Contains("Cylinder") ||
                         m.sharedMesh.name.Contains("Capsule")));
                Assert.That(nonCubeMeshes, Is.GreaterThanOrEqualTo(18),
                    "The hero silhouette must not regress to an all-cube block stack.");

                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
                Assert.That(bounds.size.x, Is.InRange(4.8f, 9.0f));
                Assert.That(bounds.size.y, Is.InRange(2.5f, 7.0f));
                Assert.That(bounds.size.z, Is.InRange(10.0f, 17.0f));

                int solidColliders = ship.GetComponentsInChildren<Collider>(true)
                    .Count(c => c.enabled && !c.isTrigger);
                Assert.That(solidColliders, Is.InRange(8, 36),
                    "Ship needs structural collision without collider soup on every visual panel.");
            }
            finally
            {
                if (ship != null) Object.DestroyImmediate(ship);
            }
        }
    }
}
