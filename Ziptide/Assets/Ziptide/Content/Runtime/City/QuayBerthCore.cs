using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// THE HANGAR WALK, as arithmetic (LEVEL1_SPATIAL_SCRIPT §3: "Berths 1–5 line the quay westward
    /// at 14 m spacing"). Pure so the one thing that can actually go wrong here — two coplanar decks
    /// overlapping and z-fighting across the whole quay — is a proven property instead of a number
    /// somebody eyeballed in a patcher.
    ///
    /// The berths are EMPTY on purpose. Berth six is yours and it is the only one with a ship in it;
    /// walking west past five empty slips on the way to the job is how the level says, without a
    /// line of dialogue, that there used to be a fleet here. It is also the runway the beacon thread
    /// descends when the joined key starts pulling you home.
    ///
    /// Everything is measured off the LIVE layout's berth (centre + width), never off the design
    /// doc's coordinates, so moving the shipyard moves the walk with it.
    /// </summary>
    public static class QuayBerthCore
    {
        /// <summary>Centre-to-centre spacing of the quay's berths (m) — the script's number.</summary>
        public const float Spacing = 14f;
        /// <summary>Deck width of an empty berth (m). Narrower than berth six, which holds a ship.</summary>
        public const float PadWidth = 12f;
        /// <summary>Deck depth of an empty berth (m).</summary>
        public const float PadDepth = 14f;
        /// <summary>Clear air between berth six's deck edge and the first empty deck (m).</summary>
        public const float EdgeMargin = 2f;
        /// <summary>Berths 1–5; berth six is the player's and is built by the shipyard.</summary>
        public const int BerthCount = 5;

        /// <summary>
        /// Deck centres for berths 1..<see cref="BerthCount"/>, running WEST from the player's berth.
        /// Index 0 is the berth nearest berth six (berth five); the last index is berth one, the far
        /// end of the walk. The first deck starts clear of berth six's own edge, then the script's
        /// 14 m rhythm carries the rest.
        /// </summary>
        public static List<Vector3> PadCentres(Vector3 berthCentre, float berthWidth)
            => PadCentres(berthCentre, berthWidth, Spacing, PadWidth);

        /// <summary>Spacing/width overload — exists so the disjointness check below can be proven
        /// to FAIL on bad geometry. With the shipped constants the layout is disjoint by
        /// construction, which makes a guard that only ever sees those constants untestable.</summary>
        public static List<Vector3> PadCentres(Vector3 berthCentre, float berthWidth,
            float spacing, float padWidth)
        {
            var pads = new List<Vector3>(BerthCount);
            // East edge of the first empty deck sits EdgeMargin clear of berth six's west edge.
            float firstCentreX = berthCentre.x - (berthWidth * 0.5f) - EdgeMargin - (padWidth * 0.5f);
            for (int i = 0; i < BerthCount; i++)
                pads.Add(new Vector3(firstCentreX - i * spacing, berthCentre.y, berthCentre.z));
            return pads;
        }

        /// <summary>Berth NUMBER for a pad index: index 0 is nearest berth six, so it is berth five.</summary>
        public static int BerthNumberAt(int index) => BerthCount - index;

        /// <summary>
        /// True when no two decks share floor space, and none of them overlaps berth six. Coplanar
        /// overlapping slabs z-fight, which on device reads as a broken quay rather than a tight one.
        /// Pinned by QuayBerthCoreTests.
        /// </summary>
        public static bool DecksAreDisjoint(Vector3 berthCentre, float berthWidth)
            => DecksAreDisjoint(berthCentre, berthWidth, Spacing, PadWidth);

        /// <summary>Spacing/width overload. The failure this detects is spacing narrower than a
        /// deck: berth width cannot cause an overlap, because the pads are placed relative to the
        /// berth's own edge and simply move west with it.</summary>
        public static bool DecksAreDisjoint(Vector3 berthCentre, float berthWidth,
            float spacing, float padWidth)
        {
            var pads = PadCentres(berthCentre, berthWidth, spacing, padWidth);
            float berthMinX = berthCentre.x - berthWidth * 0.5f;

            for (int i = 0; i < pads.Count; i++)
            {
                float maxX = pads[i].x + padWidth * 0.5f;
                if (maxX > berthMinX) return false; // runs into the player's berth

                for (int j = i + 1; j < pads.Count; j++)
                    if (Mathf.Abs(pads[i].x - pads[j].x) < padWidth) return false;
            }
            return true;
        }
    }
}
