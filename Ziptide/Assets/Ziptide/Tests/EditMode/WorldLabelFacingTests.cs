using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// DS-03/DS-14 (DEVICE_STABILIZATION_FORENSIC_PLAN) — THE facing contract, pinned. A TextMesh
    /// reads from its −Z side; facing a viewer means +Z points AWAY from them. The DS-03 mirror bug
    /// was exactly the intuitive-but-wrong LookRotation(toViewer); these tests make that regression
    /// a CI red instead of a headset discovery.
    /// </summary>
    public class WorldLabelFacingTests
    {
        [Test]
        public void FaceViewer_PutsViewerOnReadableSide()
        {
            var label = new Vector3(0f, 1.5f, 5f);
            var viewer = new Vector3(2f, 1.6f, -3f);

            Quaternion rot = WorldLabelFacing.FaceViewer(label, viewer);
            Assert.IsTrue(WorldLabelFacing.IsReadableFrom(rot, label, viewer),
                "FaceViewer must satisfy its own readability predicate");

            // The convention itself: +Z points AWAY from the viewer.
            Vector3 fwd = rot * Vector3.forward;
            Vector3 toViewer = (viewer - label).normalized;
            Assert.Less(Vector3.Dot(fwd, toViewer), 0f, "label +Z must point away from the viewer");
        }

        [Test]
        public void TheDs03Bug_LookRotationTowardViewer_ReadsMirrored()
        {
            // The exact formula DevWarpBoard shipped with — pinned as WRONG forever.
            var label = new Vector3(0f, 1.5f, 0f);
            var viewer = new Vector3(0f, 1.6f, -2f);
            Quaternion buggy = Quaternion.LookRotation((viewer - label).normalized, Vector3.up);

            Assert.IsFalse(WorldLabelFacing.IsReadableFrom(buggy, label, viewer),
                "pointing +Z AT the viewer shows the mirrored back face — the DS-03 device bug");
        }

        [Test]
        public void FaceViewer_YawOnly_KeepsLabelUpright()
        {
            var label = new Vector3(0f, 4f, 0f); // overhead sign
            var viewer = new Vector3(0f, 1.6f, -3f);

            Quaternion rot = WorldLabelFacing.FaceViewer(label, viewer, yawOnly: true);
            Vector3 up = rot * Vector3.up;
            Assert.AreEqual(1f, Vector3.Dot(up, Vector3.up), 1e-4f, "yaw-only must not pitch/roll");
            Assert.IsTrue(WorldLabelFacing.IsReadableFrom(rot, label, viewer));
        }

        [Test]
        public void FaceViewer_DegenerateSamePosition_IsSafeIdentity()
        {
            Assert.AreEqual(Quaternion.identity,
                WorldLabelFacing.FaceViewer(Vector3.one, Vector3.one));
            Assert.IsFalse(WorldLabelFacing.IsReadableFrom(Quaternion.identity, Vector3.one, Vector3.one),
                "zero-distance readability is meaningless — false, never NaN");
        }

        [Test]
        public void BothFaceLabels_OneAlwaysReadable_WhicheverSideIsApproached()
        {
            // The WorldTravelStation door fix: two labels, one per face, rotations derived from the
            // helper with synthetic viewers on each side (in the door's local space).
            Vector3 center = new Vector3(0f, 0.2f, 0f);
            Quaternion backRot = WorldLabelFacing.FaceViewer(center, center + Vector3.back, yawOnly: false);
            Quaternion frontRot = WorldLabelFacing.FaceViewer(center, center + Vector3.forward, yawOnly: false);

            var approachFromMinusZ = new Vector3(0.4f, 1.6f, -2f);
            var approachFromPlusZ = new Vector3(-0.3f, 1.6f, 2f);

            Assert.IsTrue(WorldLabelFacing.IsReadableFrom(backRot, center, approachFromMinusZ));
            Assert.IsTrue(WorldLabelFacing.IsReadableFrom(frontRot, center, approachFromPlusZ));

            // And each face's label is NOT readable from the wrong side — the mirror never shows
            // beside a readable twin pointing the same way.
            Assert.IsFalse(WorldLabelFacing.IsReadableFrom(backRot, center, approachFromPlusZ));
            Assert.IsFalse(WorldLabelFacing.IsReadableFrom(frontRot, center, approachFromMinusZ));
        }

        [Test]
        public void HomeHubBoardMath_AlreadySatisfiesTheContract()
        {
            // HomeHubRuntime.BuildSurface: origin = cam + fwd*2.2 + down*0.15, rotation =
            // LookRotation(origin − cam). Terry READ the hub fine on-device — this documents why
            // (its +Z already points away from the camera) and pins it against future edits.
            Vector3 cam = new Vector3(3f, 1.7f, -1f);
            Vector3 fwd = new Vector3(0.6f, 0f, 0.8f).normalized;
            Vector3 origin = cam + fwd * 2.2f + Vector3.down * 0.15f;
            Quaternion boardRot = Quaternion.LookRotation(origin - cam, Vector3.up);

            Assert.IsTrue(WorldLabelFacing.IsReadableFrom(boardRot, origin, cam),
                "the Home Hub board formula keeps the viewer on the readable side");
        }
    }
}
