using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Core
{
    /// <summary>
    /// Ensures all XRI input action assets referenced by ActionBasedController and other XRI components
    /// are enabled at startup so grab/select work on Quest. Run after scene load.
    /// </summary>
    public static class RuntimeInputEnabler
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnLoad()
        {
            var assetsEnabled = new HashSet<InputActionAsset>();
            int controllersProcessed = 0;
            int otherProcessed = 0;

            // ActionBasedController: get asset from selectAction and enable the whole asset
            var controllers = Object.FindObjectsOfType<ActionBasedController>(true);
            foreach (var c in controllers)
            {
                if (c == null) continue;
                InputActionAsset asset = GetAssetFromController(c);
                if (asset != null && assetsEnabled.Add(asset))
                {
                    asset.Enable();
                    controllersProcessed++;
                }
            }

            // Any other MonoBehaviour with InputActionReference fields (e.g. ActionBasedControllerManager)
            var allMono = Object.FindObjectsOfType<MonoBehaviour>(true);
            foreach (var mb in allMono)
            {
                if (mb == null || mb is ActionBasedController) continue;
                InputActionAsset asset = GetAssetFromInputActionReferences(mb);
                if (asset != null && assetsEnabled.Add(asset))
                {
                    asset.Enable();
                    otherProcessed++;
                }
            }

            int totalAssets = assetsEnabled.Count;
            Debug.Log($"[Ziptide] RuntimeInputEnabler: enabled {totalAssets} InputActionAsset(s). Controllers={controllersProcessed}, Other={otherProcessed}.");
        }

        private static InputActionAsset GetAssetFromController(ActionBasedController c)
        {
            if (c == null) return null;
            var prop = c.selectAction;
            if (prop.reference != null && prop.reference.action != null)
            {
                var map = prop.reference.action.actionMap;
                if (map != null) return map.asset;
            }
            return null;
        }

        private static InputActionAsset GetAssetFromInputActionReferences(MonoBehaviour mb)
        {
            if (mb == null) return null;
            var type = mb.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var f in fields)
            {
                if (f.FieldType != typeof(InputActionReference)) continue;
                var refVal = f.GetValue(mb) as InputActionReference;
                if (refVal != null && refVal.action != null && refVal.action.actionMap != null)
                    return refVal.action.actionMap.asset;
            }
            return null;
        }
    }
}
