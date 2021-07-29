using System.Reflection;
using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class CurveEditorSettingsWrapper
    {
        public static System.Type CurveEditorSettingsType { get; private set; }
        object instance;

        public CurveEditorSettingsWrapper()
        {
            CurveEditorSettingsType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.CurveEditorSettings");
            instance = System.Activator.CreateInstance(CurveEditorSettingsType);
        }

        public CurveEditorSettingsWrapper(object instance)
        {
            CurveEditorSettingsType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.CurveEditorSettings");
            this.instance = instance;
        }
        public object GetWrappedObject()
        {
            return instance;
        }

        public TickStyleWrapper hTickStyle
        {
            get
            {
                return new TickStyleWrapper(CurveEditorSettingsType.GetProperty("hTickStyle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(instance));
            }
            set
            {
                CurveEditorSettingsType.GetProperty("hTickStyle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(instance, value.GetWrappedObject());
            }
        }

        public TickStyleWrapper vTickStyle
        {
            get
            {
                return new TickStyleWrapper(CurveEditorSettingsType.GetProperty("vTickStyle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(instance));
            }
            set
            {
                CurveEditorSettingsType.GetProperty("vTickStyle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(instance, value.GetWrappedObject());
            }
        }


        public bool hRangeLocked
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetProperty("hRangeLocked", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetProperty("hRangeLocked", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(instance, value);
            }
        }


        public bool vRangeLocked
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetProperty("hRangeLocked", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetProperty("hRangeLocked", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(instance, value);
            }
        }

        public float hRangeMin
        {
            get
            {
                return (float)CurveEditorSettingsType.GetProperty("hRangeMin").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetProperty("hRangeMin").SetValue(instance, value);
            }
        }

        public float hRangeMax
        {
            get
            {
                return (float)CurveEditorSettingsType.GetProperty("hRangeMax").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetProperty("hRangeMax").SetValue(instance, value);
            }
        }

        public float vRangeMin
        {
            get
            {
                return (float)CurveEditorSettingsType.GetProperty("vRangeMin").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetProperty("vRangeMin").SetValue(instance, value);
            }
        }


        public float vRangeMax
        {
            get
            {
                return (float)CurveEditorSettingsType.GetProperty("vRangeMax").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetProperty("vRangeMax").SetValue(instance, value);
            }
        }


        public bool hasUnboundedRanges
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetProperty("hasUnboundedRanges").GetValue(instance);
            }
        }

        // Offset to move the labels along the horizontal axis to make room for the overlaid scrollbar in the
        // curve editor popup.
        public float hTickLabelOffset
        {
            get
            {
                return (float)CurveEditorSettingsType.GetField("hTickLabelOffset").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("hTickLabelOffset").SetValue(instance, value);
            }
        }

        public SkinnedColorWrapper wrapColor
        {
            get
            {
                return new SkinnedColorWrapper( CurveEditorSettingsType.GetField("wrapColor").GetValue(instance));
            }
            set
            {
                CurveEditorSettingsType.GetField("wrapColor").SetValue(instance, value.GetWrappedObject());
            }
        }

        public bool useFocusColors
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetField("useFocusColors").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("useFocusColors").SetValue(instance, value);
            }
        }

        public bool showAxisLabels
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetField("showAxisLabels").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("showAxisLabels").SetValue(instance, value);
            }
        }

        public bool showWrapperPopups
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetField("showWrapperPopups").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("showWrapperPopups").SetValue(instance, value);
            }
        }

        public bool allowDraggingCurvesAndRegions
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetField("allowDraggingCurvesAndRegions").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("allowDraggingCurvesAndRegions").SetValue(instance, value);
            }
        }

        public bool allowDeleteLastKeyInCurve
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetField("allowDeleteLastKeyInCurve").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("allowDeleteLastKeyInCurve").SetValue(instance, value);
            }
        }

        public bool undoRedoSelection
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetField("undoRedoSelection").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("undoRedoSelection").SetValue(instance, value);
            }
        }

        public bool flushCurveCache
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetField("flushCurveCache").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("flushCurveCache").SetValue(instance, value);
            }
        }

        public string xAxisLabel
        {
            get
            {
                return (string)CurveEditorSettingsType.GetField("xAxisLabel").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("xAxisLabel").SetValue(instance, value);
            }
        }

        public string yAxisLabel
        {
            get
            {
                return (string)CurveEditorSettingsType.GetField("yAxisLabel").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("yAxisLabel").SetValue(instance, value);
            }
        }

        public Vector2 curveRegionDomain
        {
            get
            {
                return (Vector2)CurveEditorSettingsType.GetField("curveRegionDomain").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("curveRegionDomain").SetValue(instance, value);
            }
        }

        public bool rippleTime
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetField("rippleTime").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetField("rippleTime").SetValue(instance, value);
            }
        }


        public bool hSlider
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetProperty("hSlider").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetProperty("hSlider").SetValue(instance, value);
            }
        }

        public bool vSlider
        {
            get
            {
                return (bool)CurveEditorSettingsType.GetProperty("vSlider").GetValue(instance);
            }
            set
            {
                CurveEditorSettingsType.GetProperty("vSlider").SetValue(instance, value);
            }
        }
    }
}

