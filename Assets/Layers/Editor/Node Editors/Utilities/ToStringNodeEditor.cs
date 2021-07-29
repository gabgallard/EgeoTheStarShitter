using ABXY.Layers.Editor.Node_Editors;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors.Utilities
{
    [CustomNodeEditor(typeof(ToStringNode))]
    public class ToStringNodeEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), target.GetInputPort("input"), target.GetOutputPort("output"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
