using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ziptide.Core
{
    /// <summary>
    /// ARCHITECTURE V2 Q4a — the GameObject pool: the thin Unity translator over <see cref="PoolCore{T}"/>.
    /// Replaces "new GameObject/CreatePrimitive + Destroy per shot" for HOT, short-lived, same-scene
    /// spawns (PvP bolts, taser darts, stun arcs, thump rings, nets — and the bank's belt pucks /
    /// pooled creature respawns / destructible chunks). Pooled instances park deactivated under a
    /// hidden root; Get reactivates + repositions, Release deactivates + re-parents.
    ///
    /// Scene safety: on EVERY sceneLoaded the pools are cleared and the root rebuilt — pooled objects
    /// belong to the scene that made them, so travel never hands out a destroyed instance. Keys are
    /// caller-chosen strings (usually the spawn kind, e.g. "pvp_bolt").
    ///
    /// ADOPTION (not done in this commit — see HANDOFF envelope): a call site changes from
    ///   var go = GameObject.CreatePrimitive(...); ... Destroy(go, life);
    /// to
    ///   var go = GamePool.Get("kind", () => BuildOne(), pos); ... GamePool.Release("kind", go);
    /// The factory builds a fresh instance the FIRST time only; thereafter instances are reused.
    /// </summary>
    public static class GamePool
    {
        private const int DefaultMaxRetained = 64; // per-key cap — Quest-safe, prevents runaway pools

        private static readonly Dictionary<string, PoolCore<GameObject>> _pools =
            new Dictionary<string, PoolCore<GameObject>>();
        private static Transform _root;
        private static bool _hooked;

        private static Transform Root()
        {
            if (_root == null)
            {
                var go = new GameObject("__GAMEPOOL");
                go.SetActive(false); // children parked here are inactive/non-rendering
                _root = go.transform;
            }
            return _root;
        }

        private static void EnsureHook()
        {
            if (_hooked) return;
            _hooked = true;
            SceneManager.sceneLoaded += (_, __) => ResetForNewScene();
        }

        private static PoolCore<GameObject> PoolFor(string key, Func<GameObject> factory, int maxRetained)
        {
            EnsureHook();
            if (!_pools.TryGetValue(key, out var pool))
            {
                pool = new PoolCore<GameObject>(
                    factory,
                    onGet: go => { if (go != null) go.SetActive(true); },
                    onRelease: go => { if (go != null) { go.SetActive(false); go.transform.SetParent(Root(), false); } },
                    maxRetained: maxRetained);
                _pools[key] = pool;
            }
            return pool;
        }

        /// <summary>Get a pooled instance of <paramref name="key"/> at <paramref name="position"/>,
        /// building one via <paramref name="factory"/> only when the free list is empty. The returned
        /// object is active and un-parented (world-space at position).</summary>
        public static GameObject Get(string key, Func<GameObject> factory, Vector3 position, int maxRetained = DefaultMaxRetained)
        {
            if (string.IsNullOrEmpty(key) || factory == null) return null;
            var pool = PoolFor(key, factory, maxRetained);

            GameObject go = pool.Acquire();
            // Scene-unload can leave a destroyed instance on the free list despite the reset hook
            // (e.g. additive loads) — if Unity says it's gone, build a fresh one and keep counts honest.
            if (go == null)
            {
                go = factory();
                go.SetActive(true);
            }
            go.transform.SetParent(null, false);
            go.transform.position = position;
            return go;
        }

        /// <summary>Return an instance to its pool (deactivated + parked). If the pool is over its
        /// retained cap the instance is Destroyed instead — the pool never grows without bound.</summary>
        public static void Release(string key, GameObject go)
        {
            if (go == null) return;
            if (string.IsNullOrEmpty(key) || !_pools.TryGetValue(key, out var pool))
            {
                UnityEngine.Object.Destroy(go);
                return;
            }
            bool retained = pool.Release(go);
            if (!retained) UnityEngine.Object.Destroy(go);
        }

        /// <summary>Pre-build <paramref name="count"/> instances for <paramref name="key"/> so the first
        /// burst doesn't hitch. Safe to call at scene setup. Uses the normal Acquire→Release cycle so
        /// each instance ends up correctly deactivated + parked on the free list.</summary>
        public static void Prewarm(string key, int count, Func<GameObject> factory, int maxRetained = DefaultMaxRetained)
        {
            if (string.IsNullOrEmpty(key) || factory == null || count <= 0) return;
            var pool = PoolFor(key, factory, maxRetained);
            for (int i = 0; i < count; i++)
            {
                var go = pool.Acquire();          // Created++ (empty free list at setup)
                if (go == null) go = factory();
                pool.Release(go);                 // onRelease parks + deactivates, pushes to free
            }
        }

        /// <summary>Log a one-line census for tuning: kinds, created, reused, live, free.</summary>
        public static void LogStats()
        {
            foreach (var kv in _pools)
            {
                var p = kv.Value;
                Debug.Log("ZIPTIDE: POOL_STAT key=" + kv.Key + " created=" + p.Created +
                          " reused=" + p.Reused + " live=" + p.Live + " free=" + p.Free);
            }
        }

        private static void ResetForNewScene()
        {
            foreach (var kv in _pools)
                foreach (var go in kv.Value.DrainFree())
                    if (go != null) UnityEngine.Object.Destroy(go);
            _pools.Clear();
            _root = null; // old root died with the old scene
        }
    }
}
