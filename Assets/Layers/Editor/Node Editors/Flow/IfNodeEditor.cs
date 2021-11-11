using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Flow;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(IfNode))]
    public class IfNodeEditor : FlowNodeEditor
    {
        NodePort evaluatePort;
        NodePort onTruePort;
        NodePort conditionPort;
        NodePort onFalsePort;

        public override void OnCreate()
        {
            base.OnCreate();
            evaluatePort = target.GetInputPort("evaluate");
            onTruePort = target.GetOutputPort("onTrue");
            conditionPort = target.GetInputPort("condition");
            onFalsePort = target.GetOutputPort("onFalse");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), evaluatePort, onTruePort, serializedObjectTree);
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), conditionPort, onFalsePort, serializedObjectTree);

            serializedObject.ApplyModifiedProperties ();
        }

        public override int GetWidth()
        {
            return 150;
        }
    }
}
