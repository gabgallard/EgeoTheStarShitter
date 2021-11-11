using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(DoIf))]
    public class DoIfNodeEditor : FlowNodeEditor
    {
        NodePort evaluatePort;
        NodePort onTruePort;
        NodePort conditionport;

        public override void OnCreate()
        {
            base.OnCreate();
            evaluatePort = target.GetInputPort("evaluate");
            onTruePort = target.GetOutputPort("onTrue");
            conditionport = target.GetInputPort("condition");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            if (evaluatePort == null || !evaluatePort.node) OnCreate();

            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), evaluatePort, onTruePort, serializedObjectTree);
            NodeEditorGUIDraw.PortField(layout.DrawLine(), conditionport, serializedObjectTree);

            serializedObject.ApplyModifiedProperties ();
        }

        public override int GetWidth()
        {
            return 140;
        }
    }
}
