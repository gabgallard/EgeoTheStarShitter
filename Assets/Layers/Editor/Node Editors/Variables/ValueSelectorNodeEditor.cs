using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.ThirdParty.Malee.List;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors
{
    [CustomNodeEditor(typeof(ValueSelectorNode))]
    public class ValueSelectorNodeEditor : FlowNodeEditor
    {
        //SerializedProperty variableType;
        //SerializedProperty arrayType;
        GraphVariableList variableList = null;
        TypedPortGUI outputPort;

        private static List<string> allowedTypes = new List<string>();
        private static List<string> allowedPrettyNames = new List<string>();

        public override void OnCreate()
        {
            base.OnCreate();
            SerializedPropertyTree variableType = serializedObjectTree.FindProperty("variableType");
            SerializedPropertyTree arrayType = serializedObjectTree.FindProperty("arrayType");
            
            variableList = new GraphVariableList(serializedObjectTree.FindProperty("selectionValues"), 
                VariableInspectorDrawFunctions.InputNodeFNs.GetValueHeight,
                VariableInspectorDrawFunctions.InputNodeFNs.DrawValue,
                NodePort.IO.Input, true, variableType.stringValue, Node.ShowBackingValue.Unconnected);
            variableList.arrayTypesHideTypeSelector = true;
            outputPort = new TypedPortGUI(serializedObjectTree.FindProperty("output"), NodePort.IO.Output, expectedType: variableType.stringValue, showBackingValue: Node.ShowBackingValue.Never);;
            
            if (allowedTypes.Count == 0)
            {
                allowedTypes = VariableInspectorUtility.GetManagedTypes(VariableInspectorUtility.EditorFilter.All).Where(x => x != typeof(List<GraphVariable>).FullName).ToList();
                allowedPrettyNames = allowedTypes.Select(x => VariableInspectorUtility.GetPrettyName(x)).ToList();
            }
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();

            SerializedPropertyTree variableType = serializedObjectTree.FindProperty("variableType");
            SerializedPropertyTree arrayType = serializedObjectTree.FindProperty("arrayType");

            LayersGUIUtilities.BeginNewLabelWidth(100);
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObjectTree.FindProperty("selectedBranch"), new GUIContent("Selected Value"));
            LayersGUIUtilities.EndNewLabelWidth();

            LayersGUIUtilities.DrawTypeSelector(layout.DrawLine(), variableType, "Type",VariableInspectorUtility.EditorFilter.All);

            if (variableType.stringValue == typeof(List<GraphVariable>).FullName)
                LayersGUIUtilities.DrawTypeSelector(layout.DrawLine(), arrayType, "Array Type", allowedTypes, allowedPrettyNames);
            else
                arrayType.stringValue = "";

            variableList.typeConstraint = variableType.stringValue;
            variableList.arrayTypeConstraint = arrayType.stringValue;

            variableList.DoList((layout.Draw(variableList.GetHeight())), new GUIContent("Selection Values"));


            if (variableType.stringValue == typeof(List<GraphVariable>).FullName)
            {
                System.Type arrayElementType = ReflectionUtils.FindType(arrayType.stringValue);
                if (arrayElementType != null)
                    outputPort.expectedType = arrayElementType.MakeArrayType().FullName;

            }
            else
                outputPort.expectedType = variableType.stringValue;

            outputPort.Draw(layout.DrawLine(), "Output", targetIsRuntimeGraph);

            serializedObjectTree.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 240;
        }

    }

}