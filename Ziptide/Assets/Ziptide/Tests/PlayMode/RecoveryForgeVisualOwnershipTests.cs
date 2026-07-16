using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Gameplay;
using Ziptide.Visuals;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryForgeVisualOwnershipTests
    {
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
            PistolDefinition definition =
                Resources.Load<PistolDefinition>("Items/DefaultPistol");
            Assert.IsNotNull(definition, "DefaultPistol definition is missing from Resources/Items.");
            Assert.IsFalse(string.IsNullOrEmpty(definition.forgeRecipeId),
                "DefaultPistol has no Forge recipe for the ownership canary.");

            _item = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _item.name = "__RECOVERY_SCENE_AUTHORED_FORGE_ITEM";
            _item.SetActive(false);
            _item.AddComponent<Rigidbody>();
            _item.AddComponent<XRGrabInteractable>();

            var visual = new GameObject(ForgeVisualApplier.VisualChildName);
            visual.transform.SetParent(_item.transform, false);
            GameObject stale = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stale.name = "prefab(Clone)";
            stale.transform.SetParent(visual.transform, false);
            Renderer staleRenderer = stale.GetComponent<Renderer>();
            Assert.IsNotNull(staleRenderer);
            staleRenderer.sharedMaterials = new Material[] { null };

            ItemRuntime runtime = _item.AddComponent<ItemRuntime>();
            FieldInfo definitionField = typeof(ItemRuntime).GetField(
                "definition",
                BindingFlags.Instance | BindingFlags.NonPublic);
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
    }
}
