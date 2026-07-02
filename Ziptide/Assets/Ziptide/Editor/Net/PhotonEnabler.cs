#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Ziptide.Editor.Net
{
    /// <summary>
    /// One-click toggle for the ZIPTIDE_PHOTON scripting define (docs/TWO_QUEST_SETUP.md step 4).
    /// The Photon adapter (Assets/ZiptideNet) is inert until the define is on, so the project always
    /// compiles without PUN2 — enable only AFTER the PUN2 import.
    /// </summary>
    public static class PhotonEnabler
    {
        private const string Define = "ZIPTIDE_PHOTON";
        private static readonly BuildTargetGroup[] Groups =
            { BuildTargetGroup.Android, BuildTargetGroup.Standalone };

        [MenuItem("Ziptide/Net/Enable Photon (ZIPTIDE_PHOTON)")]
        public static void Enable()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Photon"))
            {
                EditorUtility.DisplayDialog("Photon not found",
                    "Import PUN2 FREE from the Asset Store first (docs/TWO_QUEST_SETUP.md step 2), then run this again.",
                    "OK");
                return;
            }
            SetDefine(true);
            EditorUtility.DisplayDialog("Photon enabled",
                "ZIPTIDE_PHOTON is on for Android + Standalone. The adapter in Assets/ZiptideNet now compiles.\n" +
                "Next: docs/TWO_QUEST_SETUP.md step 5 (room-code smoke).", "OK");
        }

        [MenuItem("Ziptide/Net/Disable Photon")]
        public static void Disable()
        {
            SetDefine(false);
            Debug.Log("[Ziptide] ZIPTIDE_PHOTON removed — the Photon adapter is inert again.");
        }

        private static void SetDefine(bool on)
        {
            foreach (var group in Groups)
            {
                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group) ?? "";
                var list = new System.Collections.Generic.List<string>(defines.Split(';'));
                list.RemoveAll(d => d.Trim() == Define || string.IsNullOrWhiteSpace(d));
                if (on) list.Add(Define);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", list));
            }
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] " + Define + (on ? " ENABLED" : " disabled") + " for Android+Standalone.");
        }
    }
}
#endif
