using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryPerformanceSampleTests
    {
        [Test]
        public void ControlledPostSweepSample_WritesDeterministicArtifact()
        {
            string sweep = "ZIPTIDE: HEALTH_SWEEP scene=CONTROLLED freed mats=2 tex=1 mesh=0 clips=0 memMB=128";
            var frames = new List<float> { 10f, 20f, 15f, 25f, 12f };

            RecoveryPerformanceSampleRecord sample = RecoveryPerformanceSample.Capture(
                "R1_10_CONTROLLED_POST_SWEEP",
                sweep,
                frames);
            RecoveryPerformanceArtifactPaths paths = RecoveryPerformanceSample.WriteArtifacts(
                sample,
                "r1_10_controlled_post_sweep");

            Assert.AreEqual(5, sample.sampledFrames);
            Assert.AreEqual(16.4f, sample.averageFrameMs, 0.001f);
            Assert.AreEqual(15f, sample.medianFrameMs, 0.001f);
            Assert.AreEqual(25f, sample.p95FrameMs, 0.001f);
            Assert.AreEqual(25f, sample.maximumFrameMs, 0.001f);
            Assert.AreEqual(sweep, sample.healthSweepEvidence);
            StringAssert.Contains("not a Quest device budget", sample.interpretation);
            Assert.IsTrue(File.Exists(paths.JsonPath));
            Assert.IsTrue(File.Exists(paths.MarkdownPath));
            Assert.GreaterOrEqual(sample.materials, 0);
            Assert.GreaterOrEqual(sample.totalAllocatedMB, 0);
        }

        [Test]
        public void Capture_RejectsMissingHealthSweepEvidence()
        {
            Assert.Throws<ArgumentException>(() => RecoveryPerformanceSample.Capture(
                "R1_10_INVALID",
                "ZIPTIDE: TRAVEL_OK dest=W000_DriftIn",
                new[] { 13f }));
        }
    }
}
