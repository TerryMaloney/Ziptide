using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Minimal data-driven config for procedural city generation.
    /// Used by ScenePatcherD1 to build the Toxic Venice layout.
    /// </summary>
    public class CityKitDefinition : ScriptableObject
    {
        [Header("Canals")]
        public float canalWidth = 4f;
        public float canalDepthVisual = 2f;

        [Header("Walkways")]
        public float walkwayHeight = 2.5f;
        public float walkwayWidth = 3f;

        [Header("Bridges")]
        public float bridgeWidth = 2.5f;
        public int crossBridgeCount = 4;

        [Header("Platforms")]
        public float platformSize = 5f;

        [Header("Traversal Network")]
        public float mainWalkwayLength = 60f;
        public float serviceCatwalkHeight = 1.5f;
        public float rampWidth = 2f;

        [Header("Railings")]
        public float railingHeight = 1.1f;
        public float railingThickness = 0.08f;
        public bool railingOnEdges = true;

        [Header("Toxic Zone")]
        public Color toxicColor = new Color(0.2f, 0.45f, 0.1f, 0.8f);

        [Header("Generation")]
        public int seed = 42;
    }
}
