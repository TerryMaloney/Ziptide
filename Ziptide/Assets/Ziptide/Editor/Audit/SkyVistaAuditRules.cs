#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Editor.Patching;
using Ziptide.Visuals;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// Art-track audit rules (called per non-boot scene by <see cref="WorldAuditRunner"/>):
    /// a scene the SkyVistaLibrary covers must ship with its vista assigned and valid — a canon sky
    /// silently missing or malformed fails the build instead of shipping a gray dome.
    /// Scenes outside the library (legacy/dev scenes) are ignored. The directional-light-count check
    /// is a WARNING in v1 (legacy scenes were never audited for it); it graduates to a blocker with
    /// the ART-2 perf-budget rules once a green baseline exists.
    /// </summary>
    public static class SkyVistaAuditRules
    {
        private const string ThemeFolder = "Assets/Ziptide/Content/Worlds/Themes";

        public static void Run(SceneAuditReport report)
        {
            bool inLibrary = false;
            foreach (var spec in SkyVistaLibrary.Specs())
                if (spec.Key == report.sceneName) { inLibrary = true; break; }
            if (!inLibrary) return;

            string themePath = ThemeFolder + "/" + report.sceneName + "_Theme.asset";
            var theme = AssetDatabase.LoadAssetAtPath<VisualThemeProfile>(themePath);
            if (theme == null)
            {
                // No theme seam yet (ToxicCity until its patcher authors one) — the vista waits.
                report.Warning("SKY_VISTA_UNWIRED",
                    "Scene has a canon vista in SkyVistaLibrary but no generated theme asset at " + themePath
                    + ". The sky ships legacy until the scene's patcher authors a theme (ThemeAuthor).");
                return;
            }

            if (theme.skyVista == null)
            {
                report.Blocker("SKY_VISTA_MISSING",
                    "Theme '" + themePath + "' has no skyVista but SkyVistaLibrary covers this scene. "
                    + "SkyVistaAuthor.AssignAll() should have run in the build hook.");
                return;
            }

            var issues = theme.skyVista.Validate();
            if (issues.Count > 0)
                report.Blocker("SKY_VISTA_INVALID",
                    "Vista '" + theme.skyVista.vistaId + "' failed validation: " + string.Join(" | ", issues));

            int directional = 0;
            foreach (var l in Object.FindObjectsOfType<Light>())
                if (l != null && l.type == LightType.Directional) directional++;
            if (directional > 1)
                report.Warning("MULTIPLE_DIRECTIONAL_LIGHTS",
                    directional + " directional lights in scene — the Quest budget allows exactly 1.");
        }
    }
}
#endif
