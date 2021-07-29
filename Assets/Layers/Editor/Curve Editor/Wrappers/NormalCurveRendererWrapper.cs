using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class NormalCurveRendererWrapper : CurveRendererWrapper
    {
    
        public static Material curveMaterial
        {
            get
            {
                return (Material)CurveRendererType.GetProperty("curveMaterial").GetValue(null, new object[] { });
            }
        }

        public NormalCurveRendererWrapper(AnimationCurve curve)
        {
            CurveRendererType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.NormalCurveRenderer");
            instance = System.Activator.CreateInstance(CurveRendererType, curve);
        }

    

        private Vector3[] GetPoints()
        {
            return (Vector3[])CurveRendererType.GetMethod("GetPoints").Invoke(instance, new object[] { });
        }

        private Vector3[] GetPoints(float minTime, float maxTime)
        {
            return (Vector3[])CurveRendererType.GetMethod("GetPoints").Invoke(instance, new object[] { minTime, maxTime });
        }

        public static float[,] CalculateRanges(float minTime, float maxTime, float rangeStart, float rangeEnd, WrapMode preWrapMode, WrapMode postWrapMode)
        {
            return (float[,])CurveRendererType.GetMethod("CalculateRanges").Invoke(null, new object[] { minTime, maxTime, rangeStart, rangeEnd, preWrapMode, postWrapMode });
        }

        protected virtual int GetSegmentResolution(float minTime, float maxTime, float keyTime, float nextKeyTime)
        {
            return (int)CurveRendererType.GetMethod("GetSegmentResolution").Invoke(instance, new object[] { minTime, maxTime, keyTime, nextKeyTime });
        }

        protected virtual void AddPoint(ref List<Vector3> points, ref float lastTime, float sampleTime, ref float lastValue, float sampleValue)
        {
            CurveRendererType.GetMethod("AddPoint").Invoke(instance, new object[] { points, lastTime, sampleTime, lastValue, sampleValue });
        }

        private void AddPoints(ref List<Vector3> points, float minTime, float maxTime, float visibleMinTime, float visibleMaxTime)
        {
            CurveRendererType.GetMethod("AddPoints").Invoke(instance, new object[] { points, minTime, maxTime, visibleMinTime, visibleMaxTime });
        }

        private void BuildCurveMesh()
        {
            CurveRendererType.GetMethod("BuildCurveMesh").Invoke(instance, new object[] { });
        }


        public static void DrawPolyLine(Matrix4x4 transform, float minDistance, params Vector3[] points)
        {
            CurveRendererType.GetMethod("DrawPolyLine").Invoke(null, new object[] { transform , minDistance, points});
        }

        public static void DrawCurveWrapped(float minTime, float maxTime, float rangeStart, float rangeEnd,
            WrapMode preWrap, WrapMode postWrap, Mesh mesh, Vector3 firstPoint, Vector3 lastPoint,
            Matrix4x4 transform, Color color, Color wrapColor)
        {
            CurveRendererType.GetMethod("DrawCurveWrapped").Invoke(null, new object[] { minTime, maxTime, rangeStart, rangeEnd,
                preWrap, postWrap, mesh, firstPoint, lastPoint,
                transform, color, wrapColor});
        }

        public static void DrawCurveWrapped(float minTime, float maxTime, float rangeStart, float rangeEnd,
            WrapMode preWrap, WrapMode postWrap, Color color, Matrix4x4 transform, Vector3[] points, Color wrapColor)
        {
            CurveRendererType.GetMethod("DrawCurveWrapped").Invoke(null, new object[] { minTime, maxTime, rangeStart, rangeEnd,
                preWrap, postWrap, color, transform, points, wrapColor});
        }


    }
}
