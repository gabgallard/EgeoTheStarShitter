using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class ChangedCurveWrapper
    {
        public static System.Type ChangedCurveType { get; private set; }
        object instance;


        public AnimationCurve curve
        {
            get
            {
                return (AnimationCurve)ChangedCurveType.GetField("curve").GetValue(instance);
            }
            set
            {
                ChangedCurveType.GetField("curve").SetValue(instance, value);
            }
        }

        public int curveId
        {
            get
            {
                return (int)ChangedCurveType.GetField("curveId").GetValue(instance);
            }
            set
            {
                ChangedCurveType.GetField("curveId").SetValue(instance, value);
            }
        }

        public EditorCurveBindingWrapper binding
        {
            get
            {
                return new EditorCurveBindingWrapper( ChangedCurveType.GetField("binding").GetValue(instance));
            }
            set
            {
                ChangedCurveType.GetField("binding").SetValue(instance, value.GetWrappedObject());
            }
        }

        public ChangedCurveWrapper(AnimationCurve curve, int curveId, EditorCurveBindingWrapper binding)
        {
            ChangedCurveType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ChangedCurve");
            instance = System.Activator.CreateInstance(ChangedCurveType, curve, curveId, binding.GetWrappedObject());
        }

        public override int GetHashCode()
        {
            return (int)ChangedCurveType.GetMethod("GetHashCode").Invoke(instance, new object[] { });
        }

        public object GetWrappedObject()
        {
            return instance;
        }
    }
}
