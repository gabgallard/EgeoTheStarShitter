using ABXY.Layers.Editor.Analyzer.Style;
using ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlaybackAnalyzer : LayersAnalyzer
{
    private Dictionary<System.Guid, EventOnGraph> eventsInGraph = new Dictionary<Guid, EventOnGraph>();
    private List<EventOnGraph> eventsList = new List<EventOnGraph>();

    private CombinedAnalyzerStyle style = new CombinedAnalyzerStyle();

    private float speed = 10f;

    private EventOnGraph selectedEvent = null;

    public PlaybackAnalyzer(float height) : base(height)
    {
    }

    public override void DrawData(Rect position)
    {
        //drawing main blocks
        DrawData(position, eventsList.Where(x => !x.momentary).ToList());

        //Drawing momentary
        DrawData(position, eventsList.Where(x => x.momentary).ToList());
    }

    private void DrawData(Rect position, List<EventOnGraph> eventList)
    {

        double currentTime = AudioSettings.dspTime;

        for (int index = 0; index < eventList.Count; index++)
        {
            bool selected = eventList[index] == selectedEvent;

            Rect boxRect = CalculateEventRect(position.width, currentTime, eventList[index]);

            if (eventList[index].momentary)
            {
                

                Color guiColor = GUI.color;

                GUI.color = Color.black;
                GUI.DrawTexture(boxRect, style.eventItemBackground);


                Rect innerRect = new Rect(boxRect.x + 1, boxRect.y + 1, boxRect.width - 2, boxRect.height - 2);
                GUI.color = selected? style.secondaryTimelineElementColor:style.primaryTimelineElementColor;

                if (eventList[index].startEvent.wasLate)
                    GUI.color = style.errorColor;

                GUI.DrawTexture(innerRect, style.eventItemBackground);

                GUI.color = guiColor;

                // Doing label
                if (selectedEvent != eventList[index])
                {
                    string label = AnalyzerEditorUtilities.GetTitle(eventList[index]);
                    float labelWidth = EditorStyles.label.CalcSize(new GUIContent(label)).x;
                    Rect labelRect = new Rect(boxRect.x + (boxRect.width / 2f) - (labelWidth / 2f), boxRect.y + boxRect.height, labelWidth, EditorGUIUtility.singleLineHeight);
                    Rect outlineRect = new Rect(labelRect.x - 1, labelRect.y - 1, labelRect.width + 2f, labelRect.height + 2f);

                    EditorGUI.DrawRect(outlineRect, Color.black);
                    EditorGUI.DrawRect(labelRect, selected ? style.secondaryTimelineElementColor : style.primaryTimelineElementColor);

                    EditorGUI.LabelField(labelRect, label);

                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && boxRect.Contains(Event.current.mousePosition))
                {
                    selectedEvent = eventList[index];
                    Event.current.Use();

                }

            }
            else
            {
                

                EditorGUI.DrawRect(boxRect, Color.black);

                Rect innerRect = new Rect(boxRect.x + 1, boxRect.y + 1, boxRect.width - 2f, boxRect.height - 2f);

                Color drawColor = selected ? style.secondaryTimelineElementColor : style.primaryTimelineElementColor;

                if (eventList[index].startEvent.wasLate)
                    drawColor = style.errorColor;

                EditorGUI.DrawRect(innerRect, drawColor);

                EditorGUI.LabelField(boxRect, eventList[index].prettyName);


                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && boxRect.Contains(Event.current.mousePosition))
                {
                    selectedEvent = eventList[index];
                    Event.current.Use();
                }
            }

            
        }

        if (selectedEvent != null)
        {
            Rect selectedEventRect = CalculateEventRect(position.width, currentTime, selectedEvent);
            AnalyzerEditorUtilities.DrawLayersAnalyzerEvent(new Vector2(selectedEventRect.x + (selectedEventRect.width / 2f), selectedEventRect.y + selectedEventRect.height),selectedEvent);
        }

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            selectedEvent = null;
            //Event.current.Use();
        }
    }

    private Rect CalculateEventRect(float windowWidth, double currentTime, EventOnGraph eog)
    {
        return CalculateEventRect(windowWidth, currentTime, eog.momentary, eog.startEvent.time, eog.endEvent != null ? eog.endEvent.time : currentTime, eog.row);
    }

    private Rect CalculateEventRect(float windowWidth, double currentTime, bool momentary, double startTime, double endTime, int row)
    {
        if (momentary)
        {
            double boxStartTime = startTime;

            double boxStartPixels = windowWidth - ((currentTime - boxStartTime) * speed);

            float iconSize = 20f;

            return new Rect((float)(boxStartPixels) - (iconSize / 2f), row * (25 * EditorGUIUtility.standardVerticalSpacing), iconSize, iconSize);
        }
        else
        {
            double boxStartTime = startTime;

            double boxStartPixels = windowWidth - ((currentTime - boxStartTime) * speed);

            double boxEndPixels = windowWidth - ((currentTime - endTime) * speed);

            return new Rect((float)(boxStartPixels), row * (25 * EditorGUIUtility.standardVerticalSpacing), (float)(boxEndPixels - boxStartPixels), 50);
        }
    }


    public override void ReceiveEvent(LayersAnalyzerEvent levent)
    {
        switch (levent.eventType)
        {
            case LayersAnalyzerEvent.LayersEventTypes.AudioScheduled:
                StartEvent(levent, true);
                break;
            case LayersAnalyzerEvent.LayersEventTypes.AudioStarted:
                StartEvent(levent);
                break;
            case LayersAnalyzerEvent.LayersEventTypes.AudioFinished:
                FinishEvent(levent);
                break;
            case LayersAnalyzerEvent.LayersEventTypes.AudioEnded:
                FinishEvent(levent);
                break;
            case LayersAnalyzerEvent.LayersEventTypes.GraphEvent:
                StartEvent(levent, true);
                break;
            case LayersAnalyzerEvent.LayersEventTypes.EndAll:
                StartEvent(levent, true);
                break;
            case LayersAnalyzerEvent.LayersEventTypes.NodeEntered:
                StartEvent(levent);
                break;
            case LayersAnalyzerEvent.LayersEventTypes.NodeExited:
                FinishEvent(levent);
                break;
            case LayersAnalyzerEvent.LayersEventTypes.SoundGraphCreated:
                break;
            case LayersAnalyzerEvent.LayersEventTypes.SoundGraphDestroyed:
                break;
            case LayersAnalyzerEvent.LayersEventTypes.SoundGraphPlayerCreated:
                break;
            case LayersAnalyzerEvent.LayersEventTypes.SoundGraphPlayerDestroyed:
                break;
        }
    }

    private void StartEvent(LayersAnalyzerEvent levent, bool momentary = false)
    {
        if (!eventsInGraph.ContainsKey(levent.eventID))
        {
            int row = GetDrawRow(levent.time, momentary);
            EventOnGraph eventOnGraph = new EventOnGraph(levent.eventID, levent, row, momentary);
            eventsInGraph.Add(levent.eventID, eventOnGraph);
            eventsList.Add(eventOnGraph);
            
        }
    }

    private int GetDrawRow(double time, bool momentary )
    {
        return GetDrawRow(0, time, momentary);
    }



    private int GetDrawRow(int startRow, double time, bool incomingEventMomentary)
    {
        Rect incomingRect = new Rect((float)time, 0, 1, 20);
        foreach (EventOnGraph gevent in eventsList)
        {
            if (gevent.row == startRow)
            {
                double startTime = gevent.startEvent.time;

                double endTime = gevent.endEvent != null ? gevent.endEvent.time : startTime;

                Rect elementRect = new Rect((float)startTime, 0, (float)(endTime - startTime), 20);
                elementRect = ExpandMomentaryRect(elementRect);


                if (elementRect.Overlaps(incomingRect))//then won't fit on this row
                    return GetDrawRow(startRow+1, time, incomingEventMomentary);
            }
        }
        return startRow;
    }

    public Vector2 CalculateDimensions()
    {
        double earliestTime = AudioSettings.dspTime;
        int largestRow = 0;
        foreach(EventOnGraph eog in eventsList)
        {
            if (eog.startEvent.time < earliestTime)
                earliestTime = eog.startEvent.time;

            if (eog.row > largestRow)
                largestRow = eog.row;
        }
        Rect position = CalculateEventRect(0f, AudioSettings.dspTime, false, earliestTime, earliestTime + 1, largestRow);
        return new Vector2(-position.x, position.y + position.height);
    }


    private Rect ExpandMomentaryRect(Rect rect)
    {
        return new Rect(rect.x - 1, rect.y, rect.width + 2, rect.height);
    }

    private void FinishEvent(LayersAnalyzerEvent levent)
    {
        EventOnGraph eventOnGraph = null;
        if (eventsInGraph.TryGetValue(levent.eventID, out eventOnGraph))
            eventOnGraph.Finish(levent);
    }

    public void FinishAllEvents()
    {
        foreach(EventOnGraph eog in eventsList)
        {
            if (!eog.finished && !eog.momentary)
                eog.Finish(new LayersAnalyzerEvent(eog.startEvent.eventID, null, null, null, AudioSettings.dspTime, LayersAnalyzerEvent.LayersEventTypes.AudioFinished, ""));
        }
    }

    public class EventOnGraph
    {
        public System.Guid id { get; private set; }
        public LayersAnalyzerEvent startEvent { get; private set; }
        public LayersAnalyzerEvent endEvent { get; private set; }

        public bool finished { get { return (endEvent != null && endEvent.time < AudioSettings.dspTime) || momentary ; } }

        public int row = 0;

        public bool momentary { get; private set; }

        public string prettyName { get { return startEvent.prettyName; } }

        public EventOnGraph(Guid id, LayersAnalyzerEvent startEvent, int row, bool momentary = false)
        {
            this.id = id;
            this.startEvent = startEvent;
            this.row = row;

            this.momentary = momentary;
        }

        public void Finish(LayersAnalyzerEvent endEvent)
        {

            this.endEvent = endEvent;
        }
    }
}
