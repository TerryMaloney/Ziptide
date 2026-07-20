using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>Serialized identity proof attached to the generated world-improvement root.</summary>
    [DisallowMultipleComponent]
    public sealed class WorldImprovementStamp : MonoBehaviour
    {
        [SerializeField] private string gameId;
        [SerializeField] private string manifestId;
        [SerializeField] private string sceneName;
        [SerializeField] private int round;
        [SerializeField] private int recipeVersion;
        [SerializeField] private int compilerVersion;
        [SerializeField] private string recipeHash;
        [SerializeField] private string[] requiredAspects = Array.Empty<string>();
        [SerializeField] private string[] requiredEvidence = Array.Empty<string>();

        public string GameId => gameId;
        public string ManifestId => manifestId;
        public string SceneName => sceneName;
        public int Round => round;
        public int RecipeVersion => recipeVersion;
        public int CompilerVersion => compilerVersion;
        public string RecipeHash => recipeHash;
        public string[] RequiredAspects => requiredAspects;
        public string[] RequiredEvidence => requiredEvidence;

        public void Configure(WorldImprovementManifest manifest, string compiledSceneName,
            int compiledVersion, string hash)
        {
            if (manifest == null) throw new ArgumentNullException(nameof(manifest));
            gameId = manifest.gameId ?? string.Empty;
            manifestId = manifest.manifestId ?? string.Empty;
            sceneName = compiledSceneName ?? string.Empty;
            round = manifest.round;
            recipeVersion = manifest.recipeVersion;
            compilerVersion = compiledVersion;
            recipeHash = hash ?? string.Empty;
            requiredAspects = manifest.requiredAspects ?? Array.Empty<string>();
            requiredEvidence = manifest.requiredEvidence ?? Array.Empty<string>();
        }
    }
}
