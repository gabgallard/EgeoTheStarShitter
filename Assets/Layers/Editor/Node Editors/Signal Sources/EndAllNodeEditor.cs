using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors.Signal_Sources
{
    [NodeEditor.CustomNodeEditorAttribute(typeof(EndAllPlaybackNode))]
    public class EndAllNodeEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            NodeEditorGUIDraw.PortField(layout.DrawLine(), target.GetInputPort("endPlayback"));
        }

        public override int GetWidth()
        {
            return 130;
        }
    }
}