using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Timeline_Editor.Structure;
using ABXY.Layers.Editor.Timeline_Editor.Variants.Style;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Timeline;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Runtime.Timeline.Playnode;

namespace ABXY.Layers.Editor.Timeline_Editor
{
    public class TimelineUIState
    {
        private float _zoomLevel = 1f;

        public float zoomLevel
        {
            get
            {
                return _zoomLevel;
            }
            set
            {
                float zoomLevelChange = _zoomLevel / value;
                _zoomLevel = value;

                float leftBarWidth = calculateLeftBarHeight != null ? calculateLeftBarHeight.Invoke(this) : 0f;
                float paneWidth = window.position.width - leftBarWidth;
                float paneCenter = paneWidth / 2f;
                float newScrollPosition = ((scrollPosition.x + paneCenter) * zoomLevelChange) - paneCenter;
                float clampedScrollPosition = Mathf.Clamp(newScrollPosition, 0, float.MaxValue);
                scrollPosition = new Vector2(clampedScrollPosition, scrollPosition.y);
            }
        }

        public System.Func<TimelineUIState, float> calculateTopBarHeight;

        public System.Func<TimelineUIState, float> calculateBottomBarHeight;

        public System.Func<TimelineUIState, float> calculateLeftBarHeight;

        public System.Func<TimelineUIState, TimeLineRowDataItem, float> calculateRowHeight;

        public System.Action<Rect, TimelineUIState, TimeLineRowDataItem, int> onDrawRow;

        public System.Action<double, int> OnCreateEvent;

        public System.Action<int> OnItemChangeRow;

        public System.Action OnTimeBarClick;

        public delegate TimelineDataItem OnCreateByDragToRowAreaDelegate(object data, int rowNumber, double dimensionX);
        public OnCreateByDragToRowAreaDelegate onCreateThroughDragToRowArea;

        public delegate TimeLineRowDataItem OnCreateRowDelegate(int rowNumber);
        public OnCreateRowDelegate onCreateRow;
        public bool autoExpandRows = false;


        public EditorWindow window { get; private set; }

        public Vector2 scrollPosition;

        public bool gridsArrangedByTime = false;

        public List<TimelineDataItem> selectedItems = new List<TimelineDataItem>();
        public bool hasSelectedItems { get { return selectedItems.Count > 0; } }

        public TimeLineDataSource dataSource;

        public BPMDataItem selectedTempo;

        public TimeSignatureDataItem selectedTimeSignature;

        public System.Action<TimelineDataItem> onDeleteDataItem;
    

        private Texture2D _timeSignatureBackground = null;
        /*public Texture2D timeSignatureBackground
    {
        get
        {
            if (_timeSignatureBackground == null)
                _timeSignatureBackground = Resources.Load<Texture2D>("Symphony-TimeSignatureTabRendered");
            return _timeSignatureBackground;
        }
    }*/
        public Texture2D timeSignatureBackground
        {
            get
            {
                if (_timeSignatureBackground == null)
                    _timeSignatureBackground = Resources.Load<Texture2D>("Symphony-TimeSignatureTabRenderedPlain");
                return _timeSignatureBackground;
            }
        }

        /*private Texture2D _timeSignatureBackgroundSelected = null;
    public Texture2D timeSignatureBackgroundSelected
    {
        get
        {
            if (_timeSignatureBackgroundSelected == null)
                _timeSignatureBackgroundSelected = Resources.Load<Texture2D>("Symphony-TimeSignatureTabRendered-Selected");
            return _timeSignatureBackgroundSelected;
        }
    }*/


        public float bpmAreaHeight { get { return 3f * EditorGUIUtility.singleLineHeight + 4f * EditorGUIUtility.standardVerticalSpacing + 5; } }
    

        public int selectedGridDivision = 5;

        public ITimeSpan gridTimeSpan { get { return MidiUtils.gridTimeSpans[selectedGridDivision]; } }

        public delegate double conversionDelegate(double inDimension);
        public conversionDelegate dimension2Pixel;
        public conversionDelegate pixel2Dimension;
        public conversionDelegate seconds2Dimension;
        public conversionDelegate dimension2Seconds;

        public Rect lastScrollContainerRect;

        private double MinItemWidth {get{ return Pixel2Dimension(20); }}

        public TimelineStyle style = new LightTimeline();

        private List<TimelineDataItem> copyBuffer = new List<TimelineDataItem>();

        public bool copyBufferHasItems { get { return copyBuffer.Count > 0; } }

        public TimelineUIState(EditorWindow window, TimeLineDataSource dataSource)
        {
            this.window = window;
            this.dataSource = dataSource;
        }





        private bool timelineItemDragStarted = false;
        private bool leftResize;
        private bool rightResize;
        float startingYPosition;
        float startingXPosition;
        public void DoTimelineDataItemEvents(Rect area, int itemIndex, TimelineDataItem datum, TimelineEditor editor)
        {
            if (!dataSource.editable)
                return;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && area.Contains(Event.current.mousePosition))
            {
                GUI.FocusControl("");
                if (!selectedItems.Contains(datum))
                {
                    if (Event.current.modifiers != EventModifiers.Shift)
                        selectedItems.Clear();
            
                    selectedItems.Add(datum);
                    List<TimelineDataItem> allItems = dataSource.GetTimelineDataItems();
                    allItems.Remove(datum);
                    allItems.Add(datum);
                    dataSource.ApplyTimelineDataChanges(allItems);
                }
            

                startingYPosition = Event.current.mousePosition.y;
                startingXPosition = Event.current.mousePosition.x;

                if (datum.resizableInInterface && Event.current.mousePosition.x - area.x < 10)
                {
                    //changed = true;
                    leftResize = true;
                    CalculateNoteDragThresholds(datum.startTime, datum);
                }
                else if (datum.resizableInInterface && area.x + area.width - Event.current.mousePosition.x < 10)
                {
                    //changed = true;
                    rightResize = true;
                    CalculateNoteDragThresholds(datum.startTime + datum.length, datum);
                }
                else
                {
                    timelineItemDragStarted = true;
                    CalculateNoteDragThresholds(datum.startTime, datum);
                }

                Event.current.Use();
                window.Repaint();
            }
            else if (Event.current.type == UnityEngine.EventType.MouseDrag && Event.current.button == 0 && (timelineItemDragStarted || leftResize || rightResize))
            {
                if (timelineItemDragStarted && selectedItems.Contains(datum))
                {
                    float yDelta = Event.current.mousePosition.y - startingYPosition;
                    dataSource.changed = true;
                    int offset = (int)( yDelta / area.height);

                    int rowCount = dataSource.GetDataRows().Count;
                    foreach (TimelineDataItem dataItem in selectedItems)
                    {

                        int lastOffset = dataItem.rowNumberOffset;

                        //Setting new row number
                        dataItem.rowNumberOffset = offset;

                        if (lastOffset != dataItem.rowNumberOffset)
                            OnItemChangeRow?.Invoke(dataItem.rowNumber);

                        //clamping row number. If expansion is set, will do it later in this function
                        if (!autoExpandRows || (datum is PlaynodeDataItem && (datum as PlaynodeDataItem).playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Loop ))
                        {
                        
                            if (dataItem.rowNumber >= rowCount) // clamping
                                dataItem.rowNumberOffset -= dataItem.rowNumber - rowCount + 1;
                        
                        }


                        if (Event.current.modifiers != EventModifiers.Control)
                        {//Then doing normal x movement without snapping
                            dataItem.startTime = dataItem.startTime + Pixel2Dimension( Event.current.delta.x);

                        }
                    }

                    //auto expanding
                    if (autoExpandRows)
                    {
                        int maxRow = 0;
                        foreach (TimelineDataItem dataItem in selectedItems)
                        {
                            if (maxRow < dataItem.rowNumber)
                                maxRow = dataItem.rowNumber;
                        }
                        List<TimeLineRowDataItem> timeLineRows = dataSource.GetDataRows();
                        for (int index = 0; index < maxRow - rowCount + 1; index++)
                        {
                            if (onCreateRow != null)
                                timeLineRows.Add(onCreateRow.Invoke(timeLineRows.Count));
                            else
                                timeLineRows.Add(new TimeLineRowDataItem());
                        }
                        dataSource.SetDataRows(timeLineRows);

                    }
                

                    if (Event.current.modifiers == EventModifiers.Control) { //Doing snapping x movement
                        double xDelta = Pixel2Dimension(Event.current.mousePosition.x - startingXPosition);
                        if (xDelta > rightSnapThreshold) //snapping to the right
                        {
                            double newTime = TimelineEditorUtils.GetNearestTimelineSnapPoint(datum.startTime, true, this, datum,gridsArrangedByTime);
                        
                            double shift = newTime - datum.startTime;
                            foreach (TimelineDataItem item in selectedItems)
                            {
                                item.startTime = item.startTime + shift;
                            }
                            CalculateNoteDragThresholds(datum.startTime, datum);
                            startingXPosition = Event.current.mousePosition.x;
                        }
                        if (xDelta < -leftSnapThreshold) //snapping to the left
                        {
                            double newTime = TimelineEditorUtils.GetNearestTimelineSnapPoint(datum.startTime, false, this, datum, gridsArrangedByTime);
                        
                            double shift = newTime - datum.startTime;
                            foreach (TimelineDataItem item in selectedItems)
                            {
                                item.startTime = item.startTime + shift;
                            }
                            CalculateNoteDragThresholds(datum.startTime, datum);
                            startingXPosition = Event.current.mousePosition.x;


                        }
                    }
                    Event.current.Use();
                    window.Repaint();

                }
                else if (leftResize)
                {
                    dataSource.changed = true;
                    if (Event.current.modifiers == EventModifiers.Control)//snapping
                    {
                        double xDelta = Pixel2Dimension(Event.current.mousePosition.x - startingXPosition);
                        if (xDelta > rightSnapThreshold || -xDelta > leftSnapThreshold)
                        {
                            foreach (TimelineDataItem item in selectedItems)
                            {
                                double newTime = TimelineEditorUtils.GetNearestTimelineSnapPoint(item.startTime, xDelta > 0, this, item, gridsArrangedByTime);
                                newTime = Mathf.Clamp((float)newTime, 0f, float.MaxValue);
                                double newLength = item.startTime + item.length - newTime;
                                if (newLength > MinItemWidth) // making sure we don't get negative length notes
                                {
                                    if (editor.leftResizeShiftsInternalTime)
                                        item.interiorStartTime = item.interiorStartTime + newTime - item.startTime;
                                    item.startTime = newTime;
                                    item.length = newLength;
                                }
                            }
                            startingXPosition = Event.current.mousePosition.x;
                            CalculateNoteDragThresholds(selectedItems.Last().startTime + selectedItems.Last().length, datum);
                            window.Repaint();
                        }
                    }
                    else // not snapping!
                    {
                        double deltaX = Pixel2Dimension( Event.current.mousePosition.x - startingXPosition);
                        startingXPosition = Event.current.mousePosition.x;
                        foreach (TimelineDataItem item in selectedItems)
                        {
                        
                            double newTime = item.startTime + deltaX;
                            newTime = Mathf.Clamp((float)newTime, 0f, float.MaxValue);
                            double newLength = item.length - (newTime - item.startTime);
                            if (newLength > MinItemWidth) // making sure we don't get negative length notes
                            {
                                if (editor.leftResizeShiftsInternalTime)
                                    item.interiorStartTime = item.interiorStartTime + newTime - item.startTime;
                                item.startTime = newTime;
                                item.length = newLength;
                            }
                        }
                        window.Repaint();
                    }
                    Event.current.Use();
                }
                else if (rightResize)
                {
                    dataSource.changed = true;
                    if (Event.current.modifiers == EventModifiers.Control)//snapping
                    {
                        double xDelta = Pixel2Dimension(Event.current.mousePosition.x - startingXPosition);
                        if (xDelta > rightSnapThreshold || -xDelta > leftSnapThreshold)
                        {
                            foreach (TimelineDataItem item in selectedItems)
                            {
                                double newEndTime = TimelineEditorUtils.GetNearestTimelineSnapPoint(item.startTime + item.length, xDelta > 0, this, item,  gridsArrangedByTime);

                                double newLength = newEndTime - item.startTime;
                                if (newLength > MinItemWidth) // making sure we don't get negative length notes
                                {
                                    item.length = newLength;
                                }
                            }
                            startingXPosition = Event.current.mousePosition.x;
                            CalculateNoteDragThresholds(selectedItems.Last().startTime + selectedItems.Last().length, datum);
                            window.Repaint();
                        }
                    }
                    else // not snapping!
                    {
                        double deltaX = Pixel2Dimension(Event.current.mousePosition.x - startingXPosition);
                        startingXPosition = Event.current.mousePosition.x;
                        foreach (TimelineDataItem item in selectedItems)
                        {
                            if (item == null)
                                continue;
                            double newLength = item.length + deltaX;
                            if (newLength > MinItemWidth) // making sure we don't get negative length notes
                            {
                                item.length = newLength;
                            }
                        }
                        window.Repaint();
                    }
                    Event.current.Use();
                }
            
            }
            else if (Event.current.type == UnityEngine.EventType.MouseUp && Event.current.button == 0 && (timelineItemDragStarted || leftResize || rightResize))
            {
                timelineItemDragStarted = false;
                rightResize = false;
                leftResize = false;
                foreach (TimelineDataItem dataItem in selectedItems)
                {
                    dataItem?.ApplyTransformations();
                }
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && area.Contains(Event.current.mousePosition))
            {
                TimelineDataItem primarySelection = selectedItems.Contains(datum) ? datum : null;
                editor.onRightClickItem?.Invoke(primarySelection, itemIndex, selectedItems, Event.current.mousePosition, new Vector2((float)Dimension2Seconds( Pixel2Dimension(Event.current.mousePosition.x)), datum.rowNumber));
                Event.current.Use();
            }
        }

        double leftSnapThreshold = 0;
        double rightSnapThreshold = 0;
        private void CalculateNoteDragThresholds(double Time, TimelineDataItem currentItem)
        {

            double previousSnapTarget = TimelineEditorUtils.GetNearestTimelineSnapPoint(Time, false, this, currentItem,  gridsArrangedByTime);
            double nextSnapTarget = TimelineEditorUtils.GetNearestTimelineSnapPoint(Time, true, this, currentItem,  gridsArrangedByTime);

        

            leftSnapThreshold = (Time - previousSnapTarget) / 2f;
            rightSnapThreshold = (nextSnapTarget - Time) / 2f;
        }

   

        public void ItemCanvasEvents(Rect eventContainer)
        {
            if (!dataSource.editable)
                return;

            eventContainer = new Rect(0, 0, eventContainer.width, eventContainer.height);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && eventContainer.Contains(Event.current.mousePosition) && selectedItems.Count > 0)
            {
                selectedItems.Clear();
                window.Repaint();
                Event.current.Use();
            }
        }

        public void ZoomAndDeleteEvents(Rect eventContainer)
        {
            if (eventContainer.Contains(Event.current.mousePosition) && Event.current.isScrollWheel && Event.current.modifiers == EventModifiers.Control)
            {
                zoomLevel += Event.current.delta.y*zoomLevel/5f;
                zoomLevel = Mathf.Clamp(zoomLevel, 1f, float.MaxValue);
                Event.current.Use();
            }else if (eventContainer.Contains(Event.current.mousePosition) && Event.current.type == EventType.KeyDown 
                && (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace))
            {
                if (!dataSource.editable)
                    return;
                dataSource.changed = true;

                foreach(TimelineDataItem item in selectedItems)
                {
                    dataSource.OnRemoveTimelineObject(item);
                    onDeleteDataItem?.Invoke(item);
                    item?.OnDestroy();
                }
                selectedItems.Clear();
                window.Repaint();
                Event.current.Use();
            }
        }

        public void AddEvent(Rect eventContainer)
        {
            if (!dataSource.editable)
                return;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && selectedItems.Count == 0 &&  eventContainer.Contains(Event.current.mousePosition) && EditorWindow.focusedWindow == window)
            {
                List<TimeLineRowDataItem> rows = dataSource.GetDataRows();
                int rowNumber = 0;
                float currentHeight = 0;
                foreach(TimeLineRowDataItem row in rows)
                {
                    if (currentHeight > Event.current.mousePosition.y)
                        break;
                    currentHeight += calculateRowHeight != null ? calculateRowHeight(this, row) : EditorGUIUtility.singleLineHeight;
                    rowNumber++;
                }

                double timePositionOfMouse = Pixel2Dimension(Event.current.mousePosition.x);
                double leftSnap = TimelineEditorUtils.GetNearestSnapPointOnGrid(timePositionOfMouse, false, this, false);
                double rightSnap = TimelineEditorUtils.GetNearestSnapPointOnGrid(timePositionOfMouse, true, this, false);


                TimelineDataItem newItem = dataSource.OnAddTimelineObject();
                if (newItem != null)
                {
                    newItem.rowNumber = rowNumber - 1;
                    newItem.startTime = leftSnap;
                    newItem.length = rightSnap - leftSnap;
                    OnCreateEvent?.Invoke(startingXPosition, rowNumber);
                }

                rightResize = true;
                startingXPosition = Event.current.mousePosition.x;

                if (newItem != null)
                    selectedItems.Add(newItem);

                dataSource.changed = true;
                window.Repaint();
            }
        }

        private bool tsClickDown = false;
        private double tsSnapDistance = 0;
        private long tsXDelta = 0;
        public void DoTimeSignatureInput(TimeSignatureDataItem ts, Rect timeSignatureNodeArea)
        {
            //if (!editable)
            //return;
            if (!dataSource.editable)
                return;
            double dragDelta = ts.time;
            if (Event.current.type == UnityEngine.EventType.MouseDown && Event.current.button == 0 && timeSignatureNodeArea.Contains(Event.current.mousePosition))
            {
                if (selectedTimeSignature != ts)
                    selectedTimeSignature = ts;
                tsClickDown = true;
                tsXDelta = 0;
                selectedTempo = null;
                CalcTSSnapDistance(ts.time);
                GUI.FocusControl("");
                Event.current.Use();
            }
            else if (Event.current.type == UnityEngine.EventType.MouseUp && Event.current.button == 0)
                tsClickDown = false;
            else if (Event.current.type != UnityEngine.EventType.MouseDrag && Event.current.button == 0 && tsClickDown && selectedTimeSignature == ts)
            {
                dataSource.changed = true;
                tsXDelta += (int)(Event.current.delta.x * zoomLevel);
                if (Mathf.Abs(tsXDelta) > tsSnapDistance)
                {
                    //changed = true;
                    if (tsXDelta > 0) // then positive
                        dragDelta = TimelineEditorUtils.GetNearestSnapPointOnGrid(ts.time, false, this, false) + tsSnapDistance * 4;
                    else if (tsXDelta < 0) // then negative
                        dragDelta = TimelineEditorUtils.GetNearestSnapPointOnGrid(ts.time, false, this, false);
                    tsXDelta = 0;
                }
                window.Repaint();
                //Event.current.Use(); For some reason this ruins everything?
            }else if ((Event.current.type == UnityEngine.EventType.MouseDown && Event.current.button == 1 && timeSignatureNodeArea.Contains(Event.current.mousePosition)))
            {
                NonBlockingMenu rightClickMenu = new NonBlockingMenu();
                rightClickMenu.AddItem(new GUIContent("Delete"), false, () => {
                    List<TimeSignatureDataItem> timeSignatures = dataSource.GetTimeSignatureItems();
                    timeSignatures.Remove(selectedTimeSignature);
                    selectedTimeSignature = null;
                    dataSource.SetTimeSignatureChanges(timeSignatures);
                    dataSource.changed = true;
                    window.Repaint();
                });
                rightClickMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
                Event.current.Use();
            }

            if (dragDelta != 0)
                ts.time = dragDelta;
        }
        private void CalcTSSnapDistance(double time)
        {
            if (time == 0)
                tsSnapDistance = TimelineEditorUtils.GetNearestSnapPointOnGrid(time, true, this, false) / 2;
            else
                tsSnapDistance = (time - TimelineEditorUtils.GetNearestSnapPointOnGrid(time, false, this, false)) / 2;

        }


        private bool tempoClickDown = false;
        private double tempoSnapDistance = 0;
        private long tempoXDelta = 0;
        public void DoBPMInput(BPMDataItem tempo, Rect tempoNodeArea)
        {
            if (!dataSource.editable)
                return;
            //if (!editable)
            //return;

            double dragDelta = tempo.time;
            if (Event.current.type == UnityEngine.EventType.MouseDown && Event.current.button ==0 && Event.current.button == 0 && tempoNodeArea.Contains(Event.current.mousePosition))
            {
                if (selectedTempo != tempo)
                    selectedTempo = tempo;
                selectedTimeSignature = null;
                tempoClickDown = true;
                tempoXDelta = 0;
                CalcTempoSnapDistance(tempo.time);
                GUI.FocusControl("");
                Event.current.Use();
            }
            else if (Event.current.type == UnityEngine.EventType.MouseUp && Event.current.button == 0)
                tempoClickDown = false;
            else if (Event.current.type != UnityEngine.EventType.MouseDrag && Event.current.button == 0 && tempoClickDown && selectedTempo == tempo)
            {
                dataSource.changed = true;
                tempoXDelta += (int)(Event.current.delta.x * zoomLevel);
                if (Mathf.Abs(tempoXDelta) > tempoSnapDistance)
                {
                    //changed = true;
                    if (tempoXDelta > 0) // then positive
                        dragDelta = TimelineEditorUtils.GetNearestSnapPointOnGrid(tempo.time, tempo.time == 0, this, false) + tempoSnapDistance * 4;
                    else if (tempoXDelta < 0) // then negative
                        dragDelta = TimelineEditorUtils.GetNearestSnapPointOnGrid(tempo.time, false, this, false);
                    tempoXDelta = 0;
                }
                window.Repaint();
                //Event.current.Use(); For some reason this ruins everything?
            }
            else if ((Event.current.type == UnityEngine.EventType.MouseDown && Event.current.button == 1 && tempoNodeArea.Contains(Event.current.mousePosition)))
            {
                NonBlockingMenu rightClickMenu = new NonBlockingMenu();
                rightClickMenu.AddItem(new GUIContent("Delete"), false, () => {
                    List<BPMDataItem> bpms = dataSource.GetBPMItems();
                    bpms.Remove(selectedTempo);
                    selectedTempo = null;
                    dataSource.SetBPMItems(bpms);
                    dataSource.changed = true;
                    window.Repaint();
                });
                rightClickMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
                Event.current.Use();
            }

            if (dragDelta != 0)
                tempo.time = dragDelta;
        }

        private void CalcTempoSnapDistance(double time)
        {
            if (time == 0)
                tempoSnapDistance = TimelineEditorUtils.GetNearestSnapPointOnGrid(time, true, this, false) / 2;
            else
                tempoSnapDistance = (time - TimelineEditorUtils.GetNearestSnapPointOnGrid(time, false, this, false)) / 2;

        }

        public void BPMAndTSDeselectionEvents(Rect clickArea)
        {
            if (!dataSource.editable)
                return;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && clickArea.Contains(Event.current.mousePosition))
            {
                selectedTempo = null;
                selectedTimeSignature = null;
                GUI.FocusControl("");
                window.Repaint();
            }
        }

        public void AddBPMAndTSEvents(Rect clickArea)
        {
            if (!dataSource.editable)
                return;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && clickArea.Contains(Event.current.mousePosition))
            {
                double time = Pixel2Dimension(Event.current.mousePosition.x);

                if (gridsArrangedByTime)
                    time = TimeConverter.ConvertFrom(new MetricTimeSpan((long)(time * 1000000)), dataSource.GetTempoMap());

                time = TimelineEditorUtils.GetNearestSnapPointOnGrid(time, true, this, false);

            

                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Add Tempo Change"), false, () => {
                    List<BPMDataItem> bpms = dataSource.GetBPMItems();
                    BPMDataItem newBPM = new BPMDataItem(Tempo.Default);
                    newBPM.time = time;
                    selectedTempo = newBPM;
                    bpms.Add(newBPM);
                    dataSource.SetBPMItems(bpms);
                    dataSource.changed = true;
                });
                menu.AddItem(new GUIContent("Add Time Signature Change"), false, () => {
                    List<TimeSignatureDataItem> timeSignatures = dataSource.GetTimeSignatureItems();
                    TimeSignatureDataItem newTS = new TimeSignatureDataItem(time, TimeSignature.Default);
                    selectedTimeSignature = newTS;
                    timeSignatures.Add(newTS);
                    dataSource.SetTimeSignatureChanges(timeSignatures); 
                    dataSource.changed = true;
                });
                menu.ShowAsContext();
            }
        }

        public void DeleteBPMAndTSEvents()
        {
            if (!dataSource.editable)
                return;
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace))
            {
                if (selectedTempo != null)
                {
                    List<BPMDataItem> bpms = dataSource.GetBPMItems();
                    bpms.Remove(selectedTempo);
                    selectedTempo = null;
                    dataSource.SetBPMItems(bpms);
                    Event.current.Use();
                    dataSource.changed = true;
                    window.Repaint();
                }

                if (selectedTimeSignature != null)
                {
                    List<TimeSignatureDataItem> timeSignatures = dataSource.GetTimeSignatureItems();
                    timeSignatures.Remove(selectedTimeSignature);
                    selectedTimeSignature = null;
                    dataSource.SetTimeSignatureChanges(timeSignatures);
                    Event.current.Use();
                    dataSource.changed = true;
                    window.Repaint();
                }
            }
        }


        private bool endTimeClicked = false;
        private double leftEndTimeSnapDistance = 0;
        private double rightleftSnapDistanceSnapDistance = 0;
        private double positionAtStart;
        public void DoEndTimeEvents(Rect lineArea)
        {
            if (!dataSource.editable)
                return;
            //Rect lineArea = new Rect(clickableArea.x + (float)Dimension2Pixel( dataSource.GetEndTime()) - 5, clickableArea.y, 15f * PixelsToMicroseconds, clickableArea.height);

            double endTimeDimensions = Seconds2Dimension(dataSource.GetEndTime());
            double endTimePixel = Dimension2Pixel(endTimeDimensions);

            //Vector2 scaledMousePosition = new Vector2((Event.current.mousePosition.x - gutterWidth) * PixelsToMicroseconds, Event.current.mousePosition.y);

            if (Event.current.type == UnityEngine.EventType.MouseDown && lineArea.Contains(Event.current.mousePosition))
            {

                endTimeClicked = true;
                CalcEndTimeSnapDistance((long)endTimePixel,gridsArrangedByTime);
                positionAtStart = Event.current.mousePosition.x;
                if (Event.current.type != UnityEngine.EventType.Repaint && Event.current.type != UnityEngine.EventType.Layout)
                    Event.current.Use();

                selectedItems.Clear();
                selectedTempo = null;
                selectedTimeSignature = null;

                window.Repaint();
            }
            else if (Event.current.type == UnityEngine.EventType.MouseDrag && endTimeClicked)
            {
                if (Event.current.modifiers == EventModifiers.Control)
                {
                    double endTimeDelta = Pixel2Dimension( Event.current.mousePosition.x - positionAtStart);
                    if (endTimeDelta > rightleftSnapDistanceSnapDistance)
                    {

                        if (gridsArrangedByTime)
                            dataSource.SetEndTime (TimelineEditorUtils.GetNearestTimelineSnapPoint(endTimeDimensions, true, this, new List<TimelineDataItem>(), true));
                        else
                            dataSource.SetEndTime(Dimension2Seconds(TimelineEditorUtils.GetNearestTimelineSnapPoint(endTimeDimensions, true, this, new List<TimelineDataItem>(), false)));

                        endTimeDelta = 0;
                        CalcEndTimeSnapDistance((long)endTimePixel, gridsArrangedByTime);
                        positionAtStart = Event.current.mousePosition.x;
                        dataSource.changed = true;
                    }
                    else if (-endTimeDelta > leftEndTimeSnapDistance)
                    {
                        if (gridsArrangedByTime)
                            dataSource.SetEndTime(TimelineEditorUtils.GetNearestTimelineSnapPoint(endTimeDimensions, false, this, new List<TimelineDataItem>(), true));
                        else
                            dataSource.SetEndTime(Dimension2Seconds(TimelineEditorUtils.GetNearestTimelineSnapPoint(endTimeDimensions, false, this, new List<TimelineDataItem>(), false)));
                        endTimeDelta = 0;
                        CalcEndTimeSnapDistance((long)endTimePixel, gridsArrangedByTime);
                        positionAtStart = Event.current.mousePosition.x;
                        dataSource.changed = true;
                    }
                }
                else
                {

                    dataSource.SetEndTime(Dimension2Seconds(Pixel2Dimension(Mathf.Clamp((float)endTimePixel + Event.current.delta.x , 0f, (float)long.MaxValue))));
                    dataSource.changed = true;

                }
                window.Repaint();
            }
            else if (Event.current.type == UnityEngine.EventType.MouseUp)
            {
                endTimeClicked = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">time in pixel space</param>
        private void CalcEndTimeSnapDistance(long time, bool convertToTime)
        {
            double previousSnapTarget = TimelineEditorUtils.GetNearestTimelineSnapPoint(time, false, this, new List<TimelineDataItem>(), convertToTime);
            double nextSnapTarget = TimelineEditorUtils.GetNearestTimelineSnapPoint(time, true, this, new List<TimelineDataItem>(), convertToTime);
            leftEndTimeSnapDistance = (time - previousSnapTarget) / 2f;// / zoomLevel;
            rightleftSnapDistanceSnapDistance = (nextSnapTarget - time);// / 2f / zoomLevel;

        }

        public void DoDragEvents(Rect targetArea)
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                if (new Rect(0, 0, window.position.width, window.position.height).Contains(Event.current.mousePosition))
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                else
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

                Event.current.Use();

            }
            else if (Event.current.type == EventType.DragExited)
            {

                DragAndDrop.AcceptDrag();
                foreach (Object datum in DragAndDrop.objectReferences)
                {
                    List<TimeLineRowDataItem> rows = dataSource.GetDataRows();
                    int rowNumber = 0;
                    float currentHeight = 0;
                    selectedItems.Clear();
                    foreach (TimeLineRowDataItem row in rows)
                    {
                        if (currentHeight > Event.current.mousePosition.y)
                            break;
                        currentHeight += calculateRowHeight != null ? calculateRowHeight(this, row) : EditorGUIUtility.singleLineHeight;
                        rowNumber++;
                    }

                    double timePositionOfMouse = Pixel2Dimension(Event.current.mousePosition.x);

                    if (onCreateThroughDragToRowArea != null)
                    {
                        TimelineDataItem timelineObject = onCreateThroughDragToRowArea?.Invoke(datum, rowNumber-1, timePositionOfMouse);
                        if (timelineObject != null)
                        {
                            selectedItems.Add(timelineObject);
                        }
                    }

                }

                Event.current.Use();
            }
        }

        public void DoTrackRightClickEvents(Rect clickArea, int rowNumber, TimelineEditor editor)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && clickArea.Contains(Event.current.mousePosition))
            {
                editor.onRightClickItem?.Invoke(null, 0, selectedItems, Event.current.mousePosition, new Vector2((float)Dimension2Seconds(Pixel2Dimension(Event.current.mousePosition.x)), rowNumber));
                Event.current.Use();
            }
        }

        private bool draggingPlayBar = false;
        public void DoPlaybackTimeEvents(Rect lineRect)
        {
            float clickableWidth = 20f;
            Rect dragRect = new Rect(lineRect.x - clickableWidth / 2f, lineRect.y, lineRect.width + clickableWidth, lineRect.height);
            //EditorGUI.DrawRect(dragRect, Color.red);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && dragRect.Contains(Event.current.mousePosition))
            {
                draggingPlayBar = true;
                OnTimeBarClick?.Invoke();
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && draggingPlayBar)
            {
                double newTime = Dimension2Seconds( Clamp2Positive( Pixel2Dimension(Event.current.mousePosition.x)));
                dataSource.SetCurrentPlaybackTime(newTime);
                window.Repaint();
                Event.current.Use();

            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && draggingPlayBar)
            {
                draggingPlayBar = false;
                Event.current.Use();
            }
        }

        private double Clamp2Positive(double input)
        {
            if (input < 0)
                return 0;
            return input;
        }

        public void SelectBeforeMousePosition(float mousePosition)
        {
            DeselectTimelineItems();
            List<TimelineDataItem> items = dataSource.GetTimelineDataItems();
            foreach(TimelineDataItem item in items)
            {
                double startTime = item.startTime;
                double endTime = item.startTime + item.length;
                double mouseTime = mousePosition;
                if (startTime <= mouseTime || endTime <= mouseTime)
                    selectedItems.Add(item);
            }
        }

        public void SelectAfterMousePosition(float mousePosition)
        {
            DeselectTimelineItems();
            List<TimelineDataItem> items = dataSource.GetTimelineDataItems();
            foreach (TimelineDataItem item in items)
            {
                double startTime = item.startTime;
                double endTime = item.startTime + item.length;

                double mouseTime = mousePosition;
                if (startTime >= mouseTime || endTime >= mouseTime)
                    selectedItems.Add(item);
            }
        }

        public void SendSelectionToCopyBuffer()
        {
            copyBuffer.Clear();
            copyBuffer = new List<TimelineDataItem>(selectedItems);

        }

        public void PasteCopyBuffer()
        {
            List<TimelineDataItem> copies = new List<TimelineDataItem>();
            foreach(TimelineDataItem item in copyBuffer)
            {
                TimelineDataItem newItem = item.Copy();
                copies.Add(newItem);
            }
            List<TimelineDataItem> currentItems = dataSource.GetTimelineDataItems();

            foreach (TimelineDataItem item in copies)
            {
                currentItems.Insert(0,item);
            }
            dataSource.ApplyTimelineDataChanges(currentItems);
            selectedItems.Clear();
            selectedItems = copies;
            window.Repaint();
        }

        public void DeselectTimelineItems()
        {
            selectedItems.Clear();
            window.Repaint();
        }

        public double Dimension2Pixel(double input)
        {
            return (dimension2Pixel != null ? dimension2Pixel.Invoke(input) : input) / zoomLevel;
        }
        public double Pixel2Dimension(double input)
        {
            return (pixel2Dimension != null ? pixel2Dimension.Invoke(input) : input) * zoomLevel;
        }
        public double Seconds2Dimension(double input)
        {
            return seconds2Dimension != null ? seconds2Dimension.Invoke(input) : input;
        }
        public double Dimension2Seconds(double input)
        {
            return dimension2Seconds != null ? dimension2Seconds.Invoke(input) : input;
        }

    }
}
