#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class FirstHourHolsterAdapterTests
    {
        [Test]
        public void ValidItem_PublishesOnceAndSetsFirstHolsterFlag()
        {
            var profile = new PlayerProfile();
            var published = new List<string>();
            bool alreadyReported = false;

            bool first = HolsterSocketInteractor.TryReportFirstHolster(
                "pistol",
                profile,
                ref alreadyReported,
                published.Add);
            bool duplicate = HolsterSocketInteractor.TryReportFirstHolster(
                "pistol",
                profile,
                ref alreadyReported,
                published.Add);

            Assert.IsTrue(first);
            Assert.IsFalse(duplicate);
            Assert.IsTrue(alreadyReported);
            Assert.IsTrue(profile.HasFlag(ZiptideFlags.FIRST_HOLSTER));
            CollectionAssert.AreEqual(new[] { "pistol" }, published);
        }

        [TestCase(null)]
        [TestCase("")]
        public void InvalidItem_DoesNotPublishOrSetFlag(string itemId)
        {
            var profile = new PlayerProfile();
            int publishCount = 0;
            bool alreadyReported = false;

            bool result = HolsterSocketInteractor.TryReportFirstHolster(
                itemId,
                profile,
                ref alreadyReported,
                _ => publishCount++);

            Assert.IsFalse(result);
            Assert.IsFalse(alreadyReported);
            Assert.AreEqual(0, publishCount);
            Assert.IsFalse(profile.HasFlag(ZiptideFlags.FIRST_HOLSTER));
        }

        [Test]
        public void MissingProfile_PublishesSafelyAndSuppressesDuplicateCallback()
        {
            var published = new List<string>();
            bool alreadyReported = false;

            Assert.DoesNotThrow(() =>
            {
                bool first = HolsterSocketInteractor.TryReportFirstHolster(
                    "taser_dart_gun",
                    null,
                    ref alreadyReported,
                    published.Add);
                bool duplicate = HolsterSocketInteractor.TryReportFirstHolster(
                    "taser_dart_gun",
                    null,
                    ref alreadyReported,
                    published.Add);

                Assert.IsTrue(first);
                Assert.IsFalse(duplicate);
            });

            CollectionAssert.AreEqual(new[] { "taser_dart_gun" }, published);
        }

        [Test]
        public void ExistingFirstHolsterFlag_IsIdempotentAndSilent()
        {
            var profile = new PlayerProfile();
            profile.SetFlag(ZiptideFlags.FIRST_HOLSTER);
            int publishCount = 0;
            bool alreadyReported = false;

            bool result = HolsterSocketInteractor.TryReportFirstHolster(
                "gravity_gun",
                profile,
                ref alreadyReported,
                _ => publishCount++);

            Assert.IsFalse(result);
            Assert.IsTrue(alreadyReported);
            Assert.AreEqual(0, publishCount);
            Assert.AreEqual(1, profile.flags.FindAll(f => f == ZiptideFlags.FIRST_HOLSTER).Count);
        }

        [Test]
        public void NullPublisher_StillSetsFlagWithoutThrowing()
        {
            var profile = new PlayerProfile();
            bool alreadyReported = false;

            Assert.DoesNotThrow(() =>
            {
                Assert.IsTrue(HolsterSocketInteractor.TryReportFirstHolster(
                    "pistol",
                    profile,
                    ref alreadyReported,
                    null));
            });

            Assert.IsTrue(profile.HasFlag(ZiptideFlags.FIRST_HOLSTER));
        }

        [Test]
        public void RuntimeSource_PreservesSocketRulesAndDoesNotAutosave()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "Inventory",
                "HolsterSocketInteractor.cs");
            Assert.IsTrue(File.Exists(path), path);

            string source = File.ReadAllText(path);
            StringAssert.Contains("selectEntered.AddListener(OnSelectEnteredCallback)", source);
            StringAssert.Contains("base.CanHover(interactable)", source);
            StringAssert.Contains("base.CanSelect(interactable)", source);
            StringAssert.Contains("\"pistol\", \"taser_dart_gun\", \"gravity_gun\"", source);
            StringAssert.Contains("public static event Action<string> ItemHolstered", source);
            StringAssert.Contains("ZIPTIDE: FIRST_HOLSTER item=", source);
            StringAssert.DoesNotContain("AutosaveNow", source);
            StringAssert.DoesNotContain(".Save()", source);
            StringAssert.DoesNotContain("TravelCoordinator", source);
            StringAssert.DoesNotContain("InputAction", source);
        }
    }
}
#endif
