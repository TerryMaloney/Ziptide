using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>A building lot produced by <see cref="LotPartitioner"/>: a rectangle plus which of its
    /// edges FRONT a street (south/east/north/west in district-local space). The building grammar puts
    /// the door on a fronted edge — that's how "no door into a wall" is true by construction.</summary>
    public struct Lot
    {
        public Rect Bounds;
        public bool FrontS, FrontE, FrontN, FrontW;
        public bool HasFrontage => FrontS || FrontE || FrontN || FrontW;
        public float Area => Bounds.width * Bounds.height;
        public float Aspect => Bounds.width > Bounds.height
            ? (Bounds.height < 0.001f ? float.MaxValue : Bounds.width / Bounds.height)
            : (Bounds.width < 0.001f ? float.MaxValue : Bounds.height / Bounds.width);
    }

    /// <summary>
    /// ARCHITECTURE V2 Q2a (PDF §4.1) — PURE seeded lot partitioning. A district rectangle becomes
    /// building lots + right-of-ways by recursive subdivision along the longer axis, with the report's
    /// base cases: minimum lot area, street frontage (a cut that would landlock a child becomes an
    /// internal street instead — so EVERY lot fronts a street, provable), and an aspect-ratio law
    /// (a lot may only be too-narrow when it was too small to split again). Deterministic from seed:
    /// same inputs → byte-identical lots. No UnityEngine calls beyond Rect/Mathf — EditMode-testable.
    /// </summary>
    public static class LotPartitioner
    {
        public const int MaxDepth = 8;
        /// <summary>Minimum lot side in meters — cuts snap so no sliver thinner than this exists.</summary>
        public const float MinSide = 3f;

        /// <summary>Seeded xorshift (the BotRng recipe) — no UnityEngine.Random, no shared state.</summary>
        private struct Rng
        {
            private uint _s;
            public Rng(int seed) { _s = seed == 0 ? 2463534242u : (uint)seed; }
            public float Next01() { _s ^= _s << 13; _s ^= _s >> 17; _s ^= _s << 5; return (_s & 0xFFFFFF) / (float)0x1000000; }
        }

        /// <summary>
        /// Partition a district rect (local space, XZ as Rect xy) into lots. A perimeter street ring of
        /// <paramref name="streetWidth"/> is reserved first; internal streets appear where needed to
        /// keep every lot fronted. Returns an empty list when the district can't fit one lot.
        /// </summary>
        public static List<Lot> Partition(Rect district, float streetWidth, float minLotArea, float maxAspect, int seed)
        {
            var lots = new List<Lot>();
            if (streetWidth < 0f) streetWidth = 0f;
            if (minLotArea < 1f) minLotArea = 1f;
            if (maxAspect < 1.2f) maxAspect = 1.2f;

            var block = new Rect(district.x + streetWidth, district.y + streetWidth,
                district.width - 2f * streetWidth, district.height - 2f * streetWidth);
            if (block.width < MinSide || block.height < MinSide) return lots;

            var rng = new Rng(seed);
            var root = new Lot { Bounds = block, FrontS = true, FrontE = true, FrontN = true, FrontW = true };
            Split(root, streetWidth, minLotArea, maxAspect, ref rng, lots, 0);
            return lots;
        }

        private static void Split(Lot lot, float streetWidth, float minLotArea, float maxAspect,
            ref Rng rng, List<Lot> outLots, int depth)
        {
            Rect r = lot.Bounds;
            float area = r.width * r.height;
            bool needsAspectFix = lot.Aspect > maxAspect;

            bool canSplit = area >= 2f * minLotArea && depth < MaxDepth
                && Mathf.Max(r.width, r.height) >= 2f * MinSide + streetWidth;
            // Organic early stop (seeded) — but NEVER while the aspect law is violated and fixable.
            if (canSplit && !needsAspectFix && area < 4f * minLotArea && rng.Next01() < 0.25f)
                canSplit = false;

            if (!canSplit) { outLots.Add(lot); return; }

            bool vertical = r.width >= r.height; // cut across the longer axis
            float length = vertical ? r.width : r.height;

            // A plain cut keeps the parent's perpendicular frontages for both children; the child on
            // each side additionally keeps the parent's edge on ITS side. A child ends landlocked only
            // when the parent's sole frontage was the edge the OTHER child keeps — then the cut itself
            // becomes an internal street and both children front it.
            bool aFrontOk, bFrontOk;
            if (vertical)
            {
                aFrontOk = lot.FrontS || lot.FrontN || lot.FrontW;
                bFrontOk = lot.FrontS || lot.FrontN || lot.FrontE;
            }
            else
            {
                aFrontOk = lot.FrontW || lot.FrontE || lot.FrontS;
                bFrontOk = lot.FrontW || lot.FrontE || lot.FrontN;
            }
            bool forcedStreet = !aFrontOk || !bFrontOk; // a plain cut here would landlock a child
            bool streetCut = forcedStreet
                || (depth <= 1 && streetWidth > 0f && rng.Next01() < 0.35f); // big blocks get an avenue

            // Both children must clear MinSide AND minLotArea — req is the shortest legal child length.
            float shortSide = vertical ? r.height : r.width;
            float req = Mathf.Max(MinSide, minLotArea / Mathf.Max(shortSide, 0.001f));

            float gap = streetCut ? streetWidth : 0f;
            float usable = length - gap;
            float lo = Mathf.Max(0.4f, req / usable);
            float hi = Mathf.Min(0.6f, 1f - req / usable);
            if (lo > hi) { lo = req / usable; hi = 1f - req / usable; } // constrained: drop the aesthetic window
            if (lo > hi && streetCut && !forcedStreet)
            {
                // The optional avenue doesn't fit — cut plain instead.
                streetCut = false; gap = 0f; usable = length;
                lo = Mathf.Max(0.4f, req / usable);
                hi = Mathf.Min(0.6f, 1f - req / usable);
                if (lo > hi) { lo = req / usable; hi = 1f - req / usable; }
            }
            if (lo > hi) { outLots.Add(lot); return; } // no legal cut exists
            float cut = usable * (lo + (hi - lo) * rng.Next01());

            Lot a = lot, b = lot;
            if (vertical)
            {
                a.Bounds = new Rect(r.x, r.y, cut, r.height);
                b.Bounds = new Rect(r.x + cut + gap, r.y, usable - cut, r.height);
                a.FrontE = streetCut; b.FrontW = streetCut;
            }
            else
            {
                a.Bounds = new Rect(r.x, r.y, r.width, cut);
                b.Bounds = new Rect(r.x, r.y + cut + gap, r.width, usable - cut);
                a.FrontN = streetCut; b.FrontS = streetCut;
            }

            Split(a, streetWidth, minLotArea, maxAspect, ref rng, outLots, depth + 1);
            Split(b, streetWidth, minLotArea, maxAspect, ref rng, outLots, depth + 1);
        }
    }
}
