using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The device pass on 2026-08-01 produced one log line that no test could have produced:
    /// `SHIP_BOARD` immediately followed by `FALL_SAFETY y=-60.2`. Pressing BOARD SHIP dropped the
    /// player sixty metres. These pin the geometry rules that make that impossible to reintroduce.
    /// </summary>
    public class ShipDeckCoreTests
    {
        [Test]
        public void DeckPlateTopIsTheDeckPlane()
        {
            // The builder places the plate at PlateCentreY and the teleport aims at the plane. If
            // those ever disagree the player stands inside the plate or hovers above it.
            float plateTop = ShipDeckCore.PlateCentreY + ShipDeckCore.DeckThickness * 0.5f;
            Assert.AreEqual(0f, plateTop, 1e-5f, "the plate's top surface must BE the deck plane");
        }

        [Test]
        public void TheDeckIsClosedOnFourSides()
        {
            ShipDeckCore.RailOffsets(out float[] x, out float[] z);
            Assert.AreEqual(4, x.Length, "three rails is how you walk off the bow");
            Assert.AreEqual(4, z.Length);

            // One rail on each side: the four offsets must be four DISTINCT directions.
            Assert.AreEqual(-ShipDeckCore.RailOffset, x[0], 1e-5f);
            Assert.AreEqual(ShipDeckCore.RailOffset, x[1], 1e-5f);
            Assert.AreEqual(-ShipDeckCore.RailOffset, z[2], 1e-5f);
            Assert.AreEqual(ShipDeckCore.RailOffset, z[3], 1e-5f, "the forward rail is the one that was missing");
        }

        [Test]
        public void SideRailsRunForeAftAndEndRailsRunAcross()
        {
            ShipDeckCore.RailSize(0, out float lx, out _, out float lz);
            Assert.AreEqual(ShipDeckCore.RailThickness, lx, 1e-5f);
            Assert.AreEqual(ShipDeckCore.DeckSize, lz, 1e-5f);

            ShipDeckCore.RailSize(3, out float fx, out _, out float fz);
            Assert.AreEqual(ShipDeckCore.DeckSize, fx, 1e-5f);
            Assert.AreEqual(ShipDeckCore.RailThickness, fz, 1e-5f);
        }

        [Test]
        public void RailsSitInsideTheFootprintTheyGuard()
        {
            Assert.IsTrue(ShipDeckCore.IsInsideRails(0f, 0f), "the deck centre is standable");
            Assert.IsFalse(ShipDeckCore.IsInsideRails(ShipDeckCore.DeckHalf + 0.01f, 0f),
                "outside the rail line is not on the deck");
            Assert.IsFalse(ShipDeckCore.IsInsideRails(0f, -(ShipDeckCore.DeckHalf + 0.01f)));
        }

        [Test]
        public void StandPointClearsTheSurfaceItStandsOn()
        {
            Assert.AreEqual(4.1f, ShipDeckCore.StandY(4f), 1e-5f);
            Assert.Greater(ShipDeckCore.StandClearance, 0f, "landing exactly ON a collider jitters");
            Assert.Less(ShipDeckCore.StandClearance, 0.25f, "any higher and you land with a drop");
        }

        [Test]
        public void AnUnprovenStandPointIsRefused()
        {
            // THE SIXTY-METRE BUG. No ground under the candidate = the button must do nothing.
            Assert.IsFalse(ShipDeckCore.StandIsProven(groundFound: false, dropToGround: 0f));

            // Ground exists but is a storey down: still refused. That is a fall, not a deck.
            Assert.IsFalse(ShipDeckCore.StandIsProven(true, ShipDeckCore.MaxStandDrop + 0.5f));

            // Standing right on it, and a small step down, are both fine.
            Assert.IsTrue(ShipDeckCore.StandIsProven(true, 0f));
            Assert.IsTrue(ShipDeckCore.StandIsProven(true, ShipDeckCore.MaxStandDrop));

            // The probe starts ABOVE the candidate, so a hit slightly above it is legal too.
            Assert.IsTrue(ShipDeckCore.StandIsProven(true, -0.05f));
        }

        [Test]
        public void ProbeReachesFurtherThanTheDropItIsAllowedToAccept()
        {
            Assert.Greater(ShipDeckCore.StandProbeLength, ShipDeckCore.MaxStandDrop,
                "a probe that stops short would reject decks it should accept");
        }
    }
}
