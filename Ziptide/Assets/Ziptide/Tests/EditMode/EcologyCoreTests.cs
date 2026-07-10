using System.Collections.Generic;
using NUnit.Framework;
using Ziptide.Content.Ecology;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// CREATURE ECOLOGY 4.3 laws: everything deterministic per (seed, elapsed, pressures) — the
    /// save/offline law; populations settle toward capacity; predators actually suppress their prey;
    /// player disables suppress a species and then it RECOVERS (the non-lethal canon made math);
    /// nothing ever goes extinct; day and night have different casts; packs partition the abroad
    /// population; and the live-spawn budget is a hard cap. Pure, headless.
    /// </summary>
    public class EcologyCoreTests
    {
        [Test]
        public void Populations_AreDeterministic_PerSeedElapsedAndPressures()
        {
            var a = EcologyCore.PopulationsAt(worldSeed: 77, elapsedHours: 48f);
            var b = EcologyCore.PopulationsAt(worldSeed: 77, elapsedHours: 48f);
            foreach (var kv in a)
                Assert.AreEqual(kv.Value, b[kv.Key], 1e-6f, kv.Key + " must resolve identically");

            var c = EcologyCore.PopulationsAt(worldSeed: 78, elapsedHours: 48f);
            bool anyDiff = false;
            foreach (var kv in a)
                if (System.Math.Abs(kv.Value - c[kv.Key]) > 1e-3f) anyDiff = true;
            Assert.IsTrue(anyDiff, "different worlds should have different living states");
        }

        [Test]
        public void Populations_SettleTowardCapacity_AndNeverExplode()
        {
            var pops = EcologyCore.PopulationsAt(worldSeed: 5, elapsedHours: EcologyCore.MaxResolveHours);
            foreach (var s in EcologySpecies.All)
            {
                Assert.GreaterOrEqual(pops[s.CreatureId], EcologyCore.ExtinctionFloor);
                Assert.LessOrEqual(pops[s.CreatureId], s.CarryingCapacity * 1.25f + 0.001f,
                    s.CreatureId + " can never explode past its ceiling");
            }
            // Grazers with no predators sit near K; hunted species sit meaningfully below theirs.
            Assert.Greater(pops["swarm_bug"], 0.5f * EcologySpecies.Find("swarm_bug").CarryingCapacity);
        }

        [Test]
        public void Predators_SuppressTheirPrey()
        {
            // The tether swarm + witness mite hunt swarm bugs: with hunters present the bug
            // population must settle LOWER than its own untouched capacity.
            var pops = EcologyCore.PopulationsAt(worldSeed: 11, elapsedHours: 200f);
            Assert.Less(pops["swarm_bug"], EcologySpecies.Find("swarm_bug").CarryingCapacity,
                "a hunted species lives below its untouched ceiling");
        }

        [Test]
        public void Disables_Suppress_ThenTheWildHeals()
        {
            long now = 1_000_000;
            var fresh = new List<EcologyPressure>
                { new EcologyPressure { creatureId = "light_grazer", disabled = 8, atUnix = now } };
            var later = new List<EcologyPressure>
                { new EcologyPressure { creatureId = "light_grazer", disabled = 8,
                    atUnix = now - (long)(EcologyCore.PressureHalfLifeHours * 8f * 3600f) } };

            var undisturbed = EcologyCore.PopulationsAt(3, 100f)["light_grazer"];
            var justHit = EcologyCore.PopulationsAt(3, 100f, fresh, now)["light_grazer"];
            var healed = EcologyCore.PopulationsAt(3, 100f, later, now)["light_grazer"];

            Assert.Less(justHit, undisturbed, "a fresh disturbance thins the herd");
            Assert.Greater(healed, justHit, "…and the wild heals — the non-lethal canon made math");
            Assert.AreEqual(undisturbed, healed, 0.5f, "old pressure all but vanishes");
        }

        [Test]
        public void NothingEverGoesExtinct()
        {
            long now = 1_000_000;
            var massacre = new List<EcologyPressure>();
            foreach (var s in EcologySpecies.All)
                massacre.Add(new EcologyPressure { creatureId = s.CreatureId, disabled = 9999, atUnix = now });
            var pops = EcologyCore.PopulationsAt(9, 1f, massacre, now);
            foreach (var s in EcologySpecies.All)
                Assert.GreaterOrEqual(pops[s.CreatureId], EcologyCore.ExtinctionFloor,
                    s.CreatureId + " can never be wiped out — a world never empties permanently");
        }

        [Test]
        public void DayAndNight_HaveDifferentCasts()
        {
            var grazer = EcologySpecies.Find("light_grazer");
            var stalker = EcologySpecies.Find("stalker");
            Assert.Greater(EcologyCore.ActivityAt(grazer, 0.5f), EcologyCore.ActivityAt(grazer, 0f),
                "grazers are day creatures");
            Assert.Greater(EcologyCore.ActivityAt(stalker, 0f), EcologyCore.ActivityAt(stalker, 0.5f),
                "the apex hunts at night");
            var warden = EcologySpecies.Find("warden");
            Assert.AreEqual(1f, EcologyCore.ActivityAt(warden, 0.25f), 1e-4f,
                "the Warden never sleeps — it is law, not wildlife");
        }

        [Test]
        public void Packs_PartitionTheAbroadPopulation_Deterministically()
        {
            var bug = EcologySpecies.Find("swarm_bug");
            var packs = EcologyCore.PacksFor(bug, 20f, seed: 4);
            var again = EcologyCore.PacksFor(bug, 20f, seed: 4);
            CollectionAssert.AreEqual(packs, again, "the same world composes the same bands");

            int total = 0;
            foreach (var p in packs) { Assert.Greater(p, 0); total += p; }
            Assert.AreEqual(20, total, "packs partition the abroad population exactly");

            var solitary = EcologyCore.PacksFor(EcologySpecies.Find("stalker"), 3f, seed: 4);
            foreach (var p in solitary) Assert.AreEqual(1, p, "solitary species roam alone");
        }

        [Test]
        public void ActiveBudget_IsAHardCap_AndWholePacksOnly()
        {
            var packs = new List<int> { 6, 5, 5, 4, 3, 2 };
            var chosen = EcologyCore.CapToBudget(packs, budget: 14);
            int used = 0;
            foreach (var p in chosen) used += p;
            Assert.LessOrEqual(used, 14, "the perf budget is a hard cap");
            Assert.Contains(6, chosen, "the biggest encounter spawns first");
            foreach (var p in chosen)
                Assert.Contains(p, packs, "a pack spawns whole or not at all");
        }
    }
}
