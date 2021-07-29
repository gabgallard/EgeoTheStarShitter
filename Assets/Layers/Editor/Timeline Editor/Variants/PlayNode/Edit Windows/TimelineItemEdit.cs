using ABXY.Layers.Runtime.Midi;
using ABXY.Layers.Runtime.Timeline;
using ABXY.Layers.Runtime.Timeline.Playnode;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode
{
    public class TimelineItemEdit : ItemEditWindow
    {
        [SerializeField]
        PlaynodeDataItem timelineObject;

        [SerializeField]
        global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode playNode;
        SerializedProperty timelineObjectProp;


        TimelineDataItem timelineDataItem;

        public static TimelineItemEdit Show(global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode playNode, PlaynodeDataItem item,  int itemIndex)
        {
            CloseLastWindowIfOpen();
            TimelineItemEdit window = EditorWindow.CreateInstance<TimelineItemEdit>();
            window.timelineObject = item;
            window.playNode = playNode;
            window.position = new Rect(window.position.x, window.position.y, 322, 180); 
            window.timelineObjectProp = new SerializedObject(playNode).FindProperty("timelineData").GetArrayElementAtIndex(itemIndex);
            window.minSize = new Vector2(322, 180);
            window.maxSize = new Vector2(322, 180);
            window.ShowUtility();
            lastOpenedEditWindow = window;
            window.timelineDataItem = item;
            return window;
        }
        protected override void TimelineObjectRemovedInternal(TimelineDataItem dataItem)
        {
            if (timelineDataItem == dataItem)
                Close();
        }

        private void OnGUI()
        {
            
            if (timelineObjectProp == null)
            {
                Close();
                return;
            }


            timelineObjectProp.serializedObject.UpdateIfRequiredOrScript();


            this.titleContent = new GUIContent(timelineObject.dataName);

            SerializedProperty exposeProperty = timelineObjectProp.FindPropertyRelative("exposed");
            SerializedProperty oneShot = timelineObjectProp.FindPropertyRelative("oneShot");

            Rect ObjectPropertiesRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            int numObjPropsLines = exposeProperty.boolValue ? 3 : 4;
            ObjectPropertiesRect.height = numObjPropsLines * EditorGUIUtility.singleLineHeight + (numObjPropsLines + 1) * EditorGUIUtility.standardVerticalSpacing;
            GUI.Box(ObjectPropertiesRect, "Object properties", GUI.skin.window);

            if (!exposeProperty.boolValue)
            {
                if (timelineObject.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.Audio)
                    EditorGUILayout.ObjectField(timelineObjectProp.FindPropertyRelative("backingObject"), typeof(AudioClip), new GUIContent("  Audio Clip"));
                else if (timelineObject.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.MIDI)
                    EditorGUILayout.ObjectField(timelineObjectProp.FindPropertyRelative("backingObject"), typeof(MidiFileAsset), new GUIContent("  MIDI File"));


                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.FloatField("  Duration (s)", (float)timelineObject.DataSourceLength);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.PropertyField(timelineObjectProp.FindPropertyRelative("eventLabel"), new GUIContent("  Label"));
            }

            EditorGUILayout.PropertyField(exposeProperty, new GUIContent("  Expose in Node"));

            timelineObjectProp.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();


            Rect timeLinePropertiesRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            int numTimelinePropertiesLines = 2;// !exposeProperty.boolValue || !oneShot.boolValue? 5:3;
            if (exposeProperty.boolValue) numTimelinePropertiesLines++;
            if (!(oneShot.boolValue && exposeProperty.boolValue)) numTimelinePropertiesLines += 2;

            timeLinePropertiesRect.height = numTimelinePropertiesLines * EditorGUIUtility.singleLineHeight + (numTimelinePropertiesLines + 1) * EditorGUIUtility.standardVerticalSpacing;
            GUI.Box(timeLinePropertiesRect, "   Timeline properties", GUI.skin.window);


            float startTime = (float)timelineObject.startTime;
            float newStartTime = EditorGUILayout.FloatField("   Start Time (s)", startTime);
            if (startTime != newStartTime)
            {
                Undo.RecordObject(playNode, timelineObject.dataName + " start time changed");
                timelineObject.startTime = newStartTime;
            }

            if (exposeProperty.boolValue)
            {
                timelineObjectProp.serializedObject.UpdateIfRequiredOrScript();
                EditorGUILayout.PropertyField(oneShot, new GUIContent("   One Shot"));
                timelineObjectProp.serializedObject.ApplyModifiedProperties();
            }

            if (!exposeProperty.boolValue || !oneShot.boolValue)
            {

                float length = (float)timelineObject.length;
                float newLength = EditorGUILayout.FloatField("   Length (s)", length);
                if (length != newLength)
                {
                    Undo.RecordObject(playNode, timelineObject.dataName + " length changed");
                    timelineObject.length = newLength;
                }

                float timeOffset = (float)timelineObject.interiorStartTime;
                float newTimeOffset = EditorGUILayout.FloatField("   Time Offset (s)", timeOffset);
                if (timeOffset != newTimeOffset)
                {
                    Undo.RecordObject(playNode, timelineObject.dataName + " time offset changed");
                    timelineObject.interiorStartTime = newTimeOffset;
                }

            }
           

        }
    }
}
