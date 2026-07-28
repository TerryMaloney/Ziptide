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
            Component origin = FindOrigin();
            SerializedObject so = new SerializedObject(origin);
            SerializedProperty mode = so.FindProperty("m_RequestedTrackingOriginMode");
            SerializedProperty offset = so.FindProperty("m_CameraYOffset");
            if (mode == null || offset == null)
                throw new InvalidOperationException("RIG_HEIGHT_CONTRACT: XROrigin serialized fields changed");

            mode.intValue = FloorTrackingOriginMode;
            offset.floatValue = 0f;
            so.ApplyModifiedPropertiesWithoutUndo();

            Transform cameraOffset = FindCameraOffset(origin.transform);
            Vector3 local = cameraOffset.localPosition;
            local.y = 0f;
            cameraOffset.localPosition = local;

            EditorUtility.SetDirty(origin);
            EditorUtility.SetDirty(cameraOffset);
            ValidateCurrentSceneOrThrow();
            Debug.Log("ZIPTIDE: RIG_HEIGHT_CONTRACT_OK tracking=Floor cameraYOffset=0 transformY=0");
        }

        public static void ValidateCurrentSceneOrThrow()
        {
            Component origin = FindOrigin();
            SerializedObject so = new SerializedObject(origin);
            SerializedProperty mode = so.FindProperty("m_RequestedTrackingOriginMode");
            SerializedProperty offset = so.FindProperty("m_CameraYOffset");
            Transform cameraOffset = FindCameraOffset(origin.transform);

            if (mode == null || offset == null
                || mode.intValue != FloorTrackingOriginMode
                || Mathf.Abs(offset.floatValue) > 0.001f
                || Mathf.Abs(cameraOffset.localPosition.y) > 0.001f)
            {
                throw new InvalidOperationException(
                    "RIG_HEIGHT_CONTRACT: expected Floor tracking, cameraYOffset=0 and Camera Offset transform Y=0");
            }
        }

        private static Component FindOrigin()
        {
            Type xrOriginType = Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            Component origin = xrOriginType != null
                ? UnityEngine.Object.FindObjectOfType(xrOriginType, true) as Component : null;
            if (origin == null) throw new InvalidOperationException("RIG_HEIGHT_CONTRACT: XROrigin missing");
            return origin;
        }

        private static Transform FindCameraOffset(Transform origin)
        {
            Transform cameraOffset = origin.Find("Camera Offset");
            if (cameraOffset == null)
            {
                Transform[] all = origin.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < all.Length; i++)
                    if (all[i] != null && all[i].name == "Camera Offset") { cameraOffset = all[i]; break; }
            }
            if (cameraOffset == null)
                throw new InvalidOperationException("RIG_HEIGHT_CONTRACT: Camera Offset transform missing");
            return cameraOffset;
        }
    }
}
#endif
