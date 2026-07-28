#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Build-time contract for the turn stack. Smooth and snap providers may both exist for a comfort
    /// setting, but only the selected mode may be enabled. The Golden route defaults to continuous turn.
    /// </summary>
    public static class LocomotionContractEnforcer
    {
        private const string XriInputActionsGuid = "c348712bda248c246b8c49b3db54643f";

        public static void EnsureCurrentScene()
        {
            ActionBasedContinuousTurnProvider smooth =
                UnityEngine.Object.FindObjectOfType<ActionBasedContinuousTurnProvider>(true);
            ActionBasedSnapTurnProvider snap =
                UnityEngine.Object.FindObjectOfType<ActionBasedSnapTurnProvider>(true);
            if (smooth == null) throw new InvalidOperationException("TURN_CONTRACT: smooth-turn provider missing");
            if (snap == null) throw new InvalidOperationException("TURN_CONTRACT: snap-turn provider missing");

            InputActionReference turnReference = FindReference("Turn");
            if (turnReference == null || turnReference.action == null)
                throw new InvalidOperationException("TURN_CONTRACT: XRI continuous Turn action reference missing");

            SerializedObject so = new SerializedObject(smooth);
            SerializedProperty rightRef = so.FindProperty("m_RightHandTurnAction.m_Reference");
            SerializedProperty rightUse = so.FindProperty("m_RightHandTurnAction.m_UseReference");
            SerializedProperty leftUse = so.FindProperty("m_LeftHandTurnAction.m_UseReference");
            SerializedProperty speed = so.FindProperty("m_TurnSpeed");
            if (rightRef == null || rightUse == null || leftUse == null || speed == null)
                throw new InvalidOperationException("TURN_CONTRACT: XRI serialized smooth-turn fields changed");

            rightRef.objectReferenceValue = turnReference;
            rightUse.boolValue = true;
            leftUse.boolValue = false;
            speed.floatValue = TurnModeCore.DefaultSmoothTurnSpeed;
            so.ApplyModifiedPropertiesWithoutUndo();

            smooth.enabled = true;
            snap.enabled = false;
            EditorUtility.SetDirty(smooth);
            EditorUtility.SetDirty(snap);

            ValidateCurrentSceneOrThrow();
            Debug.Log("ZIPTIDE: TURN_CONTRACT_OK mode=smooth snapEnabled=false action="
                + turnReference.action.name + " speed=" + smooth.turnSpeed.ToString("0")
                + " map=" + (turnReference.action.actionMap != null
                    ? turnReference.action.actionMap.name : "NONE"));
        }

        public static void ValidateCurrentSceneOrThrow()
        {
            ActionBasedContinuousTurnProvider[] smoothProviders =
                UnityEngine.Object.FindObjectsOfType<ActionBasedContinuousTurnProvider>(true);
            ActionBasedSnapTurnProvider[] snapProviders =
                UnityEngine.Object.FindObjectsOfType<ActionBasedSnapTurnProvider>(true);

            if (smoothProviders.Length != 1)
                throw new InvalidOperationException("TURN_CONTRACT: expected exactly one smooth provider, found " + smoothProviders.Length);
            if (snapProviders.Length != 1)
                throw new InvalidOperationException("TURN_CONTRACT: expected exactly one snap provider, found " + snapProviders.Length);

            ActionBasedContinuousTurnProvider smooth = smoothProviders[0];
            ActionBasedSnapTurnProvider snap = snapProviders[0];
            SerializedObject smoothSo = new SerializedObject(smooth);
            InputActionReference smoothRef = smoothSo.FindProperty("m_RightHandTurnAction.m_Reference")?.objectReferenceValue
                as InputActionReference;
            if (smoothRef == null || smoothRef.action == null || smoothRef.action.name != "Turn")
                throw new InvalidOperationException("TURN_CONTRACT: continuous provider must use action 'Turn', found '"
                    + (smoothRef != null && smoothRef.action != null ? smoothRef.action.name : "NONE") + "'");
            if (smoothRef.action.type != InputActionType.Value
                || !string.Equals(smoothRef.action.expectedControlType, "Vector2", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("TURN_CONTRACT: continuous Turn must be Value/Vector2");

            SerializedObject snapSo = new SerializedObject(snap);
            InputActionReference snapRef = snapSo.FindProperty("m_RightHandSnapTurnAction.m_Reference")?.objectReferenceValue
                as InputActionReference;
            if (snapRef == null || snapRef.action == null || snapRef.action.name != "Snap Turn")
                throw new InvalidOperationException("TURN_CONTRACT: snap provider must use action 'Snap Turn', found '"
                    + (snapRef != null && snapRef.action != null ? snapRef.action.name : "NONE") + "'");

            if (ReferenceEquals(smoothRef, snapRef))
                throw new InvalidOperationException("TURN_CONTRACT: smooth and snap providers share one InputActionReference");
            if (!smooth.enabled || snap.enabled)
                throw new InvalidOperationException("TURN_CONTRACT: Golden route requires smooth enabled and snap disabled");
            if (Mathf.Abs(smooth.turnSpeed - TurnModeCore.DefaultSmoothTurnSpeed) > 0.01f)
                throw new InvalidOperationException("TURN_CONTRACT: unexpected smooth speed " + smooth.turnSpeed);
        }

        private static InputActionReference FindReference(string actionName)
        {
            string path = AssetDatabase.GUIDToAssetPath(XriInputActionsGuid);
            if (string.IsNullOrEmpty(path)) return null;
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            for (int i = 0; i < assets.Length; i++)
            {
                InputActionReference reference = assets[i] as InputActionReference;
                if (reference == null || reference.action == null) continue;
                if (reference.action.name != actionName) continue;
                if (reference.action.actionMap != null
                    && reference.action.actionMap.name.IndexOf("RightHand Locomotion", StringComparison.OrdinalIgnoreCase) >= 0)
                    return reference;
            }
            for (int i = 0; i < assets.Length; i++)
            {
                InputActionReference reference = assets[i] as InputActionReference;
                if (reference != null && reference.action != null && reference.action.name == actionName)
                    return reference;
            }
            return null;
        }
    }
}
#endif
