#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Build-hook glue for the Forge (SkyVistaAuthor's sibling): seeds missing recipe assets
    /// (create-only via ForgeRecipeLibrary) and points item definitions at their recipes so the next
    /// APK carries the generated look. The recipeId→definition mapping lives HERE, not in gameplay
    /// code — assigning a look to an item is data authorship, one line per pairing.
    /// </summary>
    public static class ForgeAuthor
    {
        // itemDefinition asset path → recipeId. One line per forged item.
        private static readonly KeyValuePair<string, string>[] Assignments =
        {
            new KeyValuePair<string, string>(
                "Assets/Ziptide/Resources/Items/DefaultTaserDartGun.asset", "taser_gun_mk1"),
        };

        [MenuItem("Ziptide/Art/Assign Forge Recipes To Items")]
        public static void AssignAllFromMenu()
        {
            ForgeRecipeLibrary.EnsureAllAuthored();
            int n = AssignAll();
            EditorUtility.DisplayDialog("Forge Author", n + " item definition(s) updated.", "OK");
        }

        /// <summary>Set forgeRecipeId on each mapped definition. Returns how many changed.</summary>
        public static int AssignAll()
        {
            int changed = 0;
            foreach (var pair in Assignments)
            {
                var def = AssetDatabase.LoadAssetAtPath<ItemDefinition>(pair.Key);
                if (def == null)
                {
                    Debug.LogWarning("[Ziptide] ForgeAuthor: no ItemDefinition at " + pair.Key);
                    continue;
                }
                if (def.forgeRecipeId == pair.Value) continue;
                def.forgeRecipeId = pair.Value;
                EditorUtility.SetDirty(def);
                changed++;
                Debug.Log("[Ziptide] ForgeAuthor assigned " + pair.Value + " -> " + pair.Key);
            }
            if (changed > 0) AssetDatabase.SaveAssets();
            return changed;
        }
    }
}
#endif
