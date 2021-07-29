using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ABXY.Layers.Runtime.Sound_graph_players;
using ABXY.Layers.Editor.Timeline_Editor.Variants.Style;
using ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode.Style;
using ABXY.Layers.Runtime;
using ABXY.Layers.Editor.Analyzer.Style;

public class LayersAnalyzerWindow : EditorWindow
{



    [MenuItem("Window/Analysis/Layers Analyzer")]
    public static void ShowAnalyzer()
    {
        LayersAnalyzerWindow window = EditorWindow.GetWindow<LayersAnalyzerWindow>();
        window.titleContent = new GUIContent("Layers Analyzer", EditorGUIUtility.IconContent("d_UnityEditor.ProfilerWindow").image);
    }

    private CombinedAnalyzerStyle timelineStyle = new CombinedAnalyzerStyle();

    private Dictionary<string, SoundGraphPlayerEntry> players = new Dictionary<string, SoundGraphPlayerEntry>();

    private string selectedPlayer = "";

    private bool recording = false;


    private void OnEnable()
    {
        LayersEventBus.onEventRaised -= ReceiveEvent;
        LayersEventBus.onEventRaised += ReceiveEvent;


    }

    private void OnDisable()
    {
        LayersEventBus.onEventRaised -= ReceiveEvent;
    }

    private void OnGUI()
    {
        //Rect headerRect = new Rect(0, 0, position.width, 40);
        //DrawHeader(headerRect);

        Rect drawArea = new Rect(0, 0, position.width, position.height);
        DrawAnalyzers(drawArea);
        DrawSideBar(drawArea);
        Repaint();
    }

    Vector2 analyzerScrollPosition = Vector2.zero;
    private void DrawAnalyzers(Rect drawArea)
    {
        Rect headerRect = new Rect(drawArea.x + sidebarSize, drawArea.y, drawArea.width - sidebarSize,30);
        EditorGUI.DrawRect(headerRect, timelineStyle.headerColor);
        EditorGUI.LabelField(new Rect(headerRect.x + EditorGUIUtility.singleLineHeight, headerRect.y, 100, headerRect.height), "Events", timelineStyle.headerLabelStyle);

        GUILayout.BeginArea(headerRect);
        GUILayout.Space(2);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUI.BeginChangeCheck();
        recording = GUILayout.Toggle(recording, new GUIContent("Record", EditorGUIUtility.IconContent("d_UnityEditor.ProfilerWindow").image), "Button");
        if (EditorGUI.EndChangeCheck() && !recording)
            FinishAllEvents();

        if (GUILayout.Button("Clear"))
        {
            players.Clear();
        }
        EditorGUILayout.Space();

        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        Rect analyzerClipArea = new Rect(drawArea.x + sidebarSize, drawArea.y + 30, drawArea.width - sidebarSize, drawArea.height - 30);
        GUILayout.BeginArea(analyzerClipArea);




        if (selectedPlayer != null && players.ContainsKey(selectedPlayer))
        {
            Rect analyzerArea = new Rect(0, 0, analyzerClipArea.width , analyzerClipArea.height);

            Vector2 innerDimensions = players[selectedPlayer].playbackAnalyzer.CalculateDimensions();
            innerDimensions.x = Mathf.Max(innerDimensions.x + 100, analyzerArea.width) ;


            analyzerScrollPosition = GUI.BeginScrollView(analyzerArea, analyzerScrollPosition, new Rect(Vector2.zero, innerDimensions));

            if (Mathf.Abs(innerDimensions.x - analyzerScrollPosition.x - analyzerArea.width) < 10)
                analyzerScrollPosition.x = innerDimensions.x - analyzerArea.width;

            players[selectedPlayer].playbackAnalyzer.DrawData(new Rect(0, 0, innerDimensions.x, innerDimensions.y));

            GUI.EndScrollView();
            
        }


        GUILayout.EndArea();
    }

    private float sidebarSize = 200f;
    private float dragAreaWidth = 4f;
    private float listItemHeight = 2f*EditorGUIUtility.singleLineHeight + 4f*EditorGUIUtility.standardVerticalSpacing;
    Vector2 sideBarScrollPosition = Vector2.zero;
    private void DrawSideBar(Rect drawArea)
    {
        //header 
        Rect headerRect = new Rect(drawArea.x, drawArea.y, sidebarSize, 30);
        EditorGUI.DrawRect(headerRect, timelineStyle.headerColor);
        EditorGUI.LabelField(headerRect, "Running SoundGraphs", timelineStyle.centeredHeaderLabelStyle);

        Rect listArea = new Rect(drawArea.x, drawArea.y + 30, sidebarSize, drawArea.height - 30);
        EditorGUI.DrawRect(listArea, timelineStyle.mainBackground);

        GUILayout.BeginArea(listArea);

        float calculatedHeight = players.Count * listItemHeight;
        sideBarScrollPosition = GUI.BeginScrollView(new Rect(0, 0, listArea.width, listArea.height), sideBarScrollPosition, new Rect(0, 0, listArea.width-20, calculatedHeight));

        float currentY = 0;
        foreach(SoundGraphPlayerEntry player in players.Values)
        {
            Rect drawRect = new Rect(0, currentY, sidebarSize, listItemHeight);

            bool hovered = drawRect.Contains(Event.current.mousePosition);

            Color targetColor = selectedPlayer == player.soundGraphID || hovered ? timelineStyle.secondaryTimelineElementColor : timelineStyle.primaryTimelineElementColor;

            targetColor = Color.Lerp(timelineStyle.eventHighlightColor, targetColor, Mathf.Clamp01(2f*(float)(AudioSettings.dspTime - player.timeOfLastEvent)));

            EditorGUI.DrawRect(drawRect, targetColor);

            if (hovered && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                selectedPlayer = player.soundGraphID;


            Rect soundGraphNameRect = new Rect(2f*EditorGUIUtility.standardVerticalSpacing, currentY + EditorGUIUtility.standardVerticalSpacing, sidebarSize, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(soundGraphNameRect, player.soundGraphName, EditorStyles.boldLabel);

            Rect soundGraphPlayerNameRect = new Rect(2f * EditorGUIUtility.standardVerticalSpacing, currentY + EditorGUIUtility.singleLineHeight + 2f*EditorGUIUtility.standardVerticalSpacing, sidebarSize, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(soundGraphPlayerNameRect, player.playerName);
            currentY += listItemHeight;
        }

        GUI.EndScrollView();

        GUILayout.EndArea();

        //drag line
        Rect dragArea = new Rect(drawArea.x + sidebarSize - (dragAreaWidth / 2f), drawArea.y, dragAreaWidth, drawArea.height);
        EditorGUI.DrawRect(dragArea, timelineStyle.dragAreaColor);
    }

    private void ReceiveEvent(LayersAnalyzerEvent levent)
    {
        if (!recording)
            return;
        if (levent.soundGraphObject == null) return;

        if (!players.ContainsKey(levent.soundGraphObject.graphID))
        {
            players.Add(levent.soundGraphObject.graphID, new SoundGraphPlayerEntry(levent.soundGraphObject.graphID, levent.playerObject, levent.playerName, levent.soundGraphObject.name));
        }

        players[levent.soundGraphObject.graphID].ReceiveEvent(levent);
    }

    private void FinishAllEvents()
    {
        foreach (SoundGraphPlayerEntry player in players.Values)
        {
            player.playbackAnalyzer.FinishAllEvents();
        }
    }

    private class SoundGraphPlayerEntry
    {
        public string soundGraphID;
        public SoundGraphPlayer playerObject;
        public string playerName;
        public string soundGraphName;

        public double timeOfLastEvent;

        public PlaybackAnalyzer playbackAnalyzer = new PlaybackAnalyzer(0);

        public SoundGraphPlayerEntry(string soundGraphID, SoundGraphPlayer playerObject, string playerName, string soundGraphName)
        {
            this.soundGraphID = soundGraphID;
            this.playerObject = playerObject;
            this.playerName = playerName;
            this.soundGraphName = soundGraphName;
        }

        public void ReceiveEvent(LayersAnalyzerEvent levent)
        {
            playbackAnalyzer.ReceiveEvent(levent);
            timeOfLastEvent = AudioSettings.dspTime;
        }
    }
}
