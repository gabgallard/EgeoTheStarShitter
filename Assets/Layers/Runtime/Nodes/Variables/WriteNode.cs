using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;
using UnityEngine.Serialization;

namespace ABXY.Layers.Runtime.Nodes.Variables
{
    [Node.CreateNodeMenu("Variables/Write")]
    public class WriteNode : FlowNode {
        [Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent startWrite;

        [Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent writeEnded;
        
        [SerializeField, FormerlySerializedAs("graphVariableName")]
        private string graphVariableID = "";

        //TODO: Rebuild this node

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            DoWrite(time, data, nodesCalledThisFrame);
        }


        private void DoWrite(double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {

            StartCoroutine(WaitForDSPTime(time, () => {
                GraphVariable variable = soundGraph.GetGraphVariableByID(graphVariableID);
                if (variable != null)
                    variable.SetValue(GetInputValue("Input", variable.Value()));

            }));

            
            /*switch (variable.varType)
        {
            case GraphVariable.varTypes.Int:
                variable.intValue = GetInputValue<int>("newValue", variable.intValue);
                break;
            case GraphVariable.varTypes.Float:
                variable.floatValue = GetInputValue<float>("newValue", variable.floatValue);
                break;
            case GraphVariable.varTypes.Boolean:
                variable.boolValue = GetInputValue<bool>("newValue", variable.boolValue);
                break;
        }*/
            CallFunctionOnOutputNodes("writeEnded", time, data, nodesCalledThisFrame);
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("startWrite", visitedNodes);
        }
        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Variables/Write";
        }
    }
}