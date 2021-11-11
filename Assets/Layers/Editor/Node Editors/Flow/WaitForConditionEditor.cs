using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Flow;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(WaitForCondition))]
    public class WaitForConditionEditor : FlowNodeEditor
    {
        NodePort startPort;
        NodePort resetPort;
        NodePort endPort;
        NodePort conditionPort;

        public override void OnCreate()
        {
            base.OnCreate();
            startPort = target.GetInputPort("start");
            resetPort = target.GetInputPort("reset");
            endPort = target.GetOutputPort("end");
            conditionPort = target.GetInputPort("condition");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), startPort, endPort, serializedObjectTree);
            NodeEditorGUIDraw.PortField(layout.DrawLine(), conditionPort, serializedObjectTree);
            NodeEditorGUIDraw.PortField(layout.DrawLine(), resetPort, serializedObjectTree);
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 150;
        }
    }
}
