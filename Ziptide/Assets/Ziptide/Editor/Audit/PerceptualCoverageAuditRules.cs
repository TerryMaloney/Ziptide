#if UNITY_EDITOR
using UnityEngine;
using Ziptide.Gameplay;
using Ziptide.Ship;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// PG-5: visual coverage gate for key gameplay owners. A subsystem may be logically complete but it
    /// cannot ship invisible. Single primitive fallbacks remain warnings until a dedicated authored look
    /// replaces them; zero-renderer key systems are blockers immediately.
    /// </summary>
    public static class PerceptualCoverageAuditRules
    {
        public const string ZeroRenderer = "VISUAL_COVERAGE_ZERO_RENDERER";
        public const string PrimitiveFallback = "VISUAL_COVERAGE_PRIMITIVE_FALLBACK";

        public static void Run(SceneAuditReport report)
        {
            foreach (ItemRuntime item in Object.FindObjectsOfType<ItemRuntime>(true))
                ValidateOwner(item, true, report);
            foreach (VehicleRuntime vehicle in Object.FindObjectsOfType<VehicleRuntime>(true))
                ValidateOwner(vehicle, true, report);
            foreach (ShipBoardingStation ship in Object.FindObjectsOfType<ShipBoardingStation>(true))
                ValidateOwner(ship, true, report);
            foreach (ToxicRiverRuntime river in Object.FindObjectsOfType<ToxicRiverRuntime>(true))
                ValidateOwner(river, true, report);
            foreach (DroneRuntime drone in Object.FindObjectsOfType<DroneRuntime>(true))
                ValidateOwner(drone, false, report);
            foreach (RepairableMachine machine in Object.FindObjectsOfType<RepairableMachine>(true))
                ValidateOwner(machine, false, report);
        }

        public static void ValidateOwner(Component owner, bool blockerWhenInvisible,
            SceneAuditReport report)
        {
            if (owner == null || report == null) return;
            Renderer[] renderers = owner.GetComponentsInChildren<Renderer>(true);
            int visible = 0;
            Renderer only = null;
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy) continue;
                visible++;
                only = renderer;
            }

            string path = GetPath(owner.transform);
            if (visible == 0)
            {
                string message = owner.GetType().Name + " at " + path
                    + " has no enabled visible renderer in its owned hierarchy.";
                if (blockerWhenInvisible) report.Blocker(ZeroRenderer, message, path);
                else report.Warning(ZeroRenderer, message, path);
                return;
            }

            if (visible == 1 && IsBarePrimitive(owner.transform, only))
                report.Warning(PrimitiveFallback,
                    owner.GetType().Name + " at " + path
                    + " is represented by one bare primitive renderer; keep visible but replace with authored presentation.",
                    path);
        }

        private static bool IsBarePrimitive(Transform owner, Renderer renderer)
        {
            if (owner == null || renderer == null) return false;
            MeshFilter filter = renderer.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) return false;
            string meshName = filter.sharedMesh.name;
            bool primitive = meshName.Contains("Cube") || meshName.Contains("Sphere")
                || meshName.Contains("Capsule") || meshName.Contains("Cylinder");
            return primitive && owner.GetComponentsInChildren<Renderer>(true).Length == 1;
        }

        private static string GetPath(Transform value)
        {
            if (value == null) return string.Empty;
            string path = value.name;
            while (value.parent != null)
            {
                value = value.parent;
                path = value.name + "/" + path;
            }
            return path;
        }
    }
}
#endif
