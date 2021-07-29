using System.Collections.Generic;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Timeline_Editor.Structure;
using ABXY.Layers.Editor.Timeline_Editor.Variants.Midi;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using ABXY.Layers.Runtime.Timeline;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor
{
    public class TimelineEditor
    {


        public TimelineUIState uiState;


        public System.Action<Rect, TimelineUIState> onDrawTopBar;
        public System.Func<TimelineUIState, float> calculateTopBarHeight { get { return uiState.calculateTopBarHeight; } set { uiState.calculateTopBarHeight = value; } }

        public System.Action<Rect, TimelineUIState> onDrawBottomBar;
        public System.Func<TimelineUIState,float> calculateBottomBarHeight { get { return uiState.calculateBottomBarHeight; } set { uiState.calculateBottomBarHeight = value; } }

        public System.Action<Rect, TimelineUIState> onDrawLeftBar;
        public System.Func<TimelineUIState, float> calculateLeftBarHeight { get { return uiState.calculateLeftBarHeight; } set { uiState.calculateLeftBarHeight = value; } }

        public System.Func<TimelineUIState, TimeLineRowDataItem, float> calculateRowHeight { get { return uiState.calculateRowHeight; } set { uiState.calculateRowHeight = value; } }

        public System.Action<Rect, TimelineUIState, TimeLineRowDataItem, int> onDrawRow;

        public System.Action<Rect, TimelineUIState, TimelineDataItem, bool> onDrawDataItem;

        public System.Action<TimelineDataItem, int, List<TimelineDataItem>, Vector2, Vector2> onRightClickItem;

        public delegate bool ShouldDrawItemCallback(Rect position, TimelineUIState uiStatw, TimelineDataItem item);
        public ShouldDrawItemCallback shouldDrawItem;

        public TimelineUIState.OnCreateRowDelegate onCreateRow { get { return uiState.onCreateRow; } set { uiState.onCreateRow = value; } }
        public bool autoExpandRows { get { return uiState.autoExpandRows; } set { uiState.autoExpandRows = value; } }

        public TimelineUIState.conversionDelegate dimension2Pixel { get { return uiState.dimension2Pixel; } set { uiState.dimension2Pixel = value; } }
        public TimelineUIState.conversionDelegate pixel2Dimension { get { return uiState.pixel2Dimension; } set { uiState.pixel2Dimension = value; } }
        public TimelineUIState.conversionDelegate seconds2Dimension { get { return uiState.seconds2Dimension; } set { uiState.seconds2Dimension = value; } }
        public TimelineUIState.conversionDelegate dimension2Seconds { get { return uiState.dimension2Seconds; } set { uiState.dimension2Seconds = value; } }

        public System.Action OnTimeBarClick { get { return uiState.OnTimeBarClick; } set { uiState.OnTimeBarClick = value; } }

        public bool gridsArrangedByTime { get { return uiState.gridsArrangedByTime; } set { uiState.gridsArrangedByTime = value; } }

        public TimelineUIState.OnCreateByDragToRowAreaDelegate onCreateByDragToRowArea { get { return uiState.onCreateThroughDragToRowArea; } set { uiState.onCreateThroughDragToRowArea = value; } }


        public bool leftResizeShiftsInternalTime = false;

        public System.Func<bool> showEndTime;

        public System.Action<TimelineDataItem> onDeleteDataItem { get { return uiState.onDeleteDataItem; } set { uiState.onDeleteDataItem = value; } }

        public TimelineEditor(EditorWindow parentWindow, TimeLineDataSource dataSource)
        {
            uiState = new TimelineUIState(parentWindow,dataSource);
        }

        public void Draw(Rect position)
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), uiState.style.mainBackground);
            EditorGUI.BeginDisabledGroup(!uiState.dataSource.editable);
            DrawTopBar(position);

            DrawLeftPane(position);
            DrawDefaultBPMAndTS();
            DrawBPMAndTSArea(position);
            DrawRowArea(position);
            DrawBottomBar(position);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawLeftPane(Rect position)
        {
            float topBarHeight = calculateTopBarHeight != null ? calculateTopBarHeight.Invoke(uiState) : 0f;
            float bottomBarHeight = calculateBottomBarHeight != null ? calculateBottomBarHeight.Invoke(uiState) : 0f;
            float leftBarWidth = calculateLeftBarHeight != null ? calculateLeftBarHeight.Invoke(uiState) : 0f;
            Rect leftBarRect = new Rect(0, topBarHeight + uiState.bpmAreaHeight, leftBarWidth, position.height - topBarHeight - bottomBarHeight - uiState.bpmAreaHeight);
            GUI.BeginGroup(leftBarRect);

            leftBarRect.x = 0;
            leftBarRect.y = -uiState.scrollPosition.y;

            onDrawLeftBar?.Invoke(leftBarRect, uiState);
            GUI.EndGroup();
        }

        private void DrawTopBar(Rect position)
        {
            float topBarHeight = calculateTopBarHeight != null ? calculateTopBarHeight.Invoke(uiState) : 0f;
            Rect topBarPosition = new Rect(0, 0, position.width, topBarHeight);
            onDrawTopBar?.Invoke(topBarPosition, uiState);
        }
        private void DrawBottomBar(Rect position)
        {
            float bottomBarHeight = calculateBottomBarHeight != null ? calculateBottomBarHeight.Invoke(uiState) : 0f;
            Rect bottomBarPosition = new Rect(0, position.height - bottomBarHeight, position.width, bottomBarHeight);
            onDrawBottomBar?.Invoke(bottomBarPosition, uiState);
        }

        private void DrawDefaultBPMAndTS()
        {
            float topBarHeight = calculateTopBarHeight != null ? calculateTopBarHeight.Invoke(uiState) : 0f;
            float leftBarWidth = calculateLeftBarHeight != null ? calculateLeftBarHeight.Invoke(uiState) : 0f;
            float bpmHeight = uiState.bpmAreaHeight;

            TimeSignatureDataItem defaultTimeSignature = uiState.dataSource.GetDefaultTimeSignature();
            BPMDataItem defaultBPM = uiState.dataSource.GetDefaultBPM();

            Rect drawArea = new Rect(0f, topBarHeight, leftBarWidth, bpmHeight);
            GUI.BeginGroup(drawArea);

            EditorGUI.BeginChangeCheck();

            //Drawing BPM
            EditorGUIUtility.labelWidth = 80f;
            Rect defaultBPMLabelRect = new Rect(2f*EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.standardVerticalSpacing+3, leftBarWidth - 2f*28f, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(defaultBPMLabelRect, "Start BPM");

            Rect defaultBPMRect = new Rect(leftBarWidth - 2f* 28f, 4, 2f * 28f, EditorGUIUtility.singleLineHeight);

            int newDefaultBPM = EditorGUI.IntField(defaultBPMRect, (int)defaultBPM.bpm);
            defaultBPM.bpm = Mathf.Clamp(newDefaultBPM, 1, int.MaxValue);

            //Drawing time signature
            EditorGUIUtility.labelWidth = 90f;

            Rect defaultTSLabelRect = new Rect(2f * EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 14, leftBarWidth - 2f * 28f, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(defaultTSLabelRect, "Start Time Sig.");

            Rect defaultTSNumeratorRect = 
                new Rect(leftBarWidth - (28f*2f), 
                    EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 14, 
                    28, 
                    EditorGUIUtility.singleLineHeight);

            int newDefaultTSNumerator = EditorGUI.IntField(defaultTSNumeratorRect, defaultTimeSignature.Numerator);
            defaultTimeSignature.Numerator = Mathf.Clamp(newDefaultTSNumerator, 1, int.MaxValue);

            Rect defaultTSDenominatorRect = new Rect(
                defaultTSNumeratorRect.x + defaultTSNumeratorRect.width , 
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 14, 
                28, 
                EditorGUIUtility.singleLineHeight);
            int newDefaultTSDenominator = EditorGUI.IntField(defaultTSDenominatorRect, defaultTimeSignature.Denominator);

            if (!MidiUIUtils.IsPowerOfTwo((ulong)newDefaultTSDenominator))
                newDefaultTSDenominator = MidiUIUtils.FindNearestPowerOf2(newDefaultTSDenominator);

            if (newDefaultTSDenominator <= 0)
                newDefaultTSDenominator = defaultTimeSignature.Denominator;

            defaultTimeSignature.Denominator = Mathf.Clamp(newDefaultTSDenominator, 1, int.MaxValue);

            if (EditorGUI.EndChangeCheck())
                uiState.dataSource.changed = true;

        

            GUI.EndGroup();

            uiState.dataSource.SetDefaultTimeSignature(defaultTimeSignature);
            uiState.dataSource.SetDefaultBPM(defaultBPM);
        }

        private void DrawRowArea(Rect position)
        {
            float topBarHeight = calculateTopBarHeight != null ? calculateTopBarHeight.Invoke(uiState) : 0f;
            float leftBarWidth = calculateLeftBarHeight != null ? calculateLeftBarHeight.Invoke(uiState) : 0f;
            float bpmHeight = uiState.bpmAreaHeight;
            float bottomBarHeight = calculateBottomBarHeight != null ? calculateBottomBarHeight.Invoke(uiState) : 0f;
            float rowsHeight = CalculateHeightOfRows();
            float rowsWidth = CalculateRowsWidth() + (float)uiState.Dimension2Pixel( uiState.Seconds2Dimension(1f));
            Rect scrollContainer = new Rect(leftBarWidth, topBarHeight + bpmHeight, position.width - leftBarWidth, position.height - topBarHeight - bottomBarHeight - bpmHeight);
            uiState.lastScrollContainerRect = scrollContainer;
            Rect innerContainer = new Rect(0, 0, rowsWidth, rowsHeight);


            uiState.ZoomAndDeleteEvents(scrollContainer);

            uiState.scrollPosition = GUI.BeginScrollView(scrollContainer, uiState.scrollPosition, innerContainer);
            uiState.DoDragEvents(innerContainer);

            //Drawing rows
            List<TimeLineRowDataItem> rows = uiState.dataSource.GetDataRows();
            float rowPosition = 0;
            for (int index = 0; index < rows.Count; index++)
            {
                TimeLineRowDataItem row = rows[index];
                float height = calculateRowHeight != null ? calculateRowHeight.Invoke(uiState, row) : EditorGUIUtility.singleLineHeight;
                Rect rowRect = new Rect(0, rowPosition, innerContainer.width, height);
                rowPosition += height;
                onDrawRow?.Invoke(rowRect, uiState, row, index);
            }

            //Drawing grid lines
            Rect gridRect = new Rect(innerContainer.x, innerContainer.y, innerContainer.width, Mathf.Max(innerContainer.height, uiState.window.position.height));
            TimelineEditorGrid.DrawTempoGrid(gridRect, uiState.dataSource.GetTempoMap(), uiState.zoomLevel, uiState.gridTimeSpan, new Color32(132, 132, 132, 255), 1, false, uiState);


            DrawItems();

            if (ShouldShowEndTime())
                DrawEndTime(scrollContainer, uiState);

            DoCurrentTime(innerContainer);


            DoNoteEvents();
            uiState.ItemCanvasEvents(innerContainer);
            uiState.AddEvent(innerContainer);

            rowPosition = 0;
            for (int index = 0; index < rows.Count; index++)
            {
                TimeLineRowDataItem row = rows[index];
                float height = calculateRowHeight != null ? calculateRowHeight.Invoke(uiState, row) : EditorGUIUtility.singleLineHeight;
                Rect rowRect = new Rect(0, rowPosition, innerContainer.width, height);
                rowPosition += height;
                uiState.DoTrackRightClickEvents(rowRect, index, this);
            }

            GUI.EndScrollView();

        }

        private void DoCurrentTime(Rect containingRect)
        {
            double currentTime = uiState.dataSource.GetCurrentPlaybackTime();
            Rect currentTimeRect = new Rect((float)uiState.Dimension2Pixel(uiState.Seconds2Dimension(currentTime)), 0f, 2f, Mathf.Max(uiState.window.position.height, containingRect.height));
            EditorGUI.DrawRect(currentTimeRect,uiState.style.highlightColor);
            uiState.DoPlaybackTimeEvents(currentTimeRect);

        }

        private void DrawItems() //TODO: consolidate DrawItems and DoNoteEvents
        {
            List<TimeLineRowDataItem> rows = uiState.dataSource.GetDataRows();
            float[] rowPositions = new float[rows.Count];
            float[] rowHeights = new float[rows.Count];
            float rowPosition = 0;

            // precalculating positions 
            for (int index = 0; index < rowPositions.Length; index++)
            {

                rowPositions[index] = rowPosition;
                rowPosition += calculateRowHeight != null ? calculateRowHeight.Invoke(uiState, rows[index]) : EditorGUIUtility.singleLineHeight;
                rowHeights[index] = calculateRowHeight != null ? calculateRowHeight.Invoke(uiState, rows[index]) : EditorGUIUtility.singleLineHeight;
            }


            // drawing items
            List<TimelineDataItem> dataItems = uiState.dataSource.GetTimelineDataItems();

            for (int index = 0; index < dataItems.Count; index++)
            {
                TimelineDataItem dataItem = dataItems[index];

                float startTimePixels = (float)uiState.Dimension2Pixel(dataItem.startTime);
                Rect itemRect = new Rect();
                if (dataItems[index].rangeTypes == TimelineDataItem.TimelineItemType.Momentary )
                    itemRect = new Rect(startTimePixels - 10, rowPositions[dataItem.rowNumber], 20, rowHeights[dataItem.rowNumber]);
                else
                    itemRect = new Rect( (float)uiState.Dimension2Pixel( dataItem.startTime), rowPositions[dataItem.rowNumber], (float)uiState.Dimension2Pixel(dataItem.length), rowHeights[dataItem.rowNumber]);

                bool startIsWithinBounds = itemRect.x - uiState.scrollPosition.x < uiState.window.position.width;
                bool endIsWithinBounds = itemRect.x + itemRect.width > uiState.scrollPosition.x ;
                bool shouldDraw = shouldDrawItem != null ? shouldDrawItem.Invoke(itemRect, uiState, dataItem):true;

                // Stopping all drawing if we are past the end of the window
                if ((startIsWithinBounds || endIsWithinBounds) && shouldDraw)
                {
                    //uiState.DoTimelineDataItemEvents(itemRect, dataItem, this);
                    bool selected = uiState.selectedItems.Contains(dataItem);
                    onDrawDataItem?.Invoke(itemRect, uiState, dataItem, selected);
                }
            }
        }

        private void DoNoteEvents()
        {
            List<TimeLineRowDataItem> rows = uiState.dataSource.GetDataRows();
            float[] rowPositions = new float[rows.Count];
            float[] rowHeights = new float[rows.Count];
            float rowPosition = 0;

            // precalculating positions 
            for (int index = 0; index < rowPositions.Length; index++)
            {

                rowPositions[index] = rowPosition;
                rowPosition += calculateRowHeight != null ? calculateRowHeight.Invoke(uiState, rows[index]) : EditorGUIUtility.singleLineHeight;
                rowHeights[index] = calculateRowHeight != null ? calculateRowHeight.Invoke(uiState, rows[index]) : EditorGUIUtility.singleLineHeight;
            }


            List<TimelineDataItem> dataItems = uiState.dataSource.GetTimelineDataItems();

            for (int index = dataItems.Count -1; index >= 0; index--)
            {
                TimelineDataItem dataItem = dataItems[index];
                Rect itemRect = new Rect();

                float startTimePixels = (float)uiState.Dimension2Pixel(dataItem.startTime);
                if (dataItems[index].rangeTypes == TimelineDataItem.TimelineItemType.Momentary)
                    itemRect = new Rect(startTimePixels - 10, rowPositions[dataItem.rowNumber],  20, rowHeights[dataItem.rowNumber]);
                else if (dataItems[index].rangeTypes == TimelineDataItem.TimelineItemType.Normal)
                    itemRect = new Rect(startTimePixels, rowPositions[dataItem.rowNumber], (float)uiState.Dimension2Pixel(dataItem.length), rowHeights[dataItem.rowNumber]);
                else if (dataItems[index].rangeTypes == TimelineDataItem.TimelineItemType.Ranged)
                    itemRect = new Rect(startTimePixels, 0, (float)uiState.Dimension2Pixel(dataItem.length), 15);



                bool startIsWithinBounds = itemRect.x >= uiState.scrollPosition.x && itemRect.x - uiState.scrollPosition.x < uiState.window.position.width;
                bool endIsWithinBounds = itemRect.x + itemRect.width > uiState.scrollPosition.x && itemRect.x + itemRect.width - uiState.scrollPosition.x < uiState.window.position.width;
                bool shouldDraw = shouldDrawItem != null ? shouldDrawItem.Invoke(itemRect, uiState, dataItem) : true;

                // Stopping all drawing if we are past the end of the window

                if ((startIsWithinBounds || endIsWithinBounds) && shouldDraw)
                {
                    uiState.DoTimelineDataItemEvents(itemRect, index, dataItem, this);
                    //bool selected = uiState.selectedItems.Contains(dataItem);
                    //onDrawDataItem?.Invoke(itemRect, uiState, dataItem, selected);
                }
            }
        }

        private void DrawBPMAndTSArea(Rect position)
        {
            float topBarHeight = calculateTopBarHeight != null ? calculateTopBarHeight.Invoke(uiState) : 0f;
            float leftBarWidth = calculateLeftBarHeight != null ? calculateLeftBarHeight.Invoke(uiState) : 0f;
            float bpmHeight = uiState.bpmAreaHeight;
            float innerWidth = Mathf.Max( CalculateRowsWidth(), position.width - leftBarWidth + uiState.scrollPosition.x);

            Rect containingRect = new Rect(leftBarWidth, topBarHeight, position.width - leftBarWidth, bpmHeight);
            GUI.BeginGroup(containingRect);

            Rect innerRect = new Rect(-uiState.scrollPosition.x, 0, innerWidth, bpmHeight);
            GUI.BeginGroup(innerRect);

            TimelineEditorGrid.DrawMajorMinor(
                innerRect, 
                uiState.dataSource.GetTempoMap(), 
                uiState.zoomLevel, 
                new BarBeatTicksTimeSpan(1), 
                uiState.style.gridLineColor, 
                .8f, new BarBeatTicksTimeSpan(0, 1),
                uiState.style.gridLineColor, 0.61f, 
                true, 
                uiState);

            //Drawing time bar
            Rect timeBarBG = new Rect(0, innerRect.height - 10, innerRect.width, 10);
            Color timeBackgroundColor = uiState.style.endTimeColor;
            timeBackgroundColor.a = .5f;
            EditorGUI.DrawRect(timeBarBG, timeBackgroundColor);

            if (ShouldShowEndTime())
            {
                float timeInPixels = (float)uiState.Dimension2Pixel(uiState.Seconds2Dimension(uiState.dataSource.GetEndTime()));
                EditorGUI.DrawRect(new Rect(0, innerRect.height - 10, timeInPixels + 5, 10), uiState.style.endTimeColor);
            }

            //Drawing time
            float diameter = 12;
            Rect timeCircle = new Rect((float)uiState.Dimension2Pixel(uiState.Seconds2Dimension(uiState.dataSource.GetCurrentPlaybackTime())) - (diameter/2f)+1, innerRect.height - diameter, diameter, diameter);
            NodeEditorGUILayout.DrawPortHandle(timeCircle, uiState.style.highlightColor, uiState.style.highlightColor);
            uiState.DoPlaybackTimeEvents(timeCircle);

            //drawing tempos
            EditorGUIUtility.labelWidth = 30f;
            List<BPMDataItem> bpms = uiState.dataSource.GetBPMItems();
            foreach (BPMDataItem tempo in bpms)
            {
                double time = tempo.time;

                if (uiState.gridsArrangedByTime)
                    time = uiState.Dimension2Pixel(uiState.Seconds2Dimension(TimeConverter.ConvertTo<MetricTimeSpan>((long)time, uiState.dataSource.GetTempoMap()).TotalMicroseconds / 1000000f));

                long bpm = tempo.bpm;


                Rect beatMarkerDragRect = new Rect( (float)(time / uiState.zoomLevel) - 7, 4, EditorGUIUtility.singleLineHeight, 26);
                Rect bpmBackground = new Rect(beatMarkerDragRect.x, beatMarkerDragRect.y - 3, 91, 26);

                Color guiColor = GUI.color;
                GUI.color = uiState.style.primaryTimelineElementColor;

                if (uiState.selectedTempo == tempo)
                    GUI.color = uiState.style.secondaryTimelineElementColor;

                GUI.DrawTexture(bpmBackground, uiState.timeSignatureBackground);

                uiState.DoBPMInput(tempo, beatMarkerDragRect);

                GUI.color = guiColor;

                Rect labelRect = new Rect(beatMarkerDragRect.x + 15 + EditorGUIUtility.standardVerticalSpacing, 4, 63f, EditorGUIUtility.singleLineHeight);

                string nextControlName = "bpmField" + tempo.time +""+ tempo.bpm;
                GUI.SetNextControlName(nextControlName);
                int newBPM = EditorGUI.IntField(labelRect, new GUIContent(""), (int)bpm);
                newBPM = Mathf.Clamp(newBPM, 1, int.MaxValue);
                tempo.bpm = newBPM;
                
                if (GUI.GetNameOfFocusedControl() == nextControlName)
                {
                    uiState.selectedTempo = tempo;
                    uiState.selectedTimeSignature = null;
                }

                Rect handleRect = new Rect(bpmBackground.x+7, bpmBackground.y+3, 2,17);
                EditorGUI.DrawRect(handleRect, uiState.style.dragAreaColor);

                //tempo.Value = Tempo.FromBeatsPerMinute(newBPM);
                //uiState.tempoMapManager.SetTempo(tempo.Time, Tempo.FromBeatsPerMinute(newBPM));
            }
            uiState.dataSource.SetBPMItems(bpms);

            
            List<TimeSignatureDataItem> timeSignatures = uiState.dataSource.GetTimeSignatureItems();
            foreach (TimeSignatureDataItem ts in timeSignatures)
            {
                double time = ts.time;

                if (uiState.gridsArrangedByTime)
                    time = uiState.Dimension2Pixel(uiState.Seconds2Dimension(TimeConverter.ConvertTo<MetricTimeSpan>((long)time, uiState.dataSource.GetTempoMap()).TotalMicroseconds / 1000000f));

                int numerator = ts.Numerator;
                int denominator = ts.Denominator;
                Rect tsMarkerDragRect = new Rect((float)(time / uiState.zoomLevel) - 7, 32, EditorGUIUtility.singleLineHeight, 26);

                Rect tsBackground = new Rect(tsMarkerDragRect.x, tsMarkerDragRect.y - 3, 91, 26);


                uiState.DoTimeSignatureInput(ts, tsMarkerDragRect);

                Color guiColor = GUI.color;
                GUI.color = uiState.style.primaryTimelineElementColor;

                if (uiState.selectedTimeSignature == ts)
                    GUI.color = uiState.style.secondaryTimelineElementColor;
                GUI.DrawTexture(tsBackground, uiState.timeSignatureBackground);

                GUI.color = guiColor;

                string numeratorControlName = "tsNumerator" + ts.time + "" + ts.Numerator + "" + ts.Denominator;

                Rect numeratorRect = new Rect(tsMarkerDragRect.x + 15 + EditorGUIUtility.standardVerticalSpacing, tsMarkerDragRect.y, 30f, EditorGUIUtility.singleLineHeight);

                GUI.SetNextControlName(numeratorControlName);
                int newNumerator = EditorGUI.IntField(numeratorRect, (int)numerator);

                if (newNumerator <= 0)
                    newNumerator = numerator;

            
                Rect denominatorRect = new Rect(numeratorRect.x + numeratorRect.width + EditorGUIUtility.standardVerticalSpacing, numeratorRect.y, numeratorRect.width, numeratorRect.height);

                string denominatorControlName = "tsDenominator" + ts.time + "" + ts.Numerator + "" + ts.Denominator;
                GUI.SetNextControlName(denominatorControlName);
                int newDenominator = EditorGUI.IntField(denominatorRect, (int)denominator);

                
                if (GUI.GetNameOfFocusedControl() == numeratorControlName || GUI.GetNameOfFocusedControl() == denominatorControlName)
                {
                    uiState.selectedTimeSignature = ts;
                    uiState.selectedTempo = null;
                }


                if (!MidiUIUtils.IsPowerOfTwo((ulong)newDenominator))
                    newDenominator = MidiUIUtils.FindNearestPowerOf2(newDenominator);

                if (newDenominator <= 0)
                    newDenominator = denominator;

                ts.Numerator = newNumerator;
                ts.Denominator = newDenominator;
                //ts.Value = new TimeSignature(newNumerator, newDenominator);
                //uiState.tempoMapManager.SetTimeSignature(ts.Time, new TimeSignature(newNumerator, newDenominator));

                Rect handleRect = new Rect(tsBackground.x + 7, tsBackground.y + 3, 2, 17);
                EditorGUI.DrawRect(handleRect, uiState.style.dragAreaColor);
            }
            uiState.dataSource.SetTimeSignatureChanges(timeSignatures);

            uiState.AddBPMAndTSEvents(innerRect);
            uiState.DeleteBPMAndTSEvents();
            uiState.BPMAndTSDeselectionEvents(innerRect);
            
            GUI.EndGroup();
            GUI.EndGroup();
        }

        private float CalculateHeightOfRows()
        {
            List<TimeLineRowDataItem> rows = uiState.dataSource.GetDataRows();
            float height = 0;
            for (int index = 0; index < rows.Count; index++)
            {
                TimeLineRowDataItem row = rows[index];
                height += calculateRowHeight != null ? calculateRowHeight.Invoke(uiState,row) : EditorGUIUtility.singleLineHeight;
            }
            return height;
        }

        private float CalculateRowsWidth()
        {
            TimelineDataItem lastItem = null;

            foreach(TimelineDataItem item in uiState.dataSource.GetTimelineDataItems())
            {
                if (lastItem == null || lastItem.startTime + lastItem.length < item.startTime + item.length)
                    lastItem = item;
            }

            double endTime = uiState.Seconds2Dimension( uiState.dataSource.GetEndTime());


            if (lastItem != null && lastItem.startTime + lastItem.length > endTime)
                endTime = lastItem.startTime + lastItem.length;

            endTime = uiState.Dimension2Pixel(endTime);

            if (uiState.window.position.width > endTime)
                endTime = uiState.window.position.width;
            return (float)endTime;
        }

        public static void DrawEndTime(Rect drawArea, TimelineUIState uiState)
        {
            Rect line = new Rect( (float)uiState.Dimension2Pixel( uiState.Seconds2Dimension( uiState.dataSource.GetEndTime())), uiState.scrollPosition.y, 5, drawArea.height);
            EditorGUI.DrawRect(line, uiState.style.endTimeColor);

        

            Rect highlightRect = new Rect(line.x, line.y, 2, line.height);
            EditorGUI.DrawRect(highlightRect, uiState.style.endTimeHighlightColor);

            Rect selectionRect = new Rect(line.x - 5, line.y, line.width + 10, line.height);

            Rect squareBitRect = new Rect(line.x, line.y + (line.height / 2f) - 25f, 10f, 50f);
            EditorGUI.DrawRect(squareBitRect, uiState.style.endTimeColor);

            uiState.DoEndTimeEvents(selectionRect);
        }

        public bool ShouldShowEndTime()
        {
            if (showEndTime != null)
                return showEndTime.Invoke();
            return true;

        }
    }
}
