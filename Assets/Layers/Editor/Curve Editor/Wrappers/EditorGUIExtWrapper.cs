using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public static class EditorGUIExtWrapper
    {
        private static System.Type EditorGUILayoutType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.EditorGUIExt");

        public static bool DragSelection(Rect[] positions, ref bool[] selections, GUIStyle style)
        {
            return (bool)EditorGUILayoutType.GetMethod("DragSelection").Invoke(null, new object[] { positions, selections, style });
        }
    }
}
