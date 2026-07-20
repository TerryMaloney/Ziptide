using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    public sealed class ToxicCityVehicleBuilderTests
    {
        [Test]
        public void Build_PlacesCompleteVisibleUniqueFleetAndIsIdempotent()
        {
            GameObject city = null;
            CityLayoutDefinition kit = null;
            try
            {
                city = new GameObject("__TEST_CITY_ROOT");
                kit = ScriptableObject.CreateInstance<CityLayoutDefinition>();
                kit.districts.Add(new DistrictDef { id = "Shipyard", anchor = new Vector3(0f, 0f, -30f) });
                kit.districts.Add(new DistrictDef { id = "Dispatch", anchor = new Vector3(0f, 0f, -8f) });
                kit.districts.Add(new DistrictDef { id = "Market", anchor = new Vector3(26f, 0f, 8f) });

                ToxicCityVehicleBuilder.Summary summary = ToxicCityVehicleBuilder.Build(city.transform, kit);
                Transform root = city.transform.Find(ToxicCityVehicleBuilder.RootName);

                Assert.That(root, Is.Not.Null);
                Assert.That(summary.Vehicles, Is.EqualTo(ToxicCityVehicleBuilder.ExpectedVehicleCount));
                Assert.That(summary.Pads, Is.EqualTo(ToxicCityVehicleBuilder.ExpectedVehicleCount));

                VehicleRuntime[] vehicles = root.GetComponentsInChildren<VehicleRuntime>(true);
                var ids = new HashSet<string>();
                foreach (VehicleRuntime vehicle in vehicles)
                {
                    ids.Add(vehicle.VehicleId);
                    Transform preview = vehicle.transform.Find("__VehicleVisual");
                    Assert.That(preview, Is.Not.Null, vehicle.VehicleId + " needs a saved pre-play silhouette.");
                    Assert.That(preview.GetComponentsInChildren<Renderer>(true).Length, Is.GreaterThanOrEqualTo(4));
                    Assert.That(preview.GetComponentsInChildren<Collider>(true), Is.Empty,
                        "Saved vehicle previews are presentation only; runtime owns actual ride collision.");
                }
                CollectionAssert.AreEquivalent(new[]
                {
                    "tide_skiff", "dune_hoverbike", "cavern_crawler"
                }, ids);

                foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                    if (t.name.StartsWith("ParkingPad_"))
                        Assert.That(t.GetComponent<Collider>(), Is.Null,
                            "Parking-pad paint must not create a raised collision seam over district floors.");

                // Simulate a stale generated scene from before the preview contract. Idempotent Build must
                // upgrade it rather than returning early just because the fleet root already exists.
                Object.DestroyImmediate(vehicles[0].transform.Find("__VehicleVisual").gameObject);
                ToxicCityVehicleBuilder.Summary second = ToxicCityVehicleBuilder.Build(city.transform, kit);
                Assert.That(second.Vehicles, Is.EqualTo(summary.Vehicles));
                Assert.That(second.Pads, Is.EqualTo(summary.Pads));
                Assert.That(city.transform.GetComponentsInChildren<VehicleRuntime>(true).Length,
                    Is.EqualTo(ToxicCityVehicleBuilder.ExpectedVehicleCount));
                Assert.That(vehicles[0].transform.Find("__VehicleVisual"), Is.Not.Null);
            }
            finally
            {
                if (city != null) Object.DestroyImmediate(city);
                if (kit != null) Object.DestroyImmediate(kit);
            }
        }
    }
}
