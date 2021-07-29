using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class EditorCurveBindingWrapper : MonoBehaviour
    {
        private static System.Type curveWrapperType;
        object instance;


        // The path of the game object / bone being animated.
        public string path
        {
            get
            {
                return (string)curveWrapperType.GetField("path").GetValue(instance);
            }
            set
            {
                curveWrapperType.GetField("path").SetValue(instance, value);
            }
        }

        // The type of the component / material being animated.
        private System.Type m_type
        {
            get
            {
                return (System.Type)curveWrapperType.GetField("m_type").GetValue(instance);
            }
            set
            {
                curveWrapperType.GetField("m_type").SetValue(instance, value);
            }
        }

        // The name of the property being animated.
        public string propertyName
        {
            get
            {
                return (string)curveWrapperType.GetField("propertyName").GetValue(instance);
            }
            set
            {
                curveWrapperType.GetField("propertyName").SetValue(instance, value);
            }
        }

        public bool isPPtrCurve
        {
            get
            {
                return (bool)curveWrapperType.GetField("isPPtrCurve").GetValue(instance);
            }
        }
        public bool isDiscreteCurve
        {
            get
            {
                return (bool)curveWrapperType.GetField("isDiscreteCurve").GetValue(instance);
            }
        }
        internal bool isPhantom
        {
            get
            {
                return (bool)curveWrapperType.GetField("isPhantom").GetValue(instance);
            }
            set
            {
                curveWrapperType.GetField("isPhantom").SetValue(instance, value);
            }
        }

        public static bool operator ==(EditorCurveBindingWrapper lhs, EditorCurveBindingWrapper rhs)
        {
            return lhs.instance == rhs.instance;
        }

        public static bool operator !=(EditorCurveBindingWrapper lhs, EditorCurveBindingWrapper rhs)
        {
            return lhs.instance != rhs.instance;
        }

        public override int GetHashCode()
        {
            return (int)curveWrapperType.GetMethod("GetHashCode").Invoke(instance, new object[] { });
        }

        public override bool Equals(object other)
        {
            return other is EditorCurveBindingWrapper && Equals((EditorCurveBindingWrapper)other);
        }

        public bool Equals(EditorCurveBindingWrapper other)
        {
            return this == other;
        }

        public System.Type type
        {
            get
            {
                return (System.Type)curveWrapperType.GetField("type").GetValue(instance);
            }
            set
            {
                curveWrapperType.GetField("type").SetValue(instance, value);
            }
        }

    
        static public EditorCurveBindingWrapper FloatCurve(string inPath, System.Type inType, string inPropertyName)
        {
            return new EditorCurveBindingWrapper(curveWrapperType.GetMethod("FloatCurve", System.Reflection.BindingFlags.Static).Invoke(null, new object[] { inPath, inType, inPropertyName }));
        }

        static public EditorCurveBindingWrapper PPtrCurve(string inPath, System.Type inType, string inPropertyName)
        {
            return new EditorCurveBindingWrapper(curveWrapperType.GetMethod("PPtrCurve", System.Reflection.BindingFlags.Static).Invoke(null, new object[] { inPath, inType, inPropertyName }));
        }

        static public EditorCurveBindingWrapper DiscreteCurve(string inPath, System.Type inType, string inPropertyName)
        {
            return new EditorCurveBindingWrapper(curveWrapperType.GetMethod("DiscreteCurve", System.Reflection.BindingFlags.Static).Invoke(null, new object[] { inPath, inType, inPropertyName }));
        }

        public EditorCurveBindingWrapper()
        {

            curveWrapperType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.EditorCurveBinding");
            instance = System.Activator.CreateInstance(curveWrapperType);
        }

        public EditorCurveBindingWrapper(object instance)
        {

            this.instance = instance;
        }

        public object GetWrappedObject()
        {
            return instance;
        }
    }
}
