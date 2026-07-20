namespace Ziptide.Core
{
    public enum InputMutationSettleDecision
    {
        WaitForTravel = 0,
        WaitForActions = 1,
        WaitForCleanFrame = 2,
        Restore = 3,
    }

    public readonly struct InputMutationSettleStep
    {
        public readonly InputMutationSettleDecision Decision;
        public readonly int NextCleanFrames;

        public InputMutationSettleStep(InputMutationSettleDecision decision, int nextCleanFrames)
        {
            Decision = decision;
            NextCleanFrames = nextCleanFrames < 0 ? 0 : nextCleanFrames;
        }
    }

    /// <summary>
    /// Pure state seam for the input-mutation restoration window. A new trip or one unsafe read resets
    /// progress. Restoration is permitted only after two consecutive safe frames while no travel is active.
    /// This prevents a second travel that begins during the previous trip's settle tail from being mistaken
    /// for permanent InputActionState corruption.
    /// </summary>
    public static class InputMutationSettleCore
    {
        public const int RequiredCleanFrames = 2;

        public static InputMutationSettleStep Advance(
            bool isTravelling,
            bool actionsReadSafely,
            int cleanFrames)
        {
            if (isTravelling)
                return new InputMutationSettleStep(InputMutationSettleDecision.WaitForTravel, 0);

            if (!actionsReadSafely)
                return new InputMutationSettleStep(InputMutationSettleDecision.WaitForActions, 0);

            int next = cleanFrames < 0 ? 1 : cleanFrames + 1;
            if (next >= RequiredCleanFrames)
                return new InputMutationSettleStep(InputMutationSettleDecision.Restore, next);

            return new InputMutationSettleStep(InputMutationSettleDecision.WaitForCleanFrame, next);
        }
    }
}
