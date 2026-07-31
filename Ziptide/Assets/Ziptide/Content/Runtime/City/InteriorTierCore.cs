using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>How much interior a building actually gets.</summary>
    public enum InteriorTier
    {
        /// <summary>Sealed. Lit windows, no door you can pass. Most of a city is this.</summary>
        Facade,

        /// <summary>One dressed room deep. You can step in, look, and step out — no partitioning.</summary>
        Vestibule,

        /// <summary>Partitioned, furnished, walk-through. The buildings the game is played inside.</summary>
        Full,
    }

    /// <summary>
    /// WHICH buildings get interiors — ⚖ Terry: *"we don't obviously need to make every single
    /// building playable all the way through … the first and last level will probably have the most
    /// interiors."*
    ///
    /// That constraint is the design, not a compromise. A city where every door opens costs a Quest
    /// its frame budget and gives the player no way to tell which door matters; a city where none do
    /// is a film set. So interiors are RATIONED, and the ration is spent where the game is played:
    ///
    ///   • <b>Full</b> — anything the route needs (a job giver, a mission beat). Always, regardless of
    ///     budget: a contract you cannot walk into is a broken level, and MISS_LEDGER #21 is what that
    ///     costs. This tier is uncapped BY DESIGN.
    ///   • <b>Vestibule</b> — bought with the per-district budget, biggest footprints first. One room,
    ///     dressed, no partitioning: most of the felt life of an interior for a fraction of the cost.
    ///   • <b>Facade</b> — everything else, and that is the honest majority.
    ///
    /// Pure so the rationing is provable. The failure this prevents is not visual: it is a district
    /// that silently opens forty doors, blows the renderer budget, and turns into a device-only
    /// performance mystery.
    /// </summary>
    public static class InteriorTierCore
    {
        /// <summary>Vestibules a single district may buy. Full-tier route buildings do not count.</summary>
        public const int VestibuleBudgetPerDistrict = 3;

        /// <summary>Below this floor area a building is too small to stand inside comfortably.</summary>
        public const float MinInteriorArea = 12f;

        /// <summary>Floor area at or above which a Full interior is worth partitioning into rooms.</summary>
        public const float PartitionArea = 30f;

        /// <summary>
        /// The tier for one building.
        /// <paramref name="servesRoute"/> is the override that outranks every budget — a building the
        /// contract sends you into is always enterable.
        /// <paramref name="vestibulesAlreadySpent"/> is how many this district has already bought.
        /// </summary>
        public static InteriorTier Evaluate(
            float floorArea,
            bool servesRoute,
            int vestibulesAlreadySpent)
        {
            // Too small to stand in is Facade even for the route — an interior you cannot fit inside
            // is worse than an honest sealed door, and the layout is the thing that should change.
            if (floorArea < MinInteriorArea) return InteriorTier.Facade;

            if (servesRoute) return InteriorTier.Full;

            if (vestibulesAlreadySpent < VestibuleBudgetPerDistrict) return InteriorTier.Vestibule;

            return InteriorTier.Facade;
        }

        /// <summary>True when a Full interior is big enough to be worth cutting into rooms.</summary>
        public static bool ShouldPartition(float floorArea) => floorArea >= PartitionArea;

        /// <summary>Floor area of a footprint, in square metres.</summary>
        public static float AreaOf(Vector2 footprint) =>
            Mathf.Abs(footprint.x) * Mathf.Abs(footprint.y);

        /// <summary>
        /// Interior wall inset from the shell, metres. The furnished floor is smaller than the
        /// building so furniture never intersects the walls the shell already drew.
        /// </summary>
        public const float ShellInset = 0.45f;

        /// <summary>The usable interior rect for a footprint, in local XZ centred on the building.</summary>
        public static Rect UsableFloor(Vector2 footprint)
        {
            float w = Mathf.Max(0f, Mathf.Abs(footprint.x) - ShellInset * 2f);
            float d = Mathf.Max(0f, Mathf.Abs(footprint.y) - ShellInset * 2f);
            return new Rect(-w * 0.5f, -d * 0.5f, w, d);
        }
    }
}
