using UnityEngine;
using UnityEditor;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Shared helpers for idempotent scene patchers. Reduces find-or-create boilerplate.
    /// </summary>
    public static class PatcherUtil
    {
        public static void SetFloat(SerializedObject so, string property, float value)
        {
            var prop = so.FindProperty(property);
            if (prop != null) prop.floatValue = value;
        }

        public static void SetVector3(SerializedObject so, string property, Vector3 value)
        {
            var prop = so.FindProperty(property);
            if (prop != null) prop.vector3Value = value;
        }

        public static void SetObjectRef(SerializedObject so, string property, Object value)
        {
            var prop = so.FindProperty(property);
            if (prop != null) prop.objectReferenceValue = value;
        }

        public static void SetString(SerializedObject so, string property, string value)
        {
            var prop = so.FindProperty(property);
            if (prop != null) prop.stringValue = value;
        }

        public static void SetBool(SerializedObject so, string property, bool value)
        {
            var prop = so.FindProperty(property);
            if (prop != null) prop.boolValue = value;
        }

        /// <summary>
        /// Find or create a root-level GameObject by name. Always updates position.
        /// </summary>
        public static GameObject EnsureRootObject(string name, Vector3 position)
        {
            var go = GameObject.Find(name);
            if (go == null)
            {
                go = new GameObject(name);
                Undo.RegisterCreatedObjectUndo(go, name);
            }
            go.transform.position = position;
            return go;
        }

        /// <summary>
        /// Find or create a child transform by name. Always updates localPosition.
        /// </summary>
        public static Transform EnsureChild(Transform parent, string name, Vector3 localPos)
        {
            var t = parent.Find(name);
            if (t == null)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                t = go.transform;
                Undo.RegisterCreatedObjectUndo(go, name);
            }
            t.localPosition = localPos;
            return t;
        }

        public static T EnsureComponent<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            if (c == null) c = go.AddComponent<T>();
            return c;
        }
    }
}
