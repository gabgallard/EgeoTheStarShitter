using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(InverseLerp))]
    public class InverseLerpNodeEditor : FlowNodeEditor
    {
        NodePort resultPort;
        public override void OnCreate()
        {
            base.OnCreate();
            resultPort = target.GetOutputPort("result");
        }


        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();
            LayersGUIUtilities.BeginNewLabelWidth(40);
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(),serializedObjectTree.FindProperty("value"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObjectTree.FindProperty("from"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObjectTree.FindProperty("to"));
            NodeEditorGUIDraw.PortField(layout.DrawLine(), resultPort);
            LayersGUIUtilities.EndNewLabelWidth();
            serializedObjectTree.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 130;
        }
    }
}
