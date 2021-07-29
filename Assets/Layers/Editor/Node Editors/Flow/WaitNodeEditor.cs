using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(Wait))]
    public class WaitNodeEditor : FlowNodeEditor
    {
        NodePort enterPort;
        NodePort exitPort;

        public override void OnCreate()
        {
            base.OnCreate();
            enterPort = target.GetInputPort("enter");
            exitPort = target.GetOutputPort("exit");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedPropertyTree waitTimeProp = serializedObject.FindProperty("waitTime");
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), enterPort, exitPort);
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 65;
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), waitTimeProp);
            EditorGUIUtility.labelWidth = labelWidth;
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 130;
        }
    }
}
