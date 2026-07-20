using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>Small pooled impact punctuation for pistol hits; presentation only.</summary>
    public static class WeaponImpactFx
    {
        public static void Spawn(Vector3 point, Vector3 normal, bool confirmedTarget)
        {
            string key = confirmedTarget ? "pistol_impact_confirmed" : "pistol_impact_surface";
            Color color = confirmedTarget
                ? new Color(0.35f, 1f, 0.45f)
                : new Color(1f, 0.72f, 0.24f);
            GameObject effect = GamePool.Get(key, () => Build(key, color), point + normal * 0.012f);
            if (effect == null) return;
            effect.transform.rotation = normal.sqrMagnitude > 0.001f
                ? Quaternion.LookRotation(normal, Vector3.up)
                : Quaternion.identity;
            GamePool.ReleaseAfter(key, effect, 0.14f);
        }

        private static GameObject Build(string name, Color color)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            root.name = name;
            root.transform.localScale = Vector3.one * 0.045f;
            Collider col = root.GetComponent<Collider>();
            if (col != null) col.enabled = false;
            ItemFactory.ApplyURPColor(root, color);

            for (int i = 0; i < 3; i++)
            {
                GameObject ray = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ray.name = "SparkRay_" + i;
                ray.transform.SetParent(root.transform, false);
                ray.transform.localRotation = Quaternion.Euler(0f, i * 120f, 0f);
                ray.transform.localPosition = ray.transform.localRotation * Vector3.forward * 0.55f;
                ray.transform.localScale = new Vector3(0.08f, 0.08f, 0.65f);
                Collider rayCol = ray.GetComponent<Collider>();
                if (rayCol != null) rayCol.enabled = false;
                ItemFactory.ApplyURPColor(ray, color);
            }
            return root;
        }
    }
}
