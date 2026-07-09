using System;
using System.Collections.Generic;

namespace Ziptide.Content.Ship
{
    /// <summary>The six silhouette classes (SHIP_FORGE spec: "≥6 chassis across silhouettes").</summary>
    public enum ShipSilhouette { Interceptor, Hauler, Gunship, Explorer, Salvager, Racer }

    public enum ShipSlotKind { Engine, Wings, Hardpoint, CargoPod }

    /// <summary>Resolved ship performance — PURE numbers, no Unity. The hangar's holo-readout displays
    /// these; the FlightModel feed is the coordinated seam with the flight lane (ShipDefinition carries
    /// cruiseSpeed/boostMultiplier/turnRateDegrees — an adapter maps these once both lanes agree).</summary>
    public struct ShipStats
    {
        public float Speed;      // cruise, m/s
        public float Handling;   // turn responsiveness 0..10
        public float Boost;      // boost multiplier
        public float Cargo;      // holster/cargo capacity units
        public float Armor;      // hull HP-scale units
    }

    /// <summary>A chassis as PURE data (the BotProfileData idiom — code presets are the fallback truth;
    /// SO authoring mirrors them). Proportions drive the parametric assembler; stats are the base the
    /// loadout modifies.</summary>
    public sealed class ShipChassisPreset
    {
        public string Id;
        public string DisplayName;
        public ShipSilhouette Silhouette;
        public ShipStats BaseStats;
        // Assembler proportions (unit-relative; the assembler multiplies by hull size).
        public float FuselageLength = 1f;    // 1 = the classic berth hull
        public float FuselageGirth = 1f;
        public float WingSpan = 1f;
        public float WingSweepDeg = 20f;
        public float NacelleCount = 2f;      // engines per side pair (1 or 2)
        public float FinHeight = 1f;
        public string[] SlotIds;             // which module slots this chassis exposes

        public static readonly ShipChassisPreset[] All =
        {
            new ShipChassisPreset { Id = "interceptor", DisplayName = "Interceptor",
                Silhouette = ShipSilhouette.Interceptor,
                BaseStats = new ShipStats { Speed = 34, Handling = 8.5f, Boost = 2.2f, Cargo = 2, Armor = 4 },
                FuselageLength = 1.15f, FuselageGirth = 0.7f, WingSpan = 1.1f, WingSweepDeg = 38f,
                NacelleCount = 1, FinHeight = 1.2f,
                SlotIds = new[] { "engine", "wings", "hardpoint" } },
            new ShipChassisPreset { Id = "hauler", DisplayName = "Hauler",
                Silhouette = ShipSilhouette.Hauler,
                BaseStats = new ShipStats { Speed = 20, Handling = 4f, Boost = 1.5f, Cargo = 10, Armor = 8 },
                FuselageLength = 1.25f, FuselageGirth = 1.5f, WingSpan = 0.7f, WingSweepDeg = 8f,
                NacelleCount = 2, FinHeight = 0.7f,
                SlotIds = new[] { "engine", "cargo", "cargo2" } },
            new ShipChassisPreset { Id = "gunship", DisplayName = "Gunship",
                Silhouette = ShipSilhouette.Gunship,
                BaseStats = new ShipStats { Speed = 24, Handling = 5.5f, Boost = 1.7f, Cargo = 4, Armor = 10 },
                FuselageLength = 1.0f, FuselageGirth = 1.2f, WingSpan = 0.95f, WingSweepDeg = 14f,
                NacelleCount = 2, FinHeight = 0.9f,
                SlotIds = new[] { "engine", "hardpoint", "hardpoint2", "wings" } },
            new ShipChassisPreset { Id = "explorer", DisplayName = "Explorer",
                Silhouette = ShipSilhouette.Explorer,
                BaseStats = new ShipStats { Speed = 27, Handling = 6.5f, Boost = 1.9f, Cargo = 6, Armor = 6 },
                FuselageLength = 1.1f, FuselageGirth = 1.0f, WingSpan = 1.25f, WingSweepDeg = 5f,
                NacelleCount = 1, FinHeight = 1.0f,
                SlotIds = new[] { "engine", "wings", "cargo" } },
            new ShipChassisPreset { Id = "salvager", DisplayName = "Salvager",
                Silhouette = ShipSilhouette.Salvager,
                BaseStats = new ShipStats { Speed = 22, Handling = 5f, Boost = 1.6f, Cargo = 8, Armor = 7 },
                FuselageLength = 0.95f, FuselageGirth = 1.35f, WingSpan = 0.8f, WingSweepDeg = 0f,
                NacelleCount = 2, FinHeight = 0.6f,
                SlotIds = new[] { "engine", "cargo", "hardpoint" } },
            new ShipChassisPreset { Id = "racer", DisplayName = "Racer",
                Silhouette = ShipSilhouette.Racer,
                BaseStats = new ShipStats { Speed = 40, Handling = 7.5f, Boost = 2.6f, Cargo = 1, Armor = 3 },
                FuselageLength = 1.3f, FuselageGirth = 0.55f, WingSpan = 0.9f, WingSweepDeg = 48f,
                NacelleCount = 1, FinHeight = 1.4f,
                SlotIds = new[] { "engine", "wings" } },
        };

        public static ShipChassisPreset Find(string id)
        {
            foreach (var c in All) if (c.Id == id) return c;
            return All[0]; // interceptor is the default ride
        }
    }

    /// <summary>A swappable module as PURE data: a slot kind + stat DELTAS (the NMS/Star Citizen model —
    /// modules change numbers, wraps never do).</summary>
    public sealed class ShipModulePreset
    {
        public string Id;
        public string DisplayName;
        public ShipSlotKind Kind;
        public float DSpeed, DHandling, DBoost, DCargo, DArmor;

        public static readonly ShipModulePreset[] All =
        {
            // Engines — the speed/boost axis.
            new ShipModulePreset { Id = "engine_stock", DisplayName = "Stock Drive", Kind = ShipSlotKind.Engine },
            new ShipModulePreset { Id = "engine_tide", DisplayName = "Tide Drive", Kind = ShipSlotKind.Engine,
                DSpeed = 6, DBoost = 0.3f, DHandling = -0.5f },
            new ShipModulePreset { Id = "engine_ion", DisplayName = "Ion Whisper", Kind = ShipSlotKind.Engine,
                DSpeed = -2, DHandling = 1.5f },
            // Wings — the handling axis.
            new ShipModulePreset { Id = "wings_stock", DisplayName = "Stock Foils", Kind = ShipSlotKind.Wings },
            new ShipModulePreset { Id = "wings_razor", DisplayName = "Razor Foils", Kind = ShipSlotKind.Wings,
                DHandling = 1.2f, DArmor = -1 },
            new ShipModulePreset { Id = "wings_bulwark", DisplayName = "Bulwark Foils", Kind = ShipSlotKind.Wings,
                DHandling = -0.8f, DArmor = 3 },
            // Hardpoints — armor/combat trim (weapons mount here when SPACE_COMBAT lands).
            new ShipModulePreset { Id = "hardpoint_stock", DisplayName = "Bare Mount", Kind = ShipSlotKind.Hardpoint },
            new ShipModulePreset { Id = "hardpoint_plated", DisplayName = "Plated Mount", Kind = ShipSlotKind.Hardpoint,
                DArmor = 2, DSpeed = -1 },
            // Cargo pods — the capacity axis.
            new ShipModulePreset { Id = "cargo_stock", DisplayName = "Stock Pod", Kind = ShipSlotKind.CargoPod },
            new ShipModulePreset { Id = "cargo_long", DisplayName = "Longhaul Pod", Kind = ShipSlotKind.CargoPod,
                DCargo = 4, DHandling = -0.6f },
        };

        public static ShipModulePreset Find(string id)
        {
            foreach (var m in All) if (m.Id == id) return m;
            return null;
        }
    }

    /// <summary>
    /// PURE loadout resolution (2.2's heart): chassis base + module deltas → final ShipStats, floored
    /// so no combination bricks a ship. WRAPS ARE NOT AN INPUT — the cosmetic layer can't touch these
    /// numbers by construction (the "wrap leaves stats invariant" golden law, provable because the
    /// function signature has nowhere to pass a wrap).
    /// </summary>
    public static class ShipLoadoutCore
    {
        public static ShipStats Resolve(ShipChassisPreset chassis, IEnumerable<ShipModulePreset> modules)
        {
            if (chassis == null) chassis = ShipChassisPreset.Find(null);
            var s = chassis.BaseStats;
            if (modules != null)
            {
                foreach (var m in modules)
                {
                    if (m == null) continue;
                    s.Speed += m.DSpeed;
                    s.Handling += m.DHandling;
                    s.Boost += m.DBoost;
                    s.Cargo += m.DCargo;
                    s.Armor += m.DArmor;
                }
            }
            // Floors: every ship must still fly, steer, and survive one hit.
            s.Speed = Math.Max(8f, s.Speed);
            s.Handling = Math.Max(1f, s.Handling);
            s.Boost = Math.Max(1.1f, s.Boost);
            s.Cargo = Math.Max(0f, s.Cargo);
            s.Armor = Math.Max(1f, s.Armor);
            return s;
        }
    }

    /// <summary>
    /// JOURNEY DECALS (the innovation swing): the hull ACCUMULATES the player's story — a pure
    /// function of profile flags → an ordered list of decal ids, so the ship is a wearable save file.
    /// Deterministic (same flags = same hull, everywhere, forever), capped so a long campaign reads as
    /// a veteran's hull, not noise. Milestones only — a decal must MEAN something.
    /// </summary>
    public static class ShipJourneyDecals
    {
        public const int MaxDecals = 10;

        // flag → decal id, in the order they'd appear along the hull (story order).
        private static readonly (string flag, string decal)[] Milestones =
        {
            ("W001_COMPLETE", "decal_first_contract"),
            ("C2_CONTAINMENT_REVEALED", "decal_cage_sighted"),
            ("W012_COMPLETE", "decal_maras_jump"),
            ("FRAGMENT_T1_FOUND", "decal_first_fragment"),
            ("C3_W019_RILL_REFUSED", "decal_the_refusal"),
            ("C4_W024_COLOR_NAMED", "decal_the_color"),
            ("C5_W037_WARDEN_STANDOFF", "decal_warden_eye"),
            ("C6_W051_RILL_NAMED", "decal_the_naming"),
            ("C8_W062_REVELATION", "decal_the_revelation"),
            ("C12_W068_COMPLETE", "decal_journeys_end"),
        };

        /// <summary>Ordered decal ids earned by this flag set (null-safe, capped).</summary>
        public static List<string> FromFlags(ICollection<string> flags)
        {
            var earned = new List<string>();
            if (flags == null) return earned;
            foreach (var (flag, decal) in Milestones)
            {
                if (earned.Count >= MaxDecals) break;
                if (flags.Contains(flag)) earned.Add(decal);
            }
            return earned;
        }
    }
}
