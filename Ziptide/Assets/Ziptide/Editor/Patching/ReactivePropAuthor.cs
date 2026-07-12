#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Gameplay;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// FORGE III F3.6 world wiring. The pass runs after every visual author, recognizes a closed set of
    /// Forge recipe ids, and adds one existing-interface ReactiveProp plus one tightly bounded projectile
    /// hit proxy. It does not create rewards, rigidbodies, real lights, input, navigation, or new damage.
    /// </summary>
    public static class ReactivePropAuthor
    {
        public const string HitProxyName = "__REACTIVE_HIT";
        public const string PracticalHaloName = "PracticalHalo";
        public const string PracticalPoolName = "PracticalPool";

        public struct Profile
        {
            public ReactionKind kind;
            public Vector3 proxySize;
            public Vector3 proxyCenter;

            public Profile(ReactionKind kind, Vector3 proxySize, Vector3 proxyCenter)
            {
                this.kind = kind;
                this.proxySize = proxySize;
                this.proxyCenter = proxyCenter;
            }
        }

        public static int Place(Transform parent)
        {
            if (parent == null) return 0;

            var looks = new List<ForgeModuleLook>(
                parent.GetComponentsInChildren<ForgeModuleLook>(true));
            looks.Sort((a, b) => string.CompareOrdinal(PathOf(a.transform), PathOf(b.transform)));

            var claimedOwners = new HashSet<int>();
            int placed = 0;
            foreach (ForgeModuleLook look in looks)
            {
                if (look == null || !TryProfile(look.recipeId, out Profile profile)) continue;

                GameObject owner = ResolveOwner(look);
                if (owner == null || !claimedOwners.Add(owner.GetInstanceID())) continue;

                BoxCollider proxy = EnsureProxy(look.transform, profile);
                EnsureKeepChild(look, HitProxyName);

                // Sconces and street poles keep their ForgeModuleLook on the practical root. If
                // PracticalLight.Awake happens first, the swap must not disable its halo/pool renderers.
                // Lantern looks live on a lifted child and cannot touch the root's sibling glow children.
                if (profile.kind == ReactionKind.LightFlickerOut &&
                    look.GetComponent<PracticalLight>() != null)
                {
                    EnsureKeepChild(look, PracticalHaloName);
                    EnsureKeepChild(look, PracticalPoolName);
                }

                ReactiveProp reactive = owner.GetComponent<ReactiveProp>();
                if (reactive == null) reactive = owner.AddComponent<ReactiveProp>();
                reactive.Configure(profile.kind, proxy);
                placed++;
            }

            Debug.Log("ZIPTIDE: REACTIVE_PROPS_AUTHORED count=" + placed);
            return placed;
        }

        /// <summary>Closed Forge-recipe vocabulary. Every ReactionKind has at least one concrete row.</summary>
        public static bool TryProfile(string recipeId, out Profile profile)
        {
            switch (recipeId)
            {
                case "light_sconce_wall":
                    profile = new Profile(
                        ReactionKind.LightFlickerOut,
                        new Vector3(0.55f, 0.75f, 0.35f),
                        new Vector3(0f, 0.32f, 0f));
                    return true;
                case "light_street_pole":
                    profile = new Profile(
                        ReactionKind.LightFlickerOut,
                        new Vector3(0.34f, 3.10f, 0.70f),
                        new Vector3(0f, 1.50f, 0.25f));
                    return true;
                case "light_lantern_hang":
                    profile = new Profile(
                        ReactionKind.LightFlickerOut,
                        new Vector3(0.50f, 0.75f, 0.50f),
                        new Vector3(0f, 0.35f, 0f));
                    return true;
                case SignRecipeLibrary.HangingRecipeId:
                    profile = new Profile(
                        ReactionKind.SparkShower,
                        new Vector3(1.05f, 0.90f, 0.18f),
                        new Vector3(0f, 0.18f, 0f));
                    return true;
                case SignRecipeLibrary.WallPlateRecipeId:
                    profile = new Profile(
                        ReactionKind.SparkShower,
                        new Vector3(0.78f, 0.40f, 0.15f),
                        Vector3.zero);
                    return true;
                case SignRecipeLibrary.ChevronRecipeId:
                    profile = new Profile(
                        ReactionKind.SparkShower,
                        new Vector3(0.42f, 0.42f, 0.16f),
                        Vector3.zero);
                    return true;
                case "prop_pipe_cluster":
                    profile = new Profile(
                        ReactionKind.SteamBurst,
                        new Vector3(0.90f, 1.80f, 0.90f),
                        new Vector3(0f, 0.90f, 0f));
                    return true;
                case "prop_patched_crate":
                    profile = new Profile(
                        ReactionKind.Shatter,
                        new Vector3(0.95f, 0.85f, 0.95f),
                        new Vector3(0f, 0.42f, 0f));
                    return true;
                default:
                    profile = default;
                    return false;
            }
        }

        /// <summary>
        /// Lantern recipes live on a lifted child, while the PracticalLight and hero Light live on the
        /// practical root. All other recipes own their reaction on the ForgeModuleLook object itself.
        /// </summary>
        public static GameObject ResolveOwner(ForgeModuleLook look)
        {
            if (look == null) return null;
            PracticalLight practical = look.GetComponentInParent<PracticalLight>();
            return practical != null ? practical.gameObject : look.gameObject;
        }

        private static BoxCollider EnsureProxy(Transform look, Profile profile)
        {
            Transform existing = look.Find(HitProxyName);
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            var proxyObject = new GameObject(HitProxyName);
            proxyObject.transform.SetParent(look, false);
            proxyObject.transform.localPosition = profile.proxyCenter;
            proxyObject.transform.localRotation = Quaternion.identity;
            proxyObject.transform.localScale = Vector3.one;
            proxyObject.layer = look.gameObject.layer;
            proxyObject.isStatic = look.gameObject.isStatic;

            BoxCollider proxy = proxyObject.AddComponent<BoxCollider>();
            proxy.center = Vector3.zero;
            proxy.size = profile.proxySize;
            proxy.isTrigger = false;
            return proxy;
        }

        private static void EnsureKeepChild(ForgeModuleLook look, string childName)
        {
            var keep = new List<string>();
            if (look.keepChildren != null) keep.AddRange(look.keepChildren);
            if (!keep.Contains(childName)) keep.Add(childName);
            look.keepChildren = keep.ToArray();
        }

        private static string PathOf(Transform value)
        {
            if (value == null) return string.Empty;
            string path = value.name;
            Transform at = value.parent;
            while (at != null)
            {
                path = at.name + "/" + path;
                at = at.parent;
            }
            return path;
        }
    }
}
#endif
