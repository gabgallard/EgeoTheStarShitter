using System.Collections.Generic;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Editor.Node_Editors;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor
{
    public class CallEventWithParametersWindow : EditorWindow
    {
        //SoundGraph soundGraph, GraphEvent gevent

        [SerializeField]
        private List<GraphVariable> parameters = new List<GraphVariable>();

        SerializedObjectTree thisEditor;

        private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();

        SoundGraph soundGraph;


        string eventName;

        public static void Display(Rect buttonPosition, SoundGraph soundGraph, GraphEvent gevent)
        {
            // Get existing open window or if none, make a new one:
            CallEventWithParametersWindow window = ScriptableObject.CreateInstance<CallEventWithParametersWindow>();


            window.thisEditor = new SerializedObjectTree(window);
            foreach (GraphEvent.EventParameterDef parameterDefs in gevent.parameters)
            {
                GraphVariable parameter = new GraphVariable(parameterDefs.parameterName, parameterDefs.parameterTypeName);
                parameter.variableID = System.Guid.NewGuid().ToString();
                window.parameters.Add(new GraphVariable(parameterDefs.parameterName, parameterDefs.parameterTypeName));
            }
            window.soundGraph = soundGraph;
            window.eventName = gevent.eventName;
            window.ShowPopup();
            window.minSize = new Vector2(0f, 0f);
            window.position = new Rect(buttonPosition.x, buttonPosition.y, 200f, window.CalculateHeight());
        }

        private void OnGUI()
        {
            if (thisEditor == null)
                this.Close();

            Rect windowRect = new Rect(0, 0, position.width, position.height);
            EditorGUI.DrawRect(windowRect, 0.6f * ((Color)FlowNodeEditor.style.nodeBackgroundColor));

            Rect innerColorRect = new Rect(windowRect.x+1, windowRect.y+1, windowRect.width-3, windowRect.height-3);
            EditorGUI.DrawRect(innerColorRect, FlowNodeEditor.style.nodeBackgroundColor);

            Rect headerRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight +2f * EditorGUIUtility.standardVerticalSpacing);
            Rect eventNameRect = new Rect(headerRect.x, headerRect.y+2, headerRect.width * 0.70f, headerRect.height);
            Color headerColor = 0.8f * ((Color)FlowNodeEditor.style.nodeBackgroundColor);

            Rect headerColorRect = new Rect(headerRect.x-3, headerRect.y-1, headerRect.width+9, headerRect.height+2);
            EditorGUI.DrawRect(headerColorRect, headerColor);
            EditorGUI.LabelField(eventNameRect, eventName);

            thisEditor.UpdateIfRequiredOrScript();

            LayersGUIUtilities.BeginNewLabelWidth(100f);
            SerializedPropertyTree parametersProperty = thisEditor.FindProperty("parameters");
            for (int index = 0; index < parametersProperty.arraySize; index++)
            {
                SerializedPropertyTree parameterProp = parametersProperty.GetArrayElementAtIndex(index);
                GraphVariableEditor editor = LoadEditor(parameterProp);
                string label = parameterProp.FindPropertyRelative("name").stringValue;
                Rect position = EditorGUILayout.GetControlRect(false, VariableInspectorDrawFunctions.PlayerInspectorFNs.GetValueHeight(this,parameterProp, label, editor, GraphVariable.RetrievalTypes.DefaultValue, true));
                VariableInspectorDrawFunctions.PlayerInspectorFNs.DrawValue(position, this, label, parameterProp, editor, GraphVariable.RetrievalTypes.DefaultValue, true);
            }
            LayersGUIUtilities.EndNewLabelWidth();

            Rect buttonRect = new Rect(headerRect.x + (headerRect.width * 0.70f), headerRect.y + EditorGUIUtility.standardVerticalSpacing
                , headerRect.width * 0.30f, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(buttonRect, "Trigger"))
            {
                Dictionary<string, object> parametersOut = new Dictionary<string, object>();
                foreach (GraphVariable parameter in parameters)
                    parametersOut.Add(parameter.name, parameter.DefaultValue());


                soundGraph.CallEvent(eventName, AudioSettings.dspTime, parametersOut, 0);
            }


            thisEditor.ApplyModifiedProperties();
            position = new Rect(position.x, position.y, 200f, CalculateHeight());


            if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow is NodeEditorWindow)
                Close();
        }

        private float CalculateHeight()
        {
            float height = 0f;

            SerializedPropertyTree parametersProperty = thisEditor.FindProperty("parameters");
            for (int index = 0; index < parametersProperty.arraySize; index++)
            {
                SerializedPropertyTree parameterProp = parametersProperty.GetArrayElementAtIndex(index);
                string label = parameterProp.FindPropertyRelative("name").stringValue;
                GraphVariableEditor editor = LoadEditor(parameterProp);
                height += VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(this, parameterProp, label,editor, GraphVariable.RetrievalTypes.DefaultValue, true);
                height += EditorGUIUtility.standardVerticalSpacing;
            }
            height += 2f * EditorGUIUtility.standardVerticalSpacing;
            height += 2f * EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight; //header
            return height;
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
    }
}
