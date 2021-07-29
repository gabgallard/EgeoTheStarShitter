using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Logic
{
    [Node.CreateNodeMenu("Logic/Or")]
    public class OrNode : FlowNode {

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected bool value;

        [SerializeField]
        public List<NodePort> branches = new List<NodePort>();
        public override object GetValue(NodePort port)
        {
            bool value = false;
            for (int index = 0; index < DynamicInputs.Count(); index++)
            {
                NodePort input = DynamicInputs.ElementAt(index);
                if (input.Connection != null)
                {
                    bool subValue = GetInputValue<bool>(input.fieldName);
                    if (subValue)
                        value = true;
                }
            }
            return value;

        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Logic/Or";
        }
    }
}