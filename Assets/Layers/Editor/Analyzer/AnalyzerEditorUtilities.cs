using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnalyzerEditorUtilities
{
    public static void DrawLayersAnalyzerEvent(Vector2 position, PlaybackAnalyzer.EventOnGraph eog)
    {
        float width = 300f;
        Rect eventWindowRect = new Rect(position.x - (width/2f), position.y, width, CalcLayersAnalyzerEventHeight(eog));
        GUI.Box(eventWindowRect, GetTitle(eog), GUI.skin.window);
        GUILayout.BeginArea(eventWindowRect);

        EditorGUILayout.GetControlRect();

        switch (eog.startEvent.eventType)
        {
            case LayersAnalyzerEvent.LayersEventTypes.AudioScheduled:
                EditorGUILayout.LabelField(eog.prettyName);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.DoubleField("Start Time", eog.startEvent.time);
                EditorGUI.EndDisabledGroup();
                break;
            case LayersAnalyzerEvent.LayersEventTypes.AudioStarted:
                EditorGUILayout.LabelField(eog.prettyName);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.DoubleField("Start Time", eog.startEvent.time);
                if (eog.endEvent != null)
                    EditorGUILayout.DoubleField("End Time", eog.endEvent.time);
                else
                    EditorGUILayout.LabelField("End Time: Playback Not Finished");
                EditorGUI.EndDisabledGroup();
                break;
            case LayersAnalyzerEvent.LayersEventTypes.AudioEnded:
                EditorGUILayout.LabelField(eog.prettyName);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.DoubleField("Time", eog.endEvent.time);
                EditorGUI.EndDisabledGroup();
                break;
            case LayersAnalyzerEvent.LayersEventTypes.GraphEvent:
                EditorGUILayout.LabelField(eog.prettyName);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.DoubleField("Start Time", eog.startEvent.time);
                EditorGUI.EndDisabledGroup();
                break;
            case LayersAnalyzerEvent.LayersEventTypes.EndAll:
                EditorGUILayout.LabelField(eog.prettyName);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.DoubleField("Start Time", eog.startEvent.time);
                EditorGUI.EndDisabledGroup();
                break;
            case LayersAnalyzerEvent.LayersEventTypes.NodeEntered:
                break;
            case LayersAnalyzerEvent.LayersEventTypes.NodeExited:
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

        if (eog.startEvent.wasLate)
            EditorGUILayout.LabelField("Started after scheduled time");
        GUILayout.EndArea();
    }

    public static string GetTitle(PlaybackAnalyzer.EventOnGraph eog)
    {
        switch (eog.startEvent.eventType)
        {
            case LayersAnalyzerEvent.LayersEventTypes.AudioScheduled:
                return "Audio Scheduled";
            case LayersAnalyzerEvent.LayersEventTypes.AudioStarted:
                return "Playing Audio";
            case LayersAnalyzerEvent.LayersEventTypes.AudioEnded:
                return "Audio Stopped";
            case LayersAnalyzerEvent.LayersEventTypes.GraphEvent:
                return string.Format(eog.prettyName);
            case LayersAnalyzerEvent.LayersEventTypes.EndAll:
                return "End All";
            case LayersAnalyzerEvent.LayersEventTypes.NodeEntered:
                return string.Format("Node {0} entered", eog.startEvent.nodeName);
            case LayersAnalyzerEvent.LayersEventTypes.NodeExited:
                return string.Format("Node {0} exited", eog.startEvent.nodeName);
            case LayersAnalyzerEvent.LayersEventTypes.SoundGraphCreated:
                break;
            case LayersAnalyzerEvent.LayersEventTypes.SoundGraphDestroyed:
                break;
            case LayersAnalyzerEvent.LayersEventTypes.SoundGraphPlayerCreated:
                break;
            case LayersAnalyzerEvent.LayersEventTypes.SoundGraphPlayerDestroyed:
                break;
        }
        return "";
    }

    private static float CalcLayersAnalyzerEventHeight(PlaybackAnalyzer.EventOnGraph eog)
    {
        int lineCount = 0;
        switch (eog.startEvent.eventType)
        {
            case LayersAnalyzerEvent.LayersEventTypes.AudioScheduled:
                lineCount = 2;
                break;
            case LayersAnalyzerEvent.LayersEventTypes.AudioStarted:
                lineCount = 3;
                break;
            case LayersAnalyzerEvent.LayersEventTypes.AudioEnded:
                lineCount = 2;
                break;
            case LayersAnalyzerEvent.LayersEventTypes.GraphEvent:
                lineCount = 2;
                break;
            case LayersAnalyzerEvent.LayersEventTypes.EndAll:
                lineCount = 2;
                break;
            case LayersAnalyzerEvent.LayersEventTypes.NodeEntered:
                break;
            case LayersAnalyzerEvent.LayersEventTypes.NodeExited:
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
        lineCount++;
        if (eog.startEvent.wasLate)
            lineCount++;
        return lineCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }
}
