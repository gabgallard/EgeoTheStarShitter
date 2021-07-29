using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(Pin))]
    public class PinEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            NodeEditorGUILayout.PortField(new Vector2(0f, 25f), target.GetInputPort("input"));
            NodeEditorGUILayout.PortField(new Vector2(34f, 25f), target.GetOutputPort("output"));
        }


        public override int GetWidth()
        {
            return 50;
        }
    }
}
