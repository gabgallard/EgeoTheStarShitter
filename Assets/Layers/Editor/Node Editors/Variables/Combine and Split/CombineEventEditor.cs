using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.ThirdParty.Malee.List;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using UnityEditor;
using UnityEngine;
/*
namespace ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split
{
    [CustomNodeEditor(typeof(CombineEvent))]
    public class CombineEventEditor : FlowNodeEditor
    {
        NodePort midiInfo;
        ReorderableList parameterList;

        private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();

        public override void OnCreate()
        {
            base.OnCreate();
            midiInfo = target.GetInputPort("MIDIData");
            parameterList = new ReorderableList(serializedObject.FindProperty("parameters"));
            parameterList.drawElementCallback += DrawParameter;
            parameterList.getElementHeightCallback += CalculateParameterHeight;
            parameterList.onAddCallback += OnAddParameterElement;
        }

    

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.Update();
            SetupPorts();
            NodeEditorGUILayout.PortPair(target.GetInputPort("input"), target.GetOutputPort("output"));

            LayersGUIUtilities.DrawExpandableProperty(midiInfo,serializedObject);

            LayersGUIUtilities.DrawExpandableProperty(parameterList.List.arraySize != 0, serializedObject, () => {
                parameterList.DoLayoutList();
            });
            serializedObject.ApplyModifiedProperties();
        }

        protected override bool CanExpand()
        {
            return true;
        }


        private void DrawParameter(Rect position, SerializedProperty property, GUIContent label, bool selected, bool focused)
        {
            Rect nameRect = new Rect(position.x, position.y, (position.width / 2f) - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), new GUIContent());

            SerializedProperty variableID = property.FindPropertyRelative("variableID");
            NodeEditorGUILayout.PortField(new Vector2(nameRect.x - 45, nameRect.y + EditorGUIUtility.standardVerticalSpacing), target.GetInputPort(variableID.stringValue));


            Rect dropdownRect = new Rect(position.x + (position.width / 2f), position.y, position.width / 2f, EditorGUIUtility.singleLineHeight);
        
            List<string> typeNames = VariableInspectorUtility.GetManagedTypes(VariableInspectorUtility.EditorFilter.All);
            List<string> prettyNames = typeNames.Select(x => VariableInspectorUtility.GetPrettyName(x)).ToList();

            string currentTypeName = property.FindPropertyRelative("typeName").stringValue;
            int currentSelectionIndex = typeNames.IndexOf(currentTypeName);
            if (currentSelectionIndex < 0)
            {
                currentSelectionIndex = 0;
                property.FindPropertyRelative("typeName").stringValue = typeNames[currentSelectionIndex];
                property.FindPropertyRelative("typeName").serializedObject.ApplyModifiedProperties();
            }

            LayersGUIUtilities.DrawDropdown(dropdownRect,currentSelectionIndex, prettyNames.ToArray(), false, (newSelection) => {
                property.FindPropertyRelative("typeName").stringValue = typeNames[newSelection];
                property.FindPropertyRelative("typeName").serializedObject.ApplyModifiedProperties();
            });

            GraphVariableEditor editor = LoadEditor(property);

            NodePort inputPort = target.GetInputPort(variableID.stringValue);

            if (editor == null || (inputPort != null && target.GetInputPort(variableID.stringValue).IsConnected))
                return;

            float editorHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetActualValueHeight(target as FlowNode, property, editor);

            //checking for null Values

            Rect propertyRect = new Rect(position.x, position.y + dropdownRect.height + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.standardVerticalSpacing, position.width, editorHeight);
            VariableInspectorDrawFunctions.InputNodeFNs.DrawActualValue(propertyRect, "Value", target as FlowNode, property, editor);

       
        }

        private float CalculateParameterHeight(SerializedProperty property)
        {

            float emptyHeight = (EditorGUIUtility.singleLineHeight * 1) + (EditorGUIUtility.standardVerticalSpacing * 3f);

            GraphVariableEditor editor = LoadEditor(property);

            NodePort parameterPort = target.GetInputPort(property.FindPropertyRelative("variableID").stringValue);

            if (editor == null || property == null || (parameterPort != null && parameterPort.IsConnected))
                return emptyHeight;

            return VariableInspectorDrawFunctions.InputNodeFNs.GetDefaultValueHeight(target as FlowNode, property,editor) + emptyHeight;
        }

        private void OnAddParameterElement(ReorderableList list)
        {

            SerializedProperty prop = list.AddItem();
            SerializedProperty eventName = prop.FindPropertyRelative("name");
            eventName.stringValue = "Parameter-" + Random.Range(1, 1000);

            SerializedProperty variableID = prop.FindPropertyRelative("variableID");
            variableID.stringValue = System.Guid.NewGuid().ToString();
        }

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

        private void SetupPorts()
        {
            List<string> fieldNames = new List<string>();
        
            for (int index = 0; index < parameterList.List.arraySize; index++)
            {
                SerializedProperty graphVariable = parameterList.List.GetArrayElementAtIndex(index);
                string variableID = graphVariable.FindPropertyRelative("variableID").stringValue;

                string typeName = graphVariable.FindPropertyRelative("typeName").stringValue;
                System.Type type = ReflectionUtils.FindType(typeName);
                if (type == null)
                    continue;


                fieldNames.Add(variableID);
                if (target.GetInputPort(variableID) == null)
                    target.AddDynamicInput(type, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, variableID);
                else if (target.GetInputPort(variableID).ValueType != type)
                    target.RemoveDynamicPort(variableID);
            }

            List<string> portnames = target.DynamicPorts.Select(x => x.fieldName).ToList();
            foreach (string nodeName in portnames)
            {
                if (!fieldNames.Contains(nodeName))
                    target.RemoveDynamicPort(nodeName);
            }
        }

        public override int GetWidth()
        {
            return 300;
        }
    }
}
*/