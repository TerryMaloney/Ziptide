#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// PG-1: judges scene-authored weapons from their final visible hierarchy, collider and Grip/Muzzle
    /// relationship. Definition values and root Transform scale are intentionally ignored.
    /// </summary>
    public static class WeaponPerceptualAuditRules
    {
        public const string MissingVisual = "WEAPON_VISIBLE_BOUNDS_MISSING";
        public const string TinyVisual = "WEAPON_VISIBLE_BOUNDS_TINY";
        public const string HugeVisual = "WEAPON_VISIBLE_BOUNDS_HUGE";
        public const string MissingGrip = "WEAPON_GRIP_MISSING";
        public const string BadAimAxis = "WEAPON_GRIP_MUZZLE_AXIS";
        public const string MuzzleDistance = "WEAPON_MUZZLE_DISTANCE_INVALID";
        public const string ColliderMismatch = "WEAPON_COLLIDER_VISUAL_MISMATCH";

        public static void Run(SceneAuditReport report)
        {
            foreach (ItemRuntime item in Object.FindObjectsOfType<ItemRuntime>(true))
            {
                if (item == null || item.GetComponent<XRGrabInteractable>() == null) continue;
                bool hasMuzzle = item.transform.Find("Muzzle") != null;
                bool knownWeapon = hasMuzzle
                    || item.GetComponent<PistolRuntime>() != null
                    || item.GetComponent<TaserDartGunRuntime>() != null
                    || item.GetComponent<GravityGunRuntime>() != null;
                if (!knownWeapon) continue;
                ValidateWeapon(item.gameObject, report);
            }
        }

        public static void ValidateWeapon(GameObject weapon, SceneAuditReport report)
        {
            if (weapon == null || report == null) return;
            string path = GetPath(weapon.transform);
            if (!TryVisibleBounds(weapon, out Bounds bounds))
            {
                report.Blocker(MissingVisual, path + " has no enabled visible renderer.", path);
                return;
            }

            float longest = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
            if (longest < 0.14f)
                report.Blocker(TinyVisual,
                    path + " visible longest dimension is " + longest.ToString("F3")
                    + "m; weapon is below the hand-readable floor of 0.14m.", path);
            if (longest > 2.60f)
                report.Blocker(HugeVisual,
                    path + " visible longest dimension is " + longest.ToString("F2")
                    + "m; weapon exceeds the handheld ceiling of 2.60m.", path);

            Transform grip = weapon.transform.Find("Grip");
            if (grip == null)
            {
                report.Blocker(MissingGrip, path + " has no canonical Grip transform.", path);
                return;
            }

            Transform muzzle = weapon.transform.Find("Muzzle");
            if (muzzle != null)
            {
                Vector3 gripToMuzzle = muzzle.position - grip.position;
                float distance = gripToMuzzle.magnitude;
                if (distance < 0.075f || distance > 2.40f)
                    report.Blocker(MuzzleDistance,
                        path + " Grip→Muzzle distance is " + distance.ToString("F3")
                        + "m; expected 0.075–2.40m.", path);
                if (distance > 0.0001f)
                {
                    float forwardDot = Vector3.Dot(grip.forward.normalized, gripToMuzzle.normalized);
                    if (forwardDot < 0.62f)
                        report.Blocker(BadAimAxis,
                            path + " Grip forward and Grip→Muzzle direction dot="
                            + forwardDot.ToString("F3") + " (minimum 0.62).", path);
                }
            }

            Collider[] colliders = weapon.GetComponentsInChildren<Collider>(true);
            bool overlapsVisible = false;
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider collider = colliders[i];
                if (collider == null || !collider.enabled || collider.isTrigger) continue;
                Bounds expanded = collider.bounds;
                expanded.Expand(0.04f);
                if (expanded.Intersects(bounds)) { overlapsVisible = true; break; }
            }
            if (!overlapsVisible)
                report.Blocker(ColliderMismatch,
                    path + " has no enabled solid collider overlapping its visible bounds.", path);
        }

        public static bool TryVisibleBounds(GameObject root, out Bounds bounds)
        {
            bounds = default;
            if (root == null) return false;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            bool found = false;
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy) continue;
                if (!found) { bounds = renderer.bounds; found = true; }
                else bounds.Encapsulate(renderer.bounds);
            }
            return found;
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
