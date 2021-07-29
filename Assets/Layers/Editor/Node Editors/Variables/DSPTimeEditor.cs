using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Variables;

namespace ABXY.Layers.Editor.Node_Editors.Variables
{
    [CustomNodeEditor(typeof(DSPTimeNode))]
    public class DSPTimeEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            NodeEditorGUIDraw.PortField(layout.DrawLine(),target.GetOutputPort("time"));
        }

        public override int GetWidth()
        {
            return 100;
        }
    }
}
