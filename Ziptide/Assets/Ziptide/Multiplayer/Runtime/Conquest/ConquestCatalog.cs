using System;

namespace Ziptide.Multiplayer.Conquest
{
    /// <summary>
    /// The build catalogs from docs/10_TIDEFRONT.md — 8 defenses, 8 vessels — as pure data tables (the
    /// resolver honors each special rule by id). Content-side Definition assets mirror these at B2 for
    /// designer tuning; these tables are the tested source of truth for the sim.
    /// Costs are (flux, alloy, bloommatter).
    /// </summary>
    public static class ConquestCatalog
    {
        [Serializable]
        public struct DefenseSpec
        {
            public string Id;
            public int DefenseBonus;
            public int CostFlux, CostAlloy, CostBloom;
            public float ContaminationAdded;  // Bloom Barrier's tradeoff
            public string Special;            // resolver rule hook
        }

        [Serializable]
        public struct VesselSpec
        {
            public string Id;
            public int AttackPower;
            public int CostFlux, CostAlloy, CostBloom;
            public string Special;            // resolver rule hook
        }

        public static readonly DefenseSpec[] Defenses =
        {
            new DefenseSpec { Id = "shield_spire",    DefenseBonus = 3, CostFlux = 1, CostAlloy = 3, CostBloom = 0 },
            new DefenseSpec { Id = "drone_net",       DefenseBonus = 2, CostFlux = 2, CostAlloy = 1, CostBloom = 0 },
            new DefenseSpec { Id = "gate_jammer",     DefenseBonus = 1, CostFlux = 3, CostAlloy = 1, CostBloom = 0, Special = "blocks_gate_attack" },
            new DefenseSpec { Id = "decoy_beacon",    DefenseBonus = 1, CostFlux = 1, CostAlloy = 1, CostBloom = 0, Special = "first_attack_misinfo" },
            new DefenseSpec { Id = "repair_swarm",    DefenseBonus = 1, CostFlux = 1, CostAlloy = 2, CostBloom = 1, Special = "heals_after_battle" },
            new DefenseSpec { Id = "gravity_minefield", DefenseBonus = 2, CostFlux = 2, CostAlloy = 2, CostBloom = 0, Special = "extra_attacker_losses" },
            new DefenseSpec { Id = "resource_vault",  DefenseBonus = 0, CostFlux = 0, CostAlloy = 2, CostBloom = 0, Special = "protects_stock_on_loss" },
            new DefenseSpec { Id = "bloom_barrier",   DefenseBonus = 5, CostFlux = 0, CostAlloy = 1, CostBloom = 3, ContaminationAdded = 0.25f },
        };

        public static readonly VesselSpec[] Vessels =
        {
            new VesselSpec { Id = "scout_skiff",        AttackPower = 1, CostFlux = 1, CostAlloy = 1, CostBloom = 0, Special = "reveals_defense" },
            new VesselSpec { Id = "pulse_frigate",      AttackPower = 2, CostFlux = 2, CostAlloy = 2, CostBloom = 0 },
            new VesselSpec { Id = "shieldbreaker_barge", AttackPower = 2, CostFlux = 2, CostAlloy = 3, CostBloom = 0, Special = "ignores_shield" },
            new VesselSpec { Id = "gate_piercer",       AttackPower = 3, CostFlux = 3, CostAlloy = 2, CostBloom = 0, Special = "ignores_gate_jammer" },
            new VesselSpec { Id = "siege_lantern",      AttackPower = 4, CostFlux = 3, CostAlloy = 4, CostBloom = 0 },
            new VesselSpec { Id = "drone_carrier",      AttackPower = 3, CostFlux = 4, CostAlloy = 2, CostBloom = 0, Special = "halves_drone_net" },
            new VesselSpec { Id = "null_ark",           AttackPower = 8, CostFlux = 6, CostAlloy = 4, CostBloom = 2, Special = "always_consumed" },
            new VesselSpec { Id = "resource_harvester", AttackPower = 0, CostFlux = 2, CostAlloy = 2, CostBloom = 0, Special = "non_attack_produces" },
        };

        public static DefenseSpec? Defense(string id)
        {
            foreach (var d in Defenses) if (d.Id == id) return d;
            return null;
        }

        public static VesselSpec? Vessel(string id)
        {
            foreach (var v in Vessels) if (v.Id == id) return v;
            return null;
        }
    }
}
