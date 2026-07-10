using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content.Ecology;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// CREATURE ECOLOGY 4.3b — the director that makes the population engine FELT. On every world
    /// load it takes a CENSUS of the scene's baked creatures, resolves <see cref="EcologyCore"/>
    /// (deterministic per world), and decides WHO IS ABROAD: each species activates its abroad
    /// count — population thinned by the hour's activity — under the hard active budget. The same
    /// world at noon and at midnight shows different casts (grazers by day, hunters after dark);
    /// a thin population reads as a quiet world. The clock is real UTC (the idle economy's
    /// real-time ethos): the game's wildlife keeps living while the headset is off, and visiting
    /// at different times of YOUR day genuinely changes what you meet.
    /// Self-bootstrapped on scene load — no scene edits, no patcher step; worlds with no baked
    /// creatures are untouched. Census order is deterministic (sorted by name) so the same world
    /// wakes the same individuals. Logs ZIPTIDE: ECOLOGY_RESOLVE.
    /// </summary>
    public class EcologyDirector : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            SceneManager.sceneLoaded += (scene, mode) => Ensure(scene);
            Ensure(SceneManager.GetActiveScene());
        }

        private static void Ensure(Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded) return;
            if (Object.FindObjectOfType<EcologyDirector>() != null) return;
            // Only worlds with baked fauna get a director — menus/boot/space stay untouched.
            var any = Object.FindObjectOfType<CreatureRuntime>();
            if (any == null) return;
            var go = new GameObject("EcologyDirector");
            go.AddComponent<EcologyDirector>();
        }

        private void Start()
        {
            ApplyEcology();
        }

        private void ApplyEcology()
        {
            string world = gameObject.scene.name;
            int seed = world.GetHashCode();
            float hour01 = (float)System.DateTime.UtcNow.TimeOfDay.TotalHours / 24f;
            long nowUnix = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Steady-state populations, thinned by THIS SAVE's recorded hunts (4.3c): a zone you
            // cleared yesterday is still quiet today, and loud again next week — the wild heals.
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            var pressures = profile != null
                ? profile.GetWorld(world, createIfMissing: true).ecologyPressures
                : null;
            if (pressures != null) Ziptide.Core.EcologyPressureLedger.Prune(pressures, nowUnix);
            var pops = EcologyCore.PopulationsAt(seed, EcologyCore.MaxResolveHours, pressures, nowUnix);

            // Census: every baked creature, grouped by species, deterministic order.
            var byId = new Dictionary<string, List<CreatureRuntime>>();
            foreach (var c in Object.FindObjectsOfType<CreatureRuntime>(true))
            {
                if (c == null || string.IsNullOrEmpty(c.creatureId)) continue;
                if (!byId.TryGetValue(c.creatureId, out var list))
                    byId[c.creatureId] = list = new List<CreatureRuntime>();
                list.Add(c);
            }

            int budgetLeft = EcologyCore.DefaultActiveBudget;
            var summary = new System.Text.StringBuilder();
            foreach (var s in EcologySpecies.All)
            {
                if (!byId.TryGetValue(s.CreatureId, out var creatures)) continue;
                creatures.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

                float pop = pops.TryGetValue(s.CreatureId, out float p) ? p : creatures.Count;
                int abroad = Mathf.Min(creatures.Count,
                    Mathf.Min(EcologyCore.AbroadCount(s, pop, hour01), budgetLeft));
                budgetLeft -= abroad;

                for (int i = 0; i < creatures.Count; i++)
                {
                    bool awake = i < abroad;
                    if (creatures[i].gameObject.activeSelf != awake)
                        creatures[i].gameObject.SetActive(awake);
                }
                summary.Append(s.CreatureId).Append('=').Append(abroad).Append('/')
                       .Append(creatures.Count).Append(' ');
            }

            // Species the ecology doesn't know keep their baked state — never break an authored scene.
            Debug.Log("ZIPTIDE: ECOLOGY_RESOLVE world=" + world +
                      " hour=" + (hour01 * 24f).ToString("F1") + " " + summary.ToString().TrimEnd());
        }
    }
}
