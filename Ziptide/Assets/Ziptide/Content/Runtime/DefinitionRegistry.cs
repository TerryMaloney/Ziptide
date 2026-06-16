using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Generic id → definition lookup. Mirrors ItemFactory's cache, but reusable for any
    /// <see cref="Definition"/> type and built for collision-free parallel content authoring:
    /// drop a definition asset in the conventional Resources folder and it is discovered
    /// automatically — no central list to edit (which is what causes merge conflicts).
    ///
    /// Pure cache/lookup logic is unit-tested in EditMode (no Resources/disk needed); only
    /// <see cref="EnsureLoadedFromResources"/> touches the asset database at runtime.
    /// </summary>
    public class DefinitionRegistry<TDef> where TDef : Definition
    {
        private readonly Dictionary<string, TDef> _byId = new Dictionary<string, TDef>();
        private bool _loaded;

        public int Count => _byId.Count;
        public IEnumerable<TDef> All => _byId.Values;

        /// <summary>Add one definition. Empty id is ignored; a duplicate id keeps the FIRST and warns
        /// (so two assets sharing an id surface as a content error instead of silently clobbering).</summary>
        public void Register(TDef def)
        {
            if (def == null) return;
            if (string.IsNullOrEmpty(def.id))
            {
                Debug.LogWarning("ZIPTIDE: DEF_NO_ID type=" + typeof(TDef).Name + " name=" + def.name);
                return;
            }
            if (_byId.TryGetValue(def.id, out var existing) && existing != null && existing != def)
            {
                Debug.LogWarning("ZIPTIDE: DUP_DEFINITION type=" + typeof(TDef).Name + " id=" + def.id +
                                 " (kept first, ignored '" + def.name + "')");
                return;
            }
            _byId[def.id] = def;
        }

        public void RegisterAll(IEnumerable<TDef> defs)
        {
            if (defs == null) return;
            foreach (var d in defs) Register(d);
        }

        public TDef Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return _byId.TryGetValue(id, out var d) ? d : null;
        }

        public bool TryGet(string id, out TDef def)
        {
            def = Get(id);
            return def != null;
        }

        public bool Contains(string id) => !string.IsNullOrEmpty(id) && _byId.ContainsKey(id);

        public void Clear()
        {
            _byId.Clear();
            _loaded = false;
        }

        /// <summary>Auto-discover every <typeparamref name="TDef"/> asset under a Resources subfolder,
        /// once. Call at boot/world entry. Idempotent — repeated calls are no-ops until Clear().</summary>
        public void EnsureLoadedFromResources(string resourcesPath)
        {
            if (_loaded) return;
            var found = Resources.LoadAll<TDef>(resourcesPath ?? string.Empty);
            RegisterAll(found);
            _loaded = true;
            Debug.Log("ZIPTIDE: REGISTRY_LOADED type=" + typeof(TDef).Name +
                      " path=" + resourcesPath + " count=" + _byId.Count);
        }
    }
}
