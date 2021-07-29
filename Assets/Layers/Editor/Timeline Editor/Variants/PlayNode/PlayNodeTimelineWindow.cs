using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Timeline_Editor.Variants.Midi;
using ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode.Style;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Midi;
using ABXY.Layers.Runtime.Nodes.Playback;
using ABXY.Layers.Runtime.Timeline;
using ABXY.Layers.Runtime.Timeline.Playnode;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode
{
    public class PlayNodeTimelineWindow : EditorWindow
    {
        private TimelineEditor editor;

        private static Dictionary<AudioClip, float[]> audioSamples = new Dictionary<AudioClip, float[]>();

        private static Dictionary<AudioClip, float> maxValues = new Dictionary<AudioClip, float>();

        private Material lineMaterial;

    
        [SerializeField]
        private global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode playNode;

        private VirtualAudioOut[] virtualAudioOut = new VirtualAudioOut[] { new VirtualAudioOut() };

        PlaybackSystem _playback = null;
        PlaybackSystem playback
        {
            get
            {
                if (_playback == null)
                {
                    _playback = new PlaybackSystem(
                        () => { return playNode.timelineData; },
                        playNode.StartCoroutine,
                        () => { return playNode.combinedVolume;},
                        () => { return playNode.combinedPan; },
                        () => { return playNode.endTime; },
                        () => { return playNode.name; },
                        (name, time, parameters, nodesCalledThisFrame) => {
                            if ( playNode.eventPreviewType == global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode.EventPreviewTypes.PlayOnNode)
                            {
                                playNode.CallFunctionOnOutputNodes(name, time, parameters, nodesCalledThisFrame);
                            }
                        },
                        () => { 
                            if (playNode.previewAudioType == global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode.AudioPreviewTypes.PlayAllAudio)
                                return virtualAudioOut; 
                            else
                                return playNode.GetAudioOuts(playNode.GetOutputPort("audioOut"), playNode.audioFlowOutID);
                        },
                        (timeLineChange) => {
                            if (playNode.previewAudioType == global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode.AudioPreviewTypes.PlayAllAudio)
                                return new AudioOutSource[0]; 
                            else
                                return playNode.GetAudioOuts(playNode.GetOutputPort(playNode.tracks[(int)timeLineChange.rowNumber].audioOutNodeName), playNode.tracks[(int)timeLineChange.rowNumber].audioOutSendName);
                        },
                        () => { return playNode.tracks.Select(x=>(TimeLineRowDataItem)x).ToList(); }, playNode);
                    _playback.OnTimeChange += (time) =>
                    {
                        editor.uiState.dataSource.SetCurrentPlaybackTime(playback.currentTime);
                    };
                }
                return _playback;
            }
        }

        private static PlayNodeTimelineWindow lastOpenedWindow = null;


        public static PlayNodeTimelineWindow ShowPlayNodeTimeline(global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode playNode)
        {
            if (lastOpenedWindow != null && lastOpenedWindow.playNode != playNode)
                CloseLastWindowIfOpen();

            if (lastOpenedWindow != null && lastOpenedWindow.playNode == playNode)
            {
                lastOpenedWindow.Focus();
                return lastOpenedWindow;
            }

            PlayNodeTimelineWindow timeLineWindow = CreateInstance<PlayNodeTimelineWindow>();
            timeLineWindow.playNode = playNode;
       
            //timeLineWindow.editor.shouldDrawItem += (Rect position, TimelineUIState uiState, TimelineDataItem item) => { return midiEditor.currentChannel == MidiUtils.channels.AllChannels || midiEditor.currentChannel == (item as MidiDataItem).channelNumber; };

            timeLineWindow.ShowUtility();
            lastOpenedWindow = timeLineWindow;
            //midiEditor.uiState = new TimelineUIState(midiEditor, playNode, 1f / 1000f, EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 2f, EditorGUIUtility.singleLineHeight, 250f, timeLine, tracks) ;
            return timeLineWindow;
        }

        public static void CloseLastWindowIfOpen()
        {
            lastOpenedWindow?.Close();
            lastOpenedWindow = null;
        }

        private void SetupEditor()
        {
            lineMaterial = Resources.Load<Material>("Symphony/LineMaterial");

            editor = new TimelineEditor(this, new PlayNodeDataSource(playNode));

            editor.uiState.style = new PlaynodeTimelineStyle();

            //timeLineWindow.editor.onDrawTopBar += timeLineWindow.OnDrawTopBar;
            editor.calculateTopBarHeight += (TimelineUIState uiState) => { return EditorGUIUtility.standardVerticalSpacing; };

            //timeLineWindow.editor.onDrawBottomBar += timeLineWindow.OnDrawBottomBar;
            editor.calculateBottomBarHeight += (TimelineUIState uiState) => { return EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing; };

            //timeLineWindow.editor.onDrawLeftBar += timeLineWindow.OnDrawLeftBar;
            editor.calculateLeftBarHeight += (TimelineUIState uiState) => { return 150; };


            //timeLineWindow.editor.onDrawRow += timeLineWindow.OnDrawRow;

            //timeLineWindow.editor.onDrawDataItem += timeLineWindow.OnDrawItem;

            editor.pixel2Dimension += (value) => { return value / 100; };
            editor.dimension2Pixel += (value) => { return value * 100; };
            editor.seconds2Dimension += (value) => { return value; };
            editor.dimension2Seconds += (value) => { return value; };
            editor.calculateRowHeight += (uiState, dataItem) => { return 70; };
            editor.onDrawBottomBar += OnDrawBottomBar;
            editor.onCreateByDragToRowArea += OnDrag;
            editor.onDrawRow += OnDrawRow;
            editor.onDrawDataItem += OnDrawItem;
            editor.onDrawLeftBar += OnDrawSidebar;
            editor.onCreateRow += (row) => { return new PlaynodeTrackItem(playNode); };
            editor.gridsArrangedByTime = true;
            editor.leftResizeShiftsInternalTime = true;
            editor.onRightClickItem += DoItemRightClick;
            editor.OnTimeBarClick += () => { playback.Stop(AudioSettings.dspTime); };
            editor.autoExpandRows = true;
            editor.onDeleteDataItem += (item) => {
                ItemEditWindow.OnTimelineObjectRemoved(item);
            };
            editor.showEndTime += () => {
                return playNode.playFinishedWhen == Runtime.Nodes.Playback.PlayNode.EndTimeStyles.AtSpecifiedTime;
            };
        }


        private void OnGUI()
        {
            DoObjectSelectors();
            if (Selection.activeObject != null && Selection.activeObject.GetType() == typeof(global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode) && Selection.activeObject != playNode)
            {
                editor = null;
                playNode = Selection.activeObject as global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode;
            }

            if (editor == null )
                SetupEditor();
            Rect positionRect = new Rect(0, 0, position.width, position.height);
            editor.Draw(positionRect);
            Repaint();
        }

        private TimelineDataItem OnDrag(object obj, int row, double dimensionTime)
        {
            if (obj == null)
                return null;
            List<TimelineDataItem> items = editor.uiState.dataSource.GetTimelineDataItems();

            TimelineDataItem newitem = null;
            if (obj.GetType() == typeof(MidiFileAsset))
                newitem = new PlaynodeDataItem (dimensionTime, (MidiFileAsset)obj, playNode);
            else if (obj.GetType() == typeof(AudioClip))
                newitem = new PlaynodeDataItem(dimensionTime, (AudioClip)obj, playNode);

            int additionalRowsNeeded = 0;

            List<TimeLineRowDataItem> rows = editor.uiState.dataSource.GetDataRows();

            if (newitem != null)
            {
                newitem.rowNumber = row;
                newitem.startTime = dimensionTime;
                items.Add(newitem);

                if (row >= rows.Count)
                    additionalRowsNeeded = row - rows.Count + 1;
            }

            for (int index = 0; index < additionalRowsNeeded; index++)
                rows.Add(new PlaynodeTrackItem((editor.uiState.dataSource as PlayNodeDataSource).backingPlaynode));

            editor.uiState.dataSource.SetDataRows(rows);
            editor.uiState.dataSource.ApplyTimelineDataChanges(items);

            return null;
        }

        private void OnDrawBottomBar(Rect position, TimelineUIState uiState)
        {


            Rect toolBarRect = new Rect(position.x, 
                position.y + position.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing-2, 
                position.width, position.height);
            GUILayout.BeginArea(toolBarRect);
            GUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            Color currentGUIColor = GUI.contentColor;
            GUI.contentColor = uiState.style.buttonColor;

            //Rewind button
            Rect rewindButtonRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight));
            if (GUI.Button(rewindButtonRect, new GUIContent((uiState.style as PlaynodeTimelineStyle).rewindButton),uiState.style.toolbarButtonStyle))
            {
                playback.Stop(AudioSettings.dspTime);
                editor.uiState.dataSource.SetCurrentPlaybackTime(0f);
                Repaint();
            }

            //Pause
            Rect pauseButtonRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight));
            if(GUI.Button(pauseButtonRect, new GUIContent((uiState.style as PlaynodeTimelineStyle).pauseButton), uiState.style.toolbarButtonStyle))
            {
                playback.Stop(AudioSettings.dspTime);
                Repaint();
            }

            //Play button
            Rect playButtonRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight));
            if(GUI.Button(playButtonRect, new GUIContent((uiState.style as PlaynodeTimelineStyle).playButton), uiState.style.toolbarButtonStyle))
            {
                playback.QueueAudio(AudioSettings.dspTime, editor.uiState.dataSource.GetCurrentPlaybackTime(), new Dictionary<string, object>(), 0);
                Repaint();
            }

            GUI.contentColor = currentGUIColor;

            EditorGUILayout.Space();


            float labelWidth = EditorGUIUtility.labelWidth;
            float fieldWidth = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = 90;
            EditorGUIUtility.fieldWidth = 100;

            LayersGUIUtilities.DrawDropdown(new GUIContent("Audio Preview"), playNode.previewAudioType, (newValue) => {
                playNode.previewAudioType = (global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode.AudioPreviewTypes)newValue;
            });

            EditorGUIUtility.fieldWidth = 130;
            LayersGUIUtilities.DrawDropdown(new GUIContent("Event Preview"), playNode.eventPreviewType, (newValue) => {
                playNode.eventPreviewType = (global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode.EventPreviewTypes)newValue;
            });

            EditorGUIUtility.fieldWidth = fieldWidth;

            /*if (EditorGUILayout.DropdownButton(new GUIContent("Preview Settings", "Change how MIDI is previewed"), FocusType.Keyboard))
        {
            PlaynodePreviewSettings.ShowSelector(playNode);
        }*/


            GUILayout.FlexibleSpace();



            EditorGUIUtility.labelWidth = 40;
            uiState.zoomLevel = EditorGUILayout.Slider("Zoom",uiState.zoomLevel, 1f, 10f);


            EditorGUILayout.Space();

            EditorGUIUtility.labelWidth = 65;
            uiState.selectedGridDivision = EditorGUILayout.Popup("Snap Grid", uiState.selectedGridDivision, MidiUtils.gridLabels);
            EditorGUIUtility.labelWidth = labelWidth;


            EditorGUILayout.Space();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();


        }

        private void OnDrawRow(Rect position, TimelineUIState uiState, TimeLineRowDataItem item, int itemNumber)
        {
            if (Event.current.type != UnityEngine.EventType.Repaint)
                return;

            EditorGUI.DrawRect(position, uiState.style.mainBackground);
            if (itemNumber %2 ==0)
            {
                Rect innerRect = new Rect(position.x, position.y + 1, position.width, position.height - 2);
                EditorGUI.DrawRect(innerRect, uiState.style.secondaryBackground);
            }
        }

        private void OnDrawItem(Rect drawRect, TimelineUIState uiState, TimelineDataItem item, bool selected)
        {

            if (Event.current.type != UnityEngine.EventType.Repaint)
                return;

            int outlineAmount = 1;
            PlaynodeDataItem castDataItem = item as PlaynodeDataItem;
            if ((castDataItem.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Audio || castDataItem.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.MIDI)
                && (castDataItem.exposed && castDataItem.oneShot))
            {
                Rect dotRect = new Rect(drawRect.x, drawRect.y + (drawRect.height - drawRect.width) / 2f, drawRect.width, drawRect.width);
                Color currentColor = GUI.color;
                GUI.color = Color.black;
                GUI.DrawTexture(dotRect, (uiState.style as PlaynodeTimelineStyle).eventItemBackground, ScaleMode.ScaleToFit, true);

                Rect innerRect = new Rect(dotRect.x + outlineAmount, dotRect.y + outlineAmount, dotRect.width - 2f * outlineAmount, dotRect.height - 2f * outlineAmount);

                GUI.color = selected ? uiState.style.secondaryTimelineElementColor : uiState.style.primaryTimelineElementColor;
                GUI.DrawTexture(innerRect, (uiState.style as PlaynodeTimelineStyle).eventItemBackground, ScaleMode.ScaleToFit, true);

                Rect iconRect = new Rect(innerRect.x + 2, innerRect.y + 2, innerRect.width - 4, innerRect.height - 4);
                GUI.color = Color.black;

                if (castDataItem.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Audio)
                    GUI.DrawTexture(iconRect, (uiState.style as PlaynodeTimelineStyle).audioIcon, ScaleMode.ScaleToFit, true);
                else
                    GUI.DrawTexture(iconRect, (uiState.style as PlaynodeTimelineStyle).midiIcon, ScaleMode.ScaleToFit, true);

                string title = (item as PlaynodeDataItem).eventLabel + " <Exposed>";
                Vector2 dimensions = EditorStyles.label.CalcSize(new GUIContent(title));

                Rect labelRect = new Rect(
                    Mathf.Clamp(drawRect.x + (drawRect.width / 2f) - (dimensions.x / 2f), 0f, float.MaxValue),
                    drawRect.y + ((3f / 4f) * drawRect.height - (dimensions.y / 2f) + 4),
                    dimensions.x, dimensions.y);

                GUI.color = currentColor;

                Rect boxRect = new Rect(labelRect.x - 5, labelRect.y - 2, labelRect.width + 10, labelRect.height + 4);
                EditorGUI.DrawRect(boxRect, item.rowNumber % 2 == 1 ? uiState.style.secondaryBackground : uiState.style.mainBackground);

                EditorGUI.LabelField(labelRect, title);
            }
            else if (castDataItem.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Event)
            {
                Rect dotRect = new Rect(drawRect.x, drawRect.y +  (drawRect.height - drawRect.width) / 2f, drawRect.width, drawRect.width);
                Color currentColor = GUI.color;
                GUI.color = Color.black;
                GUI.DrawTexture(dotRect, (uiState.style as PlaynodeTimelineStyle).eventItemBackground, ScaleMode.ScaleToFit, true);

                Rect innerRect = new Rect(dotRect.x + outlineAmount, dotRect.y + outlineAmount, dotRect.width - 2f * outlineAmount, dotRect.height - 2f * outlineAmount);

                GUI.color = selected? uiState.style.secondaryTimelineElementColor:uiState.style.primaryTimelineElementColor;
                GUI.DrawTexture(innerRect, (uiState.style as PlaynodeTimelineStyle).eventItemBackground, ScaleMode.ScaleToFit, true);

                string title = (item as PlaynodeDataItem).eventLabel;
                Vector2 dimensions = EditorStyles.label.CalcSize(new GUIContent(title));

                Rect labelRect = new Rect(
                    Mathf.Clamp( drawRect.x + (drawRect.width / 2f) - (dimensions.x / 2f), 0f, float.MaxValue),
                    drawRect.y + ((3f / 4f) * drawRect.height - (dimensions.y/2f) + 4),
                    dimensions.x, dimensions.y);

                GUI.color = currentColor;

                Rect boxRect = new Rect(labelRect.x - 5, labelRect.y - 2, labelRect.width + 10, labelRect.height +4);
                EditorGUI.DrawRect( boxRect, item.rowNumber % 2==1? uiState.style.secondaryBackground: uiState.style.mainBackground);

                EditorGUI.LabelField(labelRect, title);

            }
            else if ((item as PlaynodeDataItem).playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Audio || (item as PlaynodeDataItem).playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.MIDI)
            {

                int horizontalPad = 5;


                GUI.BeginGroup(drawRect);
                float underlyingDataWidth = (float)uiState.Dimension2Pixel((item as PlaynodeDataItem).DataSourceLength);
                int loopXPosition = (int)uiState.Dimension2Pixel((float)-(item as PlaynodeDataItem).interiorStartTime);
                for (int index = 0; index < (item as PlaynodeDataItem).numberRepetitions; index++)
                {


                    Rect blockRect = new Rect(loopXPosition, 0f, underlyingDataWidth, drawRect.height);

                    DrawLoopingRect(blockRect, 5, Color.black);
                    Rect innerLoopingRect = new Rect(blockRect.x + outlineAmount, blockRect.y + outlineAmount, blockRect.width - (2 * outlineAmount), blockRect.height - (2 * outlineAmount));

                    if (selected)
                        DrawLoopingRect(innerLoopingRect, 5, uiState.style.secondaryTimelineElementColor);
                    else
                        DrawLoopingRect(innerLoopingRect, 5, uiState.style.primaryTimelineElementColor);

                    loopXPosition += (int)underlyingDataWidth;
                }
                GUI.EndGroup();
                //DrawLoopingRect(drawRect, 10, Color.green);


                Texture2D icon = null;
                if ((item as PlaynodeDataItem).playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Audio)
                {
                    AudioClip clip = ((item as PlaynodeDataItem).GetBackingObject() as AudioClip);
                    if (clip != null)
                    {

                        if (!audioSamples.ContainsKey(clip))
                        {
                            float[] samples = new float[clip.samples];
                            clip.GetData(samples, 0);

                            float maxValue = 0f;
                            for (int index = 0; index < clip.samples; index++)
                                if (Mathf.Abs(samples[index]) > maxValue)
                                    maxValue = samples[index];
                            maxValues.Add(clip, maxValue);
                            audioSamples.Add(clip, samples);
                        }

                        float[] audioClipSamples = audioSamples[clip];

                        int increment = Mathf.FloorToInt(clip.samples / underlyingDataWidth);

                        GUI.BeginClip(new Rect((int)drawRect.x, (int)drawRect.y, (int)drawRect.width, (int)drawRect.height));
                        GL.PushMatrix();
                        lineMaterial.SetPass(0);

                        GL.Begin(GL.LINES);
                        //GL.Color(new Color(.51f,.51f,.51f,1f));
                        GL.Color(uiState.style.dragAreaColor);
                        int xPosition = (int)uiState.Dimension2Pixel((float)-(item as PlaynodeDataItem).interiorStartTime);
                        int centerLine = (int)(drawRect.height / 2f);
                        float scalar = (1f / maxValues[clip]) * drawRect.height / 2f;

                        int samplePixels = 1;
                        for (int loopIndex = 0; loopIndex < (item as PlaynodeDataItem).numberRepetitions; loopIndex++)
                        {
                            for (int index = 0; index + samplePixels * increment < clip.samples; index += samplePixels * increment)
                            {
                                //if (xPosition + drawRect.x > uiState.scrollPosition.x)
                                //{
                                bool withinScreen = xPosition + drawRect.x > uiState.scrollPosition.x;
                                bool withinBox = xPosition > 2f * horizontalPad && xPosition + 2f * horizontalPad < drawRect.width;
                                if (withinBox && withinScreen)
                                {
                                    float pointA = Mathf.Clamp((centerLine + audioClipSamples[index] * scalar) + drawRect.y, uiState.scrollPosition.y, uiState.lastScrollContainerRect.height + uiState.scrollPosition.y) - drawRect.y;
                                    float pointB = Mathf.Clamp((centerLine + audioClipSamples[index + samplePixels * increment] * scalar) + drawRect.y, uiState.scrollPosition.y, uiState.lastScrollContainerRect.height + uiState.scrollPosition.y) - drawRect.y;

                                    GL.Vertex(new Vector2(xPosition, (int)(pointA)));
                                    GL.Vertex(new Vector2(xPosition, (int)(pointB)));
                                }
                                //}
                                xPosition += samplePixels;

                            }
                        }
                        GL.End();
                        GL.PopMatrix();
                        GUI.EndClip();
                    }
                    icon = (uiState.style as PlaynodeTimelineStyle).audioIcon;
                }
                else
                    icon = (uiState.style as PlaynodeTimelineStyle).midiIcon;


                Rect innerRect = new Rect(drawRect.x + outlineAmount, drawRect.y + outlineAmount, drawRect.width - (2 * outlineAmount), drawRect.height - (2 * outlineAmount));

                float handlePad = 10;

                Rect leftGrabHandle = new Rect(innerRect.x + horizontalPad, innerRect.y + handlePad, 2f, innerRect.height - handlePad - handlePad);
                EditorGUI.DrawRect(leftGrabHandle, uiState.style.dragAreaColor);

                Rect rightHandleGrab = new Rect(innerRect.x + innerRect.width - 2f - horizontalPad, innerRect.y + handlePad, 2f, innerRect.height - handlePad - handlePad);
                EditorGUI.DrawRect(rightHandleGrab, uiState.style.dragAreaColor);

                int iconWidth = 20;
                Rect iconRect = new Rect(innerRect.x + innerRect.width - iconWidth - 2f * horizontalPad, innerRect.y, iconWidth, iconWidth);
                GUI.DrawTexture(iconRect, icon);

                Rect labelRect = new Rect(innerRect.x + 2f * horizontalPad, innerRect.y, innerRect.width, innerRect.height);
                EditorGUI.LabelField(labelRect, (item as PlaynodeDataItem).dataName);
            }
            else if ((item as PlaynodeDataItem).playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Loop)
            {
                int horizontalPad = 5;
                float handlePad = 10;
                Rect headerRect = new Rect(drawRect.x, 0, drawRect.width, 15);
                Rect innerHeaderRect = new Rect(headerRect.x + 1, headerRect.y + 1, headerRect.width - 2f, headerRect.height - 2);
                EditorGUI.DrawRect(headerRect, Color.black);
                EditorGUI.DrawRect(innerHeaderRect, selected? uiState.style.secondaryTimelineElementColor: uiState.style.primaryTimelineElementColor);

                Rect leftAccentBarRect = new Rect(drawRect.x, 0, 2, uiState.window.position.height);
                EditorGUI.DrawRect(leftAccentBarRect, uiState.style.highlightColor);

                Rect rightAccentBarRect = new Rect(drawRect.x + drawRect.width -2, 0, 2, uiState.window.position.height);
                EditorGUI.DrawRect(rightAccentBarRect, uiState.style.highlightColor);

                string label = (item as PlaynodeDataItem).eventLabel;
                float labelWidth = GUI.skin.label.CalcSize(new GUIContent(label)).x;
                if (headerRect.width > labelWidth)
                {
                    Rect labelRect = new Rect(((headerRect.width - labelWidth) / 2f) + headerRect.x, -1f, labelWidth, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(labelRect, label);
                }

                Rect leftGrabHandle = new Rect(headerRect.x + horizontalPad, headerRect.y + handlePad, 2f, headerRect.height - handlePad - handlePad);
                EditorGUI.DrawRect(leftGrabHandle, uiState.style.dragAreaColor);

                Rect rightHandleGrab = new Rect(headerRect.x + headerRect.width - 2f - horizontalPad, headerRect.y + handlePad, 2f, headerRect.height - handlePad - handlePad);
                EditorGUI.DrawRect(rightHandleGrab, uiState.style.dragAreaColor);
            }
        }

        private void DrawLoopingRect(Rect drawRect, int bevel, Color color)
        {
            Color currentColor = GUI.color;
            GUI.color = color;

            Rect tl = new Rect(drawRect.x, drawRect.y, bevel, bevel);
            GUI.DrawTextureWithTexCoords(tl, (editor.uiState.style as PlaynodeTimelineStyle).timelineItemBackground, new Rect(0f, .9f, .1f, .1f));

            Rect tr = new Rect(drawRect.x + drawRect.width - bevel, drawRect.y, bevel, bevel);
            GUI.DrawTextureWithTexCoords(tr, (editor.uiState.style as PlaynodeTimelineStyle).timelineItemBackground, new Rect(.9f, .9f, .1f, .1f));

            Rect br = new Rect(drawRect.x + drawRect.width - bevel, drawRect.y + drawRect.height - bevel, bevel, bevel);
            GUI.DrawTextureWithTexCoords(br, (editor.uiState.style as PlaynodeTimelineStyle).timelineItemBackground, new Rect(.9f, 0f, .1f, .1f));

            Rect bl = new Rect(drawRect.x , drawRect.y + drawRect.height - bevel, bevel, bevel);
            GUI.DrawTextureWithTexCoords(bl, (editor.uiState.style as PlaynodeTimelineStyle).timelineItemBackground, new Rect(0f, 0f, .1f, .1f));



            GUI.color = currentColor;

            EditorGUI.DrawRect(new Rect(drawRect.x, drawRect.y + bevel, drawRect.width, drawRect.height - 2f * bevel), color);


            EditorGUI.DrawRect(new Rect(drawRect.x + bevel, drawRect.y, drawRect.width - 2f*bevel, drawRect.height), color);

        }

   

        private void OnDrawSidebar(Rect area, TimelineUIState uiState)
        {
            PlayNodeDataSource dataSource = uiState.dataSource as PlayNodeDataSource;
            List<TimeLineRowDataItem> rows = dataSource.GetDataRows();
            for (int index = 0; index< rows.Count; index++)
            {
                DrawTrackControl(index, rows[index] as PlaynodeTrackItem, uiState);
            }
            //Rect dividerLine = new Rect(area.x + area.width - 2, area.y, 2, area.height);
            //EditorGUI.DrawRect(dividerLine, new Color32(132, 132, 132, 255));
        }

        private void DrawTrackControl(int index, PlaynodeTrackItem track, TimelineUIState uiState)
        {
            float trackHeight = uiState.calculateRowHeight(uiState, track);
            float trackWidth = uiState.calculateLeftBarHeight(uiState);
            Rect trackControlRect = new Rect(0, (index * trackHeight) - uiState.scrollPosition.y, trackWidth, trackHeight) ;
            //EditorGUI.DrawRect(trackControlRect, new Color32(194, 194, 194, 255));
            GUI.BeginGroup(trackControlRect);

            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = trackControlRect.width / 3f;

            Rect column1 = new Rect(EditorGUIUtility.standardVerticalSpacing,
                EditorGUIUtility.standardVerticalSpacing,
                trackControlRect.width - 2f*EditorGUIUtility.standardVerticalSpacing ,
                trackControlRect.height - (2f * EditorGUIUtility.standardVerticalSpacing));

            /*Rect column3 = new Rect(trackControlRect.width - EditorGUIUtility.singleLineHeight,
            EditorGUIUtility.standardVerticalSpacing,
            (trackControlRect.height / 2f) - (2f * EditorGUIUtility.standardVerticalSpacing),
            trackControlRect.height - (2f * EditorGUIUtility.standardVerticalSpacing));

        Rect column2 = new Rect(column1.width + EditorGUIUtility.standardVerticalSpacing,
            EditorGUIUtility.standardVerticalSpacing,
            trackControlRect.width - column1.width - (2f * EditorGUIUtility.standardVerticalSpacing) - column3.width,
            trackControlRect.height - (2f * EditorGUIUtility.standardVerticalSpacing));*/


            Rect trackTitleRect = new Rect(column1.x + EditorGUIUtility.standardVerticalSpacing, column1.y, column1.width - 32, EditorGUIUtility.singleLineHeight);
            track.trackLabel = EditorGUI.TextArea(trackTitleRect, track.trackLabel);


            Rect trackVolumeRect = new Rect(column1.x, column1.y + trackTitleRect.height + EditorGUIUtility.standardVerticalSpacing, column1.width, EditorGUIUtility.singleLineHeight);
            if (playNode.GetInputPort(track.volumeInNodeName).IsConnected)
                EditorGUI.LabelField(trackVolumeRect, "Volume (Set in Node)");
            else
                track.volume = PlaynodeEditorUtils.DrawSlider(trackVolumeRect, "Volume", track.volume, 0f, 1f, 30f);

            Rect trackPanRect = new Rect(column1.x, trackVolumeRect.y + trackVolumeRect.height + EditorGUIUtility.standardVerticalSpacing, column1.width, EditorGUIUtility.singleLineHeight);
            if (playNode.GetInputPort(track.panInNodeName).IsConnected)
                EditorGUI.LabelField(trackPanRect, "Pan (Set in Node)");
            else
                track.stereoPan = PlaynodeEditorUtils.DrawSlider(trackPanRect, "Pan", track.stereoPan, -1f, 1f, 30f);


            Rect deleteButtonRect = new Rect(trackTitleRect.x + column1.width - 16, trackTitleRect.y+ 2 , 12,12);

            Color guiColor = GUI.color;
            GUI.color = uiState.style.textColor;

            //delete button
            if (uiState.dataSource.GetDataRows().Count != 1)
            {
                if (NodeEditorGUIDraw.ImageButton(deleteButtonRect, (editor.uiState.style as PlaynodeTimelineStyle).deleteIcon))
                {
                    List<TimelineDataItem> items = uiState.dataSource.GetTimelineDataItems();
                    items.RemoveAll(x => x.rowNumber == index);

                    foreach (TimelineDataItem item in items.Where(x=>x.rowNumber > index))
                    {
                        item.rowNumber = item.rowNumber - 1;
                    }

                    uiState.dataSource.ApplyTimelineDataChanges(items);


                    List<TimeLineRowDataItem> rows = uiState.dataSource.GetDataRows();
                    rows.RemoveAt(index);
                    uiState.dataSource.SetDataRows(rows);
                }
            }

            Rect hidebuttonRect = new Rect(trackTitleRect.x + column1.width - 29, trackTitleRect.y + 2, 12, 12);
            //bool newHideValue = EditorGUI.Toggle(hidebuttonRect, new GUIContent("", "Expose this track in the node graph"), track.exposed, (editor.uiState.style as PlaynodeTimelineStyle).exposeInNodeGraphButton);
            bool newHideValue = NodeEditorGUIDraw.ImageToggle(hidebuttonRect, track.exposed, (editor.uiState.style as PlaynodeTimelineStyle).hiddenIcon, (editor.uiState.style as PlaynodeTimelineStyle).visibleIcon);
            if (newHideValue == false && newHideValue != track.exposed)
                track.ClearConnections((uiState.dataSource as PlayNodeDataSource).backingPlaynode);

            track.exposed = newHideValue;

            EditorGUIUtility.labelWidth = labelWidth;

            GUI.color = guiColor;

            GUI.EndGroup();
        }


        private Vector2 lastRightClickCoordinate = new Vector2();
        private int objectSelectorID = -1;
        private bool showAudioSelector = false;
        private bool showMIDISelector = false;
        private void DoItemRightClick(TimelineDataItem mainSelection, int itemIndex, List<TimelineDataItem> selection, Vector2 pixelPosition, Vector2 coordinates)
        {
            lastRightClickCoordinate = coordinates;
            NonBlockingMenu menu = new NonBlockingMenu();
            if (mainSelection != null && 
                ((mainSelection as PlaynodeDataItem).playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Audio
                 || (mainSelection as PlaynodeDataItem).playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.MIDI))
            {
                menu.AddItem(new GUIContent("Edit"), false, () =>
                {
                    TimelineItemEdit.Show(playNode, (PlaynodeDataItem)mainSelection, itemIndex);
                });

                menu.AddItem(new GUIContent("Find in Project"), false, () =>
                {
                    EditorGUIUtility.PingObject((mainSelection as PlaynodeDataItem).GetBackingObject() as Object);
                });

                menu.AddItem(new GUIContent("Reset Length"), false, () => {
                    (mainSelection as PlaynodeDataItem).interiorStartTime = 0;
                    (mainSelection as PlaynodeDataItem).length = (mainSelection as PlaynodeDataItem).DataSourceLength;
                });

                if ((mainSelection as PlaynodeDataItem).playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.MIDI)
                {
                    menu.AddItem(new GUIContent("Open"), false, () =>
                    {
                        MidiTimelineWindow.ShowMIDITimeline((mainSelection as PlaynodeDataItem).GetBackingObject() as MidiFileAsset);
                    });
                }
                menu.AddSeparator("");

            }
            else if (mainSelection != null && (mainSelection as PlaynodeDataItem).playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Event)
            {
                menu.AddItem(new GUIContent("Edit"), false, () =>
                {
                    EventItemEdit.Show(mainSelection,playNode, itemIndex);
                });
                menu.AddSeparator("");
            }
            else if (mainSelection != null && (mainSelection as PlaynodeDataItem).playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Loop)
            {
                menu.AddItem(new GUIContent("Edit"), false, () =>
                {
                    LoopItemEdit.Show(mainSelection,playNode, itemIndex);
                });
                menu.AddSeparator("");
            }



            menu.AddItem(new GUIContent("Add Audio Clip"), false, () => {
                showAudioSelector = true;
            }); menu.AddItem(new GUIContent("Add MIDI File"), false, () => {
                showMIDISelector = true;
            });
            menu.AddItem(new GUIContent("Add Exposed Audio One-shot"), false, () => {
                List<TimelineDataItem> items = editor.uiState.dataSource.GetTimelineDataItems();
                PlaynodeDataItem newItem = new PlaynodeDataItem(lastRightClickCoordinate.x, PlaynodeDataItem.PlaynodeDataItemTypes.Audio, playNode);
                newItem.exposed = true;
                newItem.oneShot = true;
                newItem.eventLabel = "Exposed Audio";
                newItem.rowNumber = (int)lastRightClickCoordinate.y;
                items.Add(newItem);
                editor.uiState.dataSource.ApplyTimelineDataChanges(items);
            });
            menu.AddItem(new GUIContent("Add Exposed MIDI One-shot"), false, () => {
                List<TimelineDataItem> items = editor.uiState.dataSource.GetTimelineDataItems();
                PlaynodeDataItem newItem = new PlaynodeDataItem(lastRightClickCoordinate.x, PlaynodeDataItem.PlaynodeDataItemTypes.MIDI, playNode);
                newItem.exposed = true;
                newItem.oneShot = true;
                newItem.eventLabel = "Exposed MIDI";
                newItem.rowNumber = (int)lastRightClickCoordinate.y;
                items.Add(newItem);
                editor.uiState.dataSource.ApplyTimelineDataChanges(items);
            });
            menu.AddItem(new GUIContent("Add Event"), false, () => {
                List<TimelineDataItem> items = editor.uiState.dataSource.GetTimelineDataItems();
                PlaynodeDataItem newItem = new PlaynodeDataItem(lastRightClickCoordinate.x, PlaynodeDataItem.PlaynodeDataItemTypes.Event, playNode);
                newItem.rowNumber = (int)lastRightClickCoordinate.y;
                items.Add(newItem);
                editor.uiState.dataSource.ApplyTimelineDataChanges(items);
            });
            menu.AddItem(new GUIContent("Add Loop"), false, () => {
                List<TimelineDataItem> items = editor.uiState.dataSource.GetTimelineDataItems();
                PlaynodeDataItem newItem = new PlaynodeDataItem(lastRightClickCoordinate.x, PlaynodeDataItem.PlaynodeDataItemTypes.Loop, playNode);
                newItem.rowNumber = (int)lastRightClickCoordinate.y;
                items.Add(newItem);
                editor.uiState.dataSource.ApplyTimelineDataChanges(items);
            });
            //menu.AddSeparator("");
            //menu.AddItem(new GUIContent("Copy"), false, () => { });
            //menu.AddItem(new GUIContent("Paste"), false, () => { });
            if (editor.uiState.hasSelectedItems || editor.uiState.copyBufferHasItems)
                menu.AddSeparator("");

            if (editor.uiState.hasSelectedItems)
                menu.AddItem(new GUIContent("Copy"), false, () => { editor.uiState.SendSelectionToCopyBuffer(); });

            if (editor.uiState.copyBufferHasItems)
                menu.AddItem(new GUIContent("Paste"), false, () => { editor.uiState.PasteCopyBuffer(); });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Select all items before this time"), false, ()=> { editor.uiState.SelectBeforeMousePosition(lastRightClickCoordinate.x); }) ;
            menu.AddItem(new GUIContent("Select all items after this time"), false, () => { editor.uiState.SelectAfterMousePosition(lastRightClickCoordinate.x); });
            menu.DropDown(new Rect(pixelPosition.x, pixelPosition.y, 1,1));
        }

        

        private void DoObjectSelectors()
        {
            if (showAudioSelector)
            {
                showAudioSelector = false;
                showMIDISelector = false;
                objectSelectorID = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
                EditorGUIUtility.ShowObjectPicker<AudioClip>(null, false, "", objectSelectorID);
            }

            if (showMIDISelector)
            {
                showAudioSelector = false;
                showMIDISelector = false;
                objectSelectorID = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
                EditorGUIUtility.ShowObjectPicker<MidiFileAsset>(null, false, "", objectSelectorID);
            }

            if (EditorGUIUtility.GetObjectPickerControlID() == objectSelectorID && Event.current.commandName == "ObjectSelectorClosed")
            {
                objectSelectorID = -1;
                Object selection = EditorGUIUtility.GetObjectPickerObject();

                if (selection == null)
                    return;

                List<TimelineDataItem> data = editor.uiState.dataSource.GetTimelineDataItems();
                PlaynodeDataItem newItem = null;
                if (selection is AudioClip)
                    newItem = new PlaynodeDataItem(lastRightClickCoordinate.x, (AudioClip)selection, playNode);
                else
                    newItem = new PlaynodeDataItem(lastRightClickCoordinate.x, (MidiFileAsset)selection, playNode);

                if (newItem == null)
                    return;

                newItem.rowNumber = (int)lastRightClickCoordinate.y;
                data.Add(newItem);

                editor.uiState.dataSource.ApplyTimelineDataChanges(data);
            }
        }

        private void OnDestroy()
        {
            playback.Stop(AudioSettings.dspTime);
            ItemEditWindow.CloseLastWindowIfOpen();
        }
    }
}
