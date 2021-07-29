using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;

namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
    [Node.CreateNodeMenu("Math operations/Magnitude")]
    public class Magnitude : FlowNode
    {
#pragma warning disable CS0414
        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private Vector3 input = Vector3.zero;

        [SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Inherited)]
        private float output = 0f;
#pragma warning restore CS0414

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return GetInputValue<Vector3>("input", input).magnitude;
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Math-Operations/Magnitude";
        }
    }
}