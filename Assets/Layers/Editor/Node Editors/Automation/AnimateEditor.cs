using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Automation;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Automation
{
    [NodeEditor.CustomNodeEditor(typeof(Animate))]
    public class AnimateEditor : FlowNodeEditor
    {

        NodePort startPort;
        NodePort valuePort;
        NodePort stopPort;
        //SerializedProperty animationCurve;
        public override void OnCreate()
        {
            base.OnCreate();
            startPort = target.GetInputPort("start");
            valuePort = target.GetOutputPort("value");
            stopPort = target.GetInputPort("stop");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), startPort, valuePort, serializedObjectTree);
            NodeEditorGUIDraw.PortField(layout.DrawLine(),stopPort, serializedObjectTree);

            LayersGUIUtilities.FastPropertyField(layout.DrawLine(), new GUIContent("Curve"), serializedObject.FindProperty("animationCurve"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
