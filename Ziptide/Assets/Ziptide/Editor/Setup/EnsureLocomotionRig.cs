using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Ziptide.Editor.Setup
{
    /// <summary>
    /// Ensures thumbstick locomotion rig: LocomotionSystem, ContinuousMove, SnapTurn + SmoothTurn,
    /// CharacterController, DashLocomotion, input wiring.
    /// Menu: Ziptide > Ensure Thumbstick Locomotion Rig.
    /// </summary>
    public static class EnsureLocomotionRig
    {
        private const string MenuPath = "Ziptide/Ensure Thumbstick Locomotion Rig";
        private const string XRIInputActionsGUID = "c348712bda248c246b8c49b3db54643f";
        private const string LocomotionSystemChildName = "Locomotion System";
        private const string MoveChildName = "Move";
        private const string SnapTurnChildName = "SnapTurn";
        private const string SmoothTurnChildName = "SmoothTurn";
        private const string LegacyTurnChildName = "Turn";

        private const string XROriginPrefabPath = "Assets/Samples/XR Interaction Toolkit/2.5.4/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";

        [MenuItem(MenuPath)]
        public static void Run()
        {
            GameObject xrOriginGo = FindXROrigin();
            if (xrOriginGo == null)
            {
                xrOriginGo = InstantiateXROriginFromPrefab();
            }
            if (xrOriginGo == null)
            {
                Debug.LogError("[Ziptide] Ensure Locomotion Rig: XR Origin not found and prefab missing. Import XR Interaction Toolkit Starter Assets.");
                return;
            }

            // Resolve XRI Default Input Actions
            string inputActionsPath = AssetDatabase.GUIDToAssetPath(XRIInputActionsGUID);
            if (string.IsNullOrEmpty(inputActionsPath))
            {
                Debug.LogError("[Ziptide] XRI Default Input Actions not found. Import: Package Manager → XR Interaction Toolkit → Samples → Import \"Starter Assets\".");
                return;
            }

            InputActionAsset inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputActionsPath);
            if (inputAsset == null)
            {
                Debug.LogError("[Ziptide] Could not load Input Action Asset at " + inputActionsPath + ". Import Starter Assets.");
                return;
            }

            // InputActionManager in scene
            EnsureInputActionManager(inputAsset);

            // XROrigin component (for LocomotionSystem reference)
            Component xrOriginComponent = GetXROriginComponent(xrOriginGo);
            if (xrOriginComponent == null)
            {
                Debug.LogError("[Ziptide] XR Origin GameObject has no XROrigin component. Use the official XR Origin (Action-based) prefab.");
                return;
            }

            // Locomotion structure: child "Locomotion System" with LocomotionSystem component; children Move, Turn
            Transform locomotionRoot = GetOrCreateChild(xrOriginGo.transform, LocomotionSystemChildName);
            LocomotionSystem locomotionSystem = EnsureComponent<LocomotionSystem>(locomotionRoot.gameObject);
            SerializedObject soLoc = new SerializedObject(locomotionSystem);
            soLoc.FindProperty("m_XROrigin").objectReferenceValue = xrOriginComponent;
            soLoc.ApplyModifiedPropertiesWithoutUndo();

            // Move provider (ActionBasedContinuousMoveProvider)
            Transform moveT = GetOrCreateChild(locomotionRoot, MoveChildName);
            var moveProvider = EnsureComponent<ActionBasedContinuousMoveProvider>(moveT.gameObject);
            SerializedObject soMove = new SerializedObject(moveProvider);
            soMove.FindProperty("m_System").objectReferenceValue = locomotionSystem;
            soMove.FindProperty("m_MoveSpeed").floatValue = 1.75f;
            var useGravityProp = soMove.FindProperty("m_UseGravity");
            if (useGravityProp != null) useGravityProp.boolValue = true;
            InputActionReference leftMove = FindActionReference(inputActionsPath, "XRI LeftHand Locomotion", "Move");
            InputActionReference rightMove = FindActionReference(inputActionsPath, "XRI RightHand Locomotion", "Move");
            if (leftMove != null) soMove.FindProperty("m_LeftHandMoveAction.m_Reference").objectReferenceValue = leftMove;
            if (rightMove != null) soMove.FindProperty("m_RightHandMoveAction.m_Reference").objectReferenceValue = rightMove;
            soMove.FindProperty("m_LeftHandMoveAction.m_UseReference").boolValue = true;
            soMove.FindProperty("m_RightHandMoveAction.m_UseReference").boolValue = true;
            soMove.ApplyModifiedPropertiesWithoutUndo();

            // Migrate legacy "Turn" child to "SnapTurn"
            Transform legacyTurn = locomotionRoot.Find(LegacyTurnChildName);
            if (legacyTurn != null && locomotionRoot.Find(SnapTurnChildName) == null)
                legacyTurn.name = SnapTurnChildName;

            // Snap Turn provider — RIGHT STICK ONLY for turning.
            // Left stick drives movement; assigning it to turn too creates asymmetric input conflict.
            Transform snapTurnT = GetOrCreateChild(locomotionRoot, SnapTurnChildName);
            var snapTurnProvider = EnsureComponent<ActionBasedSnapTurnProvider>(snapTurnT.gameObject);
            SerializedObject soSnap = new SerializedObject(snapTurnProvider);
            soSnap.FindProperty("m_System").objectReferenceValue = locomotionSystem;
            soSnap.FindProperty("m_TurnAmount").floatValue = 45f;
            InputActionReference rightSnap = FindActionReference(inputActionsPath, "XRI RightHand Locomotion", "Snap Turn");
            if (rightSnap != null) soSnap.FindProperty("m_RightHandSnapTurnAction.m_Reference").objectReferenceValue = rightSnap;
            soSnap.FindProperty("m_LeftHandSnapTurnAction.m_UseReference").boolValue = false;
            soSnap.FindProperty("m_RightHandSnapTurnAction.m_UseReference").boolValue = true;
            soSnap.ApplyModifiedPropertiesWithoutUndo();

            // Smooth Turn provider — RIGHT STICK ONLY for same reason.
            Transform smoothTurnT = GetOrCreateChild(locomotionRoot, SmoothTurnChildName);
            var smoothTurnProvider = EnsureComponent<ActionBasedContinuousTurnProvider>(smoothTurnT.gameObject);
            SerializedObject soSmooth = new SerializedObject(smoothTurnProvider);
            soSmooth.FindProperty("m_System").objectReferenceValue = locomotionSystem;
            soSmooth.FindProperty("m_TurnSpeed").floatValue = 120f;
            InputActionReference rightTurn = FindActionReference(inputActionsPath, "XRI RightHand Locomotion", "Snap Turn");
            if (rightTurn != null) soSmooth.FindProperty("m_RightHandTurnAction.m_Reference").objectReferenceValue = rightTurn;
            soSmooth.FindProperty("m_LeftHandTurnAction.m_UseReference").boolValue = false;
            soSmooth.FindProperty("m_RightHandTurnAction.m_UseReference").boolValue = true;
            soSmooth.ApplyModifiedPropertiesWithoutUndo();

            // DashLocomotion on XR Origin root
            EnsureComponent<Ziptide.Gameplay.DashLocomotion>(xrOriginGo);

            // CharacterController + CharacterControllerDriver on XR Origin ROOT only (ContinuousMoveProvider looks for CC on Origin)
            // Remove any stale CC/Driver from Camera Offset so gravity and bounds collision work.
            Transform cameraOffset = xrOriginGo.transform.Find("Camera Offset");
            if (cameraOffset != null)
            {
                var staleCc = cameraOffset.GetComponent<CharacterController>();
                if (staleCc != null) Object.DestroyImmediate(staleCc);
                var staleDriver = cameraOffset.GetComponent<CharacterControllerDriver>();
                if (staleDriver != null) Object.DestroyImmediate(staleDriver);
            }

            CharacterController cc = EnsureComponent<CharacterController>(xrOriginGo);
            if (cc != null)
            {
                var soCc = new SerializedObject(cc);
                if (soCc.FindProperty("m_Height").floatValue < 1f)
                    soCc.FindProperty("m_Height").floatValue = 1.36f;
                if (soCc.FindProperty("m_Radius").floatValue < 0.01f)
                    soCc.FindProperty("m_Radius").floatValue = 0.1f;
                // Step Offset must stay <= height + 2*radius, or Unity logs
                // "Step Offset must be set to a value smaller or equal to..." every frame the
                // CharacterControllerDriver shrinks the height on travel. Default is 0.5 (too big
                // when height dips). Clamp small; works for the flat walkways we have.
                soCc.FindProperty("m_StepOffset").floatValue = 0.3f;
                soCc.ApplyModifiedPropertiesWithoutUndo();
            }

            CharacterControllerDriver driver = EnsureComponent<CharacterControllerDriver>(xrOriginGo);
            if (driver != null)
            {
                SerializedObject soDriver = new SerializedObject(driver);
                soDriver.FindProperty("m_LocomotionProvider").objectReferenceValue = moveProvider;
                // Keep a minimum controller height so the driver never shrinks it below the
                // step-offset constraint (need height + 2*radius >= stepOffset). With minHeight 1.0
                // and radius 0.1: 1.0 + 0.2 = 1.2 >= 0.3 always → the step-offset error can't fire.
                soDriver.FindProperty("m_MinHeight").floatValue = 1.0f;
                soDriver.ApplyModifiedPropertiesWithoutUndo();
            }

            TuneRayInteractors(xrOriginGo);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(xrOriginGo.scene);
            Debug.Log("[Ziptide] Ensure Thumbstick Locomotion Rig: LocomotionSystem, Move, SnapTurn, SmoothTurn, DashLocomotion, CharacterController, and input actions configured.");
        }

        /// <summary>
        /// Make the controller rays behave: short, realistic grab reach (not across the map) and NO
        /// anchor control (so the thumbstick turns the player / moves, instead of rotating a held gun).
        /// Set via SerializedObject with null-safe FindProperty so a property that doesn't exist in this
        /// XRI version is simply skipped rather than breaking the build.
        /// </summary>
        private static void TuneRayInteractors(GameObject xrOriginGo)
        {
            var rays = xrOriginGo.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.XRRayInteractor>(true);
            foreach (var ray in rays)
            {
                if (ray == null) continue;
                var so = new SerializedObject(ray);

                var maxDist = so.FindProperty("m_MaxRaycastDistance");
                if (maxDist != null) maxDist.floatValue = 3f; // realistic reach, was effectively across the room

                // Stop the thumbstick from rotating/translating the held object (the "gun spins instead
                // of turning my body" bug). Disable every anchor-control flavor this version exposes.
                foreach (var prop in new[] { "m_EnableAnchorControl", "m_AnchorControl", "m_ManipulateAttachTransform" })
                {
                    var p = so.FindProperty(prop);
                    if (p != null && p.propertyType == SerializedPropertyType.Boolean) p.boolValue = false;
                }

                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static GameObject FindXROrigin()
        {
            // Prefer XROrigin component (Unity.XR.CoreUtils)
            var xrOriginType = System.Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            if (xrOriginType != null)
            {
                var xr = Object.FindObjectOfType(xrOriginType) as Component;
                if (xr != null) return xr.gameObject;
            }
            GameObject byName = GameObject.Find("XR Origin");
            if (byName != null) return byName;
            foreach (var root in Object.FindObjectsOfType<Transform>())
            {
                if (root.parent == null && root.name.Contains("XR Origin")) return root.gameObject;
            }
            var cam = Camera.main;
            if (cam != null)
            {
                Transform t = cam.transform;
                while (t != null)
                {
                    if (t.name.Contains("XR Origin") || t.name.Contains("Camera Offset"))
                    {
                        while (t.parent != null && !t.name.Contains("XR Origin")) t = t.parent;
                        return t.gameObject;
                    }
                    t = t.parent;
                }
            }
            return null;
        }

        private static Component GetXROriginComponent(GameObject go)
        {
            var t = System.Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            if (t != null) return go.GetComponent(t);
            foreach (var c in go.GetComponents<Component>())
            {
                if (c != null && c.GetType().Name == "XROrigin") return c;
            }
            return null;
        }

        private static void EnsureInputActionManager(InputActionAsset inputAsset)
        {
            var manager = Object.FindObjectOfType<InputActionManager>();
            if (manager == null)
            {
                var interactionManager = Object.FindObjectOfType<XRInteractionManager>();
                GameObject parent = interactionManager != null ? interactionManager.gameObject : new GameObject("_InputActionManager");
                if (interactionManager == null)
                {
                    Undo.RegisterCreatedObjectUndo(parent, "InputActionManager");
                    parent.name = "_InputActionManager";
                }
                manager = parent.GetComponent<InputActionManager>();
                if (manager == null) manager = parent.AddComponent<InputActionManager>();
            }

            var so = new SerializedObject(manager);
            SerializedProperty list = so.FindProperty("m_ActionAssets");
            if (list == null) list = so.FindProperty("actionAssets");
            if (list != null && list.isArray && (list.arraySize == 0 || list.GetArrayElementAtIndex(0).objectReferenceValue == null))
            {
                if (list.arraySize == 0) list.arraySize = 1;
                list.GetArrayElementAtIndex(0).objectReferenceValue = inputAsset;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static Transform GetOrCreateChild(Transform parent, string name)
        {
            Transform t = parent.Find(name);
            if (t != null) return t;
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent, false);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
            Undo.RegisterCreatedObjectUndo(child, "Locomotion Child");
            return child.transform;
        }

        private static T EnsureComponent<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            if (c == null)
            {
                c = go.AddComponent<T>();
                Undo.RegisterCreatedObjectUndo(c, "Add " + typeof(T).Name);
            }
            return c;
        }

        private static InputActionReference FindActionReference(string assetPath, string mapName, string actionName)
        {
            Object[] all = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (Object o in all)
            {
                if (o is InputActionReference refAsset && refAsset.action != null)
                {
                    if (refAsset.action.name == actionName && refAsset.action.actionMap?.name == mapName)
                        return refAsset;
                }
            }
            return null;
        }

        private static GameObject InstantiateXROriginFromPrefab()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(XROriginPrefabPath);
            if (prefab == null)
            {
                Debug.LogWarning("[Ziptide] XR Origin prefab not found at " + XROriginPrefabPath);
                return null;
            }
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (go == null) return null;
            go.name = "XR Origin";
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            Undo.RegisterCreatedObjectUndo(go, "Create XR Origin");
            Debug.Log("[Ziptide] Created XR Origin from prefab: " + XROriginPrefabPath);
            return go;
        }
    }
}
