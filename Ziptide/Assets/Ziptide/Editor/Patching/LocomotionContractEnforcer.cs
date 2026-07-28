#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Build-time contract for the turn stack. The Quest device failure exposed that the continuous
    /// provider had been wired to XRI's "Snap Turn" action, which naturally produced a short smooth
    /// segment followed by a periodic jerk. This enforcer repairs the binding and fails the build if
    /// smooth and snap providers share the snap action again.
    /// </summary>
    public static class LocomotionContractEnforcer
    {
        private const string XriInputActionsGuid = "c348712bda248c246b8c49b3db54643f";

        public static void EnsureCurrentScene()
        {
            ActionBasedContinuousTurnProvider smooth = UnityEngine.Object.FindObjectOfType<ActionBasedContinuousTurnProvider>(true);
            if (smooth == null) throw new InvalidOperationException("TURN_CONTRACT: smooth-turn provider missing");

            InputActionReference turnReference = FindReference("Turn");
            if (turnReference == null || turnReference.action == null)
                throw new InvalidOperationException("TURN_CONTRACT: XRI continuous Turn action reference missing");

            SerializedObject so = new SerializedObject(smooth);
            SerializedProperty rightRef = so.FindProperty("m_RightHandTurnAction.m_Reference");
            SerializedProperty rightUse = so.FindProperty("m_RightHandTurnAction.m_UseReference");
            SerializedProperty leftUse = so.FindProperty("m_LeftHandTurnAction.m_UseReference");
            if (rightRef == null || rightUse == null || leftUse == null)
                throw new InvalidOperationException("TURN_CONTRACT: XRI serialized turn fields changed");

            rightRef.objectReferenceValue = turnReference;
            rightUse.boolValue = true;
            leftUse.boolValue = false;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(smooth);

            ValidateCurrentSceneOrThrow();
            Debug.Log("ZIPTIDE: TURN_CONTRACT_OK smoothAction=" + turnReference.action.name
                + " map=" + (turnReference.action.actionMap != null ? turnReference.action.actionMap.name : "NONE"));
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

            SerializedObject smoothSo = new SerializedObject(smoothProviders[0]);
            InputActionReference smoothRef = smoothSo.FindProperty("m_RightHandTurnAction.m_Reference")?.objectReferenceValue
                as InputActionReference;
            if (smoothRef == null || smoothRef.action == null || smoothRef.action.name != "Turn")
                throw new InvalidOperationException("TURN_CONTRACT: continuous provider must use action 'Turn', found '"
                    + (smoothRef != null && smoothRef.action != null ? smoothRef.action.name : "NONE") + "'");

            SerializedObject snapSo = new SerializedObject(snapProviders[0]);
            InputActionReference snapRef = snapSo.FindProperty("m_RightHandSnapTurnAction.m_Reference")?.objectReferenceValue
                as InputActionReference;
            if (snapRef == null || snapRef.action == null || snapRef.action.name != "Snap Turn")
                throw new InvalidOperationException("TURN_CONTRACT: snap provider must use action 'Snap Turn', found '"
                    + (snapRef != null && snapRef.action != null ? snapRef.action.name : "NONE") + "'");

            if (ReferenceEquals(smoothRef, snapRef))
                throw new InvalidOperationException("TURN_CONTRACT: smooth and snap providers share one InputActionReference");
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
