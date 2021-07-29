using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/Pick Branch")]
    public class PickBranch : FlowNode {

#pragma warning disable CS0414
        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent input = null;
#pragma warning restore CS0414

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private int selectedBranch = 1;

        [SerializeField]
        private List<string> outputs = new List<string>();

        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            return null; // Replace this
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            int actualSelectedBranch = GetInputOrParameterValue<int>("selectedBranch", GetInputValue<int>("selectedBranch", selectedBranch),data);
            if (actualSelectedBranch >= 1 && actualSelectedBranch <= outputs.Count)
            {
                CallFunctionOnOutputNodes(outputs[actualSelectedBranch-1], time, data, nodesCalledThisFrame);
            }
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("input", visitedNodes);
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/Pick-Branch";
        }
    }
}