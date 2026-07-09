using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING Phase 0.1 — travel-autosave. The hot-path contract: AutosaveNow must be safe to
    /// call from TravelCoordinator with NO live SaveSystem (EditMode has none — the runtime
    /// bootstrap doesn't run), so a missing saver can never throw into the travel coroutine.
    /// </summary>
    public class SaveAutosaveTests
    {
        [Test]
        public void AutosaveNow_WithoutInstance_IsSafeNoOp()
        {
            Assert.IsNull(SaveSystem.Instance, "EditMode should have no bootstrapped SaveSystem");
            Assert.DoesNotThrow(() => SaveSystem.AutosaveNow("travel"));
        }
    }
}
