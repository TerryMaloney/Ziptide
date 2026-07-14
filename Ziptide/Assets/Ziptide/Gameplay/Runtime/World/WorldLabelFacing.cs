using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// DS-03/DS-14 (DEVICE_STABILIZATION_FORENSIC_PLAN) — THE ONE world-label facing contract.
    ///
    /// The device bug class: runtime <see cref="TextMesh"/> labels (travel doors, the dev warp
    /// board) were rotated with ad-hoc billboard formulas and read MIRRORED on the headset. The
    /// root convention, pinned here once: a TextMesh's glyphs are READABLE from the transform's
    /// <b>−Z side</b> (its font material culls nothing, so the +Z side shows the mirror image).
    /// Therefore facing a label at a viewer means pointing its <b>+Z AWAY from the viewer</b> —
    /// the exact opposite of the intuitive "LookRotation(toViewer)" that caused DS-03.
    ///
    /// Every runtime world-space label rotation must come from <see cref="FaceViewer"/>, and
    /// readability questions are answered by <see cref="IsReadableFrom"/> — no axis flips anywhere
    /// else. Tests pin both against the convention.
    /// </summary>
    public static class WorldLabelFacing
    {
        /// <summary>
        /// Rotation that makes a label at <paramref name="labelPos"/> readable from
        /// <paramref name="viewerPos"/>: +Z points from the viewer THROUGH the label (viewer ends
        /// up on the readable −Z side). yawOnly keeps the label upright (world-space signage);
        /// false allows pitch (labels far above/below eye height).
        /// </summary>
        public static Quaternion FaceViewer(Vector3 labelPos, Vector3 viewerPos, bool yawOnly = true)
        {
            Vector3 away = labelPos - viewerPos; // +Z away from the viewer
            if (yawOnly) away.y = 0f;
            if (away.sqrMagnitude < 1e-6f) return Quaternion.identity;
            return Quaternion.LookRotation(away.normalized, Vector3.up);
        }

        /// <summary>
        /// True when a label with the given rotation reads correctly (not mirrored) from
        /// <paramref name="viewerPos"/> — i.e. the viewer is on the label's −Z side.
        /// </summary>
        public static bool IsReadableFrom(Quaternion labelRotation, Vector3 labelPos, Vector3 viewerPos)
        {
            Vector3 toViewer = viewerPos - labelPos;
            if (toViewer.sqrMagnitude < 1e-6f) return false;
            return Vector3.Dot(labelRotation * Vector3.forward, toViewer.normalized) < 0f;
        }
    }
}
