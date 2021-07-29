using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class CurveRendererWrapper
    {
        protected static System.Type CurveRendererType;
        protected object instance;

        public void DrawCurve(float minTime, float maxTime, Color color, Matrix4x4 transform, Color wrapColor)
        {
            CurveRendererType.GetMethod("DrawCurve").Invoke(instance, new object[] { minTime, maxTime, color, transform, wrapColor });
        }

        public AnimationCurve GetCurve()
        {
            return (AnimationCurve)CurveRendererType.GetMethod("GetCurve").Invoke(instance, new object[] {});
        }
        public float RangeStart()
        {
            return (float)CurveRendererType.GetMethod("RangeStart").Invoke(instance, new object[] { });
        }
        public float RangeEnd()
        {
            return (float)CurveRendererType.GetMethod("RangeEnd").Invoke(instance, new object[] { });
        }
        public void SetWrap(WrapMode wrap)
        {
            CurveRendererType.GetMethod("SetWrap").Invoke(instance, new object[] { wrap });
        }
        public void SetWrap(WrapMode preWrap, WrapMode postWrap)
        {
            CurveRendererType.GetMethod("SetWrap").Invoke(instance, new object[] { preWrap, postWrap });
        }
        public void SetCustomRange(float start, float end)
        {
            CurveRendererType.GetMethod("SetCustomRange").Invoke(instance, new object[] { start, end });
        }
        public float EvaluateCurveSlow(float time)
        {
            return (float)CurveRendererType.GetMethod("EvaluateCurveSlow").Invoke(instance, new object[] { time});
        }
        public float EvaluateCurveDeltaSlow(float time)
        {
            return (float)CurveRendererType.GetMethod("EvaluateCurveDeltaSlow").Invoke(instance, new object[] { time });
        }
        public Bounds GetBounds()
        {
            return (Bounds)CurveRendererType.GetMethod("GetBounds").Invoke(instance, new object[] { });
        }
        public Bounds GetBounds(float minTime, float maxTime)
        {
            return (Bounds)CurveRendererType.GetMethod("GetBounds").Invoke(instance, new object[] { minTime, maxTime });

        }
        public float ClampedValue(float value)
        {
            return (float)CurveRendererType.GetMethod("ClampedValue").Invoke(instance, new object[] { value});
        }
        public void FlushCache()
        {
            CurveRendererType.GetMethod("FlushCache").Invoke(instance, new object[] { });
        }

        public object GetWrappedObject()
        {
            return instance;
        }

        public CurveRendererWrapper()
        {
        }

        public CurveRendererWrapper(object instance)
        {
            this.instance = instance;
            if (instance != null)
                CurveRendererType = instance.GetType();
        }
    }
}
