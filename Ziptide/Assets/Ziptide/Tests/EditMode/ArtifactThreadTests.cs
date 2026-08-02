using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// "It doesn't really make sense to punch it because we haven't even found the artifact yet."
    ///
    /// The build fired the full Ziptide gate at minute ten, for a routine trip to a repair job, while
    /// the approved design (`docs/design/FIRST_HOUR_DIRECTORS_CUT.md` §5) puts the first Ziptide at
    /// minute FORTY-FIVE — after the player finds half an artifact in a wreck, is paid the other half
    /// in Toxic City, joins them, and seats the key in their own ship. These tests pin the order so
    /// the spectacle can never drift back onto the bus ride.
    /// </summary>
    public sealed class ArtifactThreadTests
    {
        private static string Source(params string[] parts)
        {
            string full = Path.Combine(Application.dataPath, "Ziptide");
            foreach (string part in parts) full = Path.Combine(full, part);
            Assert.IsTrue(File.Exists(full), full);
            return File.ReadAllText(full);
        }

        [Test]
        public void TheFirstLaunchIsACastOff_NotAZiptide()
        {
            string castOff = Source("Gameplay", "Runtime", "Story", "ShipCastOffRuntime.cs");
            StringAssert.Contains("suppressGateEffect", castOff);
            StringAssert.Contains("skipGate: suppressGateEffect", castOff);
            StringAssert.Contains("ZiptideConstants.SceneSpaceLane", castOff);
            StringAssert.DoesNotContain("private string targetScene = \"ToxicCity\"", castOff,
                "the tutorial's one-way launch must not go straight to the repair job any more");
        }

        [Test]
        public void TheAuthorOwnsTheRoute_NotTheSceneFile()
        {
            // The committed W000 scene still carries targetScene: ToxicCity from the older arc. If the
            // serialized value were the truth, regenerating the world would not fix the route.
            string builder = Source("Editor", "Patching", "CityBuilder.cs");
            StringAssert.Contains("castOff.Configure(ZiptideConstants.SceneSpaceLane, suppressGate: true)", builder);
        }

        [Test]
        public void OnlyTheKeySocketReArmsTheTide()
        {
            string socket = Source("Gameplay", "Runtime", "Story", "KeySocketRuntime.cs");
            StringAssert.Contains("castOff.Configure(destinationScene, suppressGate: false)", socket);
            StringAssert.Contains("ZiptideFlags.KEY_SEATED", socket);

            // Nothing else in the game may hand the gate back.
            string builder = Source("Editor", "Patching", "CityBuilder.cs");
            StringAssert.DoesNotContain("suppressGate: false", builder);
        }

        [Test]
        public void TheJoinRequiresTwoHands_NotTwoObjectsOnTheFloor()
        {
            // The story is that Cal joins them. Two halves left lying next to each other must not do
            // it, or the beat happens off-screen in a corner of the hold.
            string join = Source("Gameplay", "Runtime", "Story", "ArtifactJoinRuntime.cs");
            // XRI 3.x renamed XRBaseControllerInteractor -> XRBaseInputInteractor.
            StringAssert.Contains("XRBaseInputInteractor", join);
            StringAssert.Contains("grab.isSelected", join);
            StringAssert.Contains("SendHapticImpulse", join);
        }

        [Test]
        public void TheJoinReachesBeforeItSnaps()
        {
            // A socket that only reacts on contact feels like a checkbox. The pull is the beat.
            string join = Source("Gameplay", "Runtime", "Story", "ArtifactJoinRuntime.cs");
            StringAssert.Contains("PullRadius", join);
            StringAssert.Contains("SnapRadius", join);
        }

        [Test]
        public void EveryStageOfTheThreadHasItsOwnFlag()
        {
            Assert.AreEqual("ARTIFACT_HALF_A", ZiptideFlags.ARTIFACT_HALF_A);
            Assert.AreEqual("ARTIFACT_HALF_B", ZiptideFlags.ARTIFACT_HALF_B);
            Assert.AreEqual("ARTIFACT_JOINED", ZiptideFlags.ARTIFACT_JOINED);
            Assert.AreEqual("KEY_SEATED", ZiptideFlags.KEY_SEATED);
            Assert.AreEqual("FIRST_ZIPTIDE_RIDDEN", ZiptideFlags.FIRST_ZIPTIDE_RIDDEN);
        }

        [Test]
        public void EveryBeatOfTheThreadHasAVoice()
        {
            string lines = Source("Editor", "Patching", "RillLineAuthor.cs");
            StringAssert.Contains("ZiptideFlags.ARTIFACT_HALF_A", lines);
            StringAssert.Contains("ZiptideFlags.ARTIFACT_HALF_B", lines);
            StringAssert.Contains("ZiptideFlags.ARTIFACT_JOINED", lines);
            StringAssert.Contains("ZiptideFlags.KEY_SEATED", lines);
            StringAssert.Contains("Vex Bootstrapper", lines);

            // Nobody says the quiet part. The Transmission layer stays sealed for the whole hour, so
            // a player replaying this after finishing the game gets chills instead of a recap.
            //
            // Scanned over DIALOGUE ONLY. The first version of this test read the whole file and
            // failed on the comment that explains the rule -- a guard that fires on its own
            // documentation is worse than no guard, because the next person deletes the comment.
            StringAssert.DoesNotContain("it knew you", DialogueOnly(lines));
        }

        /// <summary>
        /// The authored line text with comments removed. Prose about the rules must never be mistaken
        /// for prose the player hears.
        /// </summary>
        private static string DialogueOnly(string source)
        {
            var kept = new System.Text.StringBuilder();
            foreach (string line in source.Split('\n'))
            {
                string trimmed = line.TrimStart();
                if (trimmed.StartsWith("//") || trimmed.StartsWith("///")) continue;
                kept.Append(line.ToLowerInvariant()).Append('\n');
            }
            return kept.ToString();
        }

        [Test]
        public void TheSalvageLegHasAnOnwardExit_SoItIsALegAndNotACulDeSac()
        {
            string lane = Source("Editor", "Patching", "ScenePatcherSpaceLane.cs");
            StringAssert.Contains("EnsureOnwardLeg", lane);
            StringAssert.Contains("PathToxicCityWorldPack", lane);
            StringAssert.Contains("EnsureTheFind", lane);
        }
    }
}
