namespace Ziptide.Core
{
    /// <summary>
    /// WHICH HIP A THING BELONGS ON.
    ///
    /// Terry, 2026-08-01: *"lets have sword autoset to the left holster so its never in your way and
    /// shift any other weapons or items to the slots on the right."*
    ///
    /// Before this, all three belt sockets accepted everything, so where a weapon ended up was
    /// decided by whichever socket the hand happened to be nearest when it let go. That is why the
    /// blade — the longest thing on the belt, and the only one that sweeps a metre of arc when you
    /// reach past it — kept landing wherever it liked.
    ///
    /// The rule is a rule, not a preference: melee goes LEFT, everything else goes RIGHT or CENTRE.
    /// A right-handed draw crosses to the left hip for the blade (the historical baldric side, and
    /// the side your gun hand is not on), and the guns sit where the gun hand already is.
    ///
    /// THE NO-TRAP LAW applies here as it does at the ship's ramp: an unnamed socket (a test rig, a
    /// future socket, a scene that hand-built its belt) accepts everything, because a belt that
    /// silently refuses your gear is worse than a belt that is untidy.
    /// </summary>
    public static class HolsterAssignmentCore
    {
        public const string LeftSocket = "HolsterLeft";
        public const string RightSocket = "HolsterRight";
        public const string CentreSocket = "HolsterCenter";

        /// <summary>True for the contact weapons that ride the left hip.</summary>
        public static bool IsMelee(string itemId)
        {
            return itemId == "breaker_blade" || itemId == "tide_pike";
        }

        /// <summary>The socket name this item should end up on.</summary>
        public static string PreferredSocket(string itemId)
        {
            return IsMelee(itemId) ? LeftSocket : RightSocket;
        }

        /// <summary>
        /// May this socket hold this item? Names are matched loosely (contains "left"/"right") so a
        /// renamed or nested socket still reads correctly; anything unrecognised accepts everything.
        /// </summary>
        public static bool SocketAccepts(string socketName, string itemId)
        {
            SocketSide side = SideOf(socketName);
            if (side == SocketSide.Unknown) return true;   // the no-trap law
            return IsMelee(itemId) ? side == SocketSide.Left : side != SocketSide.Left;
        }

        public enum SocketSide { Unknown = 0, Left = 1, Right = 2, Centre = 3 }

        public static SocketSide SideOf(string socketName)
        {
            if (string.IsNullOrEmpty(socketName)) return SocketSide.Unknown;
            string lower = socketName.ToLowerInvariant();
            if (lower.Contains("left")) return SocketSide.Left;
            if (lower.Contains("right")) return SocketSide.Right;
            if (lower.Contains("center") || lower.Contains("centre")) return SocketSide.Centre;
            return SocketSide.Unknown;
        }
    }
}
