using System.Reflection;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public static class EditorUtilityWrapper
    {
        public static void ForceReloadInspectors()
        {
            System.Type editorUtilityType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.EditorUtility ");
            editorUtilityType.GetMethod("ForceRebuildInspectors", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { });
        }
    }
}
