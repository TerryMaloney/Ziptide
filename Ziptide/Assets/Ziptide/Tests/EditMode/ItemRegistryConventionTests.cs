using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// CI guard for the ItemFactory IL2CPP-safety convention: EVERY ItemDefinition asset must live
    /// under a Resources/Items folder, because that is the only lookup path that reliably works on
    /// device (the loaded-objects fallback only sees assets a loaded scene happens to reference — a
    /// misplaced definition "works in testing" then vanishes after travel). This test makes a
    /// misplaced item a CI failure instead of a device mystery.
    /// </summary>
    public class ItemRegistryConventionTests
    {
        [Test]
        public void EveryItemDefinitionAsset_LivesUnderResourcesItems()
        {
            var offenders = new List<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:ItemDefinition"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Contains("/Resources/Items/"))
                    offenders.Add(path);
            }
            Assert.IsEmpty(offenders,
                "ItemDefinition assets outside Resources/Items (ItemFactory can't reliably find these " +
                "on device — move them):\n" + string.Join("\n", offenders));
        }

        [Test]
        public void EveryItemDefinition_HasAUniqueNonEmptyId()
        {
            var seen = new Dictionary<string, string>();
            var problems = new List<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:ItemDefinition"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var def = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
                if (def == null) continue;
                if (string.IsNullOrEmpty(def.itemId)) { problems.Add("empty itemId: " + path); continue; }
                if (seen.TryGetValue(def.itemId, out var first))
                    problems.Add("duplicate itemId '" + def.itemId + "': " + first + " AND " + path);
                else
                    seen[def.itemId] = path;
            }
            Assert.IsEmpty(problems, string.Join("\n", problems));
        }
    }
}
