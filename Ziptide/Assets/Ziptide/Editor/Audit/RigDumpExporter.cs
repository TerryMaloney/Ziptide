#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// Dumps the ACTIVE scene's hierarchy + key component settings to a text file in the repo so
    /// the cloud AI can "see" the rig/scene (interactor settings, grab/socket setup, spawn
    /// positions, stray geometry) without a live editor or headset.
    ///
    /// Use: open a scene, run Ziptide > Diagnostics > Dump Scene + Rig Config, then commit the
    /// generated docs/_generated/scene_dump_<scene>.txt and push. Repeat per scene.
    /// Pure UnityEditor reflection — no project/XRI type dependencies (won't break on API drift).
    /// </summary>
    public static class RigDumpExporter
    {
        // Components whose serialized fields we dump in full (the ones that drive feel/behaviour).
        private static readonly string[] DetailTypes =
        {
            "Interactor", "Interactable", "Socket", "Grab", "Locomotion", "Move", "Turn",
            "Belt", "Holster", "SpawnMarker", "WorldRuntime", "WorldDirector", "Drone",
            "Target", "Pistol", "Taser", "CharacterController", "Rigidbody", "InputActionManager",
            "XROrigin", "TravelCoordinator", "PlayerRigPersistence", "DashLocomotion", "ProximityTravel"
        };

        [MenuItem("Ziptide/Diagnostics/Dump Scene + Rig Config")]
        public static void Dump()
        {
            var scene = SceneManager.GetActiveScene();
            var sb = new StringBuilder();
            sb.AppendLine("ZIPTIDE SCENE DUMP");
            sb.AppendLine("scene=" + scene.name + " path=" + scene.path);
            sb.AppendLine("generated=" + System.DateTime.Now.ToString("u"));
            sb.AppendLine(new string('=', 60));

            foreach (var root in scene.GetRootGameObjects())
                DumpGo(root.transform, 0, sb);

            string dir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "docs", "_generated"));
            Directory.CreateDirectory(dir);
            string safe = string.IsNullOrEmpty(scene.name) ? "untitled" : scene.name;
            string outPath = Path.Combine(dir, "scene_dump_" + safe + ".txt");
            File.WriteAllText(outPath, sb.ToString());
            Debug.Log("ZIPTIDE: SCENE_DUMP written to " + outPath);
            EditorUtility.RevealInFinder(outPath);
        }

        private static void DumpGo(Transform t, int depth, StringBuilder sb)
        {
            string pad = new string(' ', depth * 2);
            Vector3 p = t.position;
            sb.AppendLine(pad + "+ " + t.name + (t.gameObject.activeSelf ? "" : " [INACTIVE]")
                + "  pos=(" + p.x.ToString("F2") + "," + p.y.ToString("F2") + "," + p.z.ToString("F2") + ")");

            foreach (var c in t.GetComponents<Component>())
            {
                if (c == null) { sb.AppendLine(pad + "  - <missing script>"); continue; }
                if (c is Transform) continue;
                string tn = c.GetType().Name;

                bool detail = false;
                foreach (var key in DetailTypes) { if (tn.Contains(key)) { detail = true; break; } }

                if (detail)
                {
                    sb.AppendLine(pad + "  - " + tn + " {");
                    DumpProps(c, pad + "      ", sb);
                    sb.AppendLine(pad + "    }");
                }
                else
                {
                    sb.AppendLine(pad + "  - " + tn);
                }
            }

            foreach (Transform child in t)
                DumpGo(child, depth + 1, sb);
        }

        private static void DumpProps(Object c, string pad, StringBuilder sb)
        {
            SerializedObject so;
            try { so = new SerializedObject(c); }
            catch { return; }

            SerializedProperty prop = so.GetIterator();
            bool enterChildren = true;
            int count = 0;
            while (prop.NextVisible(enterChildren) && count < 250)
            {
                enterChildren = false;
                if (prop.name == "m_Script") continue;
                sb.AppendLine(pad + prop.name + " = " + PropValue(prop));
                count++;
            }
        }

        private static string PropValue(SerializedProperty p)
        {
            switch (p.propertyType)
            {
                case SerializedPropertyType.Boolean: return p.boolValue.ToString();
                case SerializedPropertyType.Integer: return p.intValue.ToString();
                case SerializedPropertyType.Float: return p.floatValue.ToString("F3");
                case SerializedPropertyType.String: return "\"" + p.stringValue + "\"";
                case SerializedPropertyType.Enum: return p.enumValueIndex + ":" + SafeEnumName(p);
                case SerializedPropertyType.Vector3: return p.vector3Value.ToString("F2");
                case SerializedPropertyType.Vector2: return p.vector2Value.ToString("F2");
                case SerializedPropertyType.Color: return p.colorValue.ToString();
                case SerializedPropertyType.LayerMask: return "mask:" + p.intValue;
                case SerializedPropertyType.ObjectReference:
                    return p.objectReferenceValue != null
                        ? (p.objectReferenceValue.name + " (" + p.objectReferenceValue.GetType().Name + ")")
                        : "null";
                default: return "(" + p.propertyType + ")";
            }
        }

        private static string SafeEnumName(SerializedProperty p)
        {
            try
            {
                if (p.enumDisplayNames != null && p.enumValueIndex >= 0 && p.enumValueIndex < p.enumDisplayNames.Length)
                    return p.enumDisplayNames[p.enumValueIndex];
            }
            catch { /* ignore */ }
            return "?";
        }
    }
}
#endif
