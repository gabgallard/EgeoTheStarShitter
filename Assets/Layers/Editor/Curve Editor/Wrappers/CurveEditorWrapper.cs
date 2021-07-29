using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class CurveEditorWrapper 
    {
        private System.Type curveEditorType;
        object instance;

        public delegate void CallbackFunction();

        public CallbackFunction curvesUpdated;

        public Rect rect
        {
            get
            {
                return (Rect)curveEditorType.GetProperty("rect").GetValue(instance);
            }
            set
            {
                curveEditorType.GetProperty("rect").SetValue(instance, value);
            }
        }

        public Rect drawRect
        {
            get
            {
                return (Rect)curveEditorType.GetProperty("drawRect").GetValue(instance);
            }
        }

        public bool hRangeLocked { 
            get {
                return (bool)curveEditorType.GetProperty("hRangeLocked").GetValue(instance);
            } 
            set {
                curveEditorType.GetProperty("hRangeLocked").SetValue(instance, value);
            } 
        }
        public bool vRangeLocked
        {
            get
            {
                return (bool)curveEditorType.GetProperty("vRangeLocked").GetValue(instance);
            }
            set
            {
                curveEditorType.GetProperty("vRangeLocked").SetValue(instance, value);
            }
        }


        public CurveWrapperWrapper[] animationCurves
        {
            get
            {
                Array nativeCurveWrapperArray = (Array)curveEditorType.GetProperty("animationCurves").GetValue(instance);
                CurveWrapperWrapper[] curveWrappers = new CurveWrapperWrapper[nativeCurveWrapperArray.Length];
                for (int index = 0; index < nativeCurveWrapperArray.Length; index++)
                    curveWrappers[index] = new CurveWrapperWrapper(nativeCurveWrapperArray.GetValue(index));
                return curveWrappers;
            }
            set
            {
                Array nativeCurveWrapperArray = Array.CreateInstance(CurveWrapperWrapper.curveWrapperType, value.Length);
                for (int index = 0; index < value.Length; index++)
                    nativeCurveWrapperArray.SetValue(value[index].GetWrappedObject(), index);
                curveEditorType.GetProperty("animationCurves").SetValue(instance, nativeCurveWrapperArray);
            }
        }

        public bool GetTopMostCurveID(out int curveID)
        {
            object[] parameters = new object[] { null };
            bool result = (bool)curveEditorType.GetMethod("GetTopMostCurveID").Invoke(instance, parameters);
            curveID = (int)parameters[0];

            return result;
        }

        void FlushCurvesCache()
        {
            curveEditorType.GetMethod("FlushCurvesCache").Invoke(instance, new object[] { });
        }

        void SyncDrawOrder()
        {
            curveEditorType.GetMethod("SyncDrawOrder").Invoke(instance, new object[] { });
        }

        //public ICurveEditorState state;

        public TimeFormatWrapper timeFormat
        {
            get
            {
                return TimeFormatWrapperUtils.FromNativeTimeFormat(curveEditorType.GetProperty("timeFormat").GetValue(instance));
            }
        }

        public bool rippleTime
        {
            get
            {
                return (bool)curveEditorType.GetProperty("rippleTime").GetValue(instance);
            }
        }

        internal Bounds m_DefaultBounds = new Bounds(new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 0));


        public CurveEditorSettingsWrapper settings
        {
            get
            {
                return new CurveEditorSettingsWrapper(curveEditorType.GetProperty("settings").GetValue(instance));
            }
            set
            {
                curveEditorType.GetProperty("settings").SetValue(instance, value.GetWrappedObject());
            }
        }

    

        /// 1/time to snap all keyframes to while dragging. Set to 0 for no snap (default)
        public float invSnap
        {
            get
            {
                return (float)curveEditorType.GetField("invSnap").GetValue(instance);
            }
            set
            {
                curveEditorType.GetField("invSnap").SetValue(instance, value);
            }
        }
       
        public bool hasSelection { 
            get {
                return (bool)curveEditorType.GetProperty("hasSelection").GetValue(instance);
            } 
        }


        public Bounds selectionBounds
        {
            get
            {
                return (Bounds)curveEditorType.GetProperty("selectionBounds").GetValue(instance);
            }
        }

        public Bounds selectionWithCurvesBounds
        {
            get
            {
                return (Bounds)curveEditorType.GetProperty("selectionWithCurvesBounds").GetValue(instance);
            }
        }

        public Bounds curveBounds
        {
            get
            {
                return (Bounds)curveEditorType.GetProperty("curveBounds").GetValue(instance);
            }
        }

        public Bounds drawingBounds
        {
            get
            {
                return (Bounds)curveEditorType.GetProperty("drawingBounds").GetValue(instance);
            }
        }


    

        public void OnEnable()
        {
            curveEditorType.GetMethod("OnEnable").Invoke(instance, new object[] { });
        }

        public void OnDisable()
        {
            curveEditorType.GetMethod("OnDisable").Invoke(instance, new object[] { });
        }

        public void OnDestroy()
        {
            curveEditorType.GetMethod("OnDestroy").Invoke(instance, new object[] { });
        }

        public void InvalidateBounds()
        {
            curveEditorType.GetMethod("InvalidateBounds").Invoke(instance, new object[] { });
        }

        public void InvalidateSelectionBounds()
        {
            curveEditorType.GetMethod("InvalidateSelectionBounds").Invoke(instance, new object[] { });
        }

        public Bounds GetClipBounds()
        {
            return (Bounds)curveEditorType.GetMethod("GetClipBounds").Invoke(instance, new object[] { });
        }

        public Bounds GetSelectionBounds()
        {
            return (Bounds)curveEditorType.GetMethod("GetSelectionBounds").Invoke(instance, new object[] { });
        }

        // Frame all curves to be visible.
        public void FrameClip(bool horizontally, bool vertically)
        {
            curveEditorType.GetMethod("FrameClip").Invoke(instance, new object[] { horizontally, vertically });
        }

        // Frame selected keys to be visible.
        public void FrameSelected(bool horizontally, bool vertically)
        {
            curveEditorType.GetMethod("FrameSelected").Invoke(instance, new object[] { horizontally, vertically });
        }

        public void Frame(Bounds frameBounds, bool horizontally, bool vertically)
        {
            curveEditorType.GetMethod("Frame").Invoke(instance, new object[] { frameBounds, horizontally, vertically });
        }

        public void UpdateCurves(List<int> curveIds, string undoText)
        {
            curveEditorType.GetMethod("UpdateCurves").Invoke(instance, new object[] { curveIds, undoText });
        }

        public void UpdateCurves(List<ChangedCurveWrapper> changedCurves, string undoText)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(ChangedCurveWrapper.ChangedCurveType);

            var listInstance = (IList)Activator.CreateInstance(constructedListType);
            foreach (ChangedCurveWrapper curve in changedCurves)
                listInstance.Add(curve.GetWrappedObject());

            curveEditorType.GetMethod("UpdateCurves").Invoke(instance, new object[] { changedCurves, undoText });
        }

        public void StartLiveEdit()
        {
            curveEditorType.GetMethod("StartLiveEdit").Invoke(instance, new object[] { });
        }

        public void EndLiveEdit()
        {
            curveEditorType.GetMethod("EndLiveEdit").Invoke(instance, new object[] { });
        }

        public bool InLiveEdit()
        {
            return (bool)curveEditorType.GetMethod("InLiveEdit").Invoke(instance, new object[] { });
        }

        public void OnGUI()
        {
            curveEditorType.GetMethod("OnGUI").Invoke(instance, new object[] { });
        }

        public void CurveGUI()
        {
            curveEditorType.GetMethod("CurveGUI").Invoke(instance, new object[] { });
        }

        // Recalculate curve.selected from selected curves
    
        public void AddKey(CurveWrapperWrapper cw, Keyframe key)
        {
            curveEditorType.GetMethod("AddKey").Invoke(instance, new object[] { cw.GetWrappedObject(), key });
        }

        public void SelectNone()
        {
            curveEditorType.GetMethod("SelectNone").Invoke(instance, new object[] { });
        }

        public void SelectAll()
        {
            curveEditorType.GetMethod("SelectAll").Invoke(instance, new object[] { });
        }

        public bool IsDraggingKey()
        {
            return (bool)curveEditorType.GetMethod("IsDraggingKey").Invoke(instance, new object[] { });
        }

        public bool IsDraggingCurveOrRegion()
        {
            return (bool)curveEditorType.GetMethod("IsDraggingCurveOrRegion").Invoke(instance, new object[] { });
        }

        public bool IsDraggingCurve(CurveWrapperWrapper cw)
        {
            return (bool)curveEditorType.GetMethod("IsDraggingCurve").Invoke(instance, new object[] { cw.GetWrappedObject() });
        }

        public bool IsDraggingRegion(CurveWrapperWrapper cw1, CurveWrapperWrapper cw2)
        {
            return (bool)curveEditorType.GetMethod("IsDraggingCurve").Invoke(instance, new object[] { cw1.GetWrappedObject(), cw2.GetWrappedObject() });
        }

        public void BeginTimeRangeSelection(float time, bool addToSelection)
        {
            curveEditorType.GetMethod("BeginTimeRangeSelection").Invoke(instance, new object[] { time, addToSelection });
        }

        public void TimeRangeSelectTo(float time)
        {
            curveEditorType.GetMethod("TimeRangeSelectTo").Invoke(instance, new object[] { time });
        }

        public void EndTimeRangeSelection()
        {
            curveEditorType.GetMethod("EndTimeRangeSelection").Invoke(instance, new object[] { });
        }

        public void CancelTimeRangeSelection()
        {
            curveEditorType.GetMethod("CancelTimeRangeSelection").Invoke(instance, new object[] { });
        }


        public Vector2 MovePoints()
        {
            return (Vector2)curveEditorType.GetMethod("MovePoints").Invoke(instance, new object[] { });
        }
        public void SaveKeySelection(string undoLabel)
        {
            curveEditorType.GetMethod("SaveKeySelection").Invoke(instance, new object[] { undoLabel });
        }

        public void DrawRegion(CurveWrapperWrapper curve1, CurveWrapperWrapper curve2, bool hasFocus)
        {
            curveEditorType.GetMethod("DrawRegion").Invoke(instance, new object[] { curve1.GetWrappedObject(), curve2.GetWrappedObject(), hasFocus }) ;
        }


        public void GridGUI()
        {
            curveEditorType.GetMethod("GridGUI").Invoke(instance, new object[] { });
        }
        public float margin
        {
            set
            {
                curveEditorType.GetProperty("margin").SetValue(instance, value);
            }
        }

        public float leftmargin { 
            get {
                return (float)curveEditorType.GetProperty("leftmargin").GetValue(instance); 
            } 
            set
            {
                curveEditorType.GetProperty("leftmargin").SetValue(instance, value);
            } 
        }
        public float rightmargin
        {
            get
            {
                return (float)curveEditorType.GetProperty("rightmargin").GetValue(instance);
            }
            set
            {
                curveEditorType.GetProperty("rightmargin").SetValue(instance, value);
            }
        }

        public float topmargin
        {
            get
            {
                return (float)curveEditorType.GetProperty("topmargin").GetValue(instance);
            }
            set
            {
                curveEditorType.GetProperty("topmargin").SetValue(instance, value);
            }
        }

        public float bottommargin
        {
            get
            {
                return (float)curveEditorType.GetProperty("bottommargin").GetValue(instance);
            }
            set
            {
                curveEditorType.GetProperty("bottommargin").SetValue(instance, value);
            }
        }



        public void SetShownHRangeInsideMargins(float min, float max)
        {
            curveEditorType.GetMethod("SetShownHRangeInsideMargins").Invoke(instance, new object[] { min, max });
        }

        public void SetShownVRangeInsideMargins(float min, float max)
        {
            curveEditorType.GetMethod("SetShownVRangeInsideMargins").Invoke(instance, new object[] { min, max });
        }

        public bool ignoreScrollWheelUntilClicked
        {
            get
            {
                return (bool)curveEditorType.GetProperty("ignoreScrollWheelUntilClicked").GetValue(instance);
            }
            set
            {
                curveEditorType.GetProperty("ignoreScrollWheelUntilClicked").SetValue(instance, value);
            }
        }

    

    
        public CurveWrapperWrapper GetCurveWrapperFromID(int curveID)
        {
            MethodInfo method = curveEditorType.GetMethod("GetCurveWrapperFromID", BindingFlags.NonPublic| BindingFlags.Instance);
            return new CurveWrapperWrapper(method.Invoke(instance, new object[] { curveID }));
        }

        public Vector2 DrawingToViewTransformPoint(Vector2 lhs)
        { return (Vector2)curveEditorType.GetMethod("DrawingToViewTransformPoint").Invoke(instance, new object[] { lhs }); }
        public Vector3 DrawingToViewTransformPoint(Vector3 lhs)
        { return (Vector3)curveEditorType.GetMethod("DrawingToViewTransformPoint").Invoke(instance, new object[] { lhs }); }

        public Vector2 ViewToDrawingTransformPoint(Vector2 lhs)
        { return (Vector2)curveEditorType.GetMethod("ViewToDrawingTransformPoint").Invoke(instance, new object[] { lhs }); }
        public Vector3 ViewToDrawingTransformPoint(Vector3 lhs)
        { return (Vector3)curveEditorType.GetMethod("ViewToDrawingTransformPoint").Invoke(instance, new object[] { lhs }); }

        public Vector2 DrawingToViewTransformVector(Vector2 lhs)
        { return (Vector2)curveEditorType.GetMethod("DrawingToViewTransformVector").Invoke(instance, new object[] { lhs }); }
        public Vector3 DrawingToViewTransformVector(Vector3 lhs)
        { return (Vector3)curveEditorType.GetMethod("DrawingToViewTransformVector").Invoke(instance, new object[] { lhs }); }

        public Vector2 ViewToDrawingTransformVector(Vector2 lhs)
        { return (Vector2)curveEditorType.GetMethod("ViewToDrawingTransformVector").Invoke(instance, new object[] { lhs }); }
        public Vector3 ViewToDrawingTransformVector(Vector3 lhs)
        { return (Vector3)curveEditorType.GetMethod("ViewToDrawingTransformVector").Invoke(instance, new object[] { lhs }); }



        public CurveEditorWrapper(Rect rect, CurveWrapperWrapper[] curves, bool minimalGUI)
        {
            Array castedCurves = Array.CreateInstance(CurveWrapperWrapper.curveWrapperType, curves.Length);
            for (int index = 0; index < curves.Length; index++)
                castedCurves.SetValue( curves[index], index);
            curveEditorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.CurveEditor");
            instance = System.Activator.CreateInstance(curveEditorType, rect, castedCurves, minimalGUI);
        }



        public object GetWrappedObject()
        {
            return instance;
        }
    }
}
