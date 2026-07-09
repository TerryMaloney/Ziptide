using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// A building STYLE as data (V2.5 H1): grammar numbers + the module palette. The grammar consumes
    /// the pure mirror (<see cref="BuildingStyleData"/>, the BotProfileData pattern); colors feed the
    /// primitive fallback renderer; `surfaceFamily` + `styleId` are the Art Registry keys Picasso's
    /// kits fulfill (`buildingModule:&lt;styleId&gt;/&lt;module&gt;` — docs/design/ART_REGISTRY.md).
    /// Districts opt in via DistrictDef.buildingStyleId; assets live in Resources/BuildingStyles.
    /// </summary>
    [CreateAssetMenu(menuName = "Ziptide/City/Building Style", fileName = "BuildingStyle")]
    public class BuildingStyleDefinition : ScriptableObject
    {
        [Tooltip("Registry id (e.g. salvage_row). DistrictDef.buildingStyleId points here.")]
        public string styleId = "salvage_row";
        [Tooltip("Art direction family (ART_DIRECTION_MASTER_PLAN) the kit must honor.")]
        public string surfaceFamily = "Salvage";

        [Header("Grammar")]
        public float moduleWidth = 3f;
        public float storeyHeight = 3.2f;
        public int minStoreys = 1;
        public int maxStoreys = 3;
        [Range(0f, 1f)] public float windowChance = 0.45f;
        [Range(0f, 1f)] public float rakedRoofChance = 0.35f;

        [Header("Lots (fed to LotPartitioner)")]
        public float streetWidth = 4f;
        public float minLotArea = 60f;
        public float maxLotAspect = 3f;

        [Header("Interiors (HARDWIRING 1.3 — walkable ground floors)")]
        [Tooltip("Build a walkable BSP interior (RoomPartitioner + InteriorMeshCore) on the ground " +
                 "storey of every building in this style. Renderer cost is real — the district " +
                 "renderer-budget gate audits it. Default OFF: zero change until a style opts in.")]
        public bool hasInteriors = false;

        [Header("Primitive fallback palette")]
        public Color wallColor = new Color(0.32f, 0.30f, 0.28f);
        public Color windowColor = new Color(0.08f, 0.13f, 0.16f);
        public Color doorColor = new Color(0.45f, 0.30f, 0.15f);
        public Color trimColor = new Color(0.24f, 0.23f, 0.22f);
        public Color roofColor = new Color(0.20f, 0.19f, 0.18f);

        public BuildingStyleData ToData() => new BuildingStyleData
        {
            ModuleWidth = moduleWidth,
            StoreyHeight = storeyHeight,
            MinStoreys = minStoreys,
            MaxStoreys = maxStoreys,
            WindowChance = windowChance,
            RakedRoofChance = rakedRoofChance,
        };
    }
}
