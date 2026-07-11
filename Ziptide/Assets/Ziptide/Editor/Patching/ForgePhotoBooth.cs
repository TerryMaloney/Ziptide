#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// The Forge studio's EYES: renders turnaround photos of generated assets to PNGs so an LLM session
    /// (which can view images) can iterate a recipe's look without a human or a headset in the loop.
    /// Runs from a menu (Terry) or batchmode via <c>forge-photos.yml</c> (game-ci unity-builder —
    /// graphics-enabled under xvfb; NEVER the test-runner, which may inject -nographics).
    /// Output: <c>Ziptide/Builds/Photos/&lt;id&gt;/&lt;angle&gt;.png</c>, uploaded as the
    /// <c>forge-photos</c> CI artifact. Until ForgeRecipeLibrary lands, renders a calibration target
    /// (primitive gun-like rig) proving the camera/light/PNG path end-to-end.
    /// </summary>
    public static class ForgePhotoBooth
    {
        private const int ImageSize = 768;
        private const float CameraFovDeg = 30f;

        // name, yaw°, pitch°, distance multiplier (1 = framed by bounds ×1.4)
        private static readonly (string name, float yaw, float pitch, float dist)[] Shots =
        {
            ("01_front", 0f, 10f, 1f),
            ("02_threequarter", 35f, 25f, 1f),
            ("03_right", 90f, 10f, 1f),
            ("04_back", 180f, 10f, 1f),
            ("05_left", -90f, 10f, 1f),
            ("06_top", 0f, 75f, 1f),
            ("07_closeup", 25f, 15f, 0.55f),
        };

        [MenuItem("Ziptide/Art/Forge Photo Booth (render all)")]
        public static void RenderAllFromMenu()
        {
            int shots = RenderAll();
            EditorUtility.DisplayDialog("Forge Photo Booth",
                shots + " photo(s) written to Builds/Photos.", "OK");
        }

        /// <summary>CI entrypoint (game-ci buildMethod). Throws on failure so the run goes red.</summary>
        public static void RenderAllBatch()
        {
            int shots = RenderAll();
            if (shots <= 0) throw new System.Exception("Forge photo booth produced no images.");
            Debug.Log("ZIPTIDE: FORGE_PHOTO_OK shots=" + shots);
        }

        /// <summary>Render every photo subject in a throwaway scene. Returns total PNG count.</summary>
        public static int RenderAll()
        {
            string outRoot = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Builds", "Photos");
            Directory.CreateDirectory(outRoot);

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            int shots = 0;
            foreach (var subject in Subjects())
            {
                try { shots += RenderTarget(subject.root, subject.id, outRoot); }
                finally { Object.DestroyImmediate(subject.root); }
            }
            return shots;
        }

        /// <summary>
        /// The list of things to photograph: every recipe in the studio's catalog, built through the
        /// exact runtime path (ForgeMesh + ForgeMaterials), plus every CREATURE BODY in
        /// ForgeBodyLibrary — built through the real skinning core and frozen at a mid-stride gait
        /// frame, so the photos critique the walk pose, not just the sculpt. Falls back to the
        /// calibration rig if both catalogs are ever empty so the pipeline itself stays verifiable.
        /// </summary>
        private static IEnumerable<(GameObject root, string id)> Subjects()
        {
            var specs = ForgeRecipeLibrary.Specs();
            var bodies = ForgeBodyLibrary.Specs();
            if (specs.Count == 0 && bodies.Count == 0)
            {
                yield return (BuildCalibrationTarget(), "spike_calibration");
                yield break;
            }
            foreach (var spec in specs)
            {
                var recipe = spec.Value();
                var root = new GameObject("Forge_" + spec.Key);
                var mf = root.AddComponent<MeshFilter>();
                mf.sharedMesh = Ziptide.Visuals.ForgeMesh.BuildSingle(recipe);
                var mr = root.AddComponent<MeshRenderer>();
                mr.sharedMaterial = BuildSingleTexturedMaterial(recipe);
                yield return (root, spec.Key);
            }
            foreach (var spec in bodies)
                yield return (BuildBodySubject(spec.Key, spec.Value()), "body_" + spec.Key);

            // FORGE III F3.3 — WATER: a lit water tile so the ripple normal + specular read is
            // verifiable before any runtime/device machinery exists (booth-first, like Forge was).
            yield return (BuildWaterSubject(), "water_tile");
        }

        /// <summary>A 4×4m water plane under the booth light: the baked ripple normal (swizzled for
        /// the desktop URP/Lit _BumpMap like the creature-normal path) + high smoothness + a deep
        /// canal tint. Static frame — it proves the surface SHAPE and gloss; motion/foam land in
        /// commit 2.</summary>
        private static GameObject BuildWaterSubject()
        {
            const int size = 512;
            var root = new GameObject("ZiptideWater");
            var mf = root.AddComponent<MeshFilter>();
            mf.sharedMesh = Ziptide.Visuals.ZiptideWaterMesh.Build(4f, 4f, 32);
            var mr = root.AddComponent<MeshRenderer>();

            var px = Ziptide.Visuals.WaterSurface.BakeNormalMap(size, 1.4f);
            // Desktop URP/Lit unpacks _BumpMap DXT5nm-style (x in A, y in G) — same swizzle the
            // recipe/creature booth path uses for its canonical RGB normal bake.
            for (int i = 0; i < px.Length; i++) px[i] = new Color32(255, px[i].g, 255, px[i].r);
            var normalTex = new Texture2D(size, size, TextureFormat.RGBA32, false, true) { wrapMode = TextureWrapMode.Repeat };
            normalTex.SetPixels32(px);
            normalTex.Apply(false, false);

            var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothWater" };
            m.SetColor("_BaseColor", new Color(0.06f, 0.16f, 0.18f, 1f)); // deep canal teal
            m.SetTexture("_BumpMap", normalTex);
            m.SetFloat("_BumpScale", 1f);
            m.EnableKeyword("_NORMALMAP");
            m.SetFloat("_Smoothness", 0.85f); // wet specular is most of the water read
            m.SetFloat("_Metallic", 0f);
            mr.sharedMaterial = m;
            return root;
        }

        /// <summary>
        /// A creature body posed MID-STRIDE: skinned through ForgeSkinnedBuilder, per-slot flat
        /// materials (the runtime dev look, eye emissive), and one ForgeGaitMotor frame at full
        /// speed composed onto the bones — so a bad knee bend or crossed legs shows up in the
        /// turnaround, not on the headset.
        /// </summary>
        private static GameObject BuildBodySubject(string id, Ziptide.Visuals.ForgeCreatureBody body)
        {
            var root = new GameObject("ForgeBody_" + id);
            var r = Ziptide.Visuals.ForgeSkinnedBuilder.Build(body);
            r.skeletonRoot.transform.SetParent(root.transform, false);

            var smr = root.AddComponent<SkinnedMeshRenderer>();
            smr.bones = r.bones;
            smr.sharedMesh = r.mesh;
            smr.rootBone = r.bones[0];
            smr.updateWhenOffscreen = true; // edit-mode bounds come from the skinned verts

            var mats = new Material[r.paletteSlots.Length];
            for (int i = 0; i < r.paletteSlots.Length; i++)
            {
                int slot = r.paletteSlots[i];
                Color c = body.palette != null && slot < body.palette.Length ? body.palette[slot] : Color.magenta;
                var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                m.SetColor("_BaseColor", c);
                if (slot == body.eyePaletteSlot)
                {
                    m.EnableKeyword("_EMISSION");
                    m.SetColor("_EmissionColor", c * 1.1f); // booth has no tonemapping — clamp
                }
                mats[i] = m;
            }

            // CREATURE TEXTURE BAKE: a body with authored slotStyles gets the same baked-surface
            // treatment recipes get — one textured material across the body submeshes (the SAME
            // synthetic recipe the mesh's UVs came from), while the eye keeps its live emissive.
            if (body.slotStyles != null && body.slotStyles.Length > 0)
            {
                var synth = Ziptide.Visuals.ForgeSkinnedBuilder.SyntheticRecipe(body);
                try
                {
                    var texturedMat = BuildSingleTexturedMaterial(synth);
                    for (int i = 0; i < mats.Length; i++)
                        if (r.paletteSlots[i] != body.eyePaletteSlot) mats[i] = texturedMat;
                }
                finally { Object.DestroyImmediate(synth); }
            }
            smr.sharedMaterials = mats;

            // Freeze one full-speed gait frame (t chosen mid-swing; same math the device runs).
            var q = new Quaternion[body.BoneCount()];
            Ziptide.Visuals.ForgeGaitMotor.Evaluate(body, 0.33f, 1f, q);
            for (int i = 1; i < r.bones.Length && i < q.Length; i++)
                r.bones[i].localRotation = r.bones[i].localRotation * q[i];

            return root;
        }

        /// <summary>
        /// A gun-shaped primitive rig (~taser proportions, 22 cm) with distinct colors per face-direction
        /// so the photos prove orientation, framing, lighting, and color fidelity all at once.
        /// </summary>
        private static GameObject BuildCalibrationTarget()
        {
            var root = new GameObject("ForgeCalibration");

            GameObject Part(string name, PrimitiveType type, Vector3 pos, Vector3 scale, Color color)
            {
                var go = GameObject.CreatePrimitive(type);
                go.name = name;
                go.transform.SetParent(root.transform, false);
                go.transform.localPosition = pos;
                go.transform.localScale = scale;
                Object.DestroyImmediate(go.GetComponent<Collider>());
                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.SetColor("_BaseColor", color);
                go.GetComponent<Renderer>().sharedMaterial = mat;
                return go;
            }

            // Receiver (rust red), barrel forward +Z (teal — RILL cyan family), grip down (dark), sight up (gold).
            Part("Receiver", PrimitiveType.Cube, Vector3.zero, new Vector3(0.035f, 0.045f, 0.14f), new Color(0.45f, 0.20f, 0.12f));
            Part("Barrel", PrimitiveType.Cylinder, new Vector3(0f, 0.005f, 0.10f), new Vector3(0.02f, 0.03f, 0.02f), new Color(0.30f, 0.80f, 0.95f))
                .transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Part("Grip", PrimitiveType.Cube, new Vector3(0f, -0.05f, -0.045f), new Vector3(0.028f, 0.07f, 0.035f), new Color(0.12f, 0.12f, 0.14f))
                .transform.localRotation = Quaternion.Euler(-20f, 0f, 0f);
            Part("Sight", PrimitiveType.Cube, new Vector3(0f, 0.032f, 0.03f), new Vector3(0.01f, 0.012f, 0.03f), new Color(0.85f, 0.70f, 0.30f));
            return root;
        }

        /// <summary>
        /// FORGE II E1.3: ONE URP/Lit material carrying the full baked set — albedo, tangent-space
        /// normal, _MetallicGlossMap (R=metal, A=smooth), _EmissionMap (glow slots only). Pairs
        /// with ForgeMesh.BuildSingle: per-slot identity lives in the maps, not in submeshes, so
        /// photo critique sees the shipping surface response at one draw call.
        /// </summary>
        private static Material BuildSingleTexturedMaterial(Ziptide.Visuals.ForgeRecipeDefinition recipe)
        {
            const int size = 1024;
            var meta = Ziptide.Visuals.ForgeTexture.BakeMeta(recipe, size);
            var px = new Color32[size * size];

            Texture2D Tex(bool linear)
            {
                var t = new Texture2D(size, size, TextureFormat.RGBA32, false, linear);
                t.SetPixels32(px);
                t.Apply(false, false);
                t.filterMode = FilterMode.Bilinear;
                t.anisoLevel = 0; // grazing-angle aniso footprints reach across atlas gutters
                return t;
            }

            Ziptide.Visuals.ForgeTexture.BakeAlbedo(recipe, meta, size, px);
            var albedo = Tex(false);
            Ziptide.Visuals.ForgeTexture.BakeMSA(recipe, meta, size, px);
            var msa = Tex(true);
            Ziptide.Visuals.ForgeTexture.BakeEmissive(recipe, meta, size, px);
            var emissive = Tex(false);
            Ziptide.Visuals.ForgeTexture.BakeNormal(recipe, meta, size, px);
            var normalCanonical = Tex(true);

            // Studio X-ray: dump the baked atlases beside the turnarounds so a session can inspect
            // the maps themselves when a photo shows a surface defect it can't attribute.
            string atlasDir = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Builds", "Photos", recipe.recipeId);
            Directory.CreateDirectory(atlasDir);
            File.WriteAllBytes(Path.Combine(atlasDir, "atlas_albedo.png"), albedo.EncodeToPNG());
            File.WriteAllBytes(Path.Combine(atlasDir, "atlas_msa.png"), msa.EncodeToPNG());
            File.WriteAllBytes(Path.Combine(atlasDir, "atlas_emissive.png"), emissive.EncodeToPNG());
            File.WriteAllBytes(Path.Combine(atlasDir, "atlas_normal.png"), normalCanonical.EncodeToPNG());
            Object.DestroyImmediate(normalCanonical);

            // Desktop URP/Lit unpacks _BumpMap as DXT5nm-style (x in A, y in G) — swizzle the
            // canonical RGB bake for the editor booth. E1.4's PNG import converts properly via
            // TextureImporter; the on-device runtime path skips normal maps entirely.
            for (int i = 0; i < px.Length; i++) px[i] = new Color32(255, px[i].g, 255, px[i].r);
            var normal = Tex(true);

            var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "BoothSingle_" + recipe.recipeId };
            m.SetTexture("_BaseMap", albedo);
            m.SetColor("_BaseColor", Color.white);
            m.SetTexture("_BumpMap", normal);
            m.EnableKeyword("_NORMALMAP");
            m.SetTexture("_MetallicGlossMap", msa);
            m.SetFloat("_Smoothness", 1f); // the baked A channel is the truth — don't halve it
            m.EnableKeyword("_METALLICGLOSSMAP");
            if (Ziptide.Visuals.ForgeTexture.HasLeafSlot(recipe))
            {
                // E5.3 flora: preview the same alpha-clip cutout the baked material ships with.
                m.SetFloat("_AlphaClip", 1f);
                m.SetFloat("_Cutoff", 0.5f);
                m.EnableKeyword("_ALPHATEST_ON");
            }

            float maxI = Ziptide.Visuals.ForgeTexture.MaxEmissiveIntensity(recipe);
            if (maxI > 0f)
            {
                m.SetTexture("_EmissionMap", emissive);
                // The booth camera has no tonemapping — full recipe intensity clips to white
                // blobs (first checkpoint photos). Preview clamps; the game path uses the real
                // intensity with URP handling.
                m.SetColor("_EmissionColor", Color.white * Mathf.Min(maxI, 1.1f));
                m.EnableKeyword("_EMISSION");
            }
            return m;
        }

        /// <summary>Photograph one root object from all shot angles. Returns PNG count written.</summary>
        public static int RenderTarget(GameObject target, string id, string outRoot)
        {
            var bounds = ComputeBounds(target);
            if (bounds.size.sqrMagnitude <= 0f) return 0;

            string dir = Path.Combine(outRoot, id);
            Directory.CreateDirectory(dir);

            // Pin the environment: the default procedural skybox leaks into ambient + glossy
            // reflections (E1.3's smooth surfaces picked up cyan sky rims — a critique artifact,
            // not the asset). Flat dark ambient, no reflection probe — the 3 lights are the truth.
            var prevSkybox = RenderSettings.skybox;
            var prevAmbientMode = RenderSettings.ambientMode;
            var prevAmbientLight = RenderSettings.ambientLight;
            var prevReflectionMode = RenderSettings.defaultReflectionMode;
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.16f, 0.16f, 0.17f);
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            RenderSettings.customReflectionTexture = null;

            // 3-point light rig (isolated throwaway scene — light count here never ships).
            var lights = new List<GameObject>();
            GameObject Rig(string name, Vector3 euler, float intensity, Color color)
            {
                var go = new GameObject(name);
                var l = go.AddComponent<Light>();
                l.type = LightType.Directional;
                l.intensity = intensity;
                l.color = color;
                go.transform.rotation = Quaternion.Euler(euler);
                lights.Add(go);
                return go;
            }
            Rig("Booth_Key", new Vector3(35f, -30f, 0f), 1.2f, Color.white);
            Rig("Booth_Fill", new Vector3(15f, 140f, 0f), 0.45f, new Color(0.9f, 0.95f, 1f));
            Rig("Booth_Rim", new Vector3(10f, 210f, 0f), 0.8f, new Color(1f, 0.95f, 0.9f));

            var camGo = new GameObject("Booth_Camera");
            var cam = camGo.AddComponent<Camera>();
            cam.fieldOfView = CameraFovDeg;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.18f, 0.18f, 0.19f);
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 100f;

            var rt = new RenderTexture(ImageSize, ImageSize, 24, RenderTextureFormat.ARGB32);
            var tex = new Texture2D(ImageSize, ImageSize, TextureFormat.RGB24, false);
            int written = 0;

            try
            {
                float radius = bounds.extents.magnitude;
                float baseDist = radius * 1.4f / Mathf.Tan(CameraFovDeg * Mathf.Deg2Rad * 0.5f);

                foreach (var shot in Shots)
                {
                    Quaternion orbit = Quaternion.Euler(shot.pitch, shot.yaw, 0f);
                    Vector3 offset = orbit * new Vector3(0f, 0f, -baseDist * shot.dist);
                    cam.transform.position = bounds.center + offset;
                    cam.transform.LookAt(bounds.center);

                    cam.targetTexture = rt;
                    cam.Render();
                    RenderTexture.active = rt;
                    tex.ReadPixels(new Rect(0, 0, ImageSize, ImageSize), 0, 0);
                    tex.Apply(false);
                    RenderTexture.active = null;
                    cam.targetTexture = null;

                    File.WriteAllBytes(Path.Combine(dir, shot.name + ".png"), tex.EncodeToPNG());
                    Debug.Log("ZIPTIDE: FORGE_PHOTO id=" + id + " shot=" + shot.name);
                    written++;
                }
            }
            finally
            {
                RenderTexture.active = null;
                Object.DestroyImmediate(rt);
                Object.DestroyImmediate(tex);
                Object.DestroyImmediate(camGo);
                foreach (var l in lights) Object.DestroyImmediate(l);
                RenderSettings.skybox = prevSkybox;
                RenderSettings.ambientMode = prevAmbientMode;
                RenderSettings.ambientLight = prevAmbientLight;
                RenderSettings.defaultReflectionMode = prevReflectionMode;
            }

            return written;
        }

        private static Bounds ComputeBounds(GameObject root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(root.transform.position, Vector3.zero);
            Bounds b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
            return b;
        }
    }
}
#endif
