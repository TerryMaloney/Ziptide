using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Runtime factory for creating items from their itemId.
    /// Mirrors ScenePatcherC0's item creation but works at runtime without Editor APIs.
    /// </summary>
    public static class ItemFactory
    {
        public static GameObject Create(string itemId, Vector3 position)
        {
            if (string.IsNullOrEmpty(itemId)) return null;

            var def = FindDefinition(itemId);
            if (def == null)
            {
                Debug.LogWarning("ZIPTIDE: ITEM_DEF_NOT_FOUND id=" + itemId + " known=[" + KnownIds() + "]");
                return null;
            }

            if (def is PistolDefinition pistolDef)
                return CreatePistol(pistolDef, position);

            if (def is TaserDartGunDefinition taserDef)
                return CreateTaserDartGun(taserDef, position);

            if (def is GravityGunDefinition gravDef)
                return CreateGravityGun(gravDef, position);

            return CreateGenericItem(def, position);
        }

        // Holds a static reference to every ItemDefinition we ever resolve. This keeps the
        // ScriptableObject loaded (prevents GC unload) so inventory restore after scene travel
        // can still find it even when the destination scene doesn't reference it directly
        // (fixes ITEM_DEF_NOT_FOUND on travel-restore).
        private static readonly System.Collections.Generic.Dictionary<string, ItemDefinition> _cache
            = new System.Collections.Generic.Dictionary<string, ItemDefinition>();

        private static bool _resourcesPreloaded;

        private static ItemDefinition FindDefinition(string itemId)
        {
            if (_cache.TryGetValue(itemId, out var cached) && cached != null)
                return cached;

            // CANONICAL PATH — every ItemDefinition asset lives under Resources/Items (enforced by the
            // ItemRegistryConventionTests CI guard). Resources assets are loadable at runtime in ANY
            // scene and survive IL2CPP builds, which fixes ITEM_DEF_NOT_FOUND when restoring a
            // holstered item after the scene it came from has unloaded.
            if (!_resourcesPreloaded)
            {
                _resourcesPreloaded = true;
                foreach (var d in Resources.LoadAll<ItemDefinition>("Items"))
                    if (d != null && !string.IsNullOrEmpty(d.itemId))
                        _cache[d.itemId] = d;
                Debug.Log("ZIPTIDE: ITEM_REGISTRY loaded=" + _cache.Count + " ids=[" + KnownIds() + "]");
                if (_cache.TryGetValue(itemId, out var fromRes) && fromRes != null)
                    return fromRes;
            }

            // LAST-RESORT fallback — only sees definitions something already loaded (a scene reference).
            // Unreliable on device: nothing keeps them loaded after travel. If THIS is what resolves the
            // id, the asset is misplaced — warn so it gets moved under Resources/Items.
            var allDefs = Resources.FindObjectsOfTypeAll<ItemDefinition>();
            foreach (var d in allDefs)
            {
                if (d != null && !string.IsNullOrEmpty(d.itemId))
                    _cache[d.itemId] = d;
            }

            if (_cache.TryGetValue(itemId, out var found) && found != null)
            {
                Debug.LogWarning("ZIPTIDE: ITEM_DEF_OUTSIDE_RESOURCES id=" + itemId +
                                 " (resolved only via the loaded-objects scan — move the asset under a Resources/Items folder or it will fail after travel on device)");
                return found;
            }
            return null;
        }

        private static string KnownIds()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var kv in _cache)
            {
                if (sb.Length > 0) sb.Append(',');
                sb.Append(kv.Key);
            }
            return sb.ToString();
        }

        private static GameObject CreatePistol(PistolDefinition def, Vector3 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Pistol";
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.08f, 0.04f, 0.2f);

            ApplyURPColor(go, new Color(0.3f, 0.3f, 0.35f));

            var rb = go.AddComponent<Rigidbody>();
            rb.mass = def.mass > 0 ? def.mass : 0.3f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            var grab = go.AddComponent<XRGrabInteractable>();
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            grab.useDynamicAttach = false; // snap to the Grip attach transform so the gun faces forward on grab
            grab.attachEaseInTime = 0f;    // instant snap to the grip, no slow drift toward the hand
            grab.trackPosition = true;
            grab.trackRotation = true;
            grab.retainTransformParent = false;

            var grip = new GameObject("Grip");
            grip.transform.SetParent(go.transform, false);
            grip.transform.localPosition = new Vector3(0f, -0.01f, -0.05f);
            grab.attachTransform = grip.transform;

            var itemRt = go.AddComponent<ItemRuntime>();
            itemRt.Init(def);

            var pistolRt = go.AddComponent<PistolRuntime>();
            pistolRt.Init(def);

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(go.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0f, 0.12f);

            RestorePhysicsOnRelease(go, grab);
            return go;
        }

        private static GameObject CreateTaserDartGun(TaserDartGunDefinition def, Vector3 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "TaserDartGun";
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.07f, 0.06f, 0.25f);

            ApplyURPColor(go, new Color(0.2f, 0.55f, 0.6f));

            var rb = go.AddComponent<Rigidbody>();
            rb.mass = def.mass > 0 ? def.mass : 0.4f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            var grab = go.AddComponent<XRGrabInteractable>();
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            grab.useDynamicAttach = false; // snap to the Grip attach transform so the gun faces forward on grab
            grab.attachEaseInTime = 0f;    // instant snap to the grip, no slow drift toward the hand
            grab.trackPosition = true;
            grab.trackRotation = true;
            grab.retainTransformParent = false;

            var grip = new GameObject("Grip");
            grip.transform.SetParent(go.transform, false);
            grip.transform.localPosition = new Vector3(0f, -0.01f, -0.06f);
            grab.attachTransform = grip.transform;

            var itemRt = go.AddComponent<ItemRuntime>();
            itemRt.Init(def);

            go.AddComponent<TaserDartGunRuntime>();

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(go.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0f, 0.14f);

            RestorePhysicsOnRelease(go, grab);
            return go;
        }

        private static GameObject CreateGravityGun(GravityGunDefinition def, Vector3 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "GravityGun";
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.08f, 0.07f, 0.22f);

            ApplyURPColor(go, new Color(0.45f, 0.3f, 0.7f)); // violet — distinct from the taser

            var rb = go.AddComponent<Rigidbody>();
            rb.mass = def.mass > 0 ? def.mass : 0.45f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            var grab = go.AddComponent<XRGrabInteractable>();
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            grab.useDynamicAttach = false; // snap to the Grip attach transform so it faces forward on grab
            grab.attachEaseInTime = 0f;
            grab.trackPosition = true;
            grab.trackRotation = true;
            grab.retainTransformParent = false;

            var grip = new GameObject("Grip");
            grip.transform.SetParent(go.transform, false);
            grip.transform.localPosition = new Vector3(0f, -0.01f, -0.06f);
            grab.attachTransform = grip.transform;

            var itemRt = go.AddComponent<ItemRuntime>();
            itemRt.Init(def);

            go.AddComponent<GravityGunRuntime>();

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(go.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0f, 0.15f);

            RestorePhysicsOnRelease(go, grab);
            return go;
        }

        /// <summary>
        /// On release, force the gun back to a falling rigidbody. A gun pulled out of a holster (which
        /// sets isKinematic=true for transport) is otherwise "restored" by XRGrab to kinematic on
        /// release and FLOATS in mid-air instead of dropping. This listener runs after XRGrab's restore,
        /// so it wins. Safe for holstering: the socket re-sets kinematic right after it grabs the gun.
        /// </summary>
        private static void RestorePhysicsOnRelease(GameObject go, XRGrabInteractable grab)
        {
            if (grab == null) return;
            grab.selectExited.AddListener(_ =>
            {
                var rb = go != null ? go.GetComponent<Rigidbody>() : null;
                if (rb == null) return;
                rb.isKinematic = false;
                rb.useGravity = true;
            });
        }

        private static GameObject CreateGenericItem(ItemDefinition def, Vector3 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = def.itemId;
            go.transform.position = position;
            go.transform.localScale = Vector3.one * 0.1f;

            var rb = go.AddComponent<Rigidbody>();
            rb.mass = def.mass > 0 ? def.mass : 0.5f;
            go.AddComponent<XRGrabInteractable>();

            var itemRt = go.AddComponent<ItemRuntime>();
            itemRt.Init(def);

            return go;
        }

        public static void ApplyURPColor(GameObject go, Color color)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
