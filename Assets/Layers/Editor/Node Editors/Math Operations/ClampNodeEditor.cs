using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(ClampNode))]
    public class ClampNodeEditor : FlowNodeEditor
    {
        //SerializedProperty valueSP;

        //SerializedProperty minSP;

        //SerializedProperty maxSP;

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
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("value"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("min"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("max"));
            NodeEditorGUIDraw.PortField(layout.DrawLine(), resultPort);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
