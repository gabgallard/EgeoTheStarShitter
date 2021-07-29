using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
    [Node.CreateNodeMenu("Math operations/Inverse Lerp")]
    public class InverseLerp : FlowNode {

        [Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited), SerializeField]
        private float result;

        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private float value = 0f;

        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private float from = 0f;

        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private float to = 42f;

    

        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            return Mathf.InverseLerp(GetInputValue<float>("from", from), GetInputValue<float>("to", to), Mathf.Clamp(GetInputValue<float>("value", value), GetInputValue<float>("from", from), GetInputValue<float>("to", to)));
        }

        private void OnValidate()
        {
            value = Mathf.Clamp(value, GetInputValue<float>("from", from), GetInputValue<float>("to", to));
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Math-Operations/Inverse-Lerp";
        }
    }
}