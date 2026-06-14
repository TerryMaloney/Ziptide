using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Ziptide.Editor.Setup
{
    /// <summary>
    /// Logs locomotion rig status: XR Origin, InputActionManager, Move/Turn providers, CharacterController.
    /// Menu: Ziptide > Print Locomotion Debug.
    /// </summary>
    public static class VerifyLocomotionStatus
    {
        private const string MenuPath = "Ziptide/Print Locomotion Debug";
        private const string XRIInputActionsGUID = "c348712bda248c246b8c49b3db54643f";

        [MenuItem(MenuPath)]
        public static void Run()
        {
            // XR Origin
            GameObject xrOriginGo = FindXROrigin();
            if (xrOriginGo == null)
            {
                Debug.Log("[Ziptide Locomotion] XR Origin: NOT FOUND.");
            }
            else
            {
                Debug.Log("[Ziptide Locomotion] XR Origin: FOUND (" + xrOriginGo.name + ").");
            }

            // InputActionManager + action asset
            var inputManager = Object.FindObjectOfType<InputActionManager>();
            if (inputManager == null)
            {
                Debug.Log("[Ziptide Locomotion] InputActionManager: NOT PRESENT.");
            }
            else
            {
                int count = inputManager.actionAssets != null ? inputManager.actionAssets.Count : 0;
                bool hasXri = false;
                if (count > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(XRIInputActionsGUID);
                    foreach (var asset in inputManager.actionAssets)
                    {
                        if (asset != null && AssetDatabase.GetAssetPath(asset) == path) hasXri = true;
                    }
                }
                Debug.Log("[Ziptide Locomotion] InputActionManager: PRESENT. Action assets: " + count + (hasXri ? ", XRI Default assigned." : ", XRI Default NOT assigned."));
            }

            // Move provider
            var moveProvider = Object.FindObjectOfType<ActionBasedContinuousMoveProvider>();
            if (moveProvider == null)
            {
                Debug.Log("[Ziptide Locomotion] Move provider (ActionBasedContinuousMoveProvider): NOT PRESENT.");
            }
            else
            {
                var so = new SerializedObject(moveProvider);
                var leftRef = so.FindProperty("m_LeftHandMoveAction.m_Reference");
                var rightRef = so.FindProperty("m_RightHandMoveAction.m_Reference");
                bool leftSet = leftRef != null && leftRef.objectReferenceValue != null;
                bool rightSet = rightRef != null && rightRef.objectReferenceValue != null;
                Debug.Log("[Ziptide Locomotion] Move provider: PRESENT. Left action: " + (leftSet ? "assigned" : "NOT assigned") + ", Right: " + (rightSet ? "assigned" : "NOT assigned") + ".");
            }

            // Turn provider
            var turnProvider = Object.FindObjectOfType<ActionBasedSnapTurnProvider>();
            if (turnProvider == null)
            {
                Debug.Log("[Ziptide Locomotion] Turn provider (ActionBasedSnapTurnProvider): NOT PRESENT.");
            }
            else
            {
                var so = new SerializedObject(turnProvider);
                var leftRef = so.FindProperty("m_LeftHandSnapTurnAction.m_Reference");
                var rightRef = so.FindProperty("m_RightHandSnapTurnAction.m_Reference");
                bool leftSet = leftRef != null && leftRef.objectReferenceValue != null;
                bool rightSet = rightRef != null && rightRef.objectReferenceValue != null;
                Debug.Log("[Ziptide Locomotion] Turn provider: PRESENT. Left SnapTurn: " + (leftSet ? "assigned" : "NOT assigned") + ", Right: " + (rightSet ? "assigned" : "NOT assigned") + ".");
            }

            // CharacterController (on XR Origin or Camera Offset)
            CharacterController cc = null;
            if (xrOriginGo != null)
            {
                cc = xrOriginGo.GetComponentInChildren<CharacterController>(true);
            }
            if (cc == null) cc = Object.FindObjectOfType<CharacterController>();
            Debug.Log("[Ziptide Locomotion] CharacterController: " + (cc != null ? "PRESENT (" + cc.gameObject.name + ")." : "NOT PRESENT."));
        }

        private static GameObject FindXROrigin()
        {
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
            return null;
        }
    }
}
