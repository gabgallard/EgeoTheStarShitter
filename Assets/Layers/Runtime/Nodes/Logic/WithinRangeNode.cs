using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Logic
{
    [Node.CreateNodeMenu("Logic/Within Range")]
    public class WithinRangeNode : FlowNode
    {
        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private int intMin = 0;

        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private int intMax = 42;

        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private int intValue = 0;

        [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        private bool output;

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        public override object GetValue(NodePort port)
        {
            int realInput = GetInputValue<int>("intValue", intValue);
            int realIntMin = GetInputValue<int>("intMin", intMin);
            int realIntMax = GetInputValue<int>("intMax", intMax);
            return realInput >= realIntMin && realInput <= realIntMax;
        }
    }
}