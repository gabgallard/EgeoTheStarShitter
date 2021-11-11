using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
    [Node.CreateNodeMenu("Math operations/Pow")]
    public class Pow : FlowNode
    {


#pragma warning disable CS0414
        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private float input = 0f;

        [SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Inherited)]
        private float output = 0f;

        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private float power = 0f;
#pragma warning restore CS0414

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        public override object GetValue(NodePort port)
        {
            float inputValue = GetInputValue<float>("input", input);
            float powerValue = GetInputValue<float>("power", power);
            return Mathf.Pow(inputValue, powerValue);
        }
    }
}