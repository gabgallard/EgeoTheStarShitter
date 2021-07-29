using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Variables
{
    [Node.CreateNodeMenu("Variables/Digital Signal Processor Time")]
    public class DSPTimeNode : FlowNode {

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited)]
        private double time;

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            return (double)AudioSettings.dspTime;
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Variables/Digital-Signal-Processor-Time";
        }
    }
}