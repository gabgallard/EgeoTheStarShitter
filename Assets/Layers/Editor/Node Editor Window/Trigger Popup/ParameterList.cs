using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editor_Window
{
    public class ParameterList
    {
        SoundGraphEditorWindow editorWindow;



        SerializedObjectTree thisEditor;

        TriggerPopup popup;

        GraphEvent gevent;

        public string windowName { get { return gevent != null ? gevent.eventName : "Event"; } }

        /// <summary>
        /// Dirty hack to make serialization work
        /// </summary>
        [System.Serializable]
        private class ParameterContainer : ScriptableObject {
            public List<GraphVariable> parameters = new List<GraphVariable>();

            public void Load(GraphEvent gevent)
            {
                foreach (GraphEvent.EventParameterDef parameterDefs in gevent.parameters)
                {
                    GraphVariable parameter = new GraphVariable(parameterDefs.parameterName, parameterDefs.parameterTypeName);
                    parameter.variableID = System.Guid.NewGuid().ToString();
                    parameters.Add(new GraphVariable(parameterDefs.parameterName, parameterDefs.parameterTypeName));
                }
            }
        }

        ParameterContainer parameterContainer;


        public ParameterList(SoundGraphEditorWindow editorWindow, TriggerPopup popup)
        {
            this.editorWindow = editorWindow;
            this.popup = popup;
        }

        public void LoadItems(GraphEvent gevent)
        {
            parameterContainer = ScriptableObject.CreateInstance<ParameterContainer>();
            parameterContainer.Load(gevent);
            thisEditor = new SerializedObjectTree(parameterContainer);
            this.gevent = gevent;
        }

        private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();
        public float CalculateParameterListHeight()
        {
            float height = 0f;

            SerializedPropertyTree parametersProperty = thisEditor.FindProperty("parameters");
            for (int index = 0; index < parametersProperty.arraySize; index++)
            {
                SerializedPropertyTree parameterProp = parametersProperty.GetArrayElementAtIndex(index);
                string label = parameterProp.FindPropertyRelative("name").stringValue;
                GraphVariableEditor editor = LoadEditor(parameterProp);
                height += VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(editorWindow, parameterProp, label, editor, GraphVariable.RetrievalTypes.DefaultValue, (editorWindow.graph as SoundGraph).isRunningSoundGraph);
                height += EditorGUIUtility.standardVerticalSpacing;
            }
            height += 2f * EditorGUIUtility.standardVerticalSpacing;
            height += 2f * EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight; //header
            height += 80f;
            return height;
        }

        public void DrawParameterList(Rect drawRect)
        {
            float indentLevel = 14f;
            drawRect = new Rect(drawRect.x + indentLevel, drawRect.y + indentLevel, drawRect.width - (indentLevel * 2f), drawRect.height- indentLevel);

            GUILayout.BeginArea(drawRect);


            thisEditor.UpdateIfRequiredOrScript();

            LayersGUIUtilities.BeginNewLabelWidth(100f);
            SerializedPropertyTree parametersProperty = thisEditor.FindProperty("parameters");
            for (int index = 0; index < parametersProperty.arraySize; index++)
            {
                SerializedPropertyTree parameterProp = parametersProperty.GetArrayElementAtIndex(index);
                GraphVariableEditor editor = LoadEditor(parameterProp);
                string label = parameterProp.FindPropertyRelative("name").stringValue;
                Rect position = EditorGUILayout.GetControlRect(false, VariableInspectorDrawFunctions.PlayerInspectorFNs.GetValueHeight(editorWindow, parameterProp, label, editor, GraphVariable.RetrievalTypes.DefaultValue, (editorWindow.graph as SoundGraph).isRunningSoundGraph));
                VariableInspectorDrawFunctions.PlayerInspectorFNs.DrawValue(position, editorWindow, label, parameterProp, editor, GraphVariable.RetrievalTypes.DefaultValue, (editorWindow.graph as SoundGraph).isRunningSoundGraph);
            }
            LayersGUIUtilities.EndNewLabelWidth();

            EditorGUILayout.Space();

            if (GUILayout.Button("Trigger") || (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Space || Event.current.keyCode == KeyCode.Return)))
            {
                Event.current.Use();
                popup.RunEvent(gevent, parameterContainer.parameters);
            }

            thisEditor.ApplyModifiedProperties();
            GUILayout.EndArea();
        }

        private GraphVariableEditor LoadEditor(SerializedPropertyTree property)
        {
            GraphVariableEditor editor = null;
            string targetTypeName = property.FindPropertyRelative("typeName").stringValue;
            if (!path2VariableEditor.TryGetValue(property.propertyPath, out editor) || editor.handlesType.FullName != targetTypeName)
            {
                System.Type editorType = VariableInspectorUtility.GetEditorType(targetTypeName, VariableInspectorUtility.EditorFilter.Player | VariableInspectorUtility.EditorFilter.InputNode);
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

        public void Back()
        {
            if (popup.eventCount < 2)
                popup.Hide();
            else
                popup.Show();
        }
    }
}