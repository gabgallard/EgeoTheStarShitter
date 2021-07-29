using System.Collections.Generic;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Editor.Node_Editors.Signal_Sources;
using ABXY.Layers.Runtime.Timeline;
using ABXY.Layers.ThirdParty.Malee.List;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode
{
    public class EventItemEdit : ItemEditWindow
    {

        [SerializeField]
        global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode playNode;

        ReorderableList parameterList;
        SerializedPropertyTree timelineObjectProp;

        TimelineDataItem timelineDataItem;

        public static EventItemEdit Show(TimelineDataItem target, global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode playNode,  int itemIndex)
        {
            CloseLastWindowIfOpen();
            EventItemEdit window = EditorWindow.CreateInstance<EventItemEdit>();
            window.playNode = playNode;
            window.position = new Rect(window.position.x, window.position.y, 322, 146);
            window.minSize = new Vector2(322, 146);
            window.maxSize = new Vector2(322, 146);
            window.timelineObjectProp = new SerializedObjectTree(playNode).FindProperty("timelineData").GetArrayElementAtIndex(itemIndex);
            window.parameterList = new ReorderableList(window.timelineObjectProp.FindPropertyRelative("eventParameters"));
            window.parameterList.getElementHeightCallback += window.GetEventParameterHeight;
            window.parameterList.drawElementCallback += window.DrawEventParameter;
            window.parameterList.onAddCallback += OnParameterAdd;
            window.timelineDataItem = target;
            window.ShowUtility();
            lastOpenedEditWindow = window;
            return window;
        }

        protected override void TimelineObjectRemovedInternal(TimelineDataItem dataItem)
        {
            if (timelineDataItem == dataItem)
                Close();
        }

        private static void OnParameterAdd(ReorderableList list)
        {
            SerializedProperty newItem = list.AddItem();
            newItem.FindPropertyRelative("name").stringValue = "Parameter-" + Random.Range(1, 1000);
        }

        private float GetEventParameterHeight(SerializedPropertyTree property)
        {
            float emptyHeight = (EditorGUIUtility.singleLineHeight * 1) + (EditorGUIUtility.standardVerticalSpacing * 2f);

            GraphVariableEditor editor = LoadEditor(property);

            if (editor == null)
                return emptyHeight;
            
            return VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(playNode, property, "Value",editor, Runtime.GraphVariable.RetrievalTypes.DefaultValue, playNode.soundGraph.isRunningSoundGraph) + emptyHeight;
        }

        private void DrawEventParameter(Rect position, SerializedPropertyTree property, GUIContent label, bool selected, bool focused)
        {
            GraphInputsEditor.VariablePropertyGroup variablePropertyGroup = GetVariablePropertyGroup(property);
            Rect nameRect = new Rect(position.x, position.y, (position.width / 2f) - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(nameRect, variablePropertyGroup.name, new GUIContent());

            Rect dropdownRect = new Rect(position.x + (position.width / 2f), position.y, position.width / 2f, EditorGUIUtility.singleLineHeight);
            List<string> typeNames = VariableInspectorUtility.GetManagedTypes(VariableInspectorUtility.EditorFilter.All);
            string currentTypeName = variablePropertyGroup.typeName.stringValue;
            int currentSelectionIndex = Mathf.Clamp(typeNames.IndexOf(currentTypeName), 0, int.MaxValue);
            currentSelectionIndex = EditorGUI.Popup(dropdownRect, currentSelectionIndex, typeNames.ToArray());
            variablePropertyGroup.typeName.stringValue = typeNames[currentSelectionIndex];


            GraphVariableEditor editor = LoadEditor(property);

            if (editor == null)
                return;

            float editorHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(playNode, property, "Value", editor, Runtime.GraphVariable.RetrievalTypes.ActualValue, playNode.soundGraph.isRunningSoundGraph);

            //checking for null Values

            Rect propertyRect = new Rect(position.x, position.y + dropdownRect.height + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.standardVerticalSpacing, position.width, editorHeight);
            VariableInspectorDrawFunctions.InputNodeFNs.DrawValue(propertyRect, playNode, "Value", property, editor, Runtime.GraphVariable.RetrievalTypes.ActualValue, playNode.soundGraph.isRunningSoundGraph);


            //EditorGUI.PropertyField(position, property);
        }

        Dictionary<string, GraphInputsEditor.VariablePropertyGroup> variablePropertyGroups = new Dictionary<string, GraphInputsEditor.VariablePropertyGroup>();
        private GraphInputsEditor.VariablePropertyGroup GetVariablePropertyGroup(SerializedPropertyTree property)
        {
            if (!variablePropertyGroups.ContainsKey(property.propertyPath))
                variablePropertyGroups.Add(property.propertyPath, new GraphInputsEditor.VariablePropertyGroup(property));
            return variablePropertyGroups[property.propertyPath];
        }

        private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();
        private GraphVariableEditor LoadEditor(SerializedProperty property)
        {
            GraphVariableEditor editor = null;
            string targetTypeName = property.FindPropertyRelative("typeName").stringValue;
            if (!path2VariableEditor.TryGetValue(property.propertyPath, out editor) || editor.handlesType.FullName != targetTypeName)
            {
                System.Type editorType = VariableInspectorUtility.GetEditorType(targetTypeName, VariableInspectorUtility.EditorFilter.All);
                if (editorType != null)
                {
                    editor = (GraphVariableEditor)System.Activator.CreateInstance(editorType);


                    if (path2VariableEditor.ContainsKey(property.propertyPath))
                        path2VariableEditor[property.propertyPath] = editor;
                    else
                        path2VariableEditor.Add(property.propertyPath, editor);
                }
            }
            return editor;
        }

        private void OnGUI()
        {
            if (parameterList == null)
            {
                Close();
                return;
            }

            parameterList.List.serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty nameProp = timelineObjectProp.FindPropertyRelative("eventLabel");

            titleContent = new GUIContent(nameProp.stringValue == "" ? "Edit Event" : "Edit " + nameProp.stringValue);

            EditorGUILayout.PropertyField(nameProp, new GUIContent("Label"));
            timelineObjectProp.FindPropertyRelative("_startTime").doubleValue 
                = Mathf.Clamp( (float)EditorGUILayout.DoubleField("Time (s) ", timelineObjectProp.FindPropertyRelative("_startTime").doubleValue), 0, float.MaxValue);
            parameterList.DoLayoutList();
            parameterList.List.serializedObject.ApplyModifiedProperties();

            SetHeight();
        }

        private void SetHeight()
        {
            float calculatedHeight = parameterList.GetHeight() + 5f*EditorGUIUtility.standardVerticalSpacing + 2f*EditorGUIUtility.singleLineHeight;
            minSize = new Vector2(322, calculatedHeight);
            maxSize = new Vector2(322, calculatedHeight);
        }
    }
}
