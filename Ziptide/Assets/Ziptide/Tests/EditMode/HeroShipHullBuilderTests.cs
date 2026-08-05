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
                    "Fuselage_Aft", "Fuselage_Mid", "Fuselage_Bow", "Nose_Tip", "DorsalSpine",
                    "Cab", "EngineDrum", "EngineCollar", "CargoPod",
                    "Leg_FL", "Leg_FR", "Leg_RL", "Leg_RR",
                    "ClawArm_Upper", "ClawArm_Fore", "CyanPort"
                };
                foreach (string anchor in refitAnchors)
                    Assert.That(ship.transform.Find(anchor), Is.Not.Null,
                        anchor + " is required by ShipRefit and must stay a direct child.");

                Assert.That(ship.transform.Find("BoardingDoor_Port"), Is.Not.Null);
                Assert.That(ship.transform.Find("BoardingStep_Port"), Is.Not.Null);

                // The back view is the Scrapper's signature (visual spec §2): ONE large central
                // nozzle plus FOUR smaller ones in a quincunx. Losing the quincunx loses the ship.
                Assert.That(ship.transform.Find("Nozzle_Center"), Is.Not.Null);
                foreach (string q in new[] { "Nozzle_TL", "Nozzle_TR", "Nozzle_BL", "Nozzle_BR" })
                    Assert.That(ship.transform.Find(q), Is.Not.Null,
                        q + " — the quincunx is the signature engine read.");

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

                // ── Identity guards: the things that make this the SCRAPPER and not a generic ship ──

                // The claw is mounted on the LEFT flank ONLY. Spec §2 calls the asymmetry canon, and a
                // mirrored claw is exactly the "generic spaceship" reflex this rebuild exists to kill.
                Transform[] clawParts = ship.GetComponentsInChildren<Transform>(true)
                    .Where(t => t.name.StartsWith("ClawArm_") || t.name.StartsWith("ClawFinger_"))
                    .ToArray();
                Assert.That(clawParts, Is.Not.Empty, "The salvage claw is part of the silhouette.");
                foreach (Transform part in clawParts)
                    Assert.That(part.localPosition.x, Is.LessThan(0f),
                        part.name + " must stay on the port flank — the claw is never mirrored.");

                // Landing stance must be WIDER than the hull, so the front view shows daylight between
                // legs and body. Spec §2 names that gap the "stable workhorse" read.
                float hullHalfWidth = ship.transform.Find("Fuselage_Mid").localScale.x * 0.5f;
                foreach (string leg in new[] { "Leg_FL", "Leg_FR", "Leg_RL", "Leg_RR" })
                {
                    Transform skid = ship.transform.Find(leg + "_Skid");
                    Assert.That(skid, Is.Not.Null, leg + " needs its broad flat skid.");
                    Assert.That(Mathf.Abs(skid.localPosition.x), Is.GreaterThan(hullHalfWidth),
                        leg + " must plant outboard of the hull, not tucked under it.");
                }

                // The cab breaks the roofline — spec §2: its roof is the ship's highest point.
                Transform cab = ship.transform.Find("Cab");
                Transform midHull = ship.transform.Find("Fuselage_Mid");
                Assert.That(cab.localPosition.y, Is.GreaterThan(midHull.localPosition.y),
                    "The truck cab sits ON TOP of the hull line.");
            }
            finally
            {
                if (ship != null) Object.DestroyImmediate(ship);
            }
        }
    }
}
