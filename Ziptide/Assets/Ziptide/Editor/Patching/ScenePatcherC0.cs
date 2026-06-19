using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Idempotent scene patcher for C0 (belt, pistol, targets). Ensures required objects/components exist without removing or renaming existing gameplay objects.
    /// Call from menu (Ziptide > Apply C0 To Current Scene) or from BuildAndroid.PatchScenesThenAPK in batchmode.
    /// </summary>
    public static class ScenePatcherC0
    {
        private const string MenuPath = "Ziptide/Apply C0 To Current Scene";

        [MenuItem(MenuPath)]
        public static void PatchActiveSceneFromMenu()
        {
            PatchActiveScene();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[Ziptide] C0 applied to current scene. Save scene and build.");
        }

        /// <summary>
        /// Patches the currently open active scene. Safe to call from batchmode.
        /// </summary>
        public static void PatchActiveScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded) return;
            PatchScene(scene);
        }

        /// <summary>
        /// Patches the given scene. Ensures existing B/B.1 setup then C0 (belt, holster, pistol, targets).
        /// World scenes (non-_Boot): content only — add travel station to test room, no XR rig.
        /// </summary>
        public static void PatchScene(UnityEngine.SceneManagement.Scene scene)
        {
            // World scenes: no rig. Only add content that does not require XR Origin.
            if (scene.name != "_Boot")
            {
                if (scene.name == "MilestoneA_GrabCube")
                    EnsureWorldTravelStationToD0();
                return;
            }

            // _Boot (patched by ScenePatcherBoot; C0 may not run on it in build loop): full rig + C0 objects.
            Ziptide.Editor.Setup.ApplyWorldProfileToCurrentScene.ApplyWorldProfile();

            GameObject xrOrigin = FindXROrigin();
            if (xrOrigin != null)
                Ziptide.Editor.Setup.EnsureLocomotionRig.Run();

            PatchC0Objects();
        }

        private static void PatchC0Objects()
        {
            GameObject xrOrigin = FindXROrigin();
            if (xrOrigin == null) return;

            var beltRigType = typeof(Ziptide.Gameplay.BeltRig);
            var beltRig = xrOrigin.GetComponentInChildren(beltRigType, true) as MonoBehaviour;
            if (beltRig == null)
            {
                GameObject beltGo = new GameObject("BeltRig");
                beltGo.transform.SetParent(xrOrigin.transform, false);
                beltGo.transform.localPosition = Vector3.zero;
                beltGo.transform.localRotation = Quaternion.identity;
                beltGo.transform.localScale = Vector3.one;
                beltRig = beltGo.AddComponent(beltRigType) as MonoBehaviour;
                if (beltRig != null) Undo.RegisterCreatedObjectUndo(beltGo, "BeltRig");
            }

            if (beltRig != null)
            {
                var leftPos  = new Vector3(-0.30f, 0f, -0.02f);
                var centerPos = new Vector3(0f, 0f, 0.09f);
                var rightPos = new Vector3(0.30f, 0f, -0.02f);

                var soBelt = new SerializedObject(beltRig);
                PatcherUtil.SetFloat(soBelt, "hipHeightOffset", -0.65f);
                PatcherUtil.SetVector3(soBelt, "rightHipLocalOffset", rightPos);
                PatcherUtil.SetVector3(soBelt, "leftHipLocalOffset", leftPos);
                PatcherUtil.SetVector3(soBelt, "centerHipLocalOffset", centerPos);
                soBelt.ApplyModifiedPropertiesWithoutUndo();

                EnsureOneHolsterSocket(beltRig, "HolsterLeft", leftPos);
                EnsureOneHolsterSocket(beltRig, "HolsterCenter", centerPos);
                EnsureOneHolsterSocket(beltRig, "HolsterRight", rightPos);
            }
            EnsurePistolSpawn();
            EnsureTargets();
            EnsureWorldTravelStationToD0();
        }

        private static void EnsureWorldTravelStationToD0()
        {
            var d0Pack = ScenePatcherD0.EnsureD0WorldPackAsset();
            if (d0Pack == null) return;

            var go = PatcherUtil.EnsureRootObject("WorldTravelStation", new Vector3(-1.5f, 1.3f, 2.5f));
            var station = PatcherUtil.EnsureComponent<WorldTravelStation>(go);

            var so = new SerializedObject(station);
            var listProp = so.FindProperty("destinationPacks");
            if (listProp != null)
            {
                listProp.arraySize = 1;
                listProp.GetArrayElementAtIndex(0).objectReferenceValue = d0Pack;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // Failsafe: walk-through trigger so travel works even if rays break.
            EnsureProximityTrigger(go, d0Pack.sceneName,
                new Vector3(0f, 1.1f, 0f), new Vector3(1.2f, 2.4f, 0.6f));
        }

        private static void EnsureProximityTrigger(GameObject parent, string destinationScene, Vector3 localOffset, Vector3 size)
        {
            const string triggerName = "__ProximityTravelTrigger";
            var existing = parent.transform.Find(triggerName);
            GameObject triggerGo;
            if (existing != null)
            {
                triggerGo = existing.gameObject;
            }
            else
            {
                triggerGo = new GameObject(triggerName);
                triggerGo.transform.SetParent(parent.transform, false);
                Undo.RegisterCreatedObjectUndo(triggerGo, triggerName);
            }
            triggerGo.transform.localPosition = localOffset;

            var col = PatcherUtil.EnsureComponent<BoxCollider>(triggerGo);
            col.size = size;
            col.isTrigger = true;

            var trigger = PatcherUtil.EnsureComponent<Ziptide.Gameplay.ProximityTravelTrigger>(triggerGo);
            var soTrigger = new SerializedObject(trigger);
            PatcherUtil.SetString(soTrigger, "destinationSceneName", destinationScene);
            PatcherUtil.SetFloat(soTrigger, "cooldownSeconds", 3f);
            soTrigger.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureOneHolsterSocket(MonoBehaviour beltRig, string socketName, Vector3 localPos)
        {
            if (beltRig == null) return;
            var socketType = typeof(Ziptide.Gameplay.HolsterSocketInteractor);

            Transform hip = null;
            foreach (Transform t in beltRig.transform)
            {
                if (t.name == socketName) { hip = t; break; }
            }
            if (hip == null)
            {
                var hipGo = new GameObject(socketName);
                hipGo.transform.SetParent(beltRig.transform, false);
                hipGo.transform.localRotation = Quaternion.identity;
                hipGo.transform.localScale = Vector3.one;
                hip = hipGo.transform;
                Undo.RegisterCreatedObjectUndo(hipGo, socketName);
            }

            hip.localPosition = localPos;

            if (hip.GetComponent(socketType) == null)
            {
                var socket = hip.gameObject.AddComponent(socketType);
                var attach = hip.Find("Attach");
                if (attach == null)
                {
                    var attachGo = new GameObject("Attach");
                    attachGo.transform.SetParent(hip, false);
                    attachGo.transform.localPosition = Vector3.zero;
                    attachGo.transform.localRotation = Quaternion.identity;
                    attach = attachGo.transform;
                }
                var soSocket = new SerializedObject(socket);
                var attachProp = soSocket.FindProperty("m_AttachTransform");
                if (attachProp != null) attachProp.objectReferenceValue = attach;
                soSocket.ApplyModifiedPropertiesWithoutUndo();
            }

            var col = hip.GetComponent<SphereCollider>();
            if (col == null) col = hip.gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.18f;
        }

        private static void EnsurePistolSpawn()
        {
            var existing = Object.FindObjectOfType<Ziptide.Gameplay.PistolRuntime>();
            if (existing != null)
            {
                // Safety: if the pistol fell out of the world, bring it back near spawn.
                if (existing.transform.position.y < -5f)
                    existing.transform.position = new Vector3(0f, 1f, 1.2f);
                return;
            }

            const string assetPath = "Assets/Ziptide/Resources/Items/DefaultPistol.asset";
            var def = AssetDatabase.LoadAssetAtPath<Ziptide.Content.PistolDefinition>(assetPath);
            if (def == null)
            {
                EnsureDefaultPistolAsset(assetPath);
                def = AssetDatabase.LoadAssetAtPath<Ziptide.Content.PistolDefinition>(assetPath);
            }
            if (def == null) return;

            GameObject pistol = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pistol.name = "Pistol";
            pistol.transform.position = new Vector3(0f, 1f, 1.2f);
            pistol.transform.localScale = new Vector3(0.08f, 0.04f, 0.2f);
            var rb = pistol.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            var grab = pistol.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            var grip = new GameObject("Grip");
            grip.transform.SetParent(pistol.transform, false);
            grip.transform.localPosition = new Vector3(0f, -0.01f, -0.05f);
            grip.transform.localRotation = Quaternion.identity;

            var soGrab = new SerializedObject(grab);
            var movementTypeProp = soGrab.FindProperty("m_MovementType");
            if (movementTypeProp != null) movementTypeProp.enumValueIndex = 1; // VelocityTracking = 1
            var attachProp = soGrab.FindProperty("m_AttachTransform");
            if (attachProp != null) attachProp.objectReferenceValue = grip.transform;
            var useDynamicAttachProp = soGrab.FindProperty("m_UseDynamicAttach");
            if (useDynamicAttachProp != null) useDynamicAttachProp.boolValue = true;
            soGrab.ApplyModifiedPropertiesWithoutUndo();

            var itemRt = pistol.AddComponent<Ziptide.Gameplay.ItemRuntime>();
            var soItem = new SerializedObject(itemRt);
            soItem.FindProperty("definition").objectReferenceValue = def;
            soItem.ApplyModifiedPropertiesWithoutUndo();
            var pistolRt = pistol.AddComponent<Ziptide.Gameplay.PistolRuntime>();
            var soPistol = new SerializedObject(pistolRt);
            soPistol.FindProperty("pistolDefinition").objectReferenceValue = def;
            soPistol.ApplyModifiedPropertiesWithoutUndo();
            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(pistol.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0f, 0.12f);
            muzzle.transform.localRotation = Quaternion.identity;
            Undo.RegisterCreatedObjectUndo(pistol, "Pistol");
        }

        private static void EnsureDefaultPistolAsset(string assetPath)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Resources"))
                AssetDatabase.CreateFolder("Assets/Ziptide", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Resources/Items"))
                AssetDatabase.CreateFolder("Assets/Ziptide/Resources", "Items");
            var def = ScriptableObject.CreateInstance<Ziptide.Content.PistolDefinition>();
            def.itemId = "pistol";
            def.mass = 0.3f;
            def.fireRate = 0.2f;
            def.range = 50f;
            def.hitForce = 5f;
            def.recoilKick = 0.02f;
            def.hapticAmplitude = 0.5f;
            def.hapticDuration = 0.05f;
            AssetDatabase.CreateAsset(def, assetPath);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureTargets()
        {
            int count = 0;
            foreach (var t in Object.FindObjectsOfType<Ziptide.Gameplay.TargetRuntime>())
                count++;
            if (count >= 3) return;

            float startX = -0.5f;
            for (int i = count; i < 3; i++)
            {
                float x = startX + i * 0.5f;
                GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
                target.name = "Target" + (i + 1);
                target.transform.position = new Vector3(x, 1.2f, 3f);
                target.transform.localScale = new Vector3(0.3f, 0.3f, 0.05f);
                target.AddComponent<Ziptide.Gameplay.TargetRuntime>();
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader != null)
                {
                    var mat = new Material(shader);
                    mat.color = Color.red;
                    target.GetComponent<Renderer>().material = mat;
                }
                Undo.RegisterCreatedObjectUndo(target, "Target");
            }
        }

        private static GameObject FindXROrigin()
        {
            var t = System.Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            if (t != null)
            {
                var xr = Object.FindObjectOfType(t) as Component;
                if (xr != null) return xr.gameObject;
            }
            return GameObject.Find("XR Origin");
        }
    }
}
