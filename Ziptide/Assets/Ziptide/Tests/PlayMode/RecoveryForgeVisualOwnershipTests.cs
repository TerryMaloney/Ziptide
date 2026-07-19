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
            // This existing Recovery* canary deliberately remains the exact-SHA proof trigger for the
            // 2026-07-19 weapon/coupler correction and its device-discovered egress/physics follow-up.
            // It does not add a 44th PlayMode test; touching this file routes the bounded candidate
            // through both Recovery PlayMode and Golden Android.
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
            // Match the canonical ItemFactory pistol shell. Before the 2026-07-19 correction this
            // scale was inherited by the already-metre-authored ForgeVisual and produced a tiny gun.
            _item.transform.localScale = new Vector3(0.08f, 0.04f, 0.20f);
            _item.AddComponent<Rigidbody>();
            var grab = _item.AddComponent<XRGrabInteractable>();
            grab.useDynamicAttach = false;

            var grip = new GameObject("Grip");
            grip.transform.SetParent(_item.transform, false);
            grip.transform.localPosition = new Vector3(0f, -0.01f, -0.05f);
            grip.transform.localRotation = Quaternion.Euler(45f, 0f, 0f); // stale rejected pose
            grab.attachTransform = grip.transform;

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(_item.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0f, 0.12f);

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

            // Recovery size contract: primitive dimensions become collider dimensions, while the item
            // root returns to unit scale so the metre-authored Forge mesh is not scaled a second time.
            AssertVector(_item.transform.localScale, Vector3.one, 0.0001f,
                "Forge ownership did not normalize the item root scale.");
            var box = _item.GetComponent<BoxCollider>();
            Assert.IsNotNull(box, "The canonical cube item lost its physical collider.");
            AssertVector(box.size, new Vector3(0.08f, 0.04f, 0.20f), 0.0001f,
                "Primitive dimensions were not preserved as collider dimensions.");

            // Recovery aim contract: the Grip attach must preserve local +Z as the held forward axis.
            Assert.Less(Quaternion.Angle(grip.transform.localRotation, Quaternion.identity), 0.5f,
                "Forge ownership left the rejected upward-pitched weapon grip in place.");
            Assert.Greater(Vector3.Dot(grip.transform.forward.normalized, _item.transform.forward.normalized),
                0.999f, "The held weapon's forward axis no longer agrees with the item's muzzle axis.");

            int activeSurfaceCount = 0;
            Bounds activeBounds = default;
            bool hasBounds = false;
            Renderer[] renderers = _item.GetComponentsInChildren<Renderer>(true);
            for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                Renderer renderer = renderers[rendererIndex];
                if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
                    continue;
                activeSurfaceCount++;
                if (!hasBounds)
                {
                    activeBounds = renderer.bounds;
                    hasBounds = true;
                }
                else activeBounds.Encapsulate(renderer.bounds);

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
            float largestDimension = Mathf.Max(activeBounds.size.x, activeBounds.size.y, activeBounds.size.z);
            Assert.GreaterOrEqual(largestDimension, 0.15f,
                "The real Forge pistol is still centimetre-sized after scale normalization: " +
                activeBounds.size.ToString("F3"));
            Assert.LessOrEqual(largestDimension, 0.45f,
                "The real Forge pistol exceeded its bounded handheld envelope: " +
                activeBounds.size.ToString("F3"));
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

        private static void AssertVector(Vector3 actual, Vector3 expected, float tolerance, string message)
        {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(tolerance), message + " (x)");
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(tolerance), message + " (y)");
            Assert.That(actual.z, Is.EqualTo(expected.z).Within(tolerance), message + " (z)");
        }
    }
}
