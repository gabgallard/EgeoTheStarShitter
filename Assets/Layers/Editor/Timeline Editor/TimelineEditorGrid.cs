using System.Collections.Generic;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor
{
    public static class TimelineEditorGrid
    {
        private static void DrawNumber(long number,  Rect lineRect)
        {

            EditorGUI.LabelField(new Rect(lineRect.x + 2f * EditorGUIUtility.standardVerticalSpacing,
                lineRect.y + lineRect.height - EditorGUIUtility.singleLineHeight,
                80f,
                EditorGUIUtility.singleLineHeight), number.ToString());

        }



        public static void DrawMajorMinor(Rect area, TempoMap tempoMap, float zoomLevel, ITimeSpan majorInterval, Color majorColor, float majorHeightPercent, ITimeSpan minorInterval, Color minorColor, float minorHeightPercent, bool showNumbers, TimelineUIState uiState)
        {
            // Drawing minor note grid
            DrawTempoGrid(area, tempoMap, zoomLevel, minorInterval, minorColor, minorHeightPercent, showNumbers, uiState);


            //Drawing major Grid
            DrawTempoGrid(area, tempoMap, zoomLevel, majorInterval, majorColor, majorHeightPercent, showNumbers, uiState);


        }

        public static void DrawTempoGrid(Rect area, TempoMap tempoMap, float zoomLevel, ITimeSpan interval, Color color, float heightPercent, bool showNumbers, TimelineUIState uiState)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            // Drawing minor note grid
            SteppedGrid quarterNoteGrid = new SteppedGrid(interval);
            IEnumerable<long> gridTimes = quarterNoteGrid.GetTimes(tempoMap);
            using (var sequenceEnum = gridTimes.GetEnumerator())
            {
                float lastTime = 0f;
                while (sequenceEnum.MoveNext() 
                       && (!uiState.gridsArrangedByTime && sequenceEnum.Current / zoomLevel < area.width)
                       || uiState.gridsArrangedByTime && uiState.Dimension2Pixel(uiState.Seconds2Dimension(TimeConverter.ConvertTo<MetricTimeSpan>(sequenceEnum.Current, tempoMap).TotalMicroseconds / 1000000f)) < area.width)
                {
                    float currentPosition = sequenceEnum.Current / zoomLevel;
                    if (uiState.gridsArrangedByTime)
                        currentPosition = (float)uiState.Dimension2Pixel(uiState.Seconds2Dimension(TimeConverter.ConvertTo<MetricTimeSpan>(sequenceEnum.Current, tempoMap).TotalMicroseconds / 1000000f));

                    if (currentPosition > uiState.scrollPosition.x && currentPosition - uiState.scrollPosition.x < uiState.window.position.width && Mathf.Abs(currentPosition - lastTime) > 15)
                    {
                        Rect gridLineRect = new Rect(currentPosition, area.y + area.height - (area.height * heightPercent), 2f, area.height * heightPercent);
                        EditorGUI.DrawRect(gridLineRect, color);
                        if (showNumbers && Mathf.Abs(currentPosition-lastTime) > 20)
                            DrawNumber(sequenceEnum.Current, tempoMap, gridLineRect);
                    }
                    lastTime = currentPosition;
                }
            }

        }


        private static void DrawNumber(long time, TempoMap tempoMap, Rect lineRect)
        {

            BarBeatFractionTimeSpan timespan = TimeConverter.ConvertTo<BarBeatFractionTimeSpan>(time, tempoMap);

            GUI.Label(new Rect(lineRect.x + 2f * EditorGUIUtility.standardVerticalSpacing,
                lineRect.y + lineRect.height - EditorGUIUtility.singleLineHeight - 10,
                80f,
                EditorGUIUtility.singleLineHeight), timespan.Bars + "." + timespan.Beats);

        }
    }
}
