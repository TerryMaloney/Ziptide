using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Canonical base for every data-driven content definition (resources, tools, machines, plants,
    /// creatures, biomes, recipes, …). One string <see cref="id"/> resolves it through a
    /// <see cref="DefinitionRegistry{TDef}"/> — never hard prefab references (the locked contract).
    ///
    /// New content types inherit this. The legacy <c>ItemDefinition</c> / <c>WorldPackDefinition</c>
    /// keep their own id fields (itemId / packId) for back-compat and are not retrofitted.
    /// </summary>
    public abstract class Definition : ScriptableObject
    {
        [Tooltip("Unique, stable string id used to resolve this definition by registry/factory " +
                 "(e.g. 'scrap', 'driller_mk1', 'swarmer'). Lower_snake_case by convention.")]
        public string id = "";

        [Tooltip("Human-readable name for UI. Falls back to id if empty.")]
        public string displayName = "";

        public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
    }
}
