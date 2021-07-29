using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class CurveControlPointRendererWrapper
    {
        private static System.Type CurveControlPointRendererType;
        object instance;

        public CurveControlPointRendererWrapper()
        {
            CurveControlPointRendererType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.CurveControlPointRenderer");
            instance = System.Activator.CreateInstance(CurveControlPointRendererType);
        }

        public void FlushCache()
        {
            CurveControlPointRendererType.GetMethod("FlushCache").Invoke(instance, new object[] { });
        }

        public void Clear()
        {
            CurveControlPointRendererType.GetMethod("Clear").Invoke(instance, new object[] { });
        }

        public void Render()
        {
            CurveControlPointRendererType.GetMethod("Render").Invoke(instance, new object[] { });
        }

        public void AddPoint(Rect rect, Color color)
        {
            CurveControlPointRendererType.GetMethod("AddPoint").Invoke(instance, new object[] { rect, color });
        }

        public void AddSelectedPoint(Rect rect, Color color)
        {
            CurveControlPointRendererType.GetMethod("AddSelectedPoint").Invoke(instance, new object[] { rect, color });
        }

        public void AddSemiSelectedPoint(Rect rect, Color color)
        {
            CurveControlPointRendererType.GetMethod("AddSemiSelectedPoint").Invoke(instance, new object[] { rect, color });
        }

        public void AddWeightedPoint(Rect rect, Color color)
        {
            CurveControlPointRendererType.GetMethod("AddWeightedPoint").Invoke(instance, new object[] { rect, color });
        }

        public object GetWrappedObject()
        {
            return instance;
        }
    }
}
