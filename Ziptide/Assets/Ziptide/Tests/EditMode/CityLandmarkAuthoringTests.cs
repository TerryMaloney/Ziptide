using NUnit.Framework;
using UnityEditor;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The core test next door proves the RULE. This one proves the SHIPPED DATA obeys it — which is
    /// the half that was actually broken: `LandmarkScaleCore.ReadsAsStick` would have flagged the
    /// hangar's crane at 16 m x 2 m from the day it was authored, and nothing was asking.
    ///
    /// Scope is deliberately narrow. Plenty of authored landmarks across the twelve worlds are thin on
    /// purpose (W002's LightShaft is a shaft of light; W003's prisms are prisms). A landmark that
    /// declares itself a CRANE is claiming to be a working machine with a person's ladder on it, and
    /// that claim is checkable.
    /// </summary>
    public class CityLandmarkAuthoringTests
    {
        [Test]
        public void EveryAuthoredCrane_HasEnoughSurfaceToCarryItsScaleCues()
        {
            var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(ZiptideConstants.PathToxicCityLayout);
            if (kit == null)
                Assert.Ignore("No ToxicCity layout on disk; the patcher's code default applies and is covered by review.");

            int cranes = 0;
            foreach (var d in kit.districts)
            {
                if (d == null || d.landmarks == null) continue;
                foreach (var lm in d.landmarks)
                {
                    if (lm == null || lm.kind != LandmarkKind.Crane) continue;
                    cranes++;
                    Assert.IsFalse(LandmarkScaleCore.ReadsAsStick(lm.height, lm.width),
                        d.id + "/" + lm.name + " is " + lm.height + " m tall and " + lm.width +
                        " m wide — too slender to hold a ladder, a walkway or a cab, so it will read " +
                        "as a stick no matter how tall it is.");
                }
            }

            Assert.AreEqual(1, cranes,
                "The shipyard's crane is the yard's only ruler — if it stops being a Crane, the " +
                "district loses every human-scale cue it has.");
        }
    }
}
