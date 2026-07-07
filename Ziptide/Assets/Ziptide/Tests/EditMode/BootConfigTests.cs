using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Guards the boot entry point. A June-18 dev bypass pointed <see cref="ZiptideConstants.FirstWorldScene"/>
    /// at the SandboxTestLab graybox "temporarily," and it silently stayed for weeks — booting testers
    /// into a blank room with no way out (the in-VR TMP menu is unreliable on device). These pin the
    /// invariants so that regression can't return unnoticed: the boot target must be a real world, never
    /// the sandbox graybox, and never _Boot itself.
    /// </summary>
    public class BootConfigTests
    {
        [Test]
        public void FirstWorldScene_IsARealEntryWorld_NotEmpty()
        {
            Assert.IsFalse(string.IsNullOrEmpty(ZiptideConstants.FirstWorldScene),
                "the boot entry must name a scene");
        }

        [Test]
        public void FirstWorldScene_IsNotTheSandboxGrayboxBypass()
        {
            Assert.AreNotEqual(ZiptideConstants.SceneSandbox, ZiptideConstants.FirstWorldScene,
                "boot must not land in the SandboxTestLab graybox — that stranded testers behind the broken dev menu");
        }

        [Test]
        public void FirstWorldScene_IsNeverBoot()
        {
            Assert.AreNotEqual(ZiptideConstants.SceneBoot, ZiptideConstants.FirstWorldScene,
                "_Boot is the bootstrap scene, never a travel/boot destination");
        }

        [Test]
        public void FirstWorldScene_IsW000_TheShipOpening()
        {
            // The intended opening (ship bay -> PUNCH IT cast-off -> the world chain). If the entry world
            // is ever deliberately changed, update this line in the same commit so the change is reviewed.
            Assert.AreEqual(ZiptideConstants.SceneW000, ZiptideConstants.FirstWorldScene);
        }
    }
}
