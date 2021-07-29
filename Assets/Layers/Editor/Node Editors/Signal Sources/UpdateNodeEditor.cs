using ABXY.Layers.Editor.Node_Editors;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ABXY.Layers.Editor.Node_Editors.Signal_Sources
{
    [CustomNodeEditor(typeof(UpdateNode))]
    public class UpdateNodeEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            NodeEditorGUIDraw.PortField(layout.DrawLine(), target.GetOutputPort("update"));
        }

        public override int GetWidth()
        {
            return 100;
        }
    }
}