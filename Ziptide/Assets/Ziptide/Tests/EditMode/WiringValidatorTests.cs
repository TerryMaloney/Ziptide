using System.Collections.Generic;
using NUnit.Framework;
using Ziptide.Editor.Validation;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// THE BOTH-SIDES GATE, in CI. Runs <see cref="WiringValidator"/> every push so a one-sided wiring
    /// seam (an author not build-hooked, a forgeRecipeId with no recipe, a genome with no creature) fails
    /// the build automatically — the enforcement half of the 2026-07-06 wiring audit. If this test fails,
    /// the message names the exact dangling seam; fix the missing side, don't delete the check.
    /// </summary>
    public class WiringValidatorTests
    {
        [Test]
        public void EverySeam_IsWiredBothSides()
        {
            bool ok = WiringValidator.Validate(out List<string> issues);
            Assert.IsTrue(ok, "One-sided wiring seam(s) — wire the missing side:\n  " + string.Join("\n  ", issues));
        }
    }
}
