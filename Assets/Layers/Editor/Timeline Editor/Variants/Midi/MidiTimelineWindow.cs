using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.Timeline_Editor.Variants.Midi.Style;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using Andeart.EditorCoroutines.Unity;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Midi;
using ABXY.Layers.Runtime.Nodes.Playback;
using ABXY.Layers.Runtime.Timeline;
using ABXY.Layers.Runtime.Timeline.Playnode;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Runtime.Nodes;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.Midi
{
    public class MidiTimelineWindow : EditorWindow
    {
        private MidiUtils.channels currentChannel = MidiUtils.channels.AllChannels;
        TimelineEditor editor;
    

        private PlaybackSystem playback;

        [SerializeField]
        private MidiFileAsset midiAsset;

        public static MidiTimelineWindow ShowMIDITimeline(MidiFileAsset midiAsset)
        {
            MidiTimelineWindow midiEditor = CreateInstance<MidiTimelineWindow>();

            midiEditor.midiAsset = midiAsset;
            midiEditor.Setup();
            midiEditor.titleContent = new GUIContent("Midi Editor", Resources.Load<Texture2D>("Symphony/MIDIWindowIcon"));
            midiEditor.Show();
            //midiEditor.uiState = new TimelineUIState(midiEditor, playNode, 1f / 1000f, EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 2f, EditorGUIUtility.singleLineHeight, 250f, timeLine, tracks) ;
            return midiEditor;
        }

        private void Setup()
        {
            MidiDataSource dataSource = new MidiDataSource(midiAsset);
            editor = new TimelineEditor(this, dataSource);
            editor.onDrawTopBar += OnDrawTopBar;
            editor.calculateTopBarHeight += (TimelineUIState uiState) => { return EditorGUIUtility.singleLineHeight; };

            editor.onDrawBottomBar += OnDrawBottomBar;
            editor.calculateBottomBarHeight += (TimelineUIState uiState) => { return 100; };

            editor.onDrawLeftBar += OnDrawLeftBar;
            editor.calculateLeftBarHeight += (TimelineUIState uiState) => { return 150; };

            editor.onDrawRow += OnDrawRow;

            editor.onDrawDataItem += OnDrawItem;
            editor.uiState.OnCreateEvent += (time, row) =>
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("NoteInfo", new MidiData(129 - row, MidiData.MidiChannel.All, 1f));
                currentPreviewSoundgraph.CallEventByID(currentEventID, AudioSettings.dspTime, data,0);
            };

            editor.uiState.OnItemChangeRow += (newRow) => {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("NoteInfo", new MidiData(129 - newRow-1, MidiData.MidiChannel.All, 1f));
                currentPreviewSoundgraph.CallEventByID(currentEventID, AudioSettings.dspTime, data,0);
            };

            editor.pixel2Dimension += (value) => { return value * 1; };
            editor.dimension2Pixel += (value) => { return value / 1; };
            editor.seconds2Dimension += (value) => { return TimeConverter.ConvertFrom(new MetricTimeSpan((long)(value * 1000000)), dataSource.GetTempoMap()); };
            editor.dimension2Seconds += (value) => { return TimeConverter.ConvertTo<MetricTimeSpan>((long)value, dataSource.GetTempoMap()).TotalMicroseconds / 1000000.0; };

            editor.shouldDrawItem += (Rect position, TimelineUIState uiState, TimelineDataItem item) => { return currentChannel == MidiUtils.channels.AllChannels || currentChannel == (item as MidiDataItem).channelNumber; };

            editor.uiState.style = new MIDITimelineStyle();


            playback = new PlaybackSystem(
                () => { return new List<PlaynodeDataItem>(); },
                (routine) => { return new FlowNode.LayersCoroutine(routine, EditorCoroutineService.StartCoroutine(routine)); },
                () => { return 1f; },
                () => { return 0f; },
                () => { return dataSource.GetEndTime(); },
                () => { return midiAsset.name; },
                (name, time, parameters, nodesCalledThisFrame) => {
                    currentPreviewSoundgraph.CallEventByID(currentEventID, time, parameters, nodesCalledThisFrame);
                },
                () => { return new AudioOutSource[0]; },
                (dataItem) => { return new AudioOutSource[0]; },
                () => { return dataSource.GetDataRows(); },null
            );
            playback.OnTimeChange += (time) => {
                dataSource.SetCurrentPlaybackTime(playback.currentTime);
                editor.uiState.window.Repaint();
            };

            editor.uiState.OnTimeBarClick += () => {
                playback.Stop(AudioSettings.dspTime);
                currentPreviewSoundgraph.CallEvent("EndAll", AudioSettings.dspTime,0);
                Repaint();
            };
        }


        private void OnEnable()
        {
        }

        private void OnGUI()
        {
        

            if (editor == null)
            {
                Setup();
            }
            editor.Draw(position);
        }

        private void OnDrawTopBar(Rect position, TimelineUIState uiState)
        {
            GUILayout.BeginArea(position);
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(!uiState.dataSource.editable || !uiState.dataSource.changed);
            if (GUILayout.Button("Save", EditorStyles.toolbarButton))
            {
                (editor.uiState.dataSource as MidiDataSource).DoSave();
                EditorGUIUtility.ExitGUI();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private static SoundGraph _playbackSoundgraph;
        public static SoundGraph currentPreviewSoundgraph
        {
            get
            {
                if (_playbackSoundgraph == null)
                {
                    _playbackSoundgraph = GetSoundGraph();
                    _playbackSoundgraph.GraphAwake();
                }
                return _playbackSoundgraph;
            }
        }

        public static string currentEventID
        {
            get
            {
                string id = GetCurrentEventID();
                if (string.IsNullOrEmpty(id))
                {
                    GraphEvent firstEvent = currentPreviewSoundgraph.GetAllEvents().FirstOrDefault();
                    if (firstEvent != null)
                        SetCurrentEventID(firstEvent.eventID);
                    id = firstEvent.eventID;
                }
                return id;
            }
        }


        private void OnDrawBottomBar(Rect position, TimelineUIState uiState)
        {


            Rect toolBarRect = new Rect(position.x, position.y + position.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing-2,
                position.width, EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            GUILayout.BeginArea(toolBarRect);
            GUILayout.BeginHorizontal();


            EditorGUILayout.Space();

            Color currentGUIColor = GUI.contentColor;
            GUI.contentColor = uiState.style.buttonColor;

            //Rewind button
            Rect rewindButtonRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight));
            if (GUI.Button(rewindButtonRect, new GUIContent(uiState.style.rewindButton), uiState.style.toolbarButtonStyle))
            {
                //playback.Stop(AudioSettings.dspTime);
                editor.uiState.dataSource.SetCurrentPlaybackTime(0f);
                Repaint();
            }

            //Pause
            Rect pauseButtonRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight));
            if (GUI.Button(pauseButtonRect, new GUIContent(uiState.style.pauseButton), uiState.style.toolbarButtonStyle))
            {
                playback.Stop(AudioSettings.dspTime);
                currentPreviewSoundgraph.CallEvent("EndAll", AudioSettings.dspTime,0);
                Repaint();
            }

            //Play button
            Rect playButtonRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight));
            if (GUI.Button(playButtonRect, new GUIContent(uiState.style.playButton), uiState.style.toolbarButtonStyle))
            {
                playback.QueueMIDIFile(
                    AudioSettings.dspTime,
                    editor.uiState.dataSource.GetCurrentPlaybackTime(),
                    new Dictionary<string, object>(),
                    (uiState.dataSource as MidiDataSource).midiAsset,
                    (uiState.dataSource as MidiDataSource).GetTempoMap(),
                    (uiState.dataSource as MidiDataSource).GetTimelineDataItems().Select(x => (x as MidiDataItem).underlyingNote).ToList(),
                    0
                );
                //playback.QueueAudio(AudioSettings.dspTime, editor.uiState.dataSource.GetCurrentPlaybackTime(), new Dictionary<string, object>(), MidiFlowInfo.defaultMidiFlowInfo);
                Repaint();
            }

            GUI.contentColor = currentGUIColor;

            EditorGUILayout.Space();

            float contentWidth = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.fieldWidth = 100;

            SoundGraph newSoundgraph =(SoundGraph) EditorGUILayout.ObjectField(GetSoundGraph(), typeof(SoundGraph), false);
            SetSoundGraph(newSoundgraph);

            if (newSoundgraph == null)
                return;

            List<GraphEvent> graphEvents = newSoundgraph.GetAllEvents();
            int selection = Mathf.Clamp(graphEvents.FindIndex(x => x.eventID == GetCurrentEventID()), 0, int.MaxValue);
            LayersGUIUtilities.DrawDropdown(selection, graphEvents.Select(x => x.eventName).ToArray(), false, (newSelection)=> {
                SetCurrentEventID(graphEvents[newSelection].eventID);
            });

            EditorGUIUtility.fieldWidth = contentWidth;

            GUILayout.FlexibleSpace();

            uiState.zoomLevel = EditorGUILayout.Slider(uiState.zoomLevel, 1f, 10f);

            currentChannel = (MidiUtils.channels)EditorGUILayout.EnumPopup(currentChannel);

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;
            uiState.selectedGridDivision = EditorGUILayout.Popup("Grid", uiState.selectedGridDivision, MidiUtils.gridLabels);
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUILayout.Space();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            Rect velocityRect = new Rect(position.x + uiState.calculateLeftBarHeight(editor.uiState), position.y, position.width, position.height - toolBarRect.height);
            DrawVelocityArea(velocityRect);

        }

        private static string GetCurrentEventID()
        {
            string key = GetProjectKey() + "DefaultMIDIPlaybackGraphEvent";
            return EditorPrefs.HasKey(key) ? EditorPrefs.GetString(key) : "";
        }

        private static void SetCurrentEventID(string eventID)
        {
            string key = GetProjectKey() + "DefaultMIDIPlaybackGraphEvent";
            EditorPrefs.SetString(eventID, key);
        }

        private static SoundGraph GetSoundGraph()
        {
            string key = GetProjectKey() + "DefaultMIDIPlaybackGraph";
            string graphPath = EditorPrefs.HasKey(key) ? EditorPrefs.GetString(key) : "";
            SoundGraph soundGraph = AssetDatabase.LoadAssetAtPath<SoundGraph>(graphPath);
            if (soundGraph == null)
            {
                soundGraph = Resources.Load<SoundGraph>("Symphony/Piano/Piano Sampler");
                SetSoundGraph(soundGraph);
            }
            _playbackSoundgraph = soundGraph;
            return soundGraph;
        }

        private static void SetSoundGraph(SoundGraph soundGraph)
        {
            if (soundGraph != _playbackSoundgraph)
                soundGraph.GraphAwake();

            _playbackSoundgraph = soundGraph;
            string path = AssetDatabase.GetAssetPath(soundGraph);
            EditorPrefs.SetString(GetProjectKey() + "DefaultMIDIPlaybackGraph", path);
        }

        private static string GetProjectKey()
        {
            return PlayerSettings.companyName + "." + PlayerSettings.productName;
        }

        int currentDownNote = -1;
        private void OnDrawLeftBar(Rect position, TimelineUIState uiState)
        {
            //doing click events
            float offset = 6f;
            float regNoteCount = 0;

            for (int index = 0; index <= 128; index++)
            {
                if (MidiUtils.IsSharp(index))
                {
                    Rect sharpNoteRect = new Rect(0, offset + position.y + (128 - index) * EditorGUIUtility.singleLineHeight - 8, position.width / 2f, EditorGUIUtility.singleLineHeight);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && sharpNoteRect.Contains(Event.current.mousePosition))
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        data.Add("NoteInfo", new MidiData(index, MidiData.MidiChannel.All, 1f));
                        currentPreviewSoundgraph.CallEventByID(currentEventID, AudioSettings.dspTime, data,0);
                        Event.current.Use();
                        currentDownNote = index;
                    }
                }

            }


            for (int index = 0; index <= 128; index++)
            {
                if (!MidiUtils.IsSharp(index))
                {
                    float noteSize = (12f * EditorGUIUtility.singleLineHeight / 7f);
                    Rect regNoteRect = new Rect(0, offset + position.y + (74 - regNoteCount) * noteSize, position.width, noteSize);

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && regNoteRect.Contains(Event.current.mousePosition))
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        data.Add("NoteInfo", new MidiData(index, MidiData.MidiChannel.All, 1f));
                        currentPreviewSoundgraph.CallEventByID(currentEventID, AudioSettings.dspTime, data,0);
                        Event.current.Use();
                        currentDownNote = index;
                    }

                    regNoteCount++;
                }
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                currentDownNote = -1;

            //Drawing
            if (Event.current.type != UnityEngine.EventType.Repaint)
                return;

            regNoteCount = 0;

            EditorGUI.DrawRect(position, Color.black);
            for (int index = 0; index <= 128; index++)
            {
                if (!MidiUtils.IsSharp(index))
                {
                    float noteSize = (12f * EditorGUIUtility.singleLineHeight / 7f);
                    Rect regNoteRect = new Rect(0, offset + position.y + (74 - regNoteCount) * noteSize, position.width, noteSize);
                    EditorGUI.DrawRect(regNoteRect, Color.black);
                    Rect regNoteRectInner = new Rect(regNoteRect.x + 1, regNoteRect.y + 1, regNoteRect.width - 2, regNoteRect.height - 2);

                    if (currentDownNote == index)
                        Repaint();

                    Color noteColor = currentDownNote == index ? new Color32(200, 200, 200, 255) : new Color32(220, 220, 220, 255);
                    EditorGUI.DrawRect(regNoteRectInner, noteColor);

                    Rect labelPosition = new Rect(regNoteRect.x + regNoteRect.width - 25, regNoteRect.y + 6, 25, EditorGUIUtility.singleLineHeight);

                    //if (Event.current.type == UnityEngine.EventType.mouse)// This was eating click events for some reason, so hackety hackety hack
                    GUI.Label(labelPosition, MidiUtils.NoteNumberToName(index));

                    regNoteCount++;
                }
            }
            for (int index = 0; index <= 128; index++)
            {
                if (MidiUtils.IsSharp(index))
                {
                    Rect sharpNoteRect = new Rect(0, offset + position.y + (128 - index) * EditorGUIUtility.singleLineHeight - 8 , position.width / 2f, EditorGUIUtility.singleLineHeight);
                    if (currentDownNote == index)
                        Repaint();

                    Color color = currentDownNote == index ? new Color32(20, 20, 20, 255) : new Color32(0, 0, 0, 255);

                    EditorGUI.DrawRect(sharpNoteRect, color);
                }

            }
        }

        private void OnDrawRow(Rect position, TimelineUIState uiState, TimeLineRowDataItem item, int itemNumber)
        {
            if (Event.current.type != UnityEngine.EventType.Repaint)
                return;
            itemNumber = 128 - itemNumber;
            EditorGUI.DrawRect(position, uiState.style.mainBackground);
            if (!MidiUtils.IsSharp(itemNumber))
            {
                Rect innerRect = new Rect(position.x, position.y + 1, position.width, position.height - 2);
                EditorGUI.DrawRect(innerRect, uiState.style.secondaryBackground);
            }

        
        }

        private void OnDrawItem(Rect drawRect, TimelineUIState uiState, TimelineDataItem item, bool selected)
        {

            if (Event.current.type != UnityEngine.EventType.Repaint)
                return;

            EditorGUI.DrawRect(drawRect, Color.black);
            int outlineAmount = 1;
            Rect innerRect = new Rect(drawRect.x + outlineAmount, drawRect.y + outlineAmount, drawRect.width - (2 * outlineAmount), drawRect.height - (2 * outlineAmount));

            if (selected)
                EditorGUI.DrawRect(innerRect, uiState.style.secondaryTimelineElementColor);
            else
                EditorGUI.DrawRect(innerRect, uiState.style.primaryTimelineElementColor);

            int horizontalPad = 5;
            int verticalPad = 2;
            Rect leftGrabHandle = new Rect(innerRect.x + horizontalPad, innerRect.y + verticalPad, 2f, innerRect.height - verticalPad - verticalPad);
            EditorGUI.DrawRect(leftGrabHandle, uiState.style.dragAreaColor);

            Rect rightHandleGrab = new Rect(innerRect.x + innerRect.width - 2f - horizontalPad, innerRect.y + verticalPad, 2f, innerRect.height - verticalPad - verticalPad);
            EditorGUI.DrawRect(rightHandleGrab, uiState.style.dragAreaColor);
        }

    


        List<TimelineDataItem> velocitySelection = new List<TimelineDataItem>();

        public void DrawVelocityArea(Rect drawArea)
        {
            EditorGUI.LabelField(new Rect(drawArea.x - 100, drawArea.y + drawArea.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing, 100, EditorGUIUtility.singleLineHeight), "Note Velocities:");
            GUI.BeginGroup(drawArea);
            //Rect paneRect = new Rect(0, 0, uiState.window.position.width, uiState.velocityAreaHeight);
            //EditorGUI.DrawRect(paneRect, new Color32(132, 132, 132, 255));
            foreach (TimelineDataItem timelineItem in editor.uiState.selectedItems)
            {
                float velocity = (timelineItem as MidiDataItem) .velocity / 127f;
                float minSize = 10f;
                float velocityHeight = (velocity * (drawArea.height - minSize - (2f * EditorGUIUtility.standardVerticalSpacing))) ;
                Rect velocityRect = new Rect(-editor.uiState.scrollPosition.x + (float)editor.uiState.Dimension2Pixel (timelineItem.startTime), drawArea.height - velocityHeight - minSize - EditorGUIUtility.standardVerticalSpacing
                    , 20, velocityHeight + minSize);
                EditorGUI.DrawRect(velocityRect, Color.black);

                Rect innerRect = new Rect(velocityRect.x + 1, velocityRect.y + 1, velocityRect.width - 2, velocityRect.height - 2);
                if (velocitySelection.Contains(timelineItem))
                    EditorGUI.DrawRect(innerRect, editor.uiState.style.secondaryTimelineElementColor);
                else
                    EditorGUI.DrawRect(innerRect, editor.uiState.style.primaryTimelineElementColor);

                DoVelocityInput(velocityRect, timelineItem);

                Rect labelRect = new Rect(velocityRect.x + velocityRect.width, velocityRect.y + velocityRect.height - EditorGUIUtility.singleLineHeight, 50, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelRect, (Mathf.Round(velocity * 100)).ToString() + "%");
            }

            GUI.EndGroup();
        }

        private bool velocityClick = false;
        public void DoVelocityInput(Rect velocityRect, TimelineDataItem note)
        {
            //if (!editable)
            //return;

            if (Event.current.type == UnityEngine.EventType.MouseDown && velocityRect.Contains(Event.current.mousePosition))
            {
                velocityClick = true;
                if (!velocitySelection.Contains(note))
                {
                    if (Event.current.modifiers == EventModifiers.Shift)
                        velocitySelection.Add(note);
                    else
                    {
                        velocitySelection.Clear();
                        velocitySelection.Add(note);
                    }
                }
                Event.current.Use();
            }
            else if (Event.current.type == UnityEngine.EventType.MouseDrag && velocityClick)
            {
                //changed = true;
                foreach (TimelineDataItem selectedNote in velocitySelection)
                {
                    int yDelta = (int)Event.current.delta.y;
                    (selectedNote as MidiDataItem).velocity = (ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Common.SevenBitNumber)(Mathf.Clamp((selectedNote as MidiDataItem).velocity - yDelta, 0, 127));
                    Repaint();
                }
                editor.uiState.dataSource.changed = true;
                Event.current.Use();

            }
            else if (Event.current.type == UnityEngine.EventType.MouseUp)
            {
                velocityClick = false;
                Event.current.Use();
            }
        }


        private void OnDestroy()
        {
            if (editor.uiState.dataSource.changed)
            {
                bool save = EditorUtility.DisplayDialog("Unsaved changes", "This MIDI Asset has unsaved changes. Would you like to save them?", "Save", "Discard");
                if (save)
                    (editor.uiState.dataSource as MidiDataSource).DoSave();
            }
            (editor.uiState.dataSource as MidiDataSource).Dispose();

        }

    
    }
}
