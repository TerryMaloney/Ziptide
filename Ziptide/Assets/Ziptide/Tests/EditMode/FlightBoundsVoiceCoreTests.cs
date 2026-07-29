using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests
{
    /// <summary>
    /// ⚖ Terry, 2026-07-29: *"there should be a variety of responses from Rill so it doesn't get too
    /// repetitive and stale."* That is a testable claim, so it is tested: a pool is WALKED, not
    /// rolled, so every line is spent before any repeats; and she only speaks on a rise, so holding
    /// station near a wreck is silent.
    /// </summary>
    public class FlightBoundsVoiceCoreTests
    {
        private static FlightBoundsReading Reading(FlightBoundKind kind, FlightBoundLevel level)
            => new FlightBoundsReading { Kind = kind, Level = level, Severity01 = 0.5f, Push = Vector3.right };

        [Test]
        public void APoolIsWalked_SoEveryLineIsSpentBeforeAnyRepeats()
        {
            for (int count = 2; count <= 8; count++)
            {
                var seen = new HashSet<int>();
                for (int cursor = 0; cursor < count; cursor++)
                    Assert.IsTrue(seen.Add(FlightBoundsVoiceCore.PickIndex(count, cursor)),
                        "pool size " + count + " repeats before it is exhausted");
                Assert.AreEqual(count, seen.Count);
            }
        }

        [Test]
        public void ConsecutivePicksAreNeverTheSameLine()
        {
            for (int cursor = 0; cursor < 40; cursor++)
                Assert.AreNotEqual(
                    FlightBoundsVoiceCore.PickIndex(FlightBoundsVoiceCore.PoolSize, cursor),
                    FlightBoundsVoiceCore.PickIndex(FlightBoundsVoiceCore.PoolSize, cursor + 1));
        }

        [Test]
        public void StrideIsCoprimeWithThePool()
        {
            for (int count = 3; count <= 12; count++)
            {
                int s = FlightBoundsVoiceCore.StrideFor(count);
                Assert.AreEqual(1, Gcd(s, count), "stride " + s + " does not visit every slot of " + count);
            }
        }

        private static int Gcd(int a, int b)
        {
            while (b != 0) { int t = a % b; a = b; b = t; }
            return a;
        }

        [Test]
        public void SheSaysNothingWhileEverythingIsClear()
        {
            var v = default(FlightBoundsVoiceState);
            Assert.IsFalse(FlightBoundsVoiceCore.ShouldSpeak(ref v,
                Reading(FlightBoundKind.None, FlightBoundLevel.Clear), 10f, out _));
        }

        [Test]
        public void SheSpeaksOnEntry_ThenHoldsHerTongueWhileNothingChanges()
        {
            var v = default(FlightBoundsVoiceState);
            var r = Reading(FlightBoundKind.Structure, FlightBoundLevel.Advisory);

            Assert.IsTrue(FlightBoundsVoiceCore.ShouldSpeak(ref v, r, 10f, out string first));
            Assert.AreEqual("BOUNDS_STRUCTURE_ADV_0", first);

            // Same tier, same kind, much later: still nothing. Sitting in a warning is not news.
            Assert.IsFalse(FlightBoundsVoiceCore.ShouldSpeak(ref v, r, 200f, out _));
        }

        [Test]
        public void GettingWorseEarnsANewLine()
        {
            var v = default(FlightBoundsVoiceState);
            Assert.IsTrue(FlightBoundsVoiceCore.ShouldSpeak(ref v,
                Reading(FlightBoundKind.Structure, FlightBoundLevel.Advisory), 10f, out string adv));
            Assert.IsTrue(FlightBoundsVoiceCore.ShouldSpeak(ref v,
                Reading(FlightBoundKind.Structure, FlightBoundLevel.Correcting), 20f, out string cor));
            StringAssert.Contains("_ADV_", adv);
            StringAssert.Contains("_COR_", cor);
        }

        [Test]
        public void TheCooldownStopsHerStuttering()
        {
            var v = default(FlightBoundsVoiceState);
            Assert.IsTrue(FlightBoundsVoiceCore.ShouldSpeak(ref v,
                Reading(FlightBoundKind.Structure, FlightBoundLevel.Advisory), 10f, out _));
            Assert.IsFalse(FlightBoundsVoiceCore.ShouldSpeak(ref v,
                Reading(FlightBoundKind.Structure, FlightBoundLevel.Correcting),
                10f + FlightBoundsVoiceCore.CooldownSeconds - 0.1f, out _),
                "a worsening reading still waits out the cooldown");
        }

        [Test]
        public void ADifferentProblemIsWorthSayingEvenAtTheSameTier()
        {
            var v = default(FlightBoundsVoiceState);
            Assert.IsTrue(FlightBoundsVoiceCore.ShouldSpeak(ref v,
                Reading(FlightBoundKind.Corridor, FlightBoundLevel.Advisory), 10f, out _));
            Assert.IsTrue(FlightBoundsVoiceCore.ShouldSpeak(ref v,
                Reading(FlightBoundKind.Gate, FlightBoundLevel.Advisory), 30f, out string gate));
            StringAssert.StartsWith("BOUNDS_GATE_ADV_", gate);
        }

        [Test]
        public void HoveringOnAThresholdDoesNotRetrigger_ButAGenuineSecondApproachDoes()
        {
            var v = default(FlightBoundsVoiceState);
            var advisory = Reading(FlightBoundKind.Gate, FlightBoundLevel.Advisory);
            Assert.IsTrue(FlightBoundsVoiceCore.ShouldSpeak(ref v, advisory, 10f, out _));

            // Brief flickers back to clear must NOT re-arm her.
            var clear = Reading(FlightBoundKind.None, FlightBoundLevel.Clear);
            FlightBoundsVoiceCore.ShouldSpeak(ref v, clear, 30f, out _);
            FlightBoundsVoiceCore.ShouldSpeak(ref v, clear, 31f, out _);
            Assert.IsFalse(FlightBoundsVoiceCore.ShouldSpeak(ref v, advisory, 32f, out _));

            // A sustained clear does.
            FlightBoundsVoiceCore.ShouldSpeak(ref v, clear, 60f, out _);
            FlightBoundsVoiceCore.ShouldSpeak(ref v, clear, 60f + FlightBoundsVoiceCore.ReArmSeconds, out _);
            Assert.IsTrue(FlightBoundsVoiceCore.ShouldSpeak(ref v, advisory, 70f, out _));
        }

        [Test]
        public void HardSharesTheCorrectingPool()
        {
            Assert.AreEqual(
                FlightBoundsVoiceCore.LineId(FlightBoundKind.Deep, FlightBoundLevel.Correcting, 2),
                FlightBoundsVoiceCore.LineId(FlightBoundKind.Deep, FlightBoundLevel.Hard, 2));
        }
    }
}
