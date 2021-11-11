using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Automation;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Automation
{
    [NodeEditor.CustomNodeEditor(typeof(GetMixerParameterNode))]
    public class GetMixerParameterNodeEditor : FlowNodeEditor
    {
        //SerializedProperty mixerProperty;
        //SerializedProperty parameterName;
        NodePort valuePort;
        public override void OnCreate()
        {
            base.OnCreate();
            valuePort = target.GetOutputPort("value");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("mixer"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("parameterName"), new GUIContent("Name"));
            NodeEditorGUIDraw.PortField(layout.DrawLine(), valuePort, serializedObjectTree);
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 236;
        }
    }
}
