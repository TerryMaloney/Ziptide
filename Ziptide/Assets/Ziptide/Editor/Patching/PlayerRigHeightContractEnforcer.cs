#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Enforces the Quest standing/seated/child height contract at the XR Origin itself. Floor tracking
    /// supplies the real headset height; the prefab camera Y offset must be zero or it stacks a synthetic
    /// adult offset on top of Quest tracking and elevates the player in every world.
    /// </summary>
    public static class PlayerRigHeightContractEnforcer
    {
        private const int FloorTrackingOriginMode = 2;

        public static void EnsureCurrentScene()
        {
            Type xrOriginType = Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            Component origin = xrOriginType != null
                ? UnityEngine.Object.FindObjectOfType(xrOriginType, true) as Component : null;
            if (origin == null) throw new InvalidOperationException("RIG_HEIGHT_CONTRACT: XROrigin missing");

            SerializedObject so = new SerializedObject(origin);
            SerializedProperty mode = so.FindProperty("m_RequestedTrackingOriginMode");
            SerializedProperty offset = so.FindProperty("m_CameraYOffset");
            if (mode == null || offset == null)
                throw new InvalidOperationException("RIG_HEIGHT_CONTRACT: XROrigin serialized fields changed");

            mode.intValue = FloorTrackingOriginMode;
            offset.floatValue = 0f;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(origin);
            ValidateCurrentSceneOrThrow();
            Debug.Log("ZIPTIDE: RIG_HEIGHT_CONTRACT_OK tracking=Floor cameraYOffset=0");
        }

        public static void ValidateCurrentSceneOrThrow()
        {
            Type xrOriginType = Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            Component origin = xrOriginType != null
                ? UnityEngine.Object.FindObjectOfType(xrOriginType, true) as Component : null;
            if (origin == null) throw new InvalidOperationException("RIG_HEIGHT_CONTRACT: XROrigin missing");

            SerializedObject so = new SerializedObject(origin);
            SerializedProperty mode = so.FindProperty("m_RequestedTrackingOriginMode");
            SerializedProperty offset = so.FindProperty("m_CameraYOffset");
            if (mode == null || offset == null
                || mode.intValue != FloorTrackingOriginMode || Mathf.Abs(offset.floatValue) > 0.001f)
                throw new InvalidOperationException("RIG_HEIGHT_CONTRACT: expected Floor tracking and cameraYOffset=0");
        }
    }
}
#endif
