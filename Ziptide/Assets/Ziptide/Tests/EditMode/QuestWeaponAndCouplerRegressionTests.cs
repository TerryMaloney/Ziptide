using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Regression coverage for Terry's 2026-07-19 Quest checkpoint findings:
    /// forged weapons were scaled twice and appeared centimetre-sized; every weapon family pitched
    /// upward in-hand; and the coupler's real final control was small/high/ambiguous while the visible
    /// red sphere was only a non-interactable lamp.
    /// </summary>
    public sealed class QuestWeaponAndCouplerRegressionTests
    {
        [TestCase("pistol", 0.15f, 0.45f)]
        [TestCase("taser_dart_gun", 0.15f, 0.45f)]
        [TestCase("gravity_gun", 0.30f, 0.50f)]
        [TestCase("static_net", 0.15f, 0.70f)]
        [TestCase("sonic_thumper", 0.20f, 0.90f)]
        [TestCase("prism_beam", 0.20f, 0.90f)]
        [TestCase("breaker_blade", 0.75f, 1.00f)]
        [TestCase("tide_pike", 1.35f, 1.65f)]
        public void EveryWeaponFamily_HasHumanScaleAndForwardGrip(
            string itemId,
            float minimumLargestDimension,
            float maximumLargestDimension)
        {
            GameObject item = null;
            try
            {
                item = ItemFactory.Create(itemId, Vector3.zero);
                Assert.That(item, Is.Not.Null, "ItemFactory must resolve " + itemId + ".");

                Transform grip = item.transform.Find("Grip");
                Assert.That(grip, Is.Not.Null, itemId + " must expose the canonical Grip attach.");
                Assert.That(Quaternion.Angle(grip.localRotation, Quaternion.identity), Is.LessThan(0.5f),
                    itemId + " Grip must not pitch the item away from controller/interactor forward.");
                Assert.That(Vector3.Dot(grip.forward.normalized, item.transform.forward.normalized),
                    Is.GreaterThan(0.999f), itemId + " local +Z must remain the held forward axis.");

                Bounds visibleBounds = ActiveRendererBounds(item);
                float largest = Mathf.Max(visibleBounds.size.x, visibleBounds.size.y, visibleBounds.size.z);
                Assert.That(largest, Is.GreaterThanOrEqualTo(minimumLargestDimension),
                    itemId + " is still too small for a VR hand: bounds=" + visibleBounds.size.ToString("F3"));
                Assert.That(largest, Is.LessThanOrEqualTo(maximumLargestDimension),
                    itemId + " exceeded its bounded handheld size: bounds=" + visibleBounds.size.ToString("F3"));
            }
            finally
            {
                if (item != null) UnityEngine.Object.DestroyImmediate(item);
            }
        }

        [Test]
        public void ForgedPistol_UsesMetreAuthoredMeshWithoutDoubleScaling_AndIsIdempotent()
        {
            GameObject item = null;
            try
            {
                item = ItemFactory.Create("pistol", Vector3.zero);
                Assert.That(item, Is.Not.Null);
                AssertVector(item.transform.localScale, Vector3.one, 0.0001f,
                    "Forge item root must be unit scale after primitive-size transfer.");

                var box = item.GetComponent<BoxCollider>();
                Assert.That(box, Is.Not.Null);
                AssertVector(box.size, new Vector3(0.08f, 0.04f, 0.20f), 0.0001f,
                    "The old primitive dimensions must survive as the physical collider.");

                Assert.That(ForgeVisualApplier.TryApply(item, "pistol_scrap_mk1"), Is.True);
                AssertVector(item.transform.localScale, Vector3.one, 0.0001f,
                    "Reapplying a Forge look must not re-scale the item.");
                AssertVector(box.size, new Vector3(0.08f, 0.04f, 0.20f), 0.0001f,
                    "Reapplying a Forge look must not inflate or reset the collider.");
            }
            finally
            {
                if (item != null) UnityEngine.Object.DestroyImmediate(item);
            }
        }

        [Test]
        public void Coupler_FinalControl_IsDistinctChildReachableAndForgiving()
        {
            GameObject machineRoot = null;
            GameObject loosePart = null;
            try
            {
                machineRoot = new GameObject("QuestCouplerRegression");
                var machine = machineRoot.AddComponent<RepairableMachine>();
                machine.Init(new MachineSpawnDefinition(), null);

                Transform indicator = machineRoot.transform.Find("StatusIndicator");
                Assert.That(indicator, Is.Not.Null, "Broken/running state must read as an indicator, not a button.");
                Assert.That(indicator.GetComponent<Collider>(), Is.Null,
                    "The status indicator must never masquerade as an interactable target.");
                Assert.That(machineRoot.transform.Find("StatusLamp"), Is.Null,
                    "The old round button-looking status lamp must not return.");

                Transform power = machineRoot.transform.Find("PowerSwitch_PRESS");
                Assert.That(power, Is.Not.Null);
                Assert.That(power.gameObject.activeSelf, Is.False,
                    "The final control must appear only after the replacement part seats.");
                Assert.That(power.localPosition.y, Is.LessThanOrEqualTo(0.80f),
                    "The switch centre must be reachable by a young child.");
                Assert.That(power.localPosition.x, Is.EqualTo(0f).Within(0.001f),
                    "The switch must be centred on the usable front face, not hidden off to one side.");

                var powerCollider = power.GetComponent<BoxCollider>();
                Assert.That(powerCollider, Is.Not.Null);
                float switchTop = power.localPosition.y
                    + power.localScale.y * powerCollider.size.y * 0.5f;
                Assert.That(switchTop, Is.LessThanOrEqualTo(0.95f),
                    "The complete power target must remain within the child-reach envelope.");
                Assert.That(power.localScale.x * powerCollider.size.x, Is.GreaterThanOrEqualTo(0.40f),
                    "The selectable target must be deliberately forgiving in width.");
                Assert.That(power.GetComponents<MonoBehaviour>().Any(c =>
                        c != null && c.GetType().Name == "XRSimpleInteractable"), Is.True,
                    "The visibly labelled power target must be the actual selectable control.");

                Transform panel = machineRoot.transform.Find("Panel");
                Assert.That(panel, Is.Not.Null);
                var panelBody = panel.GetComponent<Rigidbody>();
                Assert.That(panelBody, Is.Not.Null);
                Assert.That(panelBody.isKinematic, Is.False,
                    "The access panel must not enter XRI as a kinematic throw body.");
                Assert.That(panelBody.constraints, Is.EqualTo(RigidbodyConstraints.FreezeAll),
                    "The dynamic panel stays physically bolted until selected.");

                loosePart = FindLoosePart();
                Assert.That(loosePart, Is.Not.Null);
                var partBody = loosePart.GetComponent<Rigidbody>();
                Assert.That(partBody, Is.Not.Null);
                Assert.That(partBody.isKinematic, Is.False,
                    "The replacement part must not enter XRI as a kinematic throw body.");
                Assert.That(partBody.constraints, Is.EqualTo(RigidbodyConstraints.FreezeAll),
                    "The replacement part remains parked until selected.");
            }
            finally
            {
                if (machineRoot != null) UnityEngine.Object.DestroyImmediate(machineRoot);
                if (loosePart != null) UnityEngine.Object.DestroyImmediate(loosePart);
            }
        }

        private static GameObject FindLoosePart()
        {
            return Resources.FindObjectsOfTypeAll<Rigidbody>()
                .Where(rb => rb != null && rb.gameObject.scene.IsValid())
                .Select(rb => rb.gameObject)
                .FirstOrDefault(go => go.name.StartsWith("Part_", StringComparison.Ordinal));
        }

        private static Bounds ActiveRendererBounds(GameObject root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true)
                .Where(r => r != null && r.enabled && r.gameObject.activeInHierarchy)
                .ToArray();
            Assert.That(renderers.Length, Is.GreaterThan(0), root.name + " has no active visible renderer.");

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            return bounds;
        }

        private static void AssertVector(Vector3 actual, Vector3 expected, float tolerance, string message)
        {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(tolerance), message + " (x)");
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(tolerance), message + " (y)");
            Assert.That(actual.z, Is.EqualTo(expected.z).Within(tolerance), message + " (z)");
        }
    }
}
