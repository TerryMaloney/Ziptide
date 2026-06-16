using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    [CreateAssetMenu(fileName = "BiomeDefinition", menuName = "Ziptide/Definitions/Biome")]
    public class BiomeDefinition : Definition
    {
        [Tooltip("Hazard flavor (toxic, vacuum, radiation, cold…). Free-form for content flexibility.")]
        public string hazardType = "";

        [Tooltip("Art kit id (WorldArtKitDefinition) used to build this biome's geometry.")]
        public string artKitId = "";

        public List<string> nativeResourceIds = new List<string>();
        public List<string> nativeCreatureIds = new List<string>();
        public List<string> nativePlantIds = new List<string>();

        [Tooltip("Ambient / fog tint hint for the visual theme.")]
        public Color ambientTint = Color.gray;
    }
}
