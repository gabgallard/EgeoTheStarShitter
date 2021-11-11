using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Automation;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Automation
{
    [NodeEditor.CustomNodeEditor(typeof(SetMixerParameterNode))]
    public class SetMixerParameterNodeEditor : FlowNodeEditor
    {
        NodePort writePort;
        NodePort writeFinished;
        //SerializedProperty mixerProp;
        //SerializedProperty parameterNameProp;
        //SerializedProperty valueProp;
        public override void OnCreate()
        {
            base.OnCreate();
            writePort = target.GetInputPort("write");
            writeFinished = target.GetOutputPort("writeFinished");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            NodeEditorGUIDraw.PortPair(layout.DrawLine(), writePort, writeFinished, serializedObjectTree);
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("mixer"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("parameterName"), new GUIContent("Name"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("value"));
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 256;
        }
    }
}
