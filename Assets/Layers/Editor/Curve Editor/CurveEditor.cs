using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.Curve_Editor.Wrappers;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor
{
    public class CurveEditor
    {

        CurveEditorWrapper curveEditor;

        CurveEditorSettingsWrapper settingsWrapper = new CurveEditorSettingsWrapper();

        TickStyleWrapper horizontalTickStyle = new TickStyleWrapper();
        TickStyleWrapper verticalTickStyle = new TickStyleWrapper();

        #region Curve Editor Settings properties

        /// <summary>
        /// TODO: Figure out what this does
        /// </summary>
        public bool hRangeLocked
        {
            get
            {
                return settingsWrapper.hRangeLocked;
            }
            set
            {
                settingsWrapper.hRangeLocked = value;
                curveEditor.settings = settingsWrapper;
            }
        }

        /// <summary>
        /// TODO: Figure out what this does
        /// </summary>
        public bool vRangeLocked
        {
            get
            {
                return settingsWrapper.vRangeLocked;
            }
            set
            {
                settingsWrapper.vRangeLocked = value;
                curveEditor.settings = settingsWrapper;
            }
        }


        /// <summary>
        /// Locks the graph to only display between the given values on this axis. Note, Mathf.Infinity and Mathf.NegativeInfinity
        /// are valid values
        /// </summary>
        /// <param name="start">The starting value</param>
        /// <param name="end">The ending value</param>
        public void SetHorizontalRange(float start, float end)
        {
            settingsWrapper.hRangeMin = start;
            settingsWrapper.hRangeMax = end;
            curveEditor.settings = settingsWrapper;
        }

        /// <summary>
        /// Locks the graph to only display between the given values on this axis. Note, Mathf.Infinity and Mathf.NegativeInfinity
        /// are valid values
        /// </summary>
        /// <param name="start">The starting value</param>
        /// <param name="end">The ending value</param>
        public void SetVerticalRange(float start, float end)
        {
            settingsWrapper.vRangeMin = start;
            settingsWrapper.vRangeMax = end;
            curveEditor.settings = settingsWrapper;
        }

        /// <summary>
        /// TODO: This doesn't seem to do anything, why?
        /// </summary>
        public bool showAxisLabels
        {
            get
            {
                return settingsWrapper.showAxisLabels;
            }
            set
            {
                settingsWrapper.showAxisLabels = value;
                curveEditor.settings = settingsWrapper;
            }
        }

    
        /// <summary>
        /// TODO: Not sure what this does yet
        /// </summary>
        public bool showWrapperPopups
        {
            get
            {
                return settingsWrapper.showWrapperPopups;
            }
            set
            {
                settingsWrapper.showWrapperPopups = value;
                curveEditor.settings = settingsWrapper;
            }
        }

        /// <summary>
        /// TODO: Not yet sure what this does
        /// </summary>
        public bool allowDraggingCurvesAndRegions
        {
            get
            {
                return settingsWrapper.allowDraggingCurvesAndRegions;
            }
            set
            {
                settingsWrapper.allowDraggingCurvesAndRegions = value;
                curveEditor.settings = settingsWrapper;
            }
        }


        /// <summary>
        /// Set to false to prevent the last point in a curve from being deleted by the user
        /// </summary>
        public bool allowDeleteLastKeyInCurve
        {
            get
            {
                return settingsWrapper.allowDeleteLastKeyInCurve;
            }
            set
            {
                settingsWrapper.allowDeleteLastKeyInCurve = value;
                curveEditor.settings = settingsWrapper;
            }
        }

        /// <summary>
        /// Include selection events in Undo/Redo
        /// </summary>
        public bool undoRedoSelection
        {
            get
            {
                return settingsWrapper.undoRedoSelection;
            }
            set
            {
                settingsWrapper.undoRedoSelection = value;
                curveEditor.settings = settingsWrapper;
            }
        }

        /// <summary>
        /// Show vertical scroll bar
        /// </summary>
        public bool showVerticalSlider
        {
            get
            {
                return settingsWrapper.vSlider;
            }
            set
            {
                settingsWrapper.vSlider = value;
                curveEditor.settings = settingsWrapper;
            }
        }

        /// <summary>
        /// Show horizontal scroll bar
        /// </summary>
        public bool showHorizontalSlider
        {
            get
            {
                return settingsWrapper.hSlider;
            }
            set
            {
                settingsWrapper.hSlider = value;
                curveEditor.settings = settingsWrapper;
            }
        }


        #endregion

        #region Tick Settings
        private readonly Color defaultTickColor = new Color(.45f, .45f, .45f, 0.2f);

        /// <summary>
        /// Sets up how the horizontal axis is visualized
        /// </summary>
        /// <param name="tickColor">The color of this Axis' tick lines</param>
        /// <param name="labelColor">The color of this Axis' labels</param>
        /// <param name="distMin">Tick lines disappear when they are too close together. At this distance, tick lines are no longer visible</param>
        /// <param name="distFull">Tick lines disappear when they are too close together. At this distance, tick lines are fully visible</param>
        /// <param name="distLabel">Tick labels disappear when they are close together. At this distance tick labels are no longer visible</param>
        /// <param name="stubs">Should ticks be drawn as stubs or full-length lines?</param>
        /// <param name="centerLabel">Should the label be centered on this axis?</param>
        /// <param name="unit">The units label for this axis</param>
        public void SetupHorizontalAxis(Color tickColor, Color labelColor, int distMin = 10, int distFull = 80, int distLabel=50, bool stubs=false, bool centerLabel= false, string unit="")
        {
            horizontalTickStyle.tickColor = new SkinnedColorWrapper(tickColor);
            horizontalTickStyle.labelColor = new SkinnedColorWrapper(labelColor);
            horizontalTickStyle.distMin = distMin;
            horizontalTickStyle.distFull = distFull;
            horizontalTickStyle.distLabel = distLabel;
            horizontalTickStyle.stubs = stubs;
            horizontalTickStyle.centerLabel = centerLabel;
            horizontalTickStyle.unit = unit;
        }

        /// <summary>
        /// Sets up how the vertical axis is visualized
        /// </summary>
        /// <param name="tickColor">The color of this Axis' tick lines</param>
        /// <param name="labelColor">The color of this Axis' labels</param>
        /// <param name="distMin">Tick lines disappear when they are too close together. At this distance, tick lines are no longer visible</param>
        /// <param name="distFull">Tick lines disappear when they are too close together. At this distance, tick lines are fully visible</param>
        /// <param name="distLabel">Tick labels disappear when they are close together. At this distance tick labels are no longer visible</param>
        /// <param name="stubs">Should ticks be drawn as stubs or full-length lines?</param>
        /// <param name="centerLabel">Should the label be centered on this axis?</param>
        /// <param name="unit">The units label for this axis</param>
        public void SetupVerticalAxis(Color tickColor, Color labelColor, int distMin = 10, int distFull = 80, int distLabel = 50, bool stubs = false, bool centerLabel = false, string unit = "")
        {
            verticalTickStyle.tickColor = new SkinnedColorWrapper(tickColor);
            verticalTickStyle.labelColor = new SkinnedColorWrapper(labelColor);
            verticalTickStyle.distMin = distMin;
            verticalTickStyle.distFull = distFull;
            verticalTickStyle.distLabel = distLabel;
            verticalTickStyle.stubs = stubs;
            verticalTickStyle.centerLabel = centerLabel;
            verticalTickStyle.unit = unit;
        }

        #endregion

        #region Curve Editor Properties

        /// <summary>
        /// When the editor is first shown, these are the margins used to display the ranges x = 0->1 and y = 0->1. In other words, a left margin
        /// of 25 will push the position of (0,0) to the right 25 pixels. Note that if pan/zoom isn't locked, the user can move the curve canvas 
        /// and the margin will no longer apply
        /// </summary>
        public Margin margins
        {
            get
            {
                return new Margin(curveEditor.leftmargin, curveEditor.rightmargin, curveEditor.topmargin, curveEditor.rightmargin);
            }
            set
            {
                curveEditor.leftmargin = value.left;
                curveEditor.rightmargin = value.right;
                curveEditor.topmargin = value.top;
                curveEditor.bottommargin = value.bottom;
            }
        }

        /// <summary>
        /// When the editor is first shown, these are the margins used to display the ranges x = 0->1 and y = 0->1. In other words, a left margin
        /// of 25 will push the position of (0,0) to the right 25 pixels. Note that if pan/zoom isn't locked, the user can move the curve canvas 
        /// and the margin will no longer apply
        /// </summary>
        /// <param name="margin">The value to apply to the left, right, top and bottom margins</param>
        public void SetMargins(float margin)
        {
            margins = new Margin(margin, margin, margin, margin);
        }

        /// <summary>
        /// When the editor is first shown, these are the margins used to display the ranges x = 0->1 and y = 0->1. In other words, a left margin
        /// of 25 will push the position of (0,0) to the right 25 pixels. Note that if pan/zoom isn't locked, the user can move the curve canvas 
        /// and the margin will no longer apply
        /// </summary>
        /// <param name="left">The left margin</param>
        /// <param name="right">The right margin</param>
        /// <param name="top">The top margin</param>
        /// <param name="bottom">The bottom margin</param>
        public void SetMargins(float left, float right, float top, float bottom)
        {
            margins = new Margin(left, right, top, bottom);
        }

        /// <summary>
        /// Set to true to prevent the curve graph from receiving scroll events until the graph has been clicked.
        /// This is useful for preventing unintended scrolling when interacting with other UI elements
        /// </summary>
        public bool ignoreScrollWheelUntilClicked
        {
            get
            {
                return curveEditor.ignoreScrollWheelUntilClicked;
            }
            set
            {
                curveEditor.ignoreScrollWheelUntilClicked = value;
            }
        }


        #endregion

        private CurveList curveList = new CurveList();

        public System.Action<SerializedProperty> onCurveChanged;

        /// <summary>
        /// Set to false to prevent the legend area from drawing
        /// </summary>
        public bool drawLegend = true;

        public CurveEditor( )
        {
            curveEditor = new CurveEditorWrapper(new Rect(0, 0, 1000, 100), new CurveWrapperWrapper[0], false);
            curveEditor.margin = 25;
            curveEditor.SetShownHRangeInsideMargins(0.0f, 1.0f);
            curveEditor.SetShownVRangeInsideMargins(0.0f, 1.1f);
            curveEditor.ignoreScrollWheelUntilClicked = true;

            settingsWrapper.hTickStyle = horizontalTickStyle;

            settingsWrapper.vTickStyle = verticalTickStyle;
            curveEditor.settings = settingsWrapper;

        }

        public void DrawLayout(float curveEditorAspectRatio)
        {
            Rect drawRect = GUILayoutUtility.GetAspectRect(curveEditorAspectRatio, GUI.skin.textField);
            drawRect.xMin += EditorGUIWrapper.indent;
            Draw(drawRect);
        }

        public void Draw(Rect rect)
        {
            SynchronizeCurves(true);

            float legendHeight = CalculateLegendHeight();
            rect.height -= legendHeight;

            if (Event.current.type != EventType.Layout && Event.current.type != EventType.Used)
            {
                curveEditor.rect = rect;
            }

            GUI.Label(curveEditor.drawRect, GUIContent.none, "TextField");

            curveEditor.OnGUI();

            foreach (CurveList.CurveContainer curve in curveList.GetCurves())
            {
                if (curve.curveWrapper.changed)
                {
                    curve.ApplyCurve();
                    onCurveChanged?.Invoke(curve.curveSP);
                    curve.ApplyCurve();
                }
            }

            if (drawLegend)
            {
                Rect legendRect = new Rect(rect.x, rect.y + rect.height, rect.width, legendHeight);
                DrawLegends(legendRect);
            }

            foreach (CurveList.CurveContainer container in curveList.GetCurves())
                container.curveWrapper.changed = false;

        }

        private void DrawLegends(Rect drawArea)
        {
            List<List<CurveList.CurveContainer>> legendRows = new List<List<CurveList.CurveContainer>>();
            List<CurveList.CurveContainer> allCurves = new List<CurveList.CurveContainer>(curveList.GetCurves());
            while (allCurves.Count != 0)
            {
                List<CurveList.CurveContainer> newRow = allCurves.Take(Mathf.Min(4, allCurves.Count)).ToList();
                allCurves.RemoveRange(0, newRow.Count);
                legendRows.Add(newRow);
            }

            List<Rect> allLegendRects = new List<Rect>();

            for (int rowIndex = 0; rowIndex < legendRows.Count; rowIndex++)
            {
                List<CurveList.CurveContainer> row = legendRows[rowIndex];
                float legendItemWidth = drawArea.width / row.Count;
                for (int legendIndex = 0; legendIndex < row.Count; legendIndex++)
                {
                    Rect legendRect = new Rect(
                        drawArea.x + legendIndex * legendItemWidth,
                        drawArea.y + rowIndex * EditorGUIUtility.singleLineHeight,
                        legendItemWidth,
                        EditorGUIUtility.singleLineHeight);
                    allLegendRects.Add(legendRect);
                    EditorGUIWrapper.DrawLegend(legendRect, row[legendIndex].curveWrapper.color, row[legendIndex].legendString, row[legendIndex].selected);
                }
            }

            bool[] selection = curveList.GetSelectionArray();

            if (EditorGUIExtWrapper.DragSelection(allLegendRects.ToArray(), ref selection, GUIStyle.none))
            {

            }

            curveList.SetSelectionArray(selection);
        }

        private void SynchronizeCurves(bool withChangeCheck)
        {
            if (curveEditor.InLiveEdit() || (withChangeCheck && curveList.GetCurves().Where(x=>x.curveWrapper.changed).Count() != 0))
                return;

            curveList.UpdateCurves();
            curveEditor.animationCurves = curveList.GetCurves().Select(x => x.curveWrapper).ToArray();
        }

        private float CalculateLegendHeight()
        {
            return drawLegend? (Mathf.Floor((curveEditor.animationCurves.Length / 4f)) + 1) * EditorGUIUtility.singleLineHeight: 0f;
        }

        #region Curve API 

        public void AddCurve(SerializedProperty curveProperty, Color curveColor, string legendName)
        {
            curveList.Add(curveProperty, legendName, curveColor);
        }

        public void RemoveCurve(SerializedProperty curveproperty)
        {
            curveList.Remove(curveproperty);
        }

        public void SetCurveVisibility(SerializedProperty curveProperty, bool show)
        {
            CurveList.CurveContainer container = curveList.GetCurves().Where(x => x.curveSP.propertyPath == curveProperty.propertyPath).FirstOrDefault();
            if (container != null)
            {
                container.hidden = !show;
            }
        }

        public void SetCurveReadOnly(SerializedProperty curveProperty, bool readOnly)
        {
            CurveList.CurveContainer container = curveList.GetCurves().Where(x => x.curveSP.propertyPath == curveProperty.propertyPath).FirstOrDefault();
            if (container != null)
            {
                container.readOnly = readOnly;
            }
        }

        /// <summary>
        /// Sets the horizontal range for the given curve property
        /// </summary>
        /// <param name="curveProperty"></param>
        /// <param name="range"> The start and end values. Infinity and negative infinity are valid values here</param>
        public void SetHorizontalCurveRange(SerializedProperty curveProperty, Vector2 range)
        {
            CurveList.CurveContainer container = curveList.GetCurve(curveProperty);
            container.verticalRange = range;
        }

        #endregion

    }
}
