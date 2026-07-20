using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public sealed class InputMutationSettleCoreTests
    {
        [Test]
        public void TwoConsecutiveSafeNonTravellingFrames_Restore()
        {
            InputMutationSettleStep first = InputMutationSettleCore.Advance(false, true, 0);
            Assert.That(first.Decision, Is.EqualTo(InputMutationSettleDecision.WaitForCleanFrame));
            Assert.That(first.NextCleanFrames, Is.EqualTo(1));

            InputMutationSettleStep second = InputMutationSettleCore.Advance(
                false, true, first.NextCleanFrames);
            Assert.That(second.Decision, Is.EqualTo(InputMutationSettleDecision.Restore));
            Assert.That(second.NextCleanFrames, Is.EqualTo(2));
        }

        [Test]
        public void NewTravelDuringCleanTail_ResetsAndPreventsRestore()
        {
            InputMutationSettleStep first = InputMutationSettleCore.Advance(false, true, 0);
            InputMutationSettleStep travel = InputMutationSettleCore.Advance(
                true, true, first.NextCleanFrames);

            Assert.That(travel.Decision, Is.EqualTo(InputMutationSettleDecision.WaitForTravel));
            Assert.That(travel.NextCleanFrames, Is.Zero);

            InputMutationSettleStep afterTravel = InputMutationSettleCore.Advance(
                false, true, travel.NextCleanFrames);
            Assert.That(afterTravel.Decision,
                Is.EqualTo(InputMutationSettleDecision.WaitForCleanFrame));
            Assert.That(afterTravel.NextCleanFrames, Is.EqualTo(1));
        }

        [Test]
        public void UnsafeRead_ResetsProgressAndNeverRestores()
        {
            InputMutationSettleStep unsafeStep = InputMutationSettleCore.Advance(false, false, 1);
            Assert.That(unsafeStep.Decision, Is.EqualTo(InputMutationSettleDecision.WaitForActions));
            Assert.That(unsafeStep.NextCleanFrames, Is.Zero);
        }

        [Test]
        public void TravelWinsEvenWhenActionsAppearSafe()
        {
            InputMutationSettleStep step = InputMutationSettleCore.Advance(true, true, 99);
            Assert.That(step.Decision, Is.EqualTo(InputMutationSettleDecision.WaitForTravel));
            Assert.That(step.NextCleanFrames, Is.Zero);
        }

        [Test]
        public void NegativePriorCount_IsNormalized()
        {
            InputMutationSettleStep step = InputMutationSettleCore.Advance(false, true, -5);
            Assert.That(step.Decision, Is.EqualTo(InputMutationSettleDecision.WaitForCleanFrame));
            Assert.That(step.NextCleanFrames, Is.EqualTo(1));
        }
    }
}
