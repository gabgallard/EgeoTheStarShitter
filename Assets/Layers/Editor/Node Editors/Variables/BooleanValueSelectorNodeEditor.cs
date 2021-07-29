using ABXY.Layers.Runtime.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using System.Linq;

namespace ABXY.Layers.Editor.Node_Editors
{
    [CustomNodeEditor(typeof(BooleanValueSelectorNode))]
    public class BooleanValueSelectorNodeEditor : FlowNodeEditor
    {
        //SerializedProperty condition;

        TypedPortGUI value1;

        TypedPortGUI value2;

        TypedPortGUI output;

        //SerializedProperty typeProperty;
        //SerializedProperty arrayTypeProp;

        private static List<string> allowedTypes = new List<string>();
        private static List<string> allowedPrettyNames = new List<string>();

        public override void OnCreate()
        {
            base.OnCreate();
            SerializedPropertyTree condition = serializedObjectTree.FindProperty("condition");
            SerializedPropertyTree typeProperty = serializedObjectTree.FindProperty("typeName");

            SerializedPropertyTree arrayTypeProp = serializedObjectTree.FindProperty("arrayTypeName");

            string inType = typeProperty.stringValue;
            string outType = inType;

            bool isArray = inType == typeof(List<GraphVariable>).FullName;

            string arrayType = "";
            if (isArray)
            {
                outType = ReflectionUtils.FindType(arrayTypeProp.stringValue).MakeArrayType().FullName;
                inType = ReflectionUtils.FindType(arrayTypeProp.stringValue).MakeArrayType().FullName;
                arrayType = inType;
            }
            else
            {
                arrayType = "";
            }

            value1 = new TypedPortGUI(serializedObjectTree.FindProperty("value1"), expectedType: inType, arrayType:arrayType, showBackingValue: Node.ShowBackingValue.Unconnected);
            value1.hideArrayTypeSelector = true;

            value2 = new TypedPortGUI(serializedObjectTree.FindProperty("value2"), expectedType: inType, arrayType: arrayType, showBackingValue: Node.ShowBackingValue.Unconnected);
            value2.hideArrayTypeSelector = true;

            output = new TypedPortGUI(serializedObjectTree.FindProperty("output"), NodePort.IO.Output, expectedType: outType, showBackingValue: Node.ShowBackingValue.Never);

            if (allowedTypes.Count == 0)
            {
                allowedTypes = VariableInspectorUtility.GetManagedTypes(VariableInspectorUtility.EditorFilter.All).Where(x => x != typeof(List<GraphVariable>).FullName).ToList();
                allowedPrettyNames = allowedTypes.Select(x => VariableInspectorUtility.GetPrettyName(x)).ToList();
            }
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedPropertyTree condition = serializedObjectTree.FindProperty("condition");
            SerializedPropertyTree typeProperty = serializedObjectTree.FindProperty("typeName");

            SerializedPropertyTree arrayTypeProp = serializedObjectTree.FindProperty("arrayTypeName");

            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), condition);
            LayersGUIUtilities.DrawTypeSelector(layout.DrawLine(), typeProperty, VariableInspectorUtility.EditorFilter.All);


            bool isArray = typeProperty.stringValue == typeof(List<GraphVariable>).FullName;
            if (isArray)
            {
                LayersGUIUtilities.DrawTypeSelector(layout.DrawLine(), arrayTypeProp, "Array type", allowedTypes, allowedPrettyNames);
                if (arrayTypeProp.stringValue != "")
                {
                    value1.arrayType = arrayTypeProp.stringValue;
                    value2.arrayType = arrayTypeProp.stringValue;
                }
            }
            else
            {
                value1.arrayType = "";
                value2.arrayType = "";
            }
            

            

            value1.expectedType = typeProperty.stringValue;
            value2.expectedType = typeProperty.stringValue;

            if (isArray)
                output.expectedType = arrayTypeProp.stringValue == "" ? "" : ReflectionUtils.FindType(arrayTypeProp.stringValue).MakeArrayType().FullName;
            else
                output.expectedType = typeProperty.stringValue;


            value1.Draw(layout.Draw(value1.CalculateHeight("True Value")), "True Value",targetIsRuntimeGraph);
            value2.Draw(layout.Draw(value2.CalculateHeight("False Value")), "False Value", targetIsRuntimeGraph);
            output.Draw(layout.Draw(output.CalculateHeight("Output")), "Output", targetIsRuntimeGraph);

            serializedObject.ApplyModifiedProperties();
        }
    }
}