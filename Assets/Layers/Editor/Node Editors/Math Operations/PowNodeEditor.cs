using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(Pow))]

    public class PowNodeEditor : FlowNodeEditor
    {
        SerializedPropertyTree power;
        SerializedPropertyTree input;
        NodePort outputPort;
        public override void OnCreate()
        {
            base.OnCreate();

            power = serializedObject.FindProperty("power");
            input = serializedObject.FindProperty("input");
            outputPort = target.GetOutputPort("output");
        }

        public override void OnBodyGUI()
        {
            serializedObjectTree.UpdateIfRequiredOrScript();
            LayersGUIUtilities.BeginNewLabelWidth(40f);
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), input);
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(),power);
            NodeEditorGUIDraw.PortField(layout.DrawLine(), outputPort, serializedObjectTree);



            LayersGUIUtilities.EndNewLabelWidth();
            serializedObjectTree.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 110;
        }
    }
}