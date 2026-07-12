using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// FIELD CAMERA — capture-time composition translator. The expensive scene scan runs only when
    /// the shutter fires; the live viewfinder never calls it. Naming classification follows existing
    /// authored vocabulary and stays deterministic/case-insensitive.
    /// </summary>
    public static class PhotoSubjectDetector
    {
        public struct Detection
        {
            public int skyTier;
            public bool horizonInFrame;
            public bool occludedBodyInFrame;
            public bool landmarkInFrame;
            public bool creatureInFrame;
            public float framingCentered;
            public string subjectId;
        }

        public static Detection Detect(Camera camera)
        {
            var result = new Detection
            {
                skyTier = Object.FindObjectOfType<Ziptide.Visuals.SkyVistaRig>() != null ? 2 : 1,
                horizonInFrame = camera != null && HorizonInFrame(camera.transform.forward, camera.fieldOfView),
                framingCentered = 0f,
                subjectId = string.Empty
            };
            if (camera == null) return result;

            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            Renderer[] renderers = Object.FindObjectsOfType<Renderer>();
            float bestCentered = 0f;
            string bestId = string.Empty;

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy) continue;
                if (renderer.transform.IsChildOf(camera.transform)) continue;
                if (!GeometryUtility.TestPlanesAABB(planes, renderer.bounds)) continue;

                string key = ClassificationKey(renderer.transform);
                bool body = IsCelestialKey(key);
                bool landmark = IsLandmarkKey(key);
                bool creature = IsCreatureKey(key);
                if (!body && !landmark && !creature) continue;

                result.occludedBodyInFrame |= body;
                result.landmarkInFrame |= landmark;
                result.creatureInFrame |= creature;

                Vector3 viewport = camera.WorldToViewportPoint(renderer.bounds.center);
                float centered = Centering01(viewport);
                if (centered > bestCentered)
                {
                    bestCentered = centered;
                    bestId = StableSubjectId(renderer.transform);
                }
            }

            result.framingCentered = bestCentered;
            result.subjectId = bestId;
            return result;
        }

        /// <summary>Whether the horizontal world horizon falls inside a vertical camera FOV.</summary>
        public static bool HorizonInFrame(Vector3 forward, float verticalFov)
        {
            if (forward.sqrMagnitude < 0.000001f) return false;
            forward.Normalize();
            float pitch = Mathf.Abs(Mathf.Asin(Mathf.Clamp(forward.y, -1f, 1f)) * Mathf.Rad2Deg);
            float halfFov = Mathf.Clamp(verticalFov, 1f, 179f) * 0.5f;
            return pitch <= halfFov;
        }

        /// <summary>1 at the frame center, 0 at or beyond the nearest viewport edge/behind camera.</summary>
        public static float Centering01(Vector3 viewport)
        {
            if (viewport.z <= 0f) return 0f;
            float dx = Mathf.Abs(viewport.x - 0.5f) * 2f;
            float dy = Mathf.Abs(viewport.y - 0.5f) * 2f;
            return Mathf.Clamp01(1f - Mathf.Max(dx, dy));
        }

        public static bool IsCelestialKey(string key)
        {
            key = Normalize(key);
            return key.Contains("skybody") || key.Contains("celestial") || key.Contains("planet") ||
                   key.Contains("moon") || key.Contains("gasgiant") || key.Contains("orbitalbody");
        }

        public static bool IsLandmarkKey(string key)
        {
            key = Normalize(key);
            return key.Contains("hero_") || key.Contains("arrivalvista") || key.Contains("storyanchor") ||
                   key.Contains("landmark") || key.Contains("vista_") || key.Contains("poi_story");
        }

        public static bool IsCreatureKey(string key)
        {
            key = Normalize(key);
            return key.Contains("creature") || key.Contains("swarm_bug") || key.Contains("light_grazer") ||
                   key.Contains("tendril") || key.Contains("witness_mite") || key.Contains("husk_molter") ||
                   key.Contains("warden") || key.Contains("tether_swarm");
        }

        private static string ClassificationKey(Transform transform)
        {
            var builder = new System.Text.StringBuilder();
            for (Transform current = transform; current != null; current = current.parent)
            {
                if (builder.Length > 0) builder.Append('/');
                builder.Append(current.name);
            }
            return builder.ToString();
        }

        private static string StableSubjectId(Transform transform)
        {
            for (Transform current = transform; current != null; current = current.parent)
            {
                string name = current.name;
                if (IsCelestialKey(name) || IsLandmarkKey(name) || IsCreatureKey(name)) return name;
            }
            return transform != null ? transform.name : string.Empty;
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.ToLowerInvariant();
        }
    }
}
