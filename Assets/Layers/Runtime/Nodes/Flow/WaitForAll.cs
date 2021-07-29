using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/Wait for All")]
    public class WaitForAll : FlowNode
    {
    

        private List<NodePort> finishedPorts = new List<NodePort>();

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent reset;

        private double lastTime = 0;

        public override bool isActive => finishedPorts.Count != 0;

        [SerializeField]
        public List<NodePort> branches = new List<NodePort>();

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            Increment(calledBy, time, data, nodesCalledThisFrame);
        }

    

        private void Increment(NodePort calledBy, double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            if (calledBy.fieldName == "reset")
            {
                finishedPorts.Clear();
            }
            else
            {
                if (!finishedPorts.Contains(calledBy))
                    finishedPorts.Add(calledBy);

                if (time > lastTime)
                    lastTime = time;

                if (NumberDynamicInputs() <= finishedPorts.Count())
                {
                    finishedPorts.Clear();
                    CallFunctionOnOutputNodes("playFinished", lastTime, data, nodesCalledThisFrame);
                }
            }

        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
            finishedPorts.Clear();
        }


        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            List<GraphEvent.EventParameterDef> parameters = new List<GraphEvent.EventParameterDef>();
            foreach (NodePort input in DynamicInputs)
                parameters.AddRange(GetIncomingEventParameterDefsOnPort(input.fieldName, visitedNodes));
            return parameters;
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/Wait-For-All";
        }
    }
}