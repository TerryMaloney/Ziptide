namespace Ziptide.Core
{
    /// <summary>What the ship's ramp should do about the player right now.</summary>
    public enum DepartureVerdict
    {
        /// <summary>At least one weapon is on the belt. Go.</summary>
        Armed,

        /// <summary>Weapons are on the rack and none are on the player. Say so, hold the ramp.</summary>
        HoldUnarmed,

        /// <summary>Carrying one but not holstered — the specific thing new players get wrong.</summary>
        HoldInHandOnly,

        /// <summary>Nothing exists to take. The ramp OPENS — see the no-trap law below.</summary>
        ArmedNothingToTake,
    }

    /// <summary>
    /// THE SHIP IS THE ARMOURY (⚖ Terry): weapons live on a rack aboard your ship, you take one
    /// before you go out, and it has to be on your BELT — not in your fist — before the ramp lets
    /// you leave. The ship stops being a corridor with a helm in it and becomes the place you kit up.
    ///
    /// This replaces the shipped strategy, which was two guns dropped on the ground at the player's
    /// feet in the Dispatch plaza, and four of the eight authored weapons never placed anywhere at
    /// all.
    ///
    /// ── THE NO-TRAP LAW ──
    /// A gate that can refuse forever is a soft-lock, and this one sits between the player and the
    /// entire game. So the verdict is never "no" when there is nothing the player could do about it:
    /// if the rack is empty and their belt is empty, the ramp OPENS. A missing-item bug then costs a
    /// confusing walk, not a dead save. This is the same instinct as the canal stalker's six-second
    /// block yield — pressure is allowed, traps are not.
    ///
    /// Pure because the whole design is in the verdict table: which state teaches, which state
    /// blocks, and which state must never block. A headset session can tell you the wording is
    /// wrong; only a test can tell you the gate can't strand someone.
    /// </summary>
    public static class ShipArmouryCore
    {
        /// <summary>Weapon slots on the rack. Eight — the full authored arsenal fits, so a weapon
        /// gained later always has a home and the rack visibly fills over the campaign.</summary>
        public const int RackSlots = 8;

        /// <summary>Horizontal spacing between rack slots, metres — a reach apart, not a wall.</summary>
        public const float SlotSpacing = 0.34f;

        /// <summary>Height of the rack rail above the deck, metres. Mid-chest for a seated-height
        /// player so nothing needs a crouch or a stretch (INTERACTION reach law).</summary>
        public const float RailHeight = 1.15f;

        /// <summary>Cue the companion speaks for a verdict, or empty when she should stay quiet.</summary>
        public static string CueFor(DepartureVerdict verdict)
        {
            switch (verdict)
            {
                case DepartureVerdict.HoldUnarmed: return "ARM_YOURSELF";
                case DepartureVerdict.HoldInHandOnly: return "BELT_IT";
                default: return "";
            }
        }

        /// <summary>True when the ramp should physically refuse to open.</summary>
        public static bool Blocks(DepartureVerdict verdict) =>
            verdict == DepartureVerdict.HoldUnarmed || verdict == DepartureVerdict.HoldInHandOnly;

        /// <summary>
        /// The verdict. <paramref name="weaponsOnBelt"/> is what is holstered, <paramref name="weaponsInHands"/>
        /// what is gripped right now, and <paramref name="weaponsReachable"/> what the player could
        /// still take (rack contents plus anything loose aboard).
        /// </summary>
        public static DepartureVerdict Evaluate(int weaponsOnBelt, int weaponsInHands, int weaponsReachable)
        {
            if (weaponsOnBelt > 0) return DepartureVerdict.Armed;

            // Nothing on the belt, nothing in hand, and nothing to pick up: opening is the only
            // honest answer. Blocking here would punish the player for a content bug.
            if (weaponsInHands <= 0 && weaponsReachable <= 0) return DepartureVerdict.ArmedNothingToTake;

            if (weaponsInHands > 0) return DepartureVerdict.HoldInHandOnly;
            return DepartureVerdict.HoldUnarmed;
        }

        /// <summary>Local X of rack slot <paramref name="index"/>, centred on the rail.</summary>
        public static float SlotLocalX(int index)
        {
            float span = (RackSlots - 1) * SlotSpacing;
            return -span * 0.5f + index * SlotSpacing;
        }
    }
}
