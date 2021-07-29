using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using UnityEngine;
/*
namespace ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split
{
    [CustomNodeEditor(typeof(SplitEvent))]
    public class SplitEventEditor : FlowNodeEditor
    {

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.Update();
            NodeEditorGUILayout.PortPair(target.GetInputPort("input"), target.GetOutputPort("output"));
            NodeEditorGUILayout.PortField(target.GetOutputPort("midiData"));


            foreach (NodePort port in target.DynamicOutputs)
                NodeEditorGUILayout.PortField(port);

            SetupEventParameters();


            serializedObject.ApplyModifiedProperties();
        }

        private void SetupEventParameters()
        {
            if (Event.current.type == EventType.Repaint)
            {
                List<NodePort> variableOutputs = target.DynamicOutputs.ToList();
                List<GraphEvent.EventParameterDef> parameters = (target as FlowNode).GetIncomingEventParameterDefsOnPort("input", new List<Node>());

                foreach (GraphEvent.EventParameterDef parameter in parameters)
                {
                    if (variableOutputs.Find(x => x.fieldName == parameter.parameterName) == null)
                        target.AddDynamicOutput(ReflectionUtils.FindType(parameter.parameterTypeName), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, parameter.parameterName);
                }

                for (int index = 0; index < variableOutputs.Count; index++)
                {
                    NodePort output = variableOutputs[index];
                    if (parameters.FindAll(x => x.parameterName == output.fieldName).Count == 0 || ReflectionUtils.FindType(parameters.Find(x => x.parameterName == output.fieldName).parameterTypeName) != output.ValueType)
                    {
                        target.RemoveDynamicPort(output);
                    }
                }
            }
        }
    }
}
*/