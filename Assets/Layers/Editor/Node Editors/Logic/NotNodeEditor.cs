using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Logic;

namespace ABXY.Layers.Editor.Node_Editors.Logic
{
    [NodeEditor.CustomNodeEditor(typeof(NotNode))]
    public class NotNodeEditor : FlowNodeEditor
    {
        NodePort inputPort;
        NodePort outputPort;

        public override void OnCreate()
        {
            base.OnCreate();
            inputPort = target.GetInputPort("input");
            outputPort = target.GetOutputPort("output");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), inputPort, outputPort, serializedObjectTree);
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 120;
        }
    }
}
