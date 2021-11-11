using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.ThirdParty.Malee.List;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors
{
    [CustomNodeEditor(typeof(SwitchValueSelector))]
    public class SwitchValueSelectorNodeInspector : FlowNodeEditor
    {
        private Dictionary<string, GraphVariableEditor> path2VariableEditor = new Dictionary<string, GraphVariableEditor>();
        ReorderableList switchElementsValue = null;
        public override void OnCreate()
        {
            base.OnCreate();
            switchElementsValue = new ReorderableList(serializedObject.FindProperty("switchElements"));
            switchElementsValue.drawElementCallback += DrawSwitchElement;
            switchElementsValue.getElementHeightCallback += CalculateElementHeight;
            switchElementsValue.onAddCallback += OnAddElement;
            switchElementsValue.onRemoveCallback += OnRemoveElement;
            RecalculatePorts();
        }


        public override void OnBodyGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            SerializedPropertyTree comparisonType = serializedObject.FindProperty("comparisonType");
            LayersGUIUtilities.DrawTypeSelector(layout.DrawLine(), comparisonType, "Selector Type",VariableInspectorUtility.EditorFilter.All, RecalculatePorts);
            if (comparisonType.stringValue == typeof(List<GraphVariable>).FullName)
                LayersGUIUtilities.DrawTypeSelector(layout.DrawLine(), serializedObject.FindProperty("comparisonArrayType"), "Selector Array Type", VariableInspectorUtility.EditorFilter.All, RecalculatePorts);

            SerializedPropertyTree elementType = serializedObject.FindProperty("elementType");
            LayersGUIUtilities.DrawTypeSelector(layout.DrawLine(), elementType, "Value Type", VariableInspectorUtility.EditorFilter.All, RecalculatePorts);
            if (elementType.stringValue == typeof(List<GraphVariable>).FullName)
                LayersGUIUtilities.DrawTypeSelector(layout.DrawLine(), serializedObject.FindProperty("elementArrayType"), "Value Array Type", VariableInspectorUtility.EditorFilter.All, RecalculatePorts);

            switchElementsValue.DoList(layout.Draw(switchElementsValue.GetHeight()), new GUIContent("Switch Values"));

            NodeEditorGUIDraw.PortPair(layout.DrawLine(), new GUIContent("Selector"), target.GetInputPort("Input"), 
                new GUIContent("Output"), target.GetOutputPort("Output"), serializedObjectTree);

            serializedObject.ApplyModifiedProperties();
        }
        private void OnAddElement(ReorderableList list)
        {
            SerializedPropertyTree comparisonArrayType = serializedObject.FindProperty("comparisonArrayType");
            SerializedPropertyTree valueArrayType = serializedObject.FindProperty("elementArrayType");


            SerializedPropertyTree newElement = list.AddItem();
            newElement.FindPropertyRelative("comparisonValue").FindPropertyRelative("variableID").stringValue = System.Guid.NewGuid().ToString();
            newElement.FindPropertyRelative("comparisonValue").FindPropertyRelative("typeName").stringValue = serializedObject.FindProperty("comparisonType").stringValue;

            newElement.FindPropertyRelative("value").FindPropertyRelative("variableID").stringValue = System.Guid.NewGuid().ToString();
            newElement.FindPropertyRelative("value").FindPropertyRelative("typeName").stringValue = serializedObject.FindProperty("elementType").stringValue;
            serializedObject.ApplyModifiedProperties();

            string comparisonType = newElement.FindPropertyRelative("comparisonValue").FindPropertyRelative("typeName").stringValue;
            comparisonType = GetFinalType(comparisonType, comparisonArrayType.stringValue);
            string comparisonID = newElement.FindPropertyRelative("comparisonValue").FindPropertyRelative("variableID").stringValue;
            string valueType = newElement.FindPropertyRelative("value").FindPropertyRelative("typeName").stringValue;
            valueType = GetFinalType(valueType, valueArrayType.stringValue);
            string valueID = newElement.FindPropertyRelative("value").FindPropertyRelative("variableID").stringValue;




            target.AddDynamicInput(ReflectionUtils.FindType(comparisonType), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, comparisonID);
            target.AddDynamicInput(ReflectionUtils.FindType(valueType), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, valueID);

            serializedObject.Update();
        }

        private void OnRemoveElement(ReorderableList list)
        {
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            foreach (int selection in list.Selected)
            {
                SerializedPropertyTree property = list.List.GetArrayElementAtIndex(selection);
                string comparisonID = property.FindPropertyRelative("comparisonValue").FindPropertyRelative("variableID").stringValue;
                string valueID = property.FindPropertyRelative("value").FindPropertyRelative("variableID").stringValue;
                target.RemoveDynamicPort(comparisonID);
                target.RemoveDynamicPort(valueID);
            }
            serializedObject.Update();
            list.Remove(list.Selected);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSwitchElement(Rect rect, SerializedPropertyTree element, GUIContent label, bool selected, bool focused)
        {
            NodeLayoutUtility elementLayout = new NodeLayoutUtility(rect);

            SerializedPropertyTree comparisonValueProp = element.FindPropertyRelative("comparisonValue");

            string comparisonID = element.FindPropertyRelative("comparisonValue").FindPropertyRelative("variableID").stringValue;
            string valueID = element.FindPropertyRelative("value").FindPropertyRelative("variableID").stringValue;

            NodePort comparisonPort = target.GetInputPort(comparisonID);
            NodePort valuePort = target.GetInputPort(valueID);

            // setting propertyType
            comparisonValueProp.FindPropertyRelative("typeName").stringValue = serializedObject.FindProperty("comparisonType").stringValue;
            Rect selectorPosition = new Rect();
            if (!comparisonPort.IsConnected)
            {
                GraphVariableEditor editorcomparisonValue = LoadEditor(comparisonValueProp);
                float comparisonEditorHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(target as FlowNode, comparisonValueProp, "Selector", editorcomparisonValue, GraphVariable.RetrievalTypes.DefaultValue, targetIsRuntimeGraph);
                selectorPosition = elementLayout.Draw(comparisonEditorHeight);
                VariableInspectorDrawFunctions.InputNodeFNs.DrawValue(selectorPosition, target, "Selector", comparisonValueProp, editorcomparisonValue, GraphVariable.RetrievalTypes.DefaultValue, targetIsRuntimeGraph);
            }
            else
            {
                selectorPosition = elementLayout.DrawLine();
                EditorGUI.LabelField(selectorPosition, "Selector");
            }

            SerializedPropertyTree elementValueProp = element.FindPropertyRelative("value");
            // setting propertyType
            elementValueProp.FindPropertyRelative("typeName").stringValue = serializedObject.FindProperty("elementType").stringValue;
            Rect elementPosition = new Rect();
            if (!valuePort.IsConnected)
            {
                GraphVariableEditor editorElementValue = LoadEditor(elementValueProp);
                float elementEditorHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(target as FlowNode, elementValueProp, "Value", editorElementValue, GraphVariable.RetrievalTypes.DefaultValue, targetIsRuntimeGraph);
                elementPosition = elementLayout.Draw(elementEditorHeight);
                VariableInspectorDrawFunctions.InputNodeFNs.DrawValue(elementPosition, target, "Value", elementValueProp, editorElementValue, GraphVariable.RetrievalTypes.DefaultValue, targetIsRuntimeGraph);
            }
            else
            {
                elementPosition = elementLayout.DrawLine();
                EditorGUI.LabelField(elementPosition, "Value");
            }


            PortRenderer.current.DrawPort(new Vector2(selectorPosition.x - 50, selectorPosition.y), comparisonPort);
            PortRenderer.current.DrawPort(new Vector2(elementPosition.x - 50, elementPosition.y), valuePort);
        }

        private float CalculateElementHeight(SerializedPropertyTree element)
        {
            string comparisonID = element.FindPropertyRelative("comparisonValue").FindPropertyRelative("variableID").stringValue;
            string valueID = element.FindPropertyRelative("value").FindPropertyRelative("variableID").stringValue;

            NodePort comparisonPort = target.GetInputPort(comparisonID);
            NodePort valuePort = target.GetInputPort(valueID);


            SerializedPropertyTree comparisonValueProp = element.FindPropertyRelative("comparisonValue");
            // setting propertyType
            comparisonValueProp.FindPropertyRelative("typeName").stringValue = serializedObject.FindProperty("comparisonType").stringValue;
            GraphVariableEditor editorcomparisonValue = LoadEditor(comparisonValueProp);
            float comparisonEditorHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(target as FlowNode, comparisonValueProp, "Selector", editorcomparisonValue, GraphVariable.RetrievalTypes.DefaultValue, targetIsRuntimeGraph);

            SerializedPropertyTree elementValueProp = element.FindPropertyRelative("value");
            // setting propertyType
            elementValueProp.FindPropertyRelative("typeName").stringValue = serializedObject.FindProperty("elementType").stringValue;
            GraphVariableEditor editorElementValue = LoadEditor(elementValueProp);
            float elementEditorHeight = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(target as FlowNode, elementValueProp, "Value", editorElementValue, GraphVariable.RetrievalTypes.DefaultValue, targetIsRuntimeGraph);

            comparisonEditorHeight = comparisonPort.IsConnected ? EditorGUIUtility.singleLineHeight: comparisonEditorHeight;
            elementEditorHeight = valuePort.IsConnected ? EditorGUIUtility.singleLineHeight  : elementEditorHeight;

            return comparisonEditorHeight + elementEditorHeight + 2f* EditorGUIUtility.standardVerticalSpacing;
        }

        private void RecalculatePorts()
        {


            SerializedPropertyTree comparisonArrayType = serializedObject.FindProperty("comparisonArrayType");
            SerializedPropertyTree valueArrayType = serializedObject.FindProperty("elementArrayType");
            for (int index = 0; index < switchElementsValue.List.arraySize; index++)
            {
                SerializedPropertyTree property = switchElementsValue.List.GetArrayElementAtIndex(index);
                //Array Values
                property.FindPropertyRelative("comparisonValue").FindPropertyRelative("arrayType").stringValue = comparisonArrayType.stringValue;
                property.FindPropertyRelative("value").FindPropertyRelative("arrayType").stringValue = valueArrayType.stringValue;
                
            }
            serializedObject.ApplyModifiedProperties();

            NodePort inputPort = target.GetInputPort("Input");

            //Setting up input port
            string comparisonType = GetFinalType(serializedObject.FindProperty("comparisonType").stringValue,comparisonArrayType.stringValue);
            if (inputPort != null && inputPort.ValueType.FullName != comparisonType)
            {
                target.RemoveDynamicPort(inputPort);
                inputPort = null;
            }
            if (inputPort == null)
                target.AddDynamicInput(ReflectionUtils.FindType(comparisonType), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, "Input");


            NodePort outputPort = target.GetOutputPort("Output");

            //Setting up output port
            string valueType = GetFinalType( serializedObject.FindProperty("elementType").stringValue, valueArrayType.stringValue);
            if (outputPort != null && outputPort.ValueType.FullName != valueType)
            {
                target.RemoveDynamicPort(outputPort);
                outputPort = null;
            }
            if (outputPort == null)
                target.AddDynamicOutput(ReflectionUtils.FindType(valueType), Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited, "Output");


            //Setting up element ports
            for (int index = 0; index < switchElementsValue.List.arraySize; index++)
            {
                SerializedPropertyTree property = switchElementsValue.List.GetArrayElementAtIndex(index);

                property.FindPropertyRelative("comparisonValue").FindPropertyRelative("typeName").stringValue = serializedObject.FindProperty("comparisonType").stringValue;
                property.FindPropertyRelative("value").FindPropertyRelative("typeName").stringValue = serializedObject.FindProperty("elementType").stringValue;

                

                string elementComparisonType = property.FindPropertyRelative("comparisonValue").FindPropertyRelative("typeName").stringValue;
                elementComparisonType = GetFinalType(elementComparisonType, comparisonArrayType.stringValue);
                string elementComparisonID = property.FindPropertyRelative("comparisonValue").FindPropertyRelative("variableID").stringValue;
                string elementValueType = property.FindPropertyRelative("value").FindPropertyRelative("typeName").stringValue;
                elementValueType = GetFinalType(elementValueType, valueArrayType.stringValue);
                string elementValueID = property.FindPropertyRelative("value").FindPropertyRelative("variableID").stringValue;

                NodePort comparisonPort = target.GetInputPort(elementComparisonID);

                if (comparisonPort != null && comparisonPort.ValueType.FullName != elementComparisonType)
                {
                    target.RemoveDynamicPort(comparisonPort);
                    comparisonPort = null;
                }
                if (comparisonPort == null)
                    target.AddDynamicInput(ReflectionUtils.FindType(elementComparisonType), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, elementComparisonID);


                NodePort valuePortPort = target.GetInputPort(elementValueID);

                if (valuePortPort != null && valuePortPort.ValueType.FullName != elementValueType)
                {
                    target.RemoveDynamicPort(valuePortPort);
                    valuePortPort = null;
                }
                if (valuePortPort == null)
                    target.AddDynamicInput(ReflectionUtils.FindType(elementValueType), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, elementValueID);

            }
            serializedObject.Update();
        }

        private GraphVariableEditor LoadEditor(SerializedPropertyTree property)
        {
            GraphVariableEditor editor = null;
            string targetTypeName = property.FindPropertyRelative("typeName").stringValue;
            if (!path2VariableEditor.TryGetValue(property.propertyPath, out editor) || editor.handlesType.FullName != targetTypeName)
            {
                System.Type editorType = VariableInspectorUtility.GetEditorType(targetTypeName, VariableInspectorUtility.EditorFilter.InputNode);
                if (editorType != null)
                {
                    editor = (GraphVariableEditor)System.Activator.CreateInstance(editorType);

                    if (editor is ArrayVariableEditor)
                        (editor as ArrayVariableEditor).showTypeSelector = false;
                    
                    if (path2VariableEditor.ContainsKey(property.propertyPath))
                        path2VariableEditor[property.propertyPath] = editor;
                    else
                        path2VariableEditor.Add(property.propertyPath, editor);
                }
            }
            return editor;
        }

        /// <summary>
        /// Converts to an array type if needed
        /// </summary>
        /// <returns></returns>
        private string GetFinalType(string typeName, string arrayElementType)
        {
            if (typeName == typeof(List<GraphVariable>).FullName)
            {
                return ReflectionUtils.FindType(arrayElementType).MakeArrayType().FullName;
            }
            else
                return typeName;
        }

        public override int GetWidth()
        {
            SerializedPropertyTree comparisonType = serializedObject.FindProperty("comparisonType");
            SerializedPropertyTree elementType = serializedObject.FindProperty("elementType");
            if (comparisonType.stringValue == typeof(List<GraphVariable>).FullName || elementType.stringValue == typeof(List<GraphVariable>).FullName)
                return 300;
            return 200;
        }
    }
}