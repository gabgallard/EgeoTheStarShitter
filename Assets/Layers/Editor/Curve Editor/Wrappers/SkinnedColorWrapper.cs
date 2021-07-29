using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class SkinnedColorWrapper
    {
        public static System.Type skinnedColorType { get; private set; }
        object instance;

        public SkinnedColorWrapper(Color color, Color proColor)
        {

            skinnedColorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.EditorGUIUtility+SkinnedColor");
            instance = System.Activator.CreateInstance(skinnedColorType, color, proColor);
        }

        public SkinnedColorWrapper(Color color)
        {
            skinnedColorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.EditorGUIUtility+SkinnedColor");
            instance = System.Activator.CreateInstance(skinnedColorType, color);
        }

        public SkinnedColorWrapper(object instance)
        {
            skinnedColorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.EditorGUIUtility+SkinnedColor");
            this.instance = instance;
        }

        public Color color
        {
            get {
                return (Color)skinnedColorType.GetProperty("color").GetValue(instance);
            }

            set
            {
                skinnedColorType.GetProperty("color").SetValue(instance, value);
            }
        }
        public object GetWrappedObject()
        {
            return instance;
        }
        /*public static implicit operator Color(SkinnedColor colorSkin)
    {
        return colorSkin.color;
    }*/
    }
}
