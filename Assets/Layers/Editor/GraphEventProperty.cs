using ABXY.Layers.Runtime;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor
{
    [CustomPropertyDrawer(typeof(GraphEvent))]
    public class GraphEventProperty : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUIUtility.labelWidth = 0f;
            Rect nameRect = new Rect(position.x, position.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("eventName"),new GUIContent(""));

            SerializedProperty eventList = property.FindPropertyRelative("onGraphEventCalled");
            float height = EditorGUI.GetPropertyHeight(eventList);
            Rect eventListRect = new Rect(position.x, position.y + nameRect.height + EditorGUIUtility.standardVerticalSpacing, position.width, height);
            EditorGUI.PropertyField(eventListRect, eventList);
        }

   

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty eventName = property.FindPropertyRelative("eventName");
            SerializedProperty eventList = property.FindPropertyRelative("onGraphEventCalled");
            return EditorGUI.GetPropertyHeight(eventName) + EditorGUI.GetPropertyHeight(eventList) + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
