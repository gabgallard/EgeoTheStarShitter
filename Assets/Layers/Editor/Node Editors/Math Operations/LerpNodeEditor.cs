using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(Lerp))]
    public class LerpNodeEditor : FlowNodeEditor
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
            serializedObject.UpdateIfRequiredOrScript();
            SerializedPropertyTree value = serializedObject.FindProperty("value");
            SerializedPropertyTree from = serializedObject.FindProperty("from");
            SerializedPropertyTree to = serializedObject.FindProperty("to");
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), value);
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), from);
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), to);
            NodeEditorGUIDraw.PortField(layout.DrawLine(), resultPort, serializedObjectTree);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
