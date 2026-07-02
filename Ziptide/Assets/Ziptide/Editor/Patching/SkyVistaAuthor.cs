#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Assigns each authored <see cref="SkyVistaDefinition"/> onto its world's generated
    /// <c>Content/Worlds/Themes/&lt;Scene&gt;_Theme.asset</c> (worlds AND arenas — both flow through
    /// ThemeAuthor). This is the entire integration: no layout, patcher, or scene file changes, because
    /// <see cref="VisualThemeProfile.skyVista"/> rides the existing theme seam and ThemeAuthor's
    /// field-by-field regeneration never touches it. Scenes whose theme doesn't exist yet (e.g.
    /// ToxicCity until its patcher authors a theme) are skipped silently — the vista asset waits.
    /// Runs from the build hook after the per-scene loop (themes exist by then), before the audit.
    /// </summary>
    public static class SkyVistaAuthor
    {
        private const string ThemeFolder = "Assets/Ziptide/Content/Worlds/Themes";

        [MenuItem("Ziptide/Art/Assign Sky Vistas To World Themes")]
        public static void AssignAllFromMenu()
        {
            int assigned = AssignAll();
            EditorUtility.DisplayDialog("Sky Vista Author",
                assigned + " theme(s) updated with their sky vista. Themes without a generated asset were skipped.", "OK");
        }

        /// <summary>Point every generated theme at its canon vista. Returns how many were (re)assigned.</summary>
        public static int AssignAll()
        {
            int assigned = 0;
            foreach (var spec in SkyVistaLibrary.Specs())
            {
                string themePath = ThemeFolder + "/" + spec.Key + "_Theme.asset";
                var theme = AssetDatabase.LoadAssetAtPath<VisualThemeProfile>(themePath);
                if (theme == null) continue; // scene not generated (or not themed) yet — the vista waits

                var vista = AssetDatabase.LoadAssetAtPath<SkyVistaDefinition>(SkyVistaLibrary.AssetPathFor(spec.Key));
                if (vista == null) continue; // library not seeded — EnsureAllAuthored runs first in the build hook

                if (theme.skyVista == vista) continue;
                theme.skyVista = vista;
                EditorUtility.SetDirty(theme);
                assigned++;
                Debug.Log("[Ziptide] SkyVistaAuthor assigned " + vista.vistaId + " -> " + spec.Key);
            }
            if (assigned > 0) AssetDatabase.SaveAssets();
            return assigned;
        }
    }
}
#endif
