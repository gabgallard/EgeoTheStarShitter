using ABXY.Layers.Runtime.Timeline;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode
{
    public class LoopItemEdit : ItemEditWindow
    {

        [SerializeField]
        global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode playNode;

        SerializedProperty timelineObjectProp;


        TimelineDataItem timelineDataItem;

        public static LoopItemEdit Show(TimelineDataItem target, global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode playNode, int itemIndex)
        {
            CloseLastWindowIfOpen();
            LoopItemEdit window = EditorWindow.CreateInstance<LoopItemEdit>();
            window.playNode = playNode;
            window.position = new Rect(window.position.x, window.position.y, 322, 146);
            window.minSize = new Vector2(322, 146);
            window.maxSize = new Vector2(322, 146);
            window.timelineObjectProp = new SerializedObject(playNode).FindProperty("timelineData").GetArrayElementAtIndex(itemIndex);
            window.ShowUtility();
            window.timelineDataItem = target;
            lastOpenedEditWindow = window;
            return window;
        }
        protected override void TimelineObjectRemovedInternal(TimelineDataItem dataItem)
        {
            if (timelineDataItem == dataItem)
                Close();
        }

        private void OnGUI()
        {
            timelineObjectProp.serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty nameProp = timelineObjectProp.FindPropertyRelative("eventLabel");

            titleContent = new GUIContent(nameProp.stringValue == "" ? "Edit Event" : "Edit " + nameProp.stringValue);

            EditorGUILayout.PropertyField(nameProp, new GUIContent("Label"));

            timelineObjectProp.FindPropertyRelative("_startTime").doubleValue
                = Mathf.Clamp((float)EditorGUILayout.DoubleField("Time (s) ", timelineObjectProp.FindPropertyRelative("_startTime").doubleValue), 0, float.MaxValue);

            timelineObjectProp.FindPropertyRelative("_length").doubleValue
                = Mathf.Clamp((float)EditorGUILayout.DoubleField("Length (s) ", timelineObjectProp.FindPropertyRelative("_length").doubleValue), 0.2f, float.MaxValue);

            timelineObjectProp.serializedObject.ApplyModifiedProperties();

        }

    }
}
