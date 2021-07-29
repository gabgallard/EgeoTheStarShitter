using System.Reflection;
using UnityEditor;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class SerializedPropertyWrapper
    {
        public static void SetToValueOfTarget(SerializedProperty instance, object target)
        {
            System.Type serializedPropertyType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SerializedProperty ");
            serializedPropertyType.GetMethod("SetToValueOfTarget", BindingFlags.Static | BindingFlags.NonPublic).Invoke(instance, new object[] { target });
        }
    }
}
