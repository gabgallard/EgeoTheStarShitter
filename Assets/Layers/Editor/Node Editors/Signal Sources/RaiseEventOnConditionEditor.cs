using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors.Signal_Sources
{
    [CustomNodeEditor(typeof( RaiseEventOnConditionNode))]
    public class RaiseEventOnConditionEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {

            NodeEditorGUIDraw.PortPair(layout.DrawLine(), target.GetInputPort("condition"), target.GetOutputPort("OnTrue"), serializedObjectTree);

            
        }

        public override int GetWidth()
        {
            return 180;
        }
    }
}