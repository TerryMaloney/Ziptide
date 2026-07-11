#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class WristScannerResultTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();

        private sealed class FakeScannable : IScannable
        {
            public Transform ScanTransform { get; set; }
            public ScanKind ScanKind { get; set; }
            public bool ScanActive { get; set; }
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < _objects.Count; i++)
                if (_objects[i] != null)
                    UnityEngine.Object.DestroyImmediate(_objects[i]);
            _objects.Clear();
        }

        [Test]
        public void Capture_IncludesOnlyActiveNonNullInRangeTargets_InOriginalOrder()
        {
            var first = Create("first", new Vector3(1f, 0f, 0f), ScanKind.Enemy, true);
            var outOfRange = Create("far", new Vector3(8f, 0f, 0f), ScanKind.Loot, true);
            var inactive = Create("inactive", new Vector3(2f, 0f, 0f), ScanKind.Node, false);
            var second = Create("second", new Vector3(3f, 0f, 0f), ScanKind.Objective, true);
            var noTransform = new FakeScannable
            {
                ScanTransform = null,
                ScanKind = ScanKind.Node,
                ScanActive = true
            };

            WristScanResult result = WristScanResult.Capture(
                new IScannable[] { first, outOfRange, inactive, noTransform, second },
                Vector3.zero,
                5f);

            Assert.AreEqual(2, result.Count);
            Assert.AreSame(first, result.Targets[0].Source);
            Assert.AreSame(second, result.Targets[1].Source);
            Assert.AreEqual(ScanKind.Enemy, result.Targets[0].Kind);
            Assert.AreEqual(ScanKind.Objective, result.Targets[1].Kind);
        }

        [Test]
        public void Capture_CopiesPulseTimeKindPositionAndDistance_ButPreservesIdentity()
        {
            var target = Create("moving", new Vector3(3f, 4f, 0f), ScanKind.Loot, true);

            WristScanResult result = WristScanResult.Capture(
                new[] { target },
                Vector3.zero,
                10f);

            Transform originalTransform = target.ScanTransform;
            target.ScanKind = ScanKind.Node;
            target.ScanTransform.position = new Vector3(20f, 20f, 20f);

            Assert.AreEqual(1, result.Count);
            Assert.AreSame(target, result.Targets[0].Source);
            Assert.AreSame(originalTransform, result.Targets[0].ScanTransform);
            Assert.AreEqual(ScanKind.Loot, result.Targets[0].Kind);
            Assert.AreEqual(new Vector3(3f, 4f, 0f), result.Targets[0].Position);
            Assert.AreEqual(5f, result.Targets[0].Distance, 0.0001f);
        }

        [Test]
        public void Result_CopiesCallerCollection_AndExposesReadOnlyTargets()
        {
            var target = Create("immutable", Vector3.one, ScanKind.Node, true);
            var input = new List<WristScanTarget>
            {
                new WristScanTarget(target, target.ScanTransform, target.ScanKind,
                    target.ScanTransform.position, 1f)
            };

            var result = new WristScanResult(input);
            input.Clear();

            Assert.AreEqual(1, result.Count);
            var listView = (IList<WristScanTarget>)result.Targets;
            Assert.Throws<NotSupportedException>(() => listView.Add(default(WristScanTarget)));
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void EmptyPulse_PublishesExactlyOnce()
        {
            int calls = 0;
            WristScanResult seen = null;
            Action<WristScanResult> subscriber = result =>
            {
                calls++;
                seen = result;
            };

            int successful = WristScanResult.PublishSafely(
                subscriber,
                WristScanResult.Empty);

            Assert.AreEqual(1, successful);
            Assert.AreEqual(1, calls);
            Assert.AreSame(WristScanResult.Empty, seen);
            Assert.AreEqual(0, seen.Count);
            Assert.AreEqual("none", seen.KindSummary());
        }

        [Test]
        public void ThrowingSubscriber_DoesNotBlockLaterSubscriber()
        {
            int failures = 0;
            int laterCalls = 0;
            Action<WristScanResult> subscribers =
                result => throw new InvalidOperationException("expected");
            subscribers += result => laterCalls++;

            int successful = WristScanResult.PublishSafely(
                subscribers,
                WristScanResult.Empty,
                exception => failures++);

            Assert.AreEqual(1, successful);
            Assert.AreEqual(1, failures);
            Assert.AreEqual(1, laterCalls);
        }

        [Test]
        public void RuntimeSource_PublishesOnceAfterExistingPulsePresentationIsArmed()
        {
            string scannerPath = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "Pvp",
                "WristScanner.cs");
            Assert.IsTrue(File.Exists(scannerPath), scannerPath);

            string source = File.ReadAllText(scannerPath);
            int pulseStart = source.IndexOf("private void Pulse()", StringComparison.Ordinal);
            int gatherStart = source.IndexOf(
                "private WristScanResult GatherTargets()",
                pulseStart,
                StringComparison.Ordinal);
            Assert.GreaterOrEqual(pulseStart, 0);
            Assert.Greater(gatherStart, pulseStart);

            string pulse = source.Substring(pulseStart, gatherStart - pulseStart);
            int gather = pulse.IndexOf("WristScanResult result = GatherTargets();", StringComparison.Ordinal);
            int haptic = pulse.IndexOf("SendHapticImpulse(pulseHapticAmp, pulseHapticDur)", StringComparison.Ordinal);
            int radar = pulse.IndexOf("_radar.SetTargets(_targets);", StringComparison.Ordinal);
            int tags = pulse.IndexOf("BuildTags();", StringComparison.Ordinal);
            int armed = pulse.IndexOf("_scanActiveUntil = Time.time + scanDuration;", StringComparison.Ordinal);
            int existingLog = pulse.IndexOf("ZIPTIDE: WRIST_SCAN_PULSE targets=", StringComparison.Ordinal);
            int resultLog = pulse.IndexOf("ZIPTIDE: WRIST_SCAN_RESULT count=", StringComparison.Ordinal);
            int publish = pulse.IndexOf("WristScanResult.PublishSafely(", StringComparison.Ordinal);

            Assert.GreaterOrEqual(gather, 0);
            Assert.Greater(haptic, gather);
            Assert.Greater(radar, haptic);
            Assert.Greater(tags, radar);
            Assert.Greater(armed, tags);
            Assert.Greater(existingLog, armed);
            Assert.Greater(resultLog, existingLog);
            Assert.Greater(publish, resultLog);
            Assert.AreEqual(1, Count(pulse, "WristScanResult.PublishSafely("));
            StringAssert.Contains("public static event System.Action<WristScanResult> ScanResultPublished", source);

            // Existing gesture, cooldown and presentation chokepoints remain in place.
            StringAssert.Contains("_state.Tick(Time.time, Time.deltaTime, held)", source);
            StringAssert.Contains("if (firedNow) Pulse();", source);
            Assert.AreEqual(2, Count(pulse, "SendHapticImpulse(pulseHapticAmp, pulseHapticDur)"));
            StringAssert.Contains("_radar.SetTargets(_targets);", pulse);
            StringAssert.Contains("BuildTags();", pulse);
            StringAssert.Contains("EnsureChevron();", pulse);
        }

        [Test]
        public void ResultSource_HasNoStoryOrProgressionDependency()
        {
            string resultPath = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "Pvp",
                "WristScanResult.cs");
            Assert.IsTrue(File.Exists(resultPath), resultPath);

            string source = File.ReadAllText(resultPath);
            StringAssert.DoesNotContain("Story", source);
            StringAssert.DoesNotContain("FirstHour", source);
            StringAssert.DoesNotContain("SaveSystem", source);
            StringAssert.DoesNotContain("RewardRouter", source);
            StringAssert.DoesNotContain("Repair", source);
        }

        private FakeScannable Create(string name, Vector3 position, ScanKind kind, bool active)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            _objects.Add(go);
            return new FakeScannable
            {
                ScanTransform = go.transform,
                ScanKind = kind,
                ScanActive = active
            };
        }

        private static int Count(string source, string token)
        {
            int count = 0;
            int index = 0;
            while ((index = source.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }
    }
}
#endif
