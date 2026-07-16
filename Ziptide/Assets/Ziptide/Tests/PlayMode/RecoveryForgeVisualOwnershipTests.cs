using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryForgeVisualOwnershipTests
    {
        private const string ForgeVisualName = "ForgeVisual";
        private GameObject _item;

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_item != null) Object.Destroy(_item);
            _item = null;
            yield return null;
        }

        [UnityTest]
        public IEnumerator SceneAuthoredItemAwake_ReplacesStaleBakedChildWithValidOwnedSurface()
        {
            // The PlayMode assembly intentionally depends on Gameplay rather than the Content/Visuals
            // implementation assemblies. Load the real serialized definition as a Unity object and set
            // ItemRuntime's existing serialized seam through reflection; the behavior under proof remains
            // ItemRuntime.Awake -> ForgeVisualApplier, exactly as it is for a scene-authored weapon.
            Object definition = Resources.Load("Items/DefaultPistol");
            Assert.IsNotNull(definition, "DefaultPistol definition is missing from Resources/Items.");
            FieldInfo recipeField = FindField(definition.GetType(), "forgeRecipeId");
            Assert.IsNotNull(recipeField, "DefaultPistol definition has no forgeRecipeId field.");
            string recipeId = recipeField.GetValue(definition) as string;
            Assert.IsFalse(string.IsNullOrEmpty(recipeId),
                "DefaultPistol has no Forge recipe for the ownership canary.");

            _item = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _item.name = "__RECOVERY_SCENE_AUTHORED_FORGE_ITEM";
            _item.SetActive(false);
            _item.AddComponent<Rigidbody>();
            _item.AddComponent<XRGrabInteractable>();

            var visual = new GameObject(ForgeVisualName);
            visual.transform.SetParent(_item.transform, false);
            GameObject stale = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stale.name = "prefab(Clone)";
            stale.transform.SetParent(visual.transform, false);
            Renderer staleRenderer = stale.GetComponent<Renderer>();
            Assert.IsNotNull(staleRenderer);
            staleRenderer.sharedMaterials = new Material[] { null };

            ItemRuntime runtime = _item.AddComponent<ItemRuntime>();
            FieldInfo definitionField = FindField(typeof(ItemRuntime), "definition");
            Assert.IsNotNull(definitionField);
            definitionField.SetValue(runtime, definition);

            _item.SetActive(true);
            yield return null;
            yield return null;

            Assert.IsNull(visual.transform.Find("prefab(Clone)"),
                "The scene-authored stale Forge prefab child survived ItemRuntime.Awake.");
            Renderer rootRenderer = _item.GetComponent<Renderer>();
            Assert.IsNotNull(rootRenderer);
            Assert.IsFalse(rootRenderer.enabled,
                "The primitive root look remained exposed after Forge ownership was restored.");

            int activeSurfaceCount = 0;
            Renderer[] renderers = _item.GetComponentsInChildren<Renderer>(true);
            for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                Renderer renderer = renderers[rendererIndex];
                if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
                    continue;
                activeSurfaceCount++;
                Material[] materials = renderer.sharedMaterials;
                Assert.IsNotNull(materials,
                    "Active Forge renderer returned a null material array: " + renderer.name);
                Assert.Greater(materials.Length, 0,
                    "Active Forge renderer has no material slots: " + renderer.name);
                for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
                {
                    Material material = materials[materialIndex];
                    Assert.IsNotNull(material,
                        "Active Forge renderer has a null material: " + renderer.name +
                        " slot=" + materialIndex);
                    Assert.IsNotNull(material.shader,
                        "Active Forge renderer material has no shader: " + renderer.name +
                        " slot=" + materialIndex);
                }
            }

            Assert.Greater(activeSurfaceCount, 0,
                "Forge ownership repair left the scene-authored item with no active visual surface.");
        }

        private static FieldInfo FindField(System.Type type, string name)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(
                    name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (field != null) return field;
                type = type.BaseType;
            }
            return null;
        }
    }
}
