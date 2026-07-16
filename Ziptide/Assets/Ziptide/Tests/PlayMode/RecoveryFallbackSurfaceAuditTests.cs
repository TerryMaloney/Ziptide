using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryFallbackSurfaceAuditTests
    {
        [Test]
        public void ActiveKnownFallbacksAreBlockers_DisabledRenderersAreIgnored()
        {
            var root = new GameObject("__RECOVERY_FALLBACK_CANARY_ROOT");
            Material fallbackMaterial = null;
            RecoveryExposureProfile originalProfile = RecoveryRuntimeGate.ActiveProfile;
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);
            try
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Unlit/Color");
                Assert.IsNotNull(shader, "No supported canary shader was found.");

                GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);
                fallback.name = "__RECOVERY_FALLBACK_CANARY_MATERIAL";
                fallback.transform.SetParent(root.transform, false);
                Renderer fallbackRenderer = fallback.GetComponent<Renderer>();
                Assert.IsNotNull(fallbackRenderer);
                fallbackMaterial = new Material(shader)
                {
                    name = "RuntimeMaterialFixer_Canary"
                };
                fallbackRenderer.sharedMaterial = fallbackMaterial;

                GameObject nullSlot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                nullSlot.name = "__RECOVERY_FALLBACK_CANARY_NULL";
                nullSlot.transform.SetParent(root.transform, false);
                Renderer nullRenderer = nullSlot.GetComponent<Renderer>();
                Assert.IsNotNull(nullRenderer);
                nullRenderer.sharedMaterials = new Material[] { null };

                RecoveryFallbackSurfaceReport active =
                    RecoveryFallbackSurfaceAudit.Capture("R1_9_FALLBACK_CANARY", root.transform);
                Assert.AreEqual("GoldenSlice", active.activeProfile);
                var codes = new HashSet<string>();
                for (int i = 0; i < active.findings.Count; i++)
                    codes.Add(active.findings[i].code);

                CollectionAssert.Contains(codes, "VISIBLE_RUNTIME_MATERIAL_FIXER");
                CollectionAssert.Contains(codes, "VISIBLE_NULL_MATERIAL");
                Assert.AreEqual(2, active.findings.Count,
                    "The canary produced unexpected fallback-surface findings.");

                fallbackRenderer.enabled = false;
                nullRenderer.enabled = false;
                RecoveryFallbackSurfaceReport disabled =
                    RecoveryFallbackSurfaceAudit.Capture("R1_9_DISABLED_CANARY", root.transform);
                Assert.AreEqual(0, disabled.findings.Count,
                    "Disabled renderers were incorrectly treated as active Golden runtime surfaces.");
            }
            finally
            {
                if (root != null) Object.DestroyImmediate(root);
                if (fallbackMaterial != null) Object.DestroyImmediate(fallbackMaterial);
                RecoveryRuntimeGate.SetActiveProfile(originalProfile);
            }
        }
    }
}
