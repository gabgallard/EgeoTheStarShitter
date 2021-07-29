using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class EditorGUILayoutWrapper
    {
        private static System.Type EditorGUILayoutType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.EditorGUILayout");

        public static void TargetChoiceField(SerializedProperty property, GUIContent label, System.Action<SerializedProperty, Object> func)
        {
            EditorGUILayoutType.GetMethod("TargetChoiceField", System.Reflection.BindingFlags.Static).Invoke(null, new object[] { property, label, func });
        }

        public static void TargetChoiceField(SerializedProperty property, GUIContent label)
        {
            EditorGUILayoutType.GetMethod("TargetChoiceField", System.Reflection.BindingFlags.Static).Invoke(null, new object[] { property, label });
        }
    }
}
