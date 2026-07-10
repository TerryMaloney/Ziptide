using System.Collections;
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
    /// 4.3e: the census re-resolves every few minutes so dawn/dusk happen LIVE in a session —
    /// hunters emerging as the light goes, grazers denning up — with a scale emerge/burrow
    /// presentation instead of hard pops. Downed creatures are NEVER toggled (deactivation would
    /// kill their pending respawn coroutine); the wild handles its own fallen.
    /// </summary>
    public class EcologyDirector : MonoBehaviour
    {
        private const float ReResolveSeconds = 300f; // live dawn/dusk cadence
        private const float TweenSeconds = 0.8f;     // emerge/burrow presentation length
        private const float TweenFloor = 0.05f;      // never scale fully to zero mid-tween

        private bool _instantResolve = true;         // first census pops silently (load moment)
        private bool _nestsBuilt;
        private readonly Dictionary<CreatureRuntime, Vector3> _homeScales =
            new Dictionary<CreatureRuntime, Vector3>();
        private readonly Dictionary<CreatureRuntime, Coroutine> _tweens =
            new Dictionary<CreatureRuntime, Coroutine>();

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
            _instantResolve = false;
            InvokeRepeating(nameof(ApplyEcology), ReResolveSeconds, ReResolveSeconds);
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
                    var c = creatures[i];
                    bool awake = i < abroad;
                    // 4.3e respawn safety: a downed creature may be running RespawnAfter();
                    // SetActive(false) would kill that coroutine FOREVER. Leave the fallen alone.
                    if (!c.IsAlive) continue;
                    if (!_homeScales.ContainsKey(c)) _homeScales[c] = c.transform.localScale;
                    if (c.gameObject.activeSelf == awake) continue;
                    if (_instantResolve) { c.gameObject.SetActive(awake); continue; }
                    StartTween(c, awake);
                }
                summary.Append(s.CreatureId).Append('=').Append(abroad).Append('/')
                       .Append(creatures.Count).Append(' ');
            }

            // Species the ecology doesn't know keep their baked state — never break an authored scene.
            Debug.Log("ZIPTIDE: ECOLOGY_RESOLVE world=" + world +
                      " hour=" + (hour01 * 24f).ToString("F1") + " " + summary.ToString().TrimEnd());

            if (!_nestsBuilt) { BuildNests(byId); _nestsBuilt = true; }
        }

        // ── 4.3e emerge/burrow presentation ─────────────────────────────────
        // Tweens run on the DIRECTOR (a coroutine on the creature dies when it deactivates) and
        // abort the instant a creature goes down, so they never fight CreatureRuntime's crumple
        // scale or the RespawnAfter restore.

        private void StartTween(CreatureRuntime c, bool awake)
        {
            if (_tweens.TryGetValue(c, out var running) && running != null) StopCoroutine(running);
            _tweens[c] = StartCoroutine(awake ? Emerge(c) : Burrow(c));
        }

        private IEnumerator Emerge(CreatureRuntime c)
        {
            if (c == null) yield break;
            Vector3 home = _homeScales.TryGetValue(c, out var s) ? s : c.transform.localScale;
            c.gameObject.SetActive(true);
            for (float t = 0f; t < TweenSeconds; t += Time.deltaTime)
            {
                if (c == null || !c.IsAlive) yield break;
                float k = Mathf.SmoothStep(0f, 1f, t / TweenSeconds);
                c.transform.localScale = home * Mathf.Max(TweenFloor, k);
                yield return null;
            }
            if (c != null && c.IsAlive) c.transform.localScale = home;
            if (c != null) _tweens.Remove(c);
        }

        private IEnumerator Burrow(CreatureRuntime c)
        {
            if (c == null) yield break;
            Vector3 home = _homeScales.TryGetValue(c, out var s) ? s : c.transform.localScale;
            for (float t = 0f; t < TweenSeconds; t += Time.deltaTime)
            {
                if (c == null || !c.IsAlive) yield break; // downed mid-burrow: the crumple wins
                float k = Mathf.SmoothStep(0f, 1f, t / TweenSeconds);
                c.transform.localScale = home * Mathf.Max(TweenFloor, 1f - k);
                yield return null;
            }
            if (c == null || !c.IsAlive) yield break;
            c.transform.localScale = home; // restore BEFORE sleeping — a nest-disturb wake pops full-size
            c.gameObject.SetActive(false);
            _tweens.Remove(c);
        }

        /// <summary>4.3d: every species with a home count gets physical NESTS at its population's
        /// heart and far ranges (EcologyCore.NestSitesFor over the creatures' own positions) —
        /// places to find, and to regret disturbing.</summary>
        private void BuildNests(Dictionary<string, List<CreatureRuntime>> byId)
        {
            foreach (var s in EcologySpecies.All)
            {
                if (s.NestsPerZone <= 0) continue;
                if (!byId.TryGetValue(s.CreatureId, out var creatures) || creatures.Count == 0) continue;

                var homes = new List<Vector3>(creatures.Count);
                foreach (var c in creatures) homes.Add(c.transform.position);
                var sites = EcologyCore.NestSitesFor(homes, s.NestsPerZone);

                Color accent = NestAccent(s.CreatureId);
                for (int i = 0; i < sites.Count; i++)
                {
                    var go = new GameObject("Nest_" + s.CreatureId + "_" + i);
                    go.transform.SetParent(transform, false);
                    // Beside the home, not on it — the resident stands guard at its own door.
                    go.transform.position = sites[i] + new Vector3(1.6f, 0f, 1.1f);
                    go.AddComponent<NestRuntime>().Init(s.CreatureId, accent);
                }
            }
        }

        private static Color NestAccent(string creatureId)
        {
            switch (creatureId)
            {
                case "swarm_bug": return new Color(0.95f, 0.6f, 0.25f);   // ember clutch
                case "light_grazer": return new Color(0.55f, 0.9f, 0.5f); // soft green
                case "tether_swarm": return new Color(0.4f, 0.7f, 0.95f); // pale tether-blue
                case "witness_mite": return new Color(0.8f, 0.5f, 0.9f);  // watchful violet
                case "husk_molter": return new Color(0.75f, 0.7f, 0.5f);  // molt-wax amber
                case "stalker": return new Color(0.9f, 0.3f, 0.3f);       // apex red
                default: return new Color(0.6f, 0.8f, 0.8f);
            }
        }
    }
}
