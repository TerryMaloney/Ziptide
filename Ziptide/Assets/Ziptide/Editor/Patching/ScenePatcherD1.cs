using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Idempotent city generator for D0_City scene.
    /// Builds the Toxic Venice layout: canals, elevated walkways, bridges, courtyards,
    /// railings, toxic surface, building shells, loop routes, ramps, and service catwalks
    /// under a single __D1_CITY_ROOT.
    /// </summary>
    public static class ScenePatcherD1
    {
        private const string D0SceneName = "D0_City";
        private const string CityRoot = "__D1_CITY_ROOT";
        private const string CityKitAssetPath = "Assets/Ziptide/Content/City/DefaultCityKit.asset";

        private static Material _concreteMat;
        private static Material _metalMat;
        private static Material _toxicMat;
        private static Material _buildingMat1;
        private static Material _buildingMat2;
        private static Material _railingMat;
        private static Material _catwalkMat;

        [MenuItem("Ziptide/Apply D1 City To Current Scene")]
        public static void PatchActiveSceneFromMenu()
        {
            PatchActiveScene();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[Ziptide] D1 city applied to current scene (if D0_City). Save scene and build.");
        }

        public static void PatchActiveScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded) return;
            if (scene.name != D0SceneName) return;
            GenerateCity();
            EnsureDroneTutorialJob();
        }

        private static void EnsureDroneTutorialJob()
        {
            string jobFolder = "Assets/Ziptide/Content/Jobs";
            if (!AssetDatabase.IsValidFolder(jobFolder))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content"))
                    AssetDatabase.CreateFolder("Assets/Ziptide", "Content");
                AssetDatabase.CreateFolder("Assets/Ziptide/Content", "Jobs");
            }

            string goStepPath = jobFolder + "/DroneTutorial_GoDispatch.asset";
            var goStep = AssetDatabase.LoadAssetAtPath<GoToMarkerStepDefinition>(goStepPath);
            if (goStep == null)
            {
                goStep = ScriptableObject.CreateInstance<GoToMarkerStepDefinition>();
                goStep.stepLabel = "Go to Dispatch";
                goStep.markerId = "Dispatch";
                goStep.arriveDistance = 3f;
                AssetDatabase.CreateAsset(goStep, goStepPath);
            }

            string droneStepPath = jobFolder + "/DroneTutorial_DisableDrones.asset";
            var droneStep = AssetDatabase.LoadAssetAtPath<DisableDronesCountStepDefinition>(droneStepPath);
            if (droneStep == null)
            {
                droneStep = ScriptableObject.CreateInstance<DisableDronesCountStepDefinition>();
                droneStep.stepLabel = "Disable 3 drones";
                droneStep.count = 3;
                AssetDatabase.CreateAsset(droneStep, droneStepPath);
            }

            string jobPath = jobFolder + "/DroneTutorialJob.asset";
            var job = AssetDatabase.LoadAssetAtPath<JobDefinition>(jobPath);
            if (job == null)
            {
                job = ScriptableObject.CreateInstance<JobDefinition>();
                job.jobId = "drone_tutorial";
                job.title = "Drone Tutorial";
                job.steps = new System.Collections.Generic.List<JobStepDefinition> { goStep, droneStep };
                AssetDatabase.CreateAsset(job, jobPath);
            }

            AssetDatabase.SaveAssets();

            var d0Pack = ScenePatcherD0.EnsureD0WorldPackAsset();
            if (d0Pack != null)
            {
                var so = new SerializedObject(d0Pack);
                var jobsProp = so.FindProperty("jobs");
                if (jobsProp != null)
                {
                    bool found = false;
                    for (int i = 0; i < jobsProp.arraySize; i++)
                    {
                        if (jobsProp.GetArrayElementAtIndex(i).objectReferenceValue == job)
                        { found = true; break; }
                    }
                    if (!found)
                    {
                        jobsProp.arraySize++;
                        jobsProp.GetArrayElementAtIndex(jobsProp.arraySize - 1).objectReferenceValue = job;
                    }
                    so.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(d0Pack);
                }
            }
        }

        private static CityKitDefinition EnsureCityKitAsset()
        {
            var existing = AssetDatabase.LoadAssetAtPath<CityKitDefinition>(CityKitAssetPath);
            if (existing != null) return existing;

            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content/City"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content"))
                    AssetDatabase.CreateFolder("Assets/Ziptide", "Content");
                AssetDatabase.CreateFolder("Assets/Ziptide/Content", "City");
            }

            var kit = ScriptableObject.CreateInstance<CityKitDefinition>();
            AssetDatabase.CreateAsset(kit, CityKitAssetPath);
            AssetDatabase.SaveAssets();
            return kit;
        }

        private static void GenerateCity()
        {
            var kit = EnsureCityKitAsset();
            if (kit == null) return;

            var oldRoot = GameObject.Find(CityRoot);
            if (oldRoot != null)
                Object.DestroyImmediate(oldRoot);

            var root = new GameObject(CityRoot);
            Undo.RegisterCreatedObjectUndo(root, CityRoot);

            InitMaterials(kit);
            _planterIdx = 0;

            BuildToxicSurface(root.transform, kit);
            BuildCentralWalkway(root.transform, kit);
            BuildSideWalkways(root.transform, kit);
            BuildCanals(root.transform, kit);
            BuildPerimeterWalkways(root.transform, kit);
            BuildBridges(root.transform, kit);
            BuildCourtyards(root.transform, kit);
            BuildServiceCatwalks(root.transform, kit);
            BuildRamps(root.transform, kit);
            BuildRailings(root.transform, kit);
            BuildBuildings(root.transform, kit);
            PlaceDrones(root.transform, kit);
            PlaceTaserDartGun(kit);

            CleanupMaterials();
        }

        // ─── Materials ───────────────────────────────────────────────────

        private static void InitMaterials(CityKitDefinition kit)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");

            _concreteMat = MakeMat(shader, "CityConc", new Color(0.55f, 0.53f, 0.50f));
            _metalMat = MakeMat(shader, "CityMetal", new Color(0.35f, 0.36f, 0.38f));
            _toxicMat = MakeMat(shader, "CityToxic", kit.toxicColor);
            _buildingMat1 = MakeMat(shader, "CityBldg1", new Color(0.60f, 0.52f, 0.42f));
            _buildingMat2 = MakeMat(shader, "CityBldg2", new Color(0.48f, 0.45f, 0.40f));
            _railingMat = MakeMat(shader, "CityRail", new Color(0.22f, 0.22f, 0.25f));
            _catwalkMat = MakeMat(shader, "CityCatwalk", new Color(0.40f, 0.38f, 0.35f));
        }

        private static Material MakeMat(Shader shader, string name, Color color)
        {
            var mat = new Material(shader) { name = name };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            return mat;
        }

        private static void CleanupMaterials()
        {
            _concreteMat = null;
            _metalMat = null;
            _toxicMat = null;
            _buildingMat1 = null;
            _buildingMat2 = null;
            _railingMat = null;
            _catwalkMat = null;
        }

        // ─── Toxic Surface ───────────────────────────────────────────────

        private static void BuildToxicSurface(Transform root, CityKitDefinition kit)
        {
            var plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plane.name = "ToxicSurface";
            plane.transform.SetParent(root, false);
            plane.transform.localPosition = new Vector3(0f, -kit.canalDepthVisual, 0f);
            plane.transform.localScale = new Vector3(80f, 0.1f, 80f);
            plane.GetComponent<Collider>().enabled = false;
            ApplyMat(plane, _toxicMat);
        }

        // ─── Central Walkway ─────────────────────────────────────────────

        private static void BuildCentralWalkway(Transform root, CityKitDefinition kit)
        {
            float h = kit.walkwayHeight;
            float w = kit.walkwayWidth;
            float length = kit.mainWalkwayLength;

            MakeCube(root, "CentralWalkway",
                new Vector3(0f, h * 0.5f, 0f),
                new Vector3(w, h, length),
                _concreteMat);
        }

        // ─── Side Walkways (full length, parallel to central) ────────────

        private static void BuildSideWalkways(Transform root, CityKitDefinition kit)
        {
            float h = kit.walkwayHeight;
            float cw = kit.canalWidth;
            float ww = kit.walkwayWidth;
            float length = kit.mainWalkwayLength;
            float sideW = 3.5f;

            float offsetX = ww * 0.5f + cw + sideW * 0.5f;

            MakeCube(root, "SideWalkLeft",
                new Vector3(-offsetX, h * 0.5f, 0f),
                new Vector3(sideW, h, length),
                _concreteMat);

            MakeCube(root, "SideWalkRight",
                new Vector3(offsetX, h * 0.5f, 0f),
                new Vector3(sideW, h, length),
                _concreteMat);
        }

        // ─── Canals ─────────────────────────────────────────────────────

        private static void BuildCanals(Transform root, CityKitDefinition kit)
        {
            float h = kit.walkwayHeight;
            float cw = kit.canalWidth;
            float ww = kit.walkwayWidth;
            float length = kit.mainWalkwayLength;
            float wallThick = 0.2f;

            MakeCube(root, "CanalWallInnerL",
                new Vector3(-(ww * 0.5f + wallThick * 0.5f), h * 0.25f, 0f),
                new Vector3(wallThick, h * 0.5f, length), _metalMat);

            MakeCube(root, "CanalWallOuterL",
                new Vector3(-(ww * 0.5f + cw + wallThick * 0.5f), h * 0.25f, 0f),
                new Vector3(wallThick, h * 0.5f, length), _metalMat);

            MakeCube(root, "CanalWallInnerR",
                new Vector3(ww * 0.5f + wallThick * 0.5f, h * 0.25f, 0f),
                new Vector3(wallThick, h * 0.5f, length), _metalMat);

            MakeCube(root, "CanalWallOuterR",
                new Vector3(ww * 0.5f + cw + wallThick * 0.5f, h * 0.25f, 0f),
                new Vector3(wallThick, h * 0.5f, length), _metalMat);
        }

        // ─── Perimeter Walkways (connect far ends → loop route) ──────────

        private static void BuildPerimeterWalkways(Transform root, CityKitDefinition kit)
        {
            float h = kit.walkwayHeight;
            float ww = kit.walkwayWidth;
            float cw = kit.canalWidth;
            float length = kit.mainWalkwayLength;
            float halfLen = length * 0.5f;
            float sideW = 3.5f;
            float outerX = ww * 0.5f + cw + sideW;
            float perimW = 3f;
            float spanX = outerX * 2f + perimW;

            // North perimeter (connects side walk ends at +Z)
            MakeCube(root, "PerimeterNorth",
                new Vector3(0f, h - 0.05f, halfLen + perimW * 0.5f - 0.5f),
                new Vector3(spanX, 0.25f, perimW),
                _concreteMat);

            // South perimeter (connects side walk ends at -Z)
            MakeCube(root, "PerimeterSouth",
                new Vector3(0f, h - 0.05f, -(halfLen + perimW * 0.5f - 0.5f)),
                new Vector3(spanX, 0.25f, perimW),
                _concreteMat);
        }

        // ─── Bridges ────────────────────────────────────────────────────

        private static void BuildBridges(Transform root, CityKitDefinition kit)
        {
            float h = kit.walkwayHeight;
            float cw = kit.canalWidth;
            float ww = kit.walkwayWidth;
            float bw = kit.bridgeWidth;
            float bridgeThick = 0.25f;
            float length = kit.mainWalkwayLength;
            int count = Mathf.Max(2, kit.crossBridgeCount);

            float startZ = -length * 0.35f;
            float endZ = length * 0.35f;
            float step = (endZ - startZ) / (count - 1);

            for (int i = 0; i < count; i++)
            {
                float z = startZ + i * step;

                MakeCube(root, "BridgeL_" + i,
                    new Vector3(-(ww * 0.5f + cw * 0.5f), h - bridgeThick * 0.5f, z),
                    new Vector3(cw + 0.4f, bridgeThick, bw),
                    _metalMat);

                MakeCube(root, "BridgeR_" + i,
                    new Vector3(ww * 0.5f + cw * 0.5f, h - bridgeThick * 0.5f, z),
                    new Vector3(cw + 0.4f, bridgeThick, bw),
                    _metalMat);
            }
        }

        // ─── Courtyards ─────────────────────────────────────────────────

        private static int _planterIdx = 0;

        private static void BuildCourtyards(Transform root, CityKitDefinition kit)
        {
            float h = kit.walkwayHeight;
            float ps = kit.platformSize;
            float ww = kit.walkwayWidth;
            float cw = kit.canalWidth;
            float sideW = 3.5f;
            float sideX = ww * 0.5f + cw + sideW * 0.5f;

            // Central courtyards (on central walkway)
            MakeCube(root, "CourtyardA_Spawn",
                new Vector3(0f, h - 0.05f, -16f),
                new Vector3(ps + 2f, 0.1f, ps),
                _concreteMat);

            MakeCube(root, "CourtyardB_Dispatch",
                new Vector3(0f, h - 0.05f, 0f),
                new Vector3(ps + 2f, 0.1f, ps),
                _concreteMat);

            MakeCube(root, "CourtyardC_Garden",
                new Vector3(0f, h - 0.05f, 18f),
                new Vector3(ps, 0.1f, ps),
                _concreteMat);

            BuildPlanter(root, new Vector3(-1.5f, h, 18.5f));
            BuildPlanter(root, new Vector3(1.5f, h, 19.5f));
            BuildPlanter(root, new Vector3(0f, h, 17.5f));

            // Side courtyards (on side walkways, give exploration targets)
            MakeCube(root, "CourtyardD_Service",
                new Vector3(-sideX, h - 0.05f, -8f),
                new Vector3(ps, 0.1f, ps),
                _concreteMat);

            MakeCube(root, "CourtyardE_Outlook",
                new Vector3(sideX, h - 0.05f, 10f),
                new Vector3(ps, 0.1f, ps),
                _concreteMat);

            BuildPlanter(root, new Vector3(sideX - 1f, h, 11f));
            BuildPlanter(root, new Vector3(sideX + 1f, h, 9.5f));
        }

        private static void BuildPlanter(Transform root, Vector3 pos)
        {
            MakeCube(root, "Planter_" + _planterIdx,
                pos, new Vector3(0.6f, 0.4f, 0.6f),
                new Material(_concreteMat) { name = "PlanterMat", color = new Color(0.35f, 0.28f, 0.20f) });
            _planterIdx++;
        }

        // ─── Service Catwalks (lower tier) ───────────────────────────────

        private static void BuildServiceCatwalks(Transform root, CityKitDefinition kit)
        {
            float ch = kit.serviceCatwalkHeight;
            float ww = kit.walkwayWidth;
            float cw = kit.canalWidth;
            float catwalkW = 2f;

            // Left canal catwalk (runs along inside of left canal)
            float leftX = -(ww * 0.5f + catwalkW * 0.5f + 0.3f);
            MakeCube(root, "CatwalkLeft",
                new Vector3(leftX, ch * 0.5f, 0f),
                new Vector3(catwalkW, ch, 30f),
                _catwalkMat);

            // Right canal catwalk
            float rightX = ww * 0.5f + catwalkW * 0.5f + 0.3f;
            MakeCube(root, "CatwalkRight",
                new Vector3(rightX, ch * 0.5f, 0f),
                new Vector3(catwalkW, ch, 30f),
                _catwalkMat);
        }

        // ─── Ramps (connect walkway height to catwalk height) ────────────

        private static void BuildRamps(Transform root, CityKitDefinition kit)
        {
            float upperH = kit.walkwayHeight;
            float lowerH = kit.serviceCatwalkHeight;
            float rampW = kit.rampWidth;
            float ww = kit.walkwayWidth;

            float midH = (upperH + lowerH) * 0.5f;
            float dH = upperH - lowerH;
            float rampLen = dH / Mathf.Tan(15f * Mathf.Deg2Rad);
            float rampThick = 0.2f;

            // Left ramp (south end of central walkway, descends into left catwalk zone)
            var rampL = MakeCube(root, "RampLeft",
                new Vector3(-(ww * 0.5f + 0.3f + 1f), midH, -22f),
                new Vector3(rampW, rampThick, rampLen),
                _metalMat);
            rampL.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);

            // Right ramp (north end, descends into right catwalk zone)
            var rampR = MakeCube(root, "RampRight",
                new Vector3(ww * 0.5f + 0.3f + 1f, midH, 22f),
                new Vector3(rampW, rampThick, rampLen),
                _metalMat);
            rampR.transform.localRotation = Quaternion.Euler(-15f, 0f, 0f);
        }

        // ─── Railings ───────────────────────────────────────────────────

        private static void BuildRailings(Transform root, CityKitDefinition kit)
        {
            if (!kit.railingOnEdges) return;

            float h = kit.walkwayHeight;
            float rh = kit.railingHeight;
            float rt = kit.railingThickness;
            float ww = kit.walkwayWidth;
            float cw = kit.canalWidth;
            float length = kit.mainWalkwayLength;
            float sideW = 3.5f;

            float railY = h + rh * 0.5f;

            // Central walkway railings
            MakeCube(root, "RailingCentralL",
                new Vector3(-(ww * 0.5f), railY, 0f),
                new Vector3(rt, rh, length), _railingMat);

            MakeCube(root, "RailingCentralR",
                new Vector3(ww * 0.5f, railY, 0f),
                new Vector3(rt, rh, length), _railingMat);

            // Side walkway outer railings
            float outerEdgeL = -(ww * 0.5f + cw + sideW);
            float outerEdgeR = ww * 0.5f + cw + sideW;

            MakeCube(root, "RailingOuterL",
                new Vector3(outerEdgeL, railY, 0f),
                new Vector3(rt, rh, length), _railingMat);

            MakeCube(root, "RailingOuterR",
                new Vector3(outerEdgeR, railY, 0f),
                new Vector3(rt, rh, length), _railingMat);

            // Side walkway inner railings (canal side)
            float innerEdgeL = -(ww * 0.5f + cw);
            float innerEdgeR = ww * 0.5f + cw;

            MakeCube(root, "RailingInnerSideL",
                new Vector3(innerEdgeL, railY, 0f),
                new Vector3(rt, rh, length), _railingMat);

            MakeCube(root, "RailingInnerSideR",
                new Vector3(innerEdgeR, railY, 0f),
                new Vector3(rt, rh, length), _railingMat);

            // Bridge railings
            int bridgeCount = Mathf.Max(2, kit.crossBridgeCount);
            float bw = kit.bridgeWidth;
            float startZ = -length * 0.35f;
            float endZ = length * 0.35f;
            float step = (endZ - startZ) / (bridgeCount - 1);

            for (int i = 0; i < bridgeCount; i++)
            {
                float z = startZ + i * step;

                MakeCube(root, "BridgeRailLF_" + i,
                    new Vector3(-(ww * 0.5f + cw * 0.5f), railY, z - bw * 0.5f),
                    new Vector3(cw + 0.4f, rh, rt), _railingMat);
                MakeCube(root, "BridgeRailLB_" + i,
                    new Vector3(-(ww * 0.5f + cw * 0.5f), railY, z + bw * 0.5f),
                    new Vector3(cw + 0.4f, rh, rt), _railingMat);

                MakeCube(root, "BridgeRailRF_" + i,
                    new Vector3(ww * 0.5f + cw * 0.5f, railY, z - bw * 0.5f),
                    new Vector3(cw + 0.4f, rh, rt), _railingMat);
                MakeCube(root, "BridgeRailRB_" + i,
                    new Vector3(ww * 0.5f + cw * 0.5f, railY, z + bw * 0.5f),
                    new Vector3(cw + 0.4f, rh, rt), _railingMat);
            }

            // Perimeter railings (north and south edges)
            float halfLen = length * 0.5f;
            float perimSpan = (outerEdgeR - outerEdgeL);

            MakeCube(root, "RailingPerimNorth",
                new Vector3(0f, railY, halfLen + 2f),
                new Vector3(perimSpan, rh, rt), _railingMat);

            MakeCube(root, "RailingPerimSouth",
                new Vector3(0f, railY, -(halfLen + 2f)),
                new Vector3(perimSpan, rh, rt), _railingMat);

            // Service catwalk railings
            float catwalkRailY = kit.serviceCatwalkHeight + rh * 0.5f;
            float catwalkOffsetL = -(ww * 0.5f + 0.3f + 2f);
            float catwalkOffsetR = ww * 0.5f + 0.3f + 2f;

            MakeCube(root, "CatwalkRailL_Outer",
                new Vector3(catwalkOffsetL - 1f, catwalkRailY, 0f),
                new Vector3(rt, rh, 30f), _railingMat);
            MakeCube(root, "CatwalkRailL_Inner",
                new Vector3(catwalkOffsetL + 1f, catwalkRailY, 0f),
                new Vector3(rt, rh, 30f), _railingMat);

            MakeCube(root, "CatwalkRailR_Outer",
                new Vector3(catwalkOffsetR + 1f, catwalkRailY, 0f),
                new Vector3(rt, rh, 30f), _railingMat);
            MakeCube(root, "CatwalkRailR_Inner",
                new Vector3(catwalkOffsetR - 1f, catwalkRailY, 0f),
                new Vector3(rt, rh, 30f), _railingMat);
        }

        // ─── Buildings ──────────────────────────────────────────────────

        private static void BuildBuildings(Transform root, CityKitDefinition kit)
        {
            float h = kit.walkwayHeight;
            float ww = kit.walkwayWidth;
            float cw = kit.canalWidth;
            float sideW = 3.5f;

            float outerX_L = -(ww * 0.5f + cw + sideW + 2.5f);
            float outerX_R = ww * 0.5f + cw + sideW + 2.5f;

            MakeCube(root, "Building_L1",
                new Vector3(outerX_L, h + 2f, -10f),
                new Vector3(3f, 4f + h, 5f), _buildingMat1);

            MakeCube(root, "Building_L2",
                new Vector3(outerX_L - 1.5f, h + 3.5f, 6f),
                new Vector3(4f, 7f + h, 6f), _buildingMat2);

            MakeCube(root, "Building_L3",
                new Vector3(outerX_L + 1f, h + 1.5f, 20f),
                new Vector3(3.5f, 3f + h, 4f), _buildingMat1);

            MakeCube(root, "Building_R1",
                new Vector3(outerX_R, h + 1.5f, -5f),
                new Vector3(3.5f, 3f + h, 6f), _buildingMat2);

            MakeCube(root, "Building_R2",
                new Vector3(outerX_R + 1f, h + 4f, 12f),
                new Vector3(3f, 8f + h, 4f), _buildingMat1);

            MakeCube(root, "Building_R3",
                new Vector3(outerX_R - 0.5f, h + 2f, 24f),
                new Vector3(4f, 4f + h, 3.5f), _buildingMat2);

            MakeCube(root, "Building_Central_Tall",
                new Vector3(ww * 0.5f + 1f, h + 5f, -24f),
                new Vector3(2.5f, 10f + h, 3f), _buildingMat2);
        }

        // ─── Drones + Taser ──────────────────────────────────────────────

        private static void PlaceDrones(Transform root, CityKitDefinition kit)
        {
            float h = kit.walkwayHeight;
            string[] names = { "Drone_1", "Drone_2", "Drone_3" };
            Vector3[] positions = {
                new Vector3(2f, h + 3f, -6f),
                new Vector3(-3f, h + 4f, 0f),
                new Vector3(0f, h + 3.5f, 8f)
            };

            for (int i = 0; i < names.Length; i++)
            {
                var existing = root.Find(names[i]);
                if (existing != null) continue;

                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = names[i];
                go.transform.SetParent(root, false);
                go.transform.localPosition = positions[i];
                go.transform.localScale = new Vector3(0.4f, 0.2f, 0.4f);

                var fin1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                fin1.name = "Fin_L";
                fin1.transform.SetParent(go.transform, false);
                fin1.transform.localPosition = new Vector3(-0.35f, 0.1f, 0f);
                fin1.transform.localScale = new Vector3(0.4f, 0.05f, 0.15f);

                var fin2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                fin2.name = "Fin_R";
                fin2.transform.SetParent(go.transform, false);
                fin2.transform.localPosition = new Vector3(0.35f, 0.1f, 0f);
                fin2.transform.localScale = new Vector3(0.4f, 0.05f, 0.15f);

                go.AddComponent<DroneRuntime>();
                go.GetComponent<Collider>().isTrigger = false;

                Undo.RegisterCreatedObjectUndo(go, "Drone " + (i + 1));
            }
        }

        private static void PlaceTaserDartGun(CityKitDefinition kit)
        {
            if (Object.FindObjectOfType<TaserDartGunRuntime>() != null) return;

            var defPath = "Assets/Ziptide/Content/Items/DefaultTaserDartGun.asset";
            var def = AssetDatabase.LoadAssetAtPath<TaserDartGunDefinition>(defPath);
            if (def == null)
            {
                EnsureTaserDartGunAsset(defPath);
                def = AssetDatabase.LoadAssetAtPath<TaserDartGunDefinition>(defPath);
            }
            if (def == null) return;

            float h = kit.walkwayHeight;
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "TaserDartGun";
            go.transform.position = new Vector3(1f, h + 1f, -10f);
            go.transform.localScale = new Vector3(0.07f, 0.06f, 0.25f);

            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            var grab = go.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
            var soGrab = new SerializedObject(grab);
            var movProp = soGrab.FindProperty("m_MovementType");
            if (movProp != null) movProp.enumValueIndex = 1;
            var dynProp = soGrab.FindProperty("m_UseDynamicAttach");
            if (dynProp != null) dynProp.boolValue = true;
            soGrab.ApplyModifiedPropertiesWithoutUndo();

            var grip = new GameObject("Grip");
            grip.transform.SetParent(go.transform, false);
            grip.transform.localPosition = new Vector3(0f, -0.01f, -0.06f);

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(go.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0f, 0.14f);

            var itemRt = go.AddComponent<ItemRuntime>();
            var soItem = new SerializedObject(itemRt);
            soItem.FindProperty("definition").objectReferenceValue = def;
            soItem.ApplyModifiedPropertiesWithoutUndo();

            go.AddComponent<TaserDartGunRuntime>();

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader != null)
            {
                var mat = new Material(shader);
                mat.SetColor("_BaseColor", new Color(0.2f, 0.55f, 0.6f));
                go.GetComponent<Renderer>().sharedMaterial = mat;
            }

            Undo.RegisterCreatedObjectUndo(go, "TaserDartGun");
        }

        private static void EnsureTaserDartGunAsset(string path)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content/Items"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content"))
                    AssetDatabase.CreateFolder("Assets/Ziptide", "Content");
                AssetDatabase.CreateFolder("Assets/Ziptide/Content", "Items");
            }
            var def = ScriptableObject.CreateInstance<TaserDartGunDefinition>();
            def.itemId = "taser_dart_gun";
            def.mass = 0.4f;
            def.fireCooldown = 0.4f;
            def.muzzleVelocity = 25f;
            def.dartMass = 0.05f;
            def.dartLifetime = 5f;
            def.stunSeconds = 3f;
            def.hitImpulse = 2f;
            def.hapticAmplitude = 0.6f;
            def.hapticDuration = 0.08f;
            AssetDatabase.CreateAsset(def, path);
            AssetDatabase.SaveAssets();
        }

        // ─── Helpers ────────────────────────────────────────────────────

        private static GameObject MakeCube(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = pos;
            cube.transform.localScale = scale;
            cube.transform.localRotation = Quaternion.identity;
            ApplyMat(cube, mat);
            return cube;
        }

        private static void ApplyMat(GameObject go, Material mat)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null || mat == null) return;
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;
        }
    }
}
