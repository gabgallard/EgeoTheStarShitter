using System.Collections.Generic;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using ABXY.Layers.Runtime.Timeline;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor
{
    public static class TimelineEditorUtils
    {

        public static double GetNearestSnapPointOnGrid(double time, bool Right, TimelineUIState uiState, bool inTimeSpace)
        {
            time = TimeIn(time, uiState, inTimeSpace);

            SteppedGrid quarterNoteGrid = new SteppedGrid(uiState.gridTimeSpan);

            IEnumerable<long> gridTimes = quarterNoteGrid.GetTimes(uiState.dataSource.GetTempoMap());
            using (var sequenceEnum = gridTimes.GetEnumerator())
            {
                long lastTime = 0;
                while (sequenceEnum.MoveNext())
                {
                    if (sequenceEnum.Current == time)
                    {
                        if (Right)
                        {
                            sequenceEnum.MoveNext();
                            return TimeOut(sequenceEnum.Current, uiState, inTimeSpace);
                        }
                        else
                            return TimeOut(lastTime, uiState, inTimeSpace);
                    }
                    else if (sequenceEnum.Current > time)
                    {
                        return TimeOut(Right ? sequenceEnum.Current : lastTime, uiState, inTimeSpace);
                    }
                    lastTime = sequenceEnum.Current;
                }
            }
        
            return TimeOut( time, uiState, inTimeSpace);
        }

        private static double TimeIn(double time, TimelineUIState uiState, bool inTimeSpace)
        {
            if (inTimeSpace)
                return TimeConverter.ConvertFrom(new MetricTimeSpan((long)(time * 1000000)), uiState.dataSource.GetTempoMap());
            return time;
        }

        private static double TimeOut(double time, TimelineUIState uiState, bool inTimeSpace)
        {
            if (inTimeSpace)
                return TimeConverter.ConvertTo<MetricTimeSpan>((long)time, uiState.dataSource.GetTempoMap()).TotalMicroseconds / 1000000.0;
            return time;
        }

        public static double GetNearestSnapPointOnDataItems(double time, bool Right, TimelineUIState uiState, List<TimelineDataItem> ignoreList)
        {
            double lastTime = Right?double.MaxValue:0;
        

            List<TimelineDataItem> dataItems = uiState.dataSource.GetTimelineDataItems();
            
            
            foreach(TimelineDataItem item in dataItems)
            {
                if (ignoreList.Contains(item))
                    continue;

                if (Right)
                {
                    double distanceToStartTime = item.startTime - time;
                    double distanceToEndTime =  item.startTime + item.length - time;
                    double distanceToLastTime = lastTime - time;
                    if (item.startTime > time && distanceToStartTime < distanceToLastTime && distanceToStartTime != 0)
                    {
                        lastTime = item.startTime;
                        distanceToLastTime = lastTime - time;
                    }

                    if (item.startTime + item.length > time && distanceToEndTime < distanceToLastTime && distanceToEndTime != 0)
                    {
                        lastTime = item.startTime + item.length;
                        distanceToLastTime = lastTime - time;
                    }
                }
                else
                {
                    double distanceToStartTime = time - item.startTime;
                    double distanceToEndTime = time - item.startTime + item.length;
                    double distanceToLastTime = time - lastTime;
                    if (item.startTime < time && distanceToStartTime < distanceToLastTime && distanceToStartTime != 0)
                    {
                        lastTime = item.startTime;
                        distanceToLastTime = time - lastTime;
                    }

                    if (item.startTime + item.length < time && distanceToEndTime < distanceToLastTime && distanceToEndTime != 0)
                    {
                        lastTime = item.startTime + item.length;
                        distanceToLastTime = time - lastTime;
                    }


                }
            }
            
        
            return lastTime;
        }



        public static double GetNearestTimelineSnapPoint(double time, bool Right, TimelineUIState uiState, TimelineDataItem ignoreItem, bool inTimeSpace)
        {
            return GetNearestTimelineSnapPoint(time, Right, uiState, new List<TimelineDataItem>(new TimelineDataItem[] { ignoreItem }),inTimeSpace);
        }

        public static double GetNearestTimelineSnapPoint(double time, bool Right, TimelineUIState uiState, List<TimelineDataItem> ignoreList, bool inTimeSpace)
        {
            double gridPoint = GetNearestSnapPointOnGrid(time, Right, uiState, inTimeSpace);
            double boxPoint = GetNearestSnapPointOnDataItems(time, Right, uiState, ignoreList);
            return Mathf.Abs((float)(time - gridPoint)) < Mathf.Abs((float)(time - boxPoint)) ? gridPoint : boxPoint;
        }

   
        public static float DrawSlider(Rect position, string label, float value, float min, float max, float entryAreaWidth)
        {
            Rect labelRect = new Rect(position.x + EditorGUIUtility.standardVerticalSpacing, 
                position.y, 
                EditorGUIUtility.labelWidth - (2f * EditorGUIUtility.standardVerticalSpacing), 
                position.height);
            Rect valueRect = new Rect(position.x + position.width - entryAreaWidth + EditorGUIUtility.standardVerticalSpacing, 
                position.y, 
                entryAreaWidth - (2f * EditorGUIUtility.standardVerticalSpacing), position.height);
            Rect sliderRect = new Rect(position.x + labelRect.width + EditorGUIUtility.standardVerticalSpacing,
                position.y, 
                position.width - labelRect.width - valueRect.width - (3f* EditorGUIUtility.standardVerticalSpacing), position.height);
            EditorGUI.LabelField(labelRect, label);
            value = GUI.HorizontalSlider(sliderRect, value, min, max);
            value = EditorGUI.FloatField(valueRect, value);
            value = Mathf.Clamp(value, min, max);
            return value;
        }


    }
}
