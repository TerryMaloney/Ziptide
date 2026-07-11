#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// FORGE III F3.1b commit 3 — PRACTICAL PLACEMENT (called from WorldDressingBuilder.Build,
    /// same hook as the scatter): a sconce beside each generated doorway (the __DOOR markers,
    /// sorted for determinism), street poles on ~14m rhythm along the cairn route, and two
    /// lanterns at POI approaches. Each practical = an editor-visible PRIMITIVE fallback +
    /// ForgeModuleLook (baked fixture on device, the E5.1 wall pattern) + PracticalLight DATA
    /// (head local + pool point/normal — the component builds its halo/pool quads at RUNTIME;
    /// generated materials never serialize into the scene). The two lanterns carry the world's
    /// entire hero budget of REAL point lights (range 8, no shadows). ≤14 practicals per world.
    /// </summary>
    public static class PracticalAuthor
    {
        public const int MaxSconces = 6;
        public const int MaxPoles = 6;
        public const int MaxLanterns = 2; // these two ARE the hero real-light budget
        private static readonly Color LampAmber = new Color(1f, 0.76f, 0.44f);

        public static int Place(Transform parent, CityLayoutDefinition kit, List<Vector2> route)
        {
            var root = new GameObject("Practicals").transform;
            root.SetParent(parent, false);
            int placed = 0;

            // ── ① Sconces beside doorways (markers sorted by position → deterministic) ──
            var doors = new List<Transform>();
            foreach (var t in Object.FindObjectsOfType<Transform>())
                if (t != null && t.name == "__DOOR" && t.parent != null) doors.Add(t.parent);
            doors.Sort((a, b) =>
            {
                int c = a.position.x.CompareTo(b.position.x);
                return c != 0 ? c : a.position.z.CompareTo(b.position.z);
            });
            for (int i = 0; i < doors.Count && i < MaxSconces; i++)
            {
                var frame = doors[i];
                var pl = NewPractical(root, "Sconce_" + i, "light_sconce_wall",
                    new Vector3(0.10f, 0.45f, 0.06f), new Vector3(0f, 0.25f, 0.03f));
                pl.transform.position = frame.TransformPoint(new Vector3(0.9f, 1.55f, 0.14f));
                pl.transform.rotation = frame.rotation; // plate's +Z goes against the wall
                pl.headLocal = new Vector3(0f, 0.33f, -0.14f);
                pl.haloSize = 0.45f;
                // Wall WASH above the sconce: on the wall plane, facing outward with the frame.
                pl.hasPool = true;
                pl.poolPoint = frame.TransformPoint(new Vector3(0.9f, 2.3f, 0.16f));
                pl.poolNormal = frame.forward;
                pl.poolSize = 1.1f;
                placed++;
            }

            // ── ② Street poles on ~14m rhythm along the route, offset off the walking line ──
            int poles = 0;
            if (route != null)
                for (int leg = 0; leg < route.Count - 1 && poles < MaxPoles; leg++)
                {
                    Vector2 a = route[leg], b = route[leg + 1];
                    int steps = Mathf.FloorToInt(Vector2.Distance(a, b) / 14f);
                    for (int s = 1; s < steps && poles < MaxPoles; s += 2) // every other slot breathes
                    {
                        Vector2 side = new Vector2(-(b - a).y, (b - a).x).normalized * 2.2f;
                        Vector2 p = Vector2.Lerp(a, b, s / (float)steps) + side;
                        float y = WorldExperienceBuilder.HeightAt(kit, p.x, p.y);

                        var pl = NewPractical(root, "StreetPole_" + poles, "light_street_pole",
                            new Vector3(0.12f, 3.0f, 0.12f), new Vector3(0f, 1.5f, 0f));
                        pl.transform.position = new Vector3(p.x, y, p.y);
                        // The arm reaches back over the path the pole stands beside.
                        pl.transform.rotation = Quaternion.LookRotation(new Vector3(-side.x, 0f, -side.y));
                        pl.headLocal = new Vector3(0f, 2.84f, 0.52f);
                        pl.haloSize = 0.7f;
                        pl.hasPool = true; // the pool of lamplight on the path
                        pl.poolPoint = pl.transform.TransformPoint(new Vector3(0f, 0.02f, 0.52f));
                        pl.poolNormal = Vector3.up;
                        pl.poolSize = 2.4f;
                        placed++; poles++;
                    }
                }

            // ── ③ Lanterns at the first two POI approaches — the HERO practicals (real lights) ──
            int lanterns = 0;
            Vector2 spawn = route != null && route.Count > 0 ? route[0] : Vector2.zero;
            if (kit.pois != null)
                foreach (var poi in kit.pois)
                {
                    if (poi == null || poi.type == PoiType.TravelBerth || lanterns >= MaxLanterns) continue;
                    Vector2 pp = new Vector2(poi.position.x, poi.position.z);
                    Vector2 dir = (spawn - pp).sqrMagnitude > 1f ? (spawn - pp).normalized : Vector2.up;
                    Vector2 p = pp + dir * 3.5f;
                    float y = WorldExperienceBuilder.HeightAt(kit, p.x, p.y);

                    // A thin dark post carries the hook; the lantern's LOOK rides a lifted child
                    // (the recipe renders at its GO's origin) so it hangs near the post top, while
                    // the post fallback stays visible either way — the lantern needs its post.
                    var pl = NewPractical(root, "Lantern_" + lanterns, null,
                        new Vector3(0.08f, 2.4f, 0.08f), new Vector3(0f, 1.2f, 0f));
                    pl.transform.position = new Vector3(p.x, y, p.y);
                    pl.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.y));
                    var fixtureLift = new GameObject("LanternFixture");
                    fixtureLift.transform.SetParent(pl.transform, false);
                    fixtureLift.transform.localPosition = new Vector3(0f, 1.75f, 0.05f);
                    var liftLook = fixtureLift.AddComponent<ForgeModuleLook>();
                    liftLook.recipeId = "light_lantern_hang";
                    liftLook.keepChildren = new string[0];
                    pl.headLocal = new Vector3(0f, 2.13f, 0.09f); // glass (0,0.38,0.04) + lift
                    pl.haloSize = 0.5f;
                    pl.hasPool = true;
                    pl.poolPoint = pl.transform.position + Vector3.up * 0.02f;
                    pl.poolNormal = Vector3.up;
                    pl.poolSize = 1.8f;

                    // THE HERO BUDGET: the world's ≤2 real point lights live here and nowhere else.
                    var light = pl.gameObject.AddComponent<Light>();
                    light.type = LightType.Point;
                    light.range = 8f;
                    light.intensity = 1.2f;
                    light.color = LampAmber;
                    light.shadows = LightShadows.None;

                    placed++; lanterns++;
                }

            Debug.Log("[Ziptide] PracticalAuthor: placed=" + placed
                + " (sconces=" + Mathf.Min(doors.Count, MaxSconces)
                + " poles=" + poles + " lanterns=" + lanterns + ")");
            return placed;
        }

        /// <summary>Holder + editor-visible primitive fallback + ForgeModuleLook (skipped when
        /// <paramref name="recipeId"/> is null — the lantern mounts its look on a lifted child)
        /// + PracticalLight (data only — the component builds its quads at runtime).</summary>
        private static PracticalLight NewPractical(Transform root, string name, string recipeId,
            Vector3 fallbackSize, Vector3 fallbackCenter)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root, false);

            var fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fallback.name = "Fallback";
            var col = fallback.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col); // dressing never blocks movement
            fallback.transform.SetParent(go.transform, false);
            fallback.transform.localPosition = fallbackCenter;
            fallback.transform.localScale = fallbackSize;
            var fr = fallback.GetComponent<Renderer>();
            if (fr != null)
            {
                fr.sharedMaterial = FixtureMat();
                fr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            if (!string.IsNullOrEmpty(recipeId))
            {
                var look = go.AddComponent<ForgeModuleLook>();
                look.recipeId = recipeId;
                look.keepChildren = new string[0];
            }

            var pl = go.AddComponent<PracticalLight>();
            pl.glowColor = LampAmber;
            return pl;
        }

        private static Material _fixtureMat;

        private static Material FixtureMat()
        {
            if (_fixtureMat != null) return _fixtureMat;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            _fixtureMat = new Material(shader) { name = "PracticalFixture_Iron" };
            if (_fixtureMat.HasProperty("_BaseColor"))
                _fixtureMat.SetColor("_BaseColor", new Color(0.22f, 0.21f, 0.24f));
            return _fixtureMat;
        }
    }
}
#endif
