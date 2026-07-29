using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>What the ship is currently too close to. Ordered loosely by how much it matters.</summary>
    public enum FlightBoundKind
    {
        None = 0,
        Corridor,   // outside the SWEPT LANE — the only kind that never corrects you (wander freely)
        Structure,  // a hull: debris, a tender, the dock, the hauler
        Gate,       // inside a catch ring's slab but off its axis — you are about to clip the truss
        Ground,     // a floor exists below and you are dropping onto it
        Deep,       // the outer sphere: past here is nothing but float-jitter
    }

    /// <summary>
    /// How hard the game answers. ADVISORY is words only — the ship still does exactly what the
    /// pilot told it. CORRECTING adds a gentle push and bleeds speed. HARD is the last resort.
    /// The whole point of the ladder is that you are TOLD before anything takes the stick off you.
    /// </summary>
    public enum FlightBoundLevel { Clear = 0, Advisory = 1, Correcting = 2, Hard = 3 }

    /// <summary>Thresholds for every bound class. Metres unless the name says fraction.</summary>
    [Serializable]
    public struct FlightBoundsParams
    {
        // The swept lane: a tube around the ring polyline. Advisory ONLY, by design — Terry wants
        // "a clear path to play but also room to wander and explore", so straying is legal and
        // commented on, never fought.
        public float corridorRadius;

        // Hulls. Clearance is surface-to-ship, so a big wreck and a small pod read the same way.
        // Deliberately tight: the debris field is authored to stand off ~14 m from the swept lane,
        // so anything looser would have RILL narrating the ordinary act of flying down the middle.
        public float structureAdvisory, structureCorrecting, structureHard;

        // A catch ring is a 12 m bore. Fill01 = radial distance / bore radius: 0 is dead centre,
        // 1 is touching the rim. Only counts while the ship is inside the ring's slab.
        public float gateSlabHalfDepth;
        public float gateAdvisoryFill, gateCorrectingFill;

        // A floor. Off in the space lane (there isn't one); on for any atmospheric leg.
        public bool hasGround;
        public float groundY, groundAdvisory, groundCorrecting, groundHard;

        // The outer sphere. FlightModel already hard-clamps at laneRadius; these tiers exist so
        // the clamp is never the first thing the pilot learns about.
        public float laneRadius, deepAdvisoryFraction, deepCorrectingFraction;

        // Response strength.
        public float correctionSpeed;  // m/s of sideways push at full severity while Correcting
        public float speedBleed;       // fraction of speed shed per second at full severity

        public static FlightBoundsParams Default => new FlightBoundsParams
        {
            corridorRadius = 90f,
            structureAdvisory = 10f, structureCorrecting = 5f, structureHard = 2f,
            gateSlabHalfDepth = 4f, gateAdvisoryFill = 0.72f, gateCorrectingFill = 0.88f,
            hasGround = false, groundY = 0f,
            groundAdvisory = 60f, groundCorrecting = 25f, groundHard = 8f,
            laneRadius = FlightParams.Default.laneRadius,
            deepAdvisoryFraction = 0.72f, deepCorrectingFraction = 0.90f,
            correctionSpeed = 9f, speedBleed = 0.55f,
        };
    }

    /// <summary>
    /// The measured world around the ship, already reduced to numbers — so the evaluation itself
    /// never touches a collider, a Transform or a scene. The translator fills this in; the core
    /// decides what it means.
    /// </summary>
    public struct FlightBoundsSample
    {
        /// <summary>Metres from the swept-lane polyline. PositiveInfinity = no corridor authored.</summary>
        public float CorridorDistance;

        /// <summary>Free space to the nearest hull SURFACE. PositiveInfinity = nothing near.</summary>
        public float StructureClearance;

        /// <summary>Unit vector pointing from that hull toward the ship (the escape direction).</summary>
        public Vector3 StructureAway;

        /// <summary>Radial position inside the nearest ring bore, 0 (centred) .. 1 (on the rim).
        /// Negative = not inside any ring's slab.</summary>
        public float GateFill01;

        /// <summary>Unit vector from the ring's axis toward the ship — the core pushes along −this.</summary>
        public Vector3 GateOutward;

        public static FlightBoundsSample Empty => new FlightBoundsSample
        {
            CorridorDistance = float.PositiveInfinity,
            StructureClearance = float.PositiveInfinity,
            StructureAway = Vector3.zero,
            GateFill01 = -1f,
            GateOutward = Vector3.zero,
        };
    }

    /// <summary>The verdict for one frame.</summary>
    public struct FlightBoundsReading
    {
        public FlightBoundKind Kind;
        public FlightBoundLevel Level;

        /// <summary>0 at the tier's own threshold, 1 at the next tier's — how far in you are.</summary>
        public float Severity01;

        /// <summary>Unit direction the correction pushes, or zero when nothing is pushing.</summary>
        public Vector3 Push;

        public bool IsClear => Level == FlightBoundLevel.Clear;
    }

    /// <summary>
    /// THE BOUNDS LADDER — what happens when you fly somewhere you should not (Terry, 2026-07-29:
    /// *"if it gets too close to a building, too close to the ground… some sort of auto correction
    /// if you leave the zone and then some sort of message from Rill"*).
    ///
    /// Pure and deterministic, like <see cref="FlightModel"/>, and separate from it on purpose:
    /// FlightModel is the ship obeying the pilot, this is the world objecting. Three laws hold:
    /// 1. Nothing is ever taken from the pilot without a warning tier first.
    /// 2. Straying off the swept lane is ADVISORY forever — exploring is not a mistake.
    /// 3. Every response is a translation. Nothing here rotates the ship, so no correction can
    ///    ever spin a headset (comfort law: yaw is snap-only, and only the player snaps it).
    /// </summary>
    public static class FlightBoundsCore
    {
        /// <summary>Distance from a point to a polyline, with the LAST segment extended forever —
        /// past the final ring the corridor keeps running out into the Overrun, so flying onward
        /// is never treated as straying. Behind the first point it does not extend: turning around
        /// and leaving out the back IS straying.</summary>
        public static float CorridorDistance(Vector3 p, Vector3[] path)
        {
            if (path == null || path.Length == 0) return float.PositiveInfinity;
            if (path.Length == 1) return Vector3.Distance(p, path[0]);

            float best = float.PositiveInfinity;
            for (int i = 0; i < path.Length - 1; i++)
            {
                Vector3 a = path[i], b = path[i + 1];
                Vector3 ab = b - a;
                float len2 = ab.sqrMagnitude;
                if (len2 < 1e-6f) continue;
                float t = Vector3.Dot(p - a, ab) / len2;
                bool last = i == path.Length - 2;
                t = last ? Mathf.Max(t, 0f) : Mathf.Clamp01(t);
                best = Mathf.Min(best, Vector3.Distance(p, a + ab * t));
            }
            return best;
        }

        /// <summary>
        /// The facing of node <paramref name="index"/> on a polyline: the bisector of its incoming
        /// and outgoing segments (a single segment at the ends). The catch rings USE this — a ring
        /// square to the trajectory is the whole reason a line of rings reads as a trajectory —
        /// and the bounds math reads the same function, so the hole you aim at and the hole the
        /// game measures can never drift apart.
        /// </summary>
        public static Vector3 PathAxis(Vector3[] path, int index)
        {
            if (path == null || path.Length < 2) return Vector3.forward;
            index = Mathf.Clamp(index, 0, path.Length - 1);

            Vector3 incoming = index > 0 ? (path[index] - path[index - 1]) : Vector3.zero;
            Vector3 outgoing = index < path.Length - 1 ? (path[index + 1] - path[index]) : Vector3.zero;
            if (incoming.sqrMagnitude > 1e-6f) incoming.Normalize(); else incoming = Vector3.zero;
            if (outgoing.sqrMagnitude > 1e-6f) outgoing.Normalize(); else outgoing = Vector3.zero;

            Vector3 axis = incoming + outgoing;
            return axis.sqrMagnitude < 1e-6f ? Vector3.forward : axis.normalized;
        }

        /// <summary>Where the ship sits inside a ring's bore, if it is inside that ring's slab at
        /// all. fill01 = 0 dead centre, 1 on the rim, &gt;1 already overlapping the truss.</summary>
        public static bool TryGateFill(Vector3 p, Vector3 ringCenter, Vector3 ringAxis,
            float boreRadius, float slabHalfDepth, out float fill01, out Vector3 outward)
        {
            fill01 = -1f;
            outward = Vector3.zero;
            if (boreRadius <= 0.01f) return false;
            Vector3 axis = ringAxis.sqrMagnitude < 1e-6f ? Vector3.forward : ringAxis.normalized;

            Vector3 delta = p - ringCenter;
            float along = Vector3.Dot(delta, axis);
            if (Mathf.Abs(along) > slabHalfDepth) return false;

            Vector3 radial = delta - axis * along;
            float r = radial.magnitude;
            fill01 = r / boreRadius;
            outward = r > 1e-4f ? radial / r : Vector3.zero;
            return true;
        }

        /// <summary>Tier a shrinking clearance. Smaller is worse; severity runs 0→1 across the tier.</summary>
        private static FlightBoundLevel FromClearance(float clearance, float adv, float cor, float hard,
            out float severity)
        {
            severity = 0f;
            if (float.IsPositiveInfinity(clearance) || clearance > adv) return FlightBoundLevel.Clear;
            if (clearance > cor)
            {
                severity = Mathf.InverseLerp(adv, cor, clearance);
                return FlightBoundLevel.Advisory;
            }
            if (clearance > hard)
            {
                severity = Mathf.InverseLerp(cor, hard, clearance);
                return FlightBoundLevel.Correcting;
            }
            severity = 1f;
            return FlightBoundLevel.Hard;
        }

        /// <summary>Tier a growing quantity (fill, radius). Bigger is worse.</summary>
        private static FlightBoundLevel FromEncroachment(float value, float adv, float cor, float hard,
            out float severity)
        {
            severity = 0f;
            if (value < adv) return FlightBoundLevel.Clear;
            if (value < cor)
            {
                severity = Mathf.InverseLerp(adv, cor, value);
                return FlightBoundLevel.Advisory;
            }
            if (value < hard)
            {
                severity = Mathf.InverseLerp(cor, hard, value);
                return FlightBoundLevel.Correcting;
            }
            severity = 1f;
            return FlightBoundLevel.Hard;
        }

        private static void Consider(ref FlightBoundsReading best, FlightBoundKind kind,
            FlightBoundLevel level, float severity, Vector3 push)
        {
            if (level == FlightBoundLevel.Clear) return;
            if (level < best.Level) return;
            if (level == best.Level && severity <= best.Severity01) return;
            best.Kind = kind;
            best.Level = level;
            best.Severity01 = Mathf.Clamp01(severity);
            best.Push = push;
        }

        /// <summary>Read the world once. The worst tier wins; ties go to the deeper severity.</summary>
        public static FlightBoundsReading Evaluate(Vector3 position, FlightBoundsSample sample,
            FlightBoundsParams p)
        {
            var best = new FlightBoundsReading { Kind = FlightBoundKind.None, Level = FlightBoundLevel.Clear };

            // Corridor — advisory forever, and it never contributes a push. Wandering is allowed.
            if (!float.IsPositiveInfinity(sample.CorridorDistance)
                && p.corridorRadius > 0f && sample.CorridorDistance > p.corridorRadius)
            {
                float sev = Mathf.Clamp01((sample.CorridorDistance - p.corridorRadius)
                                          / Mathf.Max(1f, p.corridorRadius));
                Consider(ref best, FlightBoundKind.Corridor, FlightBoundLevel.Advisory, sev, Vector3.zero);
            }

            // Structure.
            var lvl = FromClearance(sample.StructureClearance, p.structureAdvisory,
                p.structureCorrecting, p.structureHard, out float sevS);
            Consider(ref best, FlightBoundKind.Structure, lvl, sevS, sample.StructureAway.normalized);

            // Gate — push back toward the bore's axis, which is also the direction you wanted anyway.
            if (sample.GateFill01 >= 0f)
            {
                lvl = FromEncroachment(sample.GateFill01, p.gateAdvisoryFill, p.gateCorrectingFill,
                    1f, out float sevG);
                Consider(ref best, FlightBoundKind.Gate, lvl, sevG, -sample.GateOutward.normalized);
            }

            // Ground.
            if (p.hasGround)
            {
                float clearance = position.y - p.groundY;
                lvl = FromClearance(clearance, p.groundAdvisory, p.groundCorrecting, p.groundHard,
                    out float sevY);
                Consider(ref best, FlightBoundKind.Ground, lvl, sevY, Vector3.up);
            }

            // Deep — the outer sphere. Push is straight back toward the middle of everything.
            if (p.laneRadius > 0f)
            {
                float r = position.magnitude;
                lvl = FromEncroachment(r, p.laneRadius * p.deepAdvisoryFraction,
                    p.laneRadius * p.deepCorrectingFraction, p.laneRadius, out float sevD);
                Vector3 inward = r > 1e-4f ? -position / r : Vector3.zero;
                Consider(ref best, FlightBoundKind.Deep, lvl, sevD, inward);
            }

            return best;
        }

        /// <summary>
        /// Apply the correction. ADVISORY returns the state untouched — the words are the whole
        /// response. CORRECTING and HARD translate the ship away and bleed speed; HARD is simply
        /// the same push at double authority, because a correction that ever ROTATED the ship
        /// would move the horizon under a player who did not ask for it.
        /// </summary>
        public static FlightState Apply(FlightState s, FlightBoundsReading r, FlightBoundsParams p,
            float dt)
        {
            if (dt <= 0f || float.IsNaN(dt)) return s;
            if (r.Level < FlightBoundLevel.Correcting) return s;
            if (r.Push.sqrMagnitude < 1e-6f) return s;

            float authority = r.Level == FlightBoundLevel.Hard ? 2f : 1f;
            float sev = Mathf.Max(0.15f, r.Severity01); // entering the tier already does something
            s.position += r.Push.normalized * (p.correctionSpeed * authority * sev * dt);
            s.speed -= s.speed * Mathf.Clamp01(p.speedBleed * authority * sev * dt);
            return s;
        }
    }
}
