using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// ⚖ Terry: the sword lives on the LEFT hip so it is never in the way; everything else shifts
    /// right. Pinned here because on device you cannot tell "the rule worked" from "the blade
    /// happened to be nearer the left socket that time".
    /// </summary>
    public class HolsterAssignmentCoreTests
    {
        [Test]
        public void TheBladeGoesLeft()
        {
            Assert.AreEqual(HolsterAssignmentCore.LeftSocket,
                HolsterAssignmentCore.PreferredSocket("breaker_blade"));
            Assert.AreEqual(HolsterAssignmentCore.LeftSocket,
                HolsterAssignmentCore.PreferredSocket("tide_pike"));
        }

        [Test]
        public void EverythingElseGoesRight()
        {
            foreach (string id in new[] { "pistol", "taser_dart_gun", "gravity_gun",
                                          "static_net", "sonic_thumper", "prism_beam", "handheld_camera" })
                Assert.AreEqual(HolsterAssignmentCore.RightSocket,
                    HolsterAssignmentCore.PreferredSocket(id), id + " belongs on the gun side");
        }

        [Test]
        public void TheLeftSocketOnlyTakesMelee_AndTheRightNeverDoes()
        {
            Assert.IsTrue(HolsterAssignmentCore.SocketAccepts("HolsterLeft", "breaker_blade"));
            Assert.IsFalse(HolsterAssignmentCore.SocketAccepts("HolsterLeft", "pistol"));

            Assert.IsTrue(HolsterAssignmentCore.SocketAccepts("HolsterRight", "pistol"));
            Assert.IsFalse(HolsterAssignmentCore.SocketAccepts("HolsterRight", "breaker_blade"));

            Assert.IsTrue(HolsterAssignmentCore.SocketAccepts("HolsterCenter", "gravity_gun"));
            Assert.IsFalse(HolsterAssignmentCore.SocketAccepts("HolsterCenter", "tide_pike"));
        }

        [Test]
        public void AnUnnamedSocketAcceptsAnything()
        {
            // The no-trap law: never let a naming change silently make gear unbeltable.
            Assert.IsTrue(HolsterAssignmentCore.SocketAccepts("Socket_07", "breaker_blade"));
            Assert.IsTrue(HolsterAssignmentCore.SocketAccepts(null, "pistol"));
            Assert.IsTrue(HolsterAssignmentCore.SocketAccepts("", "gravity_gun"));
        }

        [Test]
        public void SideIsReadFromTheNameLoosely()
        {
            Assert.AreEqual(HolsterAssignmentCore.SocketSide.Left,
                HolsterAssignmentCore.SideOf("Belt_LEFT_hip"));
            Assert.AreEqual(HolsterAssignmentCore.SocketSide.Right,
                HolsterAssignmentCore.SideOf("holsterRight"));
            Assert.AreEqual(HolsterAssignmentCore.SocketSide.Centre,
                HolsterAssignmentCore.SideOf("HolsterCenter"));
            Assert.AreEqual(HolsterAssignmentCore.SocketSide.Centre,
                HolsterAssignmentCore.SideOf("front-centre-pouch"));
            Assert.AreEqual(HolsterAssignmentCore.SocketSide.Unknown,
                HolsterAssignmentCore.SideOf("pouch"));
        }
    }
}
