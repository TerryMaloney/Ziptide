using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Ziptide.Editor.Validation
{
    /// <summary>
    /// Editor diagnostic: Ziptide > Diagnostics > XR Grab Readiness.
    /// Validates all prerequisites for grab interaction on Quest and prints a pass/fail checklist.
    /// </summary>
    public static class XRGrabReadiness
    {
        private const string MenuPath = "Ziptide/Diagnostics/XR Grab Readiness";
        private const string XRIInputActionsGUID = "c348712bda248c246b8c49b3db54643f";

        [MenuItem(MenuPath)]
        public static void Run()
        {
            var results = new List<string>();
            bool allPass = true;

            // 1) XRI Default Input Actions asset exists
            string inputActionsPath = AssetDatabase.GUIDToAssetPath(XRIInputActionsGUID);
            if (string.IsNullOrEmpty(inputActionsPath))
            {
                results.Add("FAIL: XRI Default Input Actions asset not found (GUID " + XRIInputActionsGUID + "). Import XR Interaction Toolkit Starter Assets sample.");
                allPass = false;
            }
            else
            {
                var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputActionsPath);
                if (asset == null)
                {
                    results.Add("FAIL: Asset at " + inputActionsPath + " is not an InputActionAsset.");
                    allPass = false;
                }
                else
                    results.Add("PASS: XRI Default Input Actions found at " + inputActionsPath);
            }

            // 2) Scene has XR Interaction Manager
            var interactionManager = Object.FindObjectOfType<XRInteractionManager>();
            if (interactionManager == null)
            {
                results.Add("FAIL: No XRInteractionManager in scene. Add via GameObject > XR > XR Interaction Manager.");
                allPass = false;
            }
            else
                results.Add("PASS: XR Interaction Manager present.");

            // 3) Scene has InputActionManager with input actions assigned
            var inputManager = Object.FindObjectOfType<InputActionManager>();
            if (inputManager == null)
            {
                results.Add("WARN: No InputActionManager in scene. RuntimeInputEnabler will enable actions at runtime; or add Input Action Manager and assign XRI Default Input Actions.");
            }
            else
            {
                int count = inputManager.actionAssets != null ? inputManager.actionAssets.Count : 0;
                if (count == 0)
                {
                    results.Add("FAIL: InputActionManager has no action assets assigned. Assign XRI Default Input Actions.");
                    allPass = false;
                }
                else
                    results.Add("PASS: InputActionManager has " + count + " action asset(s) assigned.");
            }

            // 4) XR Origin with Ray + Direct interactors
            var rayInteractors = Object.FindObjectsOfType<XRRayInteractor>(true).ToList();
            var directInteractors = Object.FindObjectsOfType<XRDirectInteractor>(true).ToList();
            bool hasRay = rayInteractors.Any(r => r.gameObject.name.Contains("Ray") && !r.gameObject.name.Contains("Teleport"));
            if (rayInteractors.Count == 0)
            {
                results.Add("FAIL: No XRRayInteractor in scene. XR Origin should have Ray Interactor for distance grab.");
                allPass = false;
            }
            else if (!hasRay)
                results.Add("WARN: Ray interactors may be teleport-only. Ensure an interaction Ray Interactor exists (not just Teleport).");
            else
                results.Add("PASS: XRRayInteractor(s) present.");
            if (directInteractors.Count == 0)
            {
                results.Add("FAIL: No XRDirectInteractor in scene. Add Direct Interactor to controllers for touch grab.");
                allPass = false;
            }
            else
                results.Add("PASS: XRDirectInteractor(s) present (" + directInteractors.Count + ").");

            // 5) Controllers are Action-based
            var actionControllers = Object.FindObjectsOfType<ActionBasedController>(true);
            if (actionControllers.Length == 0)
            {
                results.Add("WARN: No ActionBasedController found. Ensure XR Origin uses Action-based controllers (not device-based).");
            }
            else
                results.Add("PASS: ActionBasedController(s) present (" + actionControllers.Length + ").");

            // 6) GrabbableCube has Rigidbody + XRGrabInteractable + Collider
            var cube = GameObject.Find("GrabbableCube");
            if (cube == null)
            {
                results.Add("WARN: No GameObject named 'GrabbableCube' in scene.");
            }
            else
            {
                var rb = cube.GetComponent<Rigidbody>();
                var grab = cube.GetComponent<XRGrabInteractable>();
                var col = cube.GetComponent<Collider>();
                if (rb == null) { results.Add("FAIL: GrabbableCube has no Rigidbody."); allPass = false; }
                else if (rb.isKinematic) { results.Add("FAIL: GrabbableCube Rigidbody is Kinematic. Set to non-kinematic for grab."); allPass = false; }
                else results.Add("PASS: GrabbableCube has non-kinematic Rigidbody.");
                if (grab == null) { results.Add("FAIL: GrabbableCube has no XRGrabInteractable."); allPass = false; }
                else results.Add("PASS: GrabbableCube has XRGrabInteractable.");
                if (col == null) { results.Add("FAIL: GrabbableCube has no Collider."); allPass = false; }
                else results.Add("PASS: GrabbableCube has Collider.");
            }

            // 7) Interaction layer overlap (simplified: interactable layer 1 Default, interactors usually include Default)
            if (cube != null)
            {
                var grab = cube.GetComponent<XRGrabInteractable>();
                if (grab != null)
                    results.Add("INFO: GrabbableCube interaction layers = " + grab.interactionLayers.value + ". Ensure interactors include this layer.");
            }

            // 8) OpenXR Oculus Touch profile enabled
            string openXRSettingsPath = "Assets/XR/Settings/OpenXR Package Settings.asset";
            var openXR = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(openXRSettingsPath);
            if (openXR == null)
                results.Add("WARN: OpenXR Package Settings not found at " + openXRSettingsPath + ". Enable Oculus Touch Controller Profile for Android.");
            else
            {
                string fullPath = System.IO.Path.Combine(Application.dataPath, "..", openXRSettingsPath.Replace('/', System.IO.Path.DirectorySeparatorChar));
                fullPath = System.IO.Path.GetFullPath(fullPath);
                if (System.IO.File.Exists(fullPath))
                {
                    string content = System.IO.File.ReadAllText(fullPath);
                    bool oculusEnabled = content.Contains("oculustouch") && content.Contains("m_enabled: 1");
                    if (!oculusEnabled)
                        results.Add("WARN: Oculus Touch Controller Profile may be disabled. Check Edit > Project Settings > XR Plug-in Management > OpenXR > Android.");
                    else
                        results.Add("PASS: Oculus Touch profile enabled in OpenXR settings.");
                }
                else
                    results.Add("PASS: OpenXR Package Settings asset found.");
            }

            // Print to console
            Debug.Log("[Ziptide] XR Grab Readiness ---\n" + string.Join("\n", results) + "\n--- " + (allPass ? "ALL CHECKS PASSED" : "SOME CHECKS FAILED"));
            if (!allPass)
                EditorUtility.DisplayDialog("XR Grab Readiness", string.Join("\n", results), "OK");
        }
    }
}
