using NUnit.Framework;
using Ziptide.Editor.Patching;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// SKYSCAPE_DESIGN is a north star, not a systems doc — Terry named the skyscape as one of the
    /// biggest reasons this game exists. Its §6 step 1 was proven on W005, a world the first level
    /// never reaches, while ToxicCity — THE FIRST PLANET THE PLAYER EVER STANDS ON — shipped with
    /// perfectly still, perfectly clear air, failing rubric items §5.1 ("is something drifting?")
    /// and §5.2 ("is the horizon hazy?").
    ///
    /// These pin the first level's air specifically, so a library retune cannot quietly return the
    /// game's opening world to a backdrop.
    /// </summary>
    public class Level1AtmosphereTests
    {
        private static SkyVistaDefinition Vista(string sceneName)
        {
            foreach (var spec in SkyVistaLibrary.Specs())
                if (spec.Key == sceneName) return spec.Value();
            return null;
        }

        [Test]
        public void TheFirstPlanet_HasAir()
        {
            SkyVistaDefinition toxic = Vista("ToxicCity");
            Assert.IsNotNull(toxic, "ToxicCity must stay in the sky library");
            Assert.IsTrue(toxic.atmosphere.enabled,
                "the first planet the player stands on cannot ship as a backdrop");

            AtmosphereSpec air = SkyAtmosphere.ForHazard(
                toxic.atmosphere.hazardTag, toxic.atmosphere.intensity);
            Assert.IsTrue(air.HazeEnabled, "§5.2 — the horizon must read as hazy, not crisp");
            Assert.Greater(air.MoteCount, 0, "§5.1 — if the air is perfectly still, it isn't air");
        }

        [Test]
        public void TheFirstPlanetsAir_IsAcid_NotACopyOfTheBloomPrototype()
        {
            // The likeliest bad change is someone extending the atmosphere stack by pasting W005's
            // Bloom block. A toxic canal city that smells of warm pollen is the wrong world.
            SkyVistaDefinition toxic = Vista("ToxicCity");
            Assert.AreEqual("acid", toxic.atmosphere.hazardTag);

            AtmosphereSpec acid = SkyAtmosphere.ForHazard("acid", 1f);
            AtmosphereSpec bloom = SkyAtmosphere.ForHazard("Bloom", 1f);
            Assert.Greater(acid.MoteTint.g, acid.MoteTint.b, "acid air reads green, not blue");
            Assert.Greater(acid.HazeTint.g, acid.HazeTint.r, "acid haze is green-yellow");
            Assert.Greater(bloom.HazeTint.r, bloom.HazeTint.g, "…and Bloom's is amber — different worlds");
        }

        [Test]
        public void TheFirstPlanetsAir_IsWorkable_NotASetPiece()
        {
            // The player spends the whole contract inside this air, repairing machinery and reading
            // wayfinding lanterns through it. Denser than the signature prototype would be a comfort
            // and legibility problem, not a win.
            SkyVistaDefinition toxic = Vista("ToxicCity");
            SkyVistaDefinition w005 = Vista("W005_OxidizedCanopy");
            Assert.Less(toxic.atmosphere.intensity, w005.atmosphere.intensity,
                "the working world's air stays thinner than the signature set-piece's");
            Assert.Greater(toxic.atmosphere.intensity, 0.3f, "…but thin enough to vanish is not air either");
        }

        [Test]
        public void TheFirstPlanet_DeclaresWhoCarriesPillar4()
        {
            // ToxicCity has no generated theme, so SkyVistaAuthor never assigns this vista and its
            // light/ambient fields are never read. Pillar 4 is real here (the city's fog is green) —
            // it is just owned elsewhere, and the vista has to say so rather than set dead fields.
            SkyVistaDefinition toxic = Vista("ToxicCity");
            Assert.IsNotEmpty(toxic.atmosphere.groundCouplingOwner,
                "a binder-driven world must name what carries the sky's colour to the ground");
            Assert.AreEqual(0f, toxic.directionalLightIntensity,
                "…and must not also set light fields nothing reads");
            Assert.IsFalse(toxic.overrideAmbient);
        }

        [Test]
        public void TheAtmosphereChange_DidNotTouchTheCitysExistingLook()
        {
            // The whole reason this landed as a binder instead of a theme: it must be impossible for
            // "the city gained air" to also mean "the city looks different" the first time it reaches
            // a headset. The dome, the body and the smog-killed stars are exactly as they shipped.
            SkyVistaDefinition toxic = Vista("ToxicCity");
            Assert.AreEqual(1, toxic.bodies.Count, "still the one banded planet");
            Assert.AreEqual(0f, toxic.stars.density, "smog still kills the stars over the city");
            Assert.AreEqual(0f, toxic.shellGridIntensity, "the Shell is still invisible at W001");
            Assert.IsFalse(toxic.atmosphere.bodyGlow, "no glow — the legacy path owns the planet");
            Assert.IsFalse(toxic.overrideFog, "the world's own fog is untouched");
        }

        [Test]
        public void TheSpaceLeg_StaysAirless_AndThatIsCorrect()
        {
            // §4.1's `void` row: sparse-to-none particulate, stars dominant. Drifting motes in vacuum
            // would be a bug that looks like polish, so pin the absence deliberately rather than
            // leaving it to whoever next extends the stack.
            SkyVistaDefinition lane = Vista("SpaceLane_Trial");
            Assert.IsNotNull(lane);
            Assert.IsFalse(lane.atmosphere.enabled, "vacuum has no weather");
            Assert.Greater(lane.stars.density, 0.5f, "…the starfield is the point out there");
            Assert.Greater(lane.directionalLightIntensity, 0f,
                "and the space leg DOES own its own light coupling (vista-driven, not binder-driven)");
        }
    }
}
