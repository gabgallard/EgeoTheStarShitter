using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Variables
{
    [CustomNodeEditor(typeof(VariableNode))]
    public class VariableNodeInspector : FlowNodeEditor
    {
        //SerializedProperty typeNameProp;
        //SerializedProperty variableObjectProp;
        //SerializedProperty isGraphInputProp;
        //SerializedProperty graphVariableIDProp;

        GraphVariableEditor editor = null;

        TypedPortGUI output;

        public override void OnCreate()
        {
            base.OnCreate();
            SerializedPropertyTree variableObjectProp = serializedObjectTree.FindProperty("variableObject");

            System.Type currentVarType = GetCurrentVariableType();

            if (currentVarType == typeof(List<GraphVariable>))
            {
                System.Type arrayType = GetCurrentArrayType();
                if (arrayType != null)
                    currentVarType = arrayType.MakeArrayType();
            }

            output = new TypedPortGUI(serializedObjectTree.FindProperty("value"), NodePort.IO.Output, currentVarType == null ? "" : currentVarType.FullName, showBackingValue: Node.ShowBackingValue.Never);
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();
            SerializedPropertyTree variableObjectProp = serializedObjectTree.FindProperty("variableObject");
            SerializedPropertyTree typeNameProp = variableObjectProp.FindPropertyRelative("typeName");
            SerializedPropertyTree isGraphInputProp = serializedObjectTree.FindProperty("isGraphInput");
            SerializedPropertyTree graphVariableIDProp = serializedObjectTree.FindProperty("graphVariableID");

            LayersGUIUtilities.DrawRightAlignedCheckbox(layout.DrawLine(), isGraphInputProp);

            serializedObjectTree.ApplyModifiedProperties();

            if (isGraphInputProp.boolValue)
            {


                LayersGUIUtilities.DrawVariableSelector(layout.DrawLine(), "Variable", graphVariableIDProp, (target as VariableNode).soundGraph);

                GraphVariable variable = ((target as VariableNode).graph as SoundGraph).GetGraphVariableByID(graphVariableIDProp.stringValue);
                if (variable != null && variable.typeName != typeof(List<GraphVariable>).FullName)
                {
                    object value = ((target as VariableNode).graph as SoundGraph).GetVariableValueByID(graphVariableIDProp.stringValue);
                    EditorGUI.LabelField(layout.DrawLine(), "Value: " + value);
                }
                else if (variable != null)
                    EditorGUI.LabelField(layout.DrawLine(), "Value: Array");
            }
            else
            {

                LayersGUIUtilities.DrawTypeSelector(layout.DrawLine(), typeNameProp, "Type", VariableInspectorUtility.EditorFilter.All);

                serializedObjectTree.ApplyModifiedProperties();



                GraphVariableEditor editor = LoadEditor(variableObjectProp);
                if (editor == null)
                    return;
                float height = VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight(target as FlowNode, variableObjectProp, "Value", editor, GraphVariable.RetrievalTypes.ActualValue, targetIsRuntimeGraph);
                Rect controlRect = layout.Draw(height);
                VariableInspectorDrawFunctions.InputNodeFNs.DrawValue(controlRect, target as FlowNode, "Value", variableObjectProp, editor, GraphVariable.RetrievalTypes.ActualValue, targetIsRuntimeGraph);
            }

            System.Type currentVarType = GetCurrentVariableType();

            if (currentVarType == typeof(List<GraphVariable>))
            {
                System.Type arrayType = GetCurrentArrayType();
                if (arrayType != null)
                    output.expectedType = arrayType.MakeArrayType().FullName;
            }
            else if (currentVarType != null)
                output.expectedType = currentVarType.FullName;


            output.Draw(layout.DrawLine(), "Value", targetIsRuntimeGraph);

            //DoPorts();
            //NodeEditorGUILayout.PortField(target.GetOutputPort("value"));
            serializedObjectTree.ApplyModifiedProperties();
        }

        private void DoPorts()
        {
            System.Type currentVarType = GetCurrentVariableType();
            FlowNode castTarget = (FlowNode)target;


            if (castTarget.NumberDynamicOutputs() == 0)
                castTarget.AddDynamicOutput(currentVarType, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited, "value");

            if (castTarget.GetDynamicOutputPort(0).ValueType != currentVarType)
            {
                castTarget.RemoveDynamicPort(castTarget.GetDynamicOutputPort(0));
                castTarget.AddDynamicOutput(currentVarType, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited, "value");
            }
        }

        private System.Type GetCurrentVariableType()
        {
            SerializedPropertyTree variableObjectProp = serializedObjectTree.FindProperty("variableObject");
            SerializedPropertyTree isGraphInputProp = serializedObjectTree.FindProperty("isGraphInput");
            SerializedPropertyTree graphVariableIDProp = serializedObjectTree.FindProperty("graphVariableID");

            if (isGraphInputProp.boolValue)
            {
                GraphVariable variable = ((target as VariableNode).graph as SoundGraph).GetGraphVariableByID(graphVariableIDProp.stringValue);
                if (variable == null)
                    return null;
                return variable.GetVariableType();
            }
            else
            {
                SerializedProperty varType = variableObjectProp.FindPropertyRelative("typeName");
                return ReflectionUtils.FindType(varType.stringValue);
            }
        }

        private System.Type GetCurrentArrayType()
        {
            SerializedPropertyTree variableObjectProp = serializedObjectTree.FindProperty("variableObject");
            SerializedPropertyTree isGraphInputProp = serializedObjectTree.FindProperty("isGraphInput");
            SerializedPropertyTree graphVariableIDProp = serializedObjectTree.FindProperty("graphVariableID");

            if (isGraphInputProp.boolValue)
            {
                GraphVariable variable = ((target as VariableNode).graph as SoundGraph).GetGraphVariableByID(graphVariableIDProp.stringValue);
                if (variable == null)
                    return null;
                return ReflectionUtils.FindType(variable.arrayType);
            }
            else
            {
                SerializedProperty varType = variableObjectProp.FindPropertyRelative("arrayType");
                return ReflectionUtils.FindType(varType.stringValue);
            }
        }

        private GraphVariableEditor LoadEditor(SerializedProperty property)
        {
            string targetTypeName = property.FindPropertyRelative("typeName").stringValue;
            if (editor == null || editor.handlesType.FullName != targetTypeName)
            {
                System.Type editorType = VariableInspectorUtility.GetEditorType(targetTypeName, VariableInspectorUtility.EditorFilter.InputNode);
                if (editorType != null)
                {
                    editor = (GraphVariableEditor)System.Activator.CreateInstance(editorType);

                }
            }
            return editor;
        }

        public override int GetWidth()
        {
            return 220;
        }
    }
}
