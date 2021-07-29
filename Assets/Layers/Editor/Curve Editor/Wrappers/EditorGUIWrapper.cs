using System.Reflection;
using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class EditorGUIWrapper : MonoBehaviour
    {
        public static float indent
        {
            get
            {
                System.Type editorUtilityType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.EditorGUI");
                MemberInfo[] property = editorUtilityType.GetMembers(BindingFlags.NonPublic | BindingFlags.Static);
                return 0f;// (float)property.GetValue(null);
            }
        }

        public static void DrawLegend(Rect position, Color color, string label, bool enabled)
        {
            System.Type editorUtilityType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.EditorGUI");
            editorUtilityType.GetMethod("DrawLegend", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { position, color, label, enabled });
        }
    }
}
