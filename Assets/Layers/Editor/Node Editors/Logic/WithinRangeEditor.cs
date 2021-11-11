using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors.Logic
{
    [NodeEditor.CustomNodeEditor(typeof(WithinRangeNode))]
    public class WithinRangeEditor : FlowNodeEditor
    {
        SerializedPropertyTree intMin;
        SerializedPropertyTree intMax;
        SerializedPropertyTree intValue;
        NodePort output;

        public override void OnCreate()
        {
            base.OnCreate();
            intMin = serializedObjectTree.FindProperty("intMin");
            intMax = serializedObjectTree.FindProperty("intMax");
            intValue = serializedObjectTree.FindProperty("intValue");
            output = target.GetOutputPort("output");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();

            LayersGUIUtilities.BeginNewLabelWidth(40);
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), intValue, new GUIContent("Value"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), intMin, new GUIContent("Min"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), intMax, new GUIContent("Max"));
            NodeEditorGUIDraw.PortField(layout.DrawLine(), output, serializedObjectTree);
            LayersGUIUtilities.EndNewLabelWidth();
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 110;
        }
    }
}