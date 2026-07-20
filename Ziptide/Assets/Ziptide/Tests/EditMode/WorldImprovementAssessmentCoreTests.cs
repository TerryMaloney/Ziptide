using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public sealed class WorldImprovementAssessmentCoreTests
    {
        [Test]
        public void MissingAspect_RanksBeforeCoveredAspects()
        {
            WorldAspectAssessment[] result = WorldImprovementAssessmentCore.Assess(
                new[] { "Identity", "Interaction", "Story" },
                new[]
                {
                    new WorldAspectEvidence("Identity", 1, 5),
                    new WorldAspectEvidence("Story", 2, 10),
                });

            Assert.That(result[0].Aspect, Is.EqualTo("Interaction"));
            Assert.That(result[0].Score, Is.Zero);
            Assert.That(result[2].Aspect, Is.EqualTo("Story"));
            Assert.That(result[2].Score, Is.GreaterThan(result[1].Score));
        }

        [Test]
        public void IndependentModules_IncreaseDepthMoreThanPrimitiveSpam()
        {
            WorldAspectAssessment[] result = WorldImprovementAssessmentCore.Assess(
                new[] { "Navigation", "Perception" },
                new[]
                {
                    new WorldAspectEvidence("Navigation", 1, 20),
                    new WorldAspectEvidence("Perception", 1, 5),
                    new WorldAspectEvidence("Perception", 1, 5),
                });

            WorldAspectAssessment navigation = result[0].Aspect == "Navigation" ? result[0] : result[1];
            WorldAspectAssessment perception = result[0].Aspect == "Perception" ? result[0] : result[1];
            Assert.That(perception.ModuleCount, Is.EqualTo(2));
            Assert.That(perception.Score, Is.GreaterThan(navigation.Score));
        }

        [Test]
        public void DuplicateCaseVariants_AreMergedAndRankingIsDeterministic()
        {
            var required = new[] { "Story", "story", "Ambience" };
            var evidence = new[]
            {
                new WorldAspectEvidence("STORY", 1, 4),
                new WorldAspectEvidence("Ambience", 1, 4),
            };

            WorldAspectAssessment[] first = WorldImprovementAssessmentCore.Assess(required, evidence);
            WorldAspectAssessment[] second = WorldImprovementAssessmentCore.Assess(required, evidence);

            Assert.That(first.Length, Is.EqualTo(2));
            Assert.That(second.Length, Is.EqualTo(first.Length));
            for (int i = 0; i < first.Length; i++)
            {
                Assert.That(second[i].Aspect, Is.EqualTo(first[i].Aspect));
                Assert.That(second[i].Score, Is.EqualTo(first[i].Score));
            }
        }

        [Test]
        public void Weakest_ReturnsBoundedPrefix()
        {
            WorldAspectAssessment[] result = WorldImprovementAssessmentCore.Assess(
                new[] { "C", "A", "B" }, new WorldAspectEvidence[0]);

            CollectionAssert.AreEqual(new[] { "A", "B" },
                WorldImprovementAssessmentCore.Weakest(result, 2));
        }
    }
}
