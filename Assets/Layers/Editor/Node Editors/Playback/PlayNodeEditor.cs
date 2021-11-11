using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Playback;
using ABXY.Layers.Runtime.Timeline.Playnode;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Playback
{
    [NodeEditor.CustomNodeEditor(typeof(PlayNode))]
    public class PlayNodeEditor : FlowNodeEditor {

        NodePort playPort = null;
        NodePort playFinishedPort = null;
        NodePort pausePort = null;
        NodePort audioOutPort = null;
        NodePort resumePort = null;
        NodePort midiOutPort = null;
        NodePort stopPort = null;

        SerializedProperty combinedVolume = null;
        SerializedProperty panVolume = null;
        SerializedProperty playFinishedWhen;

        public override void OnCreate()
        {
            base.OnCreate();
            playPort = target.GetInputPort("play");
            playFinishedPort = target.GetOutputPort("playFinished");
            pausePort = target.GetInputPort("pause");
            audioOutPort = target.GetOutputPort("audioOut");
            resumePort = target.GetInputPort("resume");
            midiOutPort = target.GetOutputPort("midiOut");
            stopPort = target.GetInputPort("stop");
            combinedVolume = serializedObject.FindProperty("combinedVolume");
            panVolume = serializedObject.FindProperty("combinedPan");
            playFinishedWhen = serializedObject.FindProperty("playFinishedWhen");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            PlayNode castTarget = target as PlayNode;
            serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty tracksArray = serializedObject.FindProperty("tracks");

            List<PlaynodeDataItem> exposedTrackItems = castTarget.timelineData
                .Where(x => (x.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.MIDI
                || x.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Audio) && x.exposed).ToList();

            List<PlaynodeDataItem> loops = castTarget.timelineData.Where(x => x.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Loop).ToList();




            int mainBusRows =  9;
            Rect mainBusRect = layout.DrawLine();
            mainBusRect = new Rect(mainBusRect.x, mainBusRect.y, mainBusRect.width, EditorGUIUtility.singleLineHeight * mainBusRows + EditorGUIUtility.standardVerticalSpacing * (mainBusRows+1));
            GUI.Box(mainBusRect, "Combined Audio Bus", GUI.skin.window);

            //NodeEditorGUILayout.PortField(new GUIContent("  Volume"), target.GetInputPort("busVolume"));

            NodeEditorGUIDraw.PortPair(layout.DrawLine(),
                new GUIContent("  Play"), playPort, new GUIContent("Play Finished   "), playFinishedPort, serializedObjectTree);

            // NodeEditorGUILayout.PortPair(new GUIContent("  Pause"), pausePort, new GUIContent("Audio Out   "), audioOutPort);

            Rect pauseRect = layout.DrawLine();
            NodeEditorGUIDraw.PortField(pauseRect, new GUIContent("  Pause"), pausePort, serializedObjectTree);

            Rect audioOutRect = new Rect(pauseRect.x + pauseRect.width / 2f, pauseRect.y, pauseRect.width / 2f, pauseRect.height);
            DrawAudioOutSelector(audioOutRect, new GUIContent("Audio Out"), audioOutPort, serializedObject.FindProperty("audioFlowOutID"), 9);


            NodeEditorGUIDraw.PortPair(layout.DrawLine(),new GUIContent("  Resume"), resumePort, new GUIContent("Event Out   "), midiOutPort, serializedObjectTree);
            NodeEditorGUIDraw.PortField(layout.DrawLine(),new GUIContent("  Stop"), stopPort, serializedObjectTree);

            NodeEditorGUIDraw.PropertyField(layout.DrawLine(),combinedVolume,new GUIContent("  Volume"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(),panVolume, new GUIContent("  Pan"));

            LayersGUIUtilities.DrawDropdown(layout.DrawLine(),new GUIContent("  Play Finished"), playFinishedWhen,false);


            //if (!target.GetOutputPort("audioOut").IsConnected)
            //EditorGUILayout.LabelField("  Audio directed to default Audio Out");

            if (GUI.Button(layout.DrawLine(),"Open"))
            {
                //PlaynodeEditorWindow.ShowPlayNodeTimeline(castTarget, castTarget.endTime,  castTarget.timeLine, castTarget.tracks);
                PlayNodeTimelineWindow window = PlayNodeTimelineWindow.ShowPlayNodeTimeline(castTarget);
                window.titleContent = new GUIContent(target.name);
            }

            

            DrawTracks(layout, tracksArray);

            //Drawing loops

            DrawLoops(layout, loops);


            //Drawing tracks
            DrawExposedTrackItems(layout, exposedTrackItems);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTracks(NodeLayoutUtility layout, SerializedProperty tracksArray)
        {
            if (tracksArray.arraySize == 0)
                return;

            for (int index = 0; index < tracksArray.arraySize; index++)
            {
                SerializedProperty trackProperty = tracksArray.GetArrayElementAtIndex(index);
                if (!trackProperty.FindPropertyRelative("exposed").boolValue)
                    continue;
                layout.Draw(EditorGUIUtility.standardVerticalSpacing);
                Rect controlRect = layout.DrawLine();
                controlRect = new Rect(controlRect.x + EditorGUIUtility.singleLineHeight, controlRect.y,
                    controlRect.width - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 5f);
                GUI.Box(controlRect, trackProperty.FindPropertyRelative("trackLabel").stringValue, GUI.skin.window);


                Rect sliderControlRect = layout.Draw(0f);
                //NodeEditorGUILayout.PortField(new GUIContent("Audio Out   "), target.GetOutputPort(trackProperty.FindPropertyRelative("audioOutNodeName").stringValue));
                Rect trackAudioOutRect = layout.DrawLine();
                trackAudioOutRect = new Rect(trackAudioOutRect.x + trackAudioOutRect.width - 105, trackAudioOutRect.y, 105, trackAudioOutRect.height);
                DrawAudioOutSelector(trackAudioOutRect, new GUIContent("Audio Out"), target.GetOutputPort(trackProperty.FindPropertyRelative("audioOutNodeName").stringValue), trackProperty.FindPropertyRelative("audioOutSendName"), 9);
                NodeEditorGUIDraw.PortField(layout.DrawLine(),new GUIContent("Event Out   "), 
                    target.GetOutputPort(trackProperty.FindPropertyRelative("midiOutNodeName").stringValue), serializedObjectTree);


                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 50;
                Rect volumeSliderRect = new Rect(sliderControlRect.x + 20, sliderControlRect.y, sliderControlRect.width - 105f, EditorGUIUtility.singleLineHeight);
                NodePort trackVolumePort = target.GetInputPort(trackProperty.FindPropertyRelative("volumeInNodeName").stringValue);
                if (trackVolumePort.IsConnected)
                    EditorGUI.LabelField(volumeSliderRect, "Volume");
                else
                    trackProperty.FindPropertyRelative("volume").floatValue = PlaynodeEditorUtils.DrawSlider(volumeSliderRect, "Volume", trackProperty.FindPropertyRelative("volume").floatValue, 0f, 1f, 30f);
                NodeEditorGUILayout.PortField(new Vector2(volumeSliderRect.x - 35, volumeSliderRect.y), trackVolumePort);


                Rect panRect = new Rect(sliderControlRect.x + 20, volumeSliderRect.y + volumeSliderRect.height + EditorGUIUtility.standardVerticalSpacing, sliderControlRect.width - 105, EditorGUIUtility.singleLineHeight);
                NodePort trackPanPort = target.GetInputPort(trackProperty.FindPropertyRelative("panInNodeName").stringValue);
                if (trackPanPort.IsConnected)
                    EditorGUI.LabelField(panRect, "Pan");
                else
                    trackProperty.FindPropertyRelative("stereoPan").floatValue = PlaynodeEditorUtils.DrawSlider(panRect, "Pan", trackProperty.FindPropertyRelative("stereoPan").floatValue, -1f, 1f, 30f);
                NodeEditorGUILayout.PortField(new Vector2(panRect.x - 35, panRect.y), trackPanPort);

                EditorGUIUtility.labelWidth = labelWidth;
                
            }
        }

        private void DrawExposedTrackItems(NodeLayoutUtility layout, List<PlaynodeDataItem> exposedTrackItems)
        {
            PlayNode castTarget = target as PlayNode;
            int numExposedTracks = exposedTrackItems.Count();

            layout.Draw(EditorGUIUtility.standardVerticalSpacing);

            if (numExposedTracks != 0)
            {

                Rect exposedTrackItemRect = layout.DrawLine();
                exposedTrackItemRect = new Rect(exposedTrackItemRect.x, exposedTrackItemRect.y,
                    exposedTrackItemRect.width, EditorGUIUtility.singleLineHeight * (numExposedTracks + 1) + EditorGUIUtility.standardVerticalSpacing * (numExposedTracks + 2));
                GUI.Box(exposedTrackItemRect, "Exposed Track Items", GUI.skin.window);

                foreach (PlaynodeDataItem item in exposedTrackItems)
                {
                    NodePort port = target.GetInputPort(item.itemID);
                    if (port != null)
                    {
                        if (item.exposed)
                            NodeEditorGUIDraw.PortField(layout.DrawLine(),new GUIContent("  " + item.eventLabel), port, serializedObjectTree);
                        else
                            port.ClearConnections();
                    }
                }
            }
        }

        private void DrawLoops(NodeLayoutUtility layout, List<PlaynodeDataItem> loops)
        {
            layout.Draw(EditorGUIUtility.standardVerticalSpacing);
            if (loops.Count > 0)
            {
                Rect loopsRect = layout.DrawLine();
                loopsRect = new Rect(loopsRect.x, loopsRect.y,
                    loopsRect.width, EditorGUIUtility.singleLineHeight * (loops.Count + 1) + EditorGUIUtility.standardVerticalSpacing * (loops.Count + 2));
                GUI.Box(loopsRect, "Loops", GUI.skin.window);

                foreach (PlaynodeDataItem loop in loops)
                {
                    NodeEditorGUIDraw.PortField(layout.DrawLine(),new GUIContent("  Can Exit " + loop.eventLabel), target.GetInputPort(loop.itemID), serializedObjectTree);
                }
            }
        }


       

        public override int GetWidth()
        {
            return 270;
        }
    }
}