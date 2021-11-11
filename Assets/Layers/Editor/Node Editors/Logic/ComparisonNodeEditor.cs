using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Logic;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Logic
{
    [NodeEditor.CustomNodeEditor(typeof(Comparison))]
    public class ComparisonNodeEditor : FlowNodeEditor
    {
        NodePort value1Port;
        NodePort resultPort;
        NodePort value2;

        public override void OnCreate()
        {
            base.OnCreate();
            value1Port = target.GetInputPort("value1");
            resultPort = target.GetOutputPort("result");
            value2 = target.GetInputPort("value2");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedPropertyTree comparisonOperatorProp = serializedObject.FindProperty("comparisonOperator");
            NodeEditorGUIDraw.PortField(layout.DrawLine(), value1Port, serializedObjectTree);

            Rect comparisonRect = layout.DrawLine();
            DoDropdown(comparisonRect);

            NodeEditorGUIDraw.AddPortToRect(comparisonRect, resultPort);

            NodeEditorGUIDraw.PortField(layout.DrawLine(), value2, serializedObjectTree);
            serializedObject.ApplyModifiedProperties();
        }

        private void DoDropdown(Rect position)
        {
            SerializedPropertyTree comparisonOperatorProp = serializedObject.FindProperty("comparisonOperator");
            int enumSelection = comparisonOperatorProp.enumValueIndex;
            LayersGUIUtilities.DrawDropdown(position, enumSelection, (target as Comparison).comparisonOperatorPrettyNames, false, (newSelection) =>
            {
                comparisonOperatorProp.enumValueIndex = newSelection;
                comparisonOperatorProp.serializedObject.ApplyModifiedProperties();
            });
        }

        public override int GetWidth()
        {
            return 100;
        }
    }
}
