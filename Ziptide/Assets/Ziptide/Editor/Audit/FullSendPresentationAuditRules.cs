#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;
using Ziptide.Gameplay;
using Ziptide.Ship;

namespace Ziptide.Editor.Audit
{
    /// <summary>Round-two visual exposure gates for the shared hero ship and ToxicCity starter fleet.</summary>
    public static class FullSendPresentationAuditRules
    {
        private static readonly HashSet<string> ExpectedVehicleIds = new HashSet<string>
        {
            "tide_skiff", "dune_hoverbike", "cavern_crawler"
        };

        public static void Run(SceneAuditReport report)
        {
            CheckHeroShips(report);
            if (report.sceneName == ScenePatcherToxicCity.SceneName)
                CheckToxicCityFleet(report);
        }

        private static void CheckHeroShips(SceneAuditReport report)
        {
            ShipBoardingStation[] stations = Object.FindObjectsOfType<ShipBoardingStation>();
            foreach (ShipBoardingStation station in stations)
            {
                if (station == null) continue;
                Transform ship = station.transform;
                string path = ship.name;
                Renderer[] renderers = ship.GetComponentsInChildren<Renderer>(true);
                if (renderers.Length < ShipHullBuilder.MinimumHeroRenderers)
                    report.Blocker("HERO_SHIP_SPARSE_HULL",
                        path + " has " + renderers.Length + " hull renderers; minimum hero fallback is "
                        + ShipHullBuilder.MinimumHeroRenderers + ".", path);

                string[] required =
                {
                    "Fuselage_Aft", "Fuselage_Mid", "Fuselage_Bow", "Nose_Tip",
                    "Wing_L", "Wing_R", "TailFin", "Exhaust_L", "Exhaust_R",
                    "CargoPod", "BoardingDoor_Port", "BoardingStep_Port"
                };
                foreach (string name in required)
                    if (ship.Find(name) == null)
                        report.Blocker("HERO_SHIP_PART_MISSING",
                            path + " is missing required hull/refit part '" + name + "'.", path);

                var materials = new HashSet<int>();
                foreach (Renderer renderer in renderers)
                    if (renderer != null && renderer.sharedMaterial != null)
                        materials.Add(renderer.sharedMaterial.GetInstanceID());
                if (materials.Count > 12)
                    report.Blocker("HERO_SHIP_MATERIAL_BUDGET",
                        path + " uses " + materials.Count + " materials (cap 12).", path);

                int nonCube = ship.GetComponentsInChildren<MeshFilter>(true)
                    .Count(m => m.sharedMesh != null &&
                        (m.sharedMesh.name.Contains("Sphere") ||
                         m.sharedMesh.name.Contains("Cylinder") ||
                         m.sharedMesh.name.Contains("Capsule")));
                if (nonCube < 18)
                    report.Blocker("HERO_SHIP_BLOCK_STACK",
                        path + " has only " + nonCube + " non-cube hull primitives; hero fallback regressed toward a block stack.", path);
            }
        }

        private static void CheckToxicCityFleet(SceneAuditReport report)
        {
            Transform root = Object.FindObjectsOfType<Transform>()
                .FirstOrDefault(t => t != null && t.name == ToxicCityVehicleBuilder.RootName);
            if (root == null)
            {
                report.Blocker("TOXIC_CITY_FLEET_MISSING",
                    "ToxicCity has no visible starter-fleet root. Vehicle save hook did not run.");
                return;
            }

            VehicleRuntime[] vehicles = root.GetComponentsInChildren<VehicleRuntime>(true);
            if (vehicles.Length != ToxicCityVehicleBuilder.ExpectedVehicleCount)
                report.Blocker("TOXIC_CITY_FLEET_COUNT",
                    "ToxicCity has " + vehicles.Length + " ride(s); expected "
                    + ToxicCityVehicleBuilder.ExpectedVehicleCount + ".", root.name);

            var found = new HashSet<string>();
            foreach (VehicleRuntime vehicle in vehicles)
            {
                if (vehicle == null) continue;
                string id = vehicle.VehicleId;
                if (!found.Add(id))
                    report.Blocker("TOXIC_CITY_FLEET_DUPLICATE",
                        "Vehicle id '" + id + "' appears more than once.", vehicle.name);
                if (Resources.Load<VehicleDefinition>("Vehicles/" + id) == null)
                    report.Blocker("TOXIC_CITY_VEHICLE_DEF_MISSING",
                        "Vehicle '" + id + "' has no Resources/Vehicles definition.", vehicle.name);

                if (!Physics.Raycast(vehicle.transform.position + Vector3.up * 2f,
                        Vector3.down, out RaycastHit hit, 8f, ~0, QueryTriggerInteraction.Ignore))
                    report.Blocker("TOXIC_CITY_VEHICLE_UNSUPPORTED",
                        "Vehicle '" + id + "' has no solid floor below its parking bay.", vehicle.name);
                else if (hit.collider.GetComponentInParent<ToxicRiverRuntime>() != null)
                    report.Blocker("TOXIC_CITY_VEHICLE_IN_RIVER",
                        "Vehicle '" + id + "' is parked in a toxic-river basin instead of on a district pad.", vehicle.name);
            }

            foreach (string expected in ExpectedVehicleIds)
                if (!found.Contains(expected))
                    report.Blocker("TOXIC_CITY_VEHICLE_ID_MISSING",
                        "ToxicCity starter fleet is missing '" + expected + "'.", root.name);

            int pads = root.GetComponentsInChildren<Transform>(true)
                .Count(t => t.name.StartsWith("ParkingPad_"));
            if (pads != ToxicCityVehicleBuilder.ExpectedVehicleCount)
                report.Blocker("TOXIC_CITY_VEHICLE_PAD_COUNT",
                    "ToxicCity has " + pads + " parking pads; expected "
                    + ToxicCityVehicleBuilder.ExpectedVehicleCount + ".", root.name);
        }
    }
}
#endif
