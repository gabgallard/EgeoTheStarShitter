using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Flow;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(ProbabilityGate))]
    public class ProbabilityGateEditor : FlowNodeEditor
    {
        NodePort input;
        NodePort output;
        SerializedPropertyTree probability;
        NodePort probabilityPort;
        public override void OnCreate()
        {
            base.OnCreate();
            input = target.GetInputPort("input");
            output = target.GetOutputPort("output");
            probability = serializedObjectTree.FindProperty("probability");
            probabilityPort = target.GetInputPort("probability");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), input, output, serializedObjectTree);

            Rect sliderRect = layout.DrawLine();

            if (probabilityPort.IsConnected)
                NodeEditorGUIDraw.PortField(sliderRect, probabilityPort, serializedObjectTree);
            else
            {
                probability.floatValue = EditorGUI.Slider(sliderRect, probability.floatValue, 0f, 100f);
                NodeEditorGUIDraw.AddPortToRect(sliderRect, probabilityPort);
            }

            serializedObjectTree.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 150;
        }
    }
}