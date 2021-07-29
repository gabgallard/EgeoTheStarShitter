using System.Collections.Generic;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(WaitForSceneLoad))]
    public class WaitForSceneLoadEditor : FlowNodeEditor
    {
        NodePort beginWait = null;
        NodePort reset = null;
        NodePort endWait = null;
        public override void OnCreate()
        {
            base.OnCreate();
            beginWait = target.GetInputPort("beginWait");
            reset = target.GetInputPort("reset");
            endWait = target.GetOutputPort("endWait");
        }
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedPropertyTree sceneName = serializedObject.FindProperty("sceneName");

            NodeEditorGUIDraw.PortPair(layout.DrawLine(), beginWait, endWait);
            DrawEventDerivableProperty(layout.DrawLine(), sceneName, 
                (target as FlowNode).GetIncomingEventParameterDefsOnPort("beginWait", new List<Node>()), (drawRect) => {
                NodeEditorGUIDraw.PropertyField(drawRect, sceneName);
            });
            NodeEditorGUIDraw.PortField(layout.DrawLine(), reset);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
