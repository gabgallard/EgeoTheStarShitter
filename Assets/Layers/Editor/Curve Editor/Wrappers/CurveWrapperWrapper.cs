using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class CurveWrapperWrapper// Yes the name is silly, but I'm following a convention :-)
    {

        private static System.Type _curveWrapperType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.CurveWrapper");
        public static System.Type curveWrapperType { get { return _curveWrapperType; } }

        object instance;

        public delegate Vector2 GetAxisScalarsCallback();
        public delegate void SetAxisScalarsCallback(Vector2 newAxisScalars);
        public delegate void PreProcessKeyMovement(ref Keyframe key);


        public AnimationCurve curve { 
            get { 
                return (AnimationCurve)curveWrapperType.GetProperty("curve").GetValue(instance); 
            } 
        }

        public CurveRendererWrapper renderer//TODO: probably need to make the appropriate renderer manually
        {
            get
            {
                return new CurveRendererWrapper(curveWrapperType.GetProperty("renderer").GetValue(instance));
            }
            set
            {
                curveWrapperType.GetProperty("renderer").SetValue(instance, value.GetWrappedObject());
            }
        }

        public GameObject rootGameObjet { 
            get {
                return (GameObject)curveWrapperType.GetProperty("rootGameObjet").GetValue(instance);
            } 
        }
        public AnimationClip animationClip { 
            get {
                return (AnimationClip)curveWrapperType.GetProperty("animationClip").GetValue(instance);
            } 
        }
        public bool clipIsEditable { 
            get {
                return (bool)curveWrapperType.GetProperty("clipIsEditable").GetValue(instance);
            } 
        }
        public bool animationIsEditable { 
            get {
                return (bool)curveWrapperType.GetProperty("animationIsEditable").GetValue(instance);
            } 
        }
        public int selectionID { 
            get { 
                return (int)curveWrapperType.GetProperty("selectionID").GetValue(instance);
            } 
        }

        //public ISelectionBinding selectionBindingInterface { get { return m_SelectionBinding; } set { m_SelectionBinding = value; } }

        public Bounds bounds { 
            get { 
                return (Bounds)curveWrapperType.GetProperty("bounds").GetValue(instance);
            } 
        }

        // Input - should not be changed by curve editor
        public int id
        {
            get
            {
                return (int)curveWrapperType.GetField("id").GetValue(instance);
            }

            set
            {
                curveWrapperType.GetField("id").SetValue(instance, value);
            }
        }


        public EditorCurveBindingWrapper binding
        {
            get
            {
                return new EditorCurveBindingWrapper(curveWrapperType.GetField("binding").GetValue(instance));
            }
            set
            {
                curveWrapperType.GetField("binding").SetValue(instance, value.GetWrappedObject());
            }
        }
        public int groupId
        {
            get
            {
                return (int)curveWrapperType.GetField("groupId").GetValue(instance);
            }

            set
            {
                curveWrapperType.GetField("groupId").SetValue(instance, value);
            }
        }

        // Regions are defined by two curves added after each other with the same regionId.
        public int regionId
        {
            get
            {
                return (int)curveWrapperType.GetField("regionId").GetValue(instance);
            }

            set
            {
                curveWrapperType.GetField("regionId").SetValue(instance, value);
            }
        }
        public Color color
        {
            get
            {
                return (Color)curveWrapperType.GetField("color").GetValue(instance);
            }

            set
            {
                curveWrapperType.GetField("color").SetValue(instance, value);
            }
        }

        public Color wrapColorMultiplier
        {
            get
            {
                return (Color)curveWrapperType.GetField("wrapColorMultiplier").GetValue(instance);
            }

            set
            {
                curveWrapperType.GetField("wrapColorMultiplier").SetValue(instance, value);
            }
        }
        public bool readOnly
        {
            get
            {
                return (bool)curveWrapperType.GetField("readOnly").GetValue(instance);
            }

            set
            {
                curveWrapperType.GetField("readOnly").SetValue(instance, value);
            }
        }

        public bool hidden
        {
            get
            {
                return (bool)curveWrapperType.GetField("hidden").GetValue(instance);
            }

            set
            {
                curveWrapperType.GetField("hidden").SetValue(instance, value);
            }
        }


        public GetAxisScalarsCallback getAxisUiScalarsCallback; // Delegate used to fetch values that are multiplied on x and y axis ui values
        public SetAxisScalarsCallback setAxisUiScalarsCallback; // Delegate used to set values back that has been changed by this curve editor

        public PreProcessKeyMovement preProcessKeyMovementDelegate; // Delegate used limit key manipulation to fit curve constraints

        // Should be updated by curve editor as appropriate
        public SelectionModeWrapper selected
        {
            get
            {
                return SelectionModeWrapperUtils.FromNativeSelectionMode(curveWrapperType.GetField("selected").GetValue(instance));
            }
            set
            {
                curveWrapperType.GetField("selected").SetValue(instance, SelectionModeWrapperUtils.ToNativeSelectionMode(value));
            }
        }

        // Index into m_AnimationCurves list
        public int listIndex
        {
            get
            {
                return (int)curveWrapperType.GetProperty("listIndex").GetValue(instance);
            }

            set
            {
                curveWrapperType.GetProperty("listIndex").SetValue(instance, value);
            }
        }                                       

        public bool changed
        {
            get
            {
                return (bool)curveWrapperType.GetProperty("changed").GetValue(instance);
            }

            set
            {
                curveWrapperType.GetProperty("changed").SetValue(instance, value);
            }
        }

        public int AddKey(Keyframe key)
        {
            return (int)curveWrapperType.GetMethod("AddKey").Invoke(instance, new object[] { key });
        }

        public void PreProcessKey(ref Keyframe key)
        {
            curveWrapperType.GetMethod("PreProcessKey").Invoke(instance, new object[] {key});
        }

        public int MoveKey(int index, ref Keyframe key)
        {
            return (int)curveWrapperType.GetMethod("MoveKey").Invoke(instance, new object[] { index, key });
        }

        // An additional vertical min / max range clamp when editing multiple curves with different ranges
        public float vRangeMin
        {
            get
            {
                return (float)curveWrapperType.GetField("vRangeMin").GetValue(instance);
            }
            set
            {
                curveWrapperType.GetField("vRangeMin").SetValue(instance, value);
            }
        }

        public float vRangeMax
        {
            get
            {
                return (float)curveWrapperType.GetField("vRangeMax").GetValue(instance);
            }
            set
            {
                curveWrapperType.GetField("vRangeMax").SetValue(instance, value);
            }
        }

        public bool useScalingInKeyEditor
        {
            get
            {
                return (bool)curveWrapperType.GetField("useScalingInKeyEditor").GetValue(instance);
            }
            set
            {
                curveWrapperType.GetField("useScalingInKeyEditor").SetValue(instance, value);
            }
        }
        public string xAxisLabel
        {
            get
            {
                return (string)curveWrapperType.GetField("xAxisLabel").GetValue(instance);
            }
            set
            {
                curveWrapperType.GetField("xAxisLabel").SetValue(instance, value);
            }
        }
        public string yAxisLabel
        {
            get
            {
                return (string)curveWrapperType.GetField("yAxisLabel").GetValue(instance);
            }
            set
            {
                curveWrapperType.GetField("yAxisLabel").SetValue(instance, value);
            }
        }



        public CurveWrapperWrapper()
        {
        
            instance = System.Activator.CreateInstance(curveWrapperType);
        }

        public CurveWrapperWrapper(object instance)
        {

            this.instance = instance;
        }

        public object GetWrappedObject()
        {
            return instance;
        }
    }
}
