using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class ControlPointRendererWrapper
    {
        private static System.Type ControlPointRendererType;
        object instance;

        public static Material material
        {
            get
            {
                return (Material)ControlPointRendererType.GetField("material").GetValue(null);
            }
        }


        public void FlushCache()
        {
            ControlPointRendererType.GetMethod("FlushCache").Invoke(instance, null);
        }

        public void Clear()
        {
            ControlPointRendererType.GetMethod("Clear").Invoke(instance, null);
        }

        public void Render()
        {
            ControlPointRendererType.GetMethod("Render").Invoke(instance, null);
        }

        public void AddPoint(Rect rect, Color color)
        {
            ControlPointRendererType.GetMethod("AddPoint").Invoke(instance, new object[] { rect, color });
        }

        public ControlPointRendererWrapper(Texture2D icon)
        {

            ControlPointRendererType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ControlPointRenderer");
            instance = System.Activator.CreateInstance(ControlPointRendererType, icon);
        }

        public object GetWrappedObject()
        {
            return instance;
        }
    }
}
