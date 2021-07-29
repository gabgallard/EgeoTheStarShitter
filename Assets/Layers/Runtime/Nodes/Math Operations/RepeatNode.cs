using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
    [Node.CreateNodeMenu("Math operations/Repeat")]
    public class RepeatNode : FlowNode {

#pragma warning disable CS0414
        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private float floatValue = 0f;

        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private float floatLength = 1f;

        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private int intValue = 0;

        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private int intLength = 1;

        [Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited), SerializeField]
        private int intResult = 0;

        [Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited), SerializeField]
        private float floatResult = 0f;
#pragma warning restore CS0414
        public enum numericTypes { Integer, WholeNumber}

        [SerializeField]
        private numericTypes numericType = numericTypes.Integer;

        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            if (numericType == numericTypes.Integer)
            {
                return (int)Mathf.Repeat(GetInputValue<int>("intValue", intValue), GetInputValue<int>("intLength", intLength));
            }
            else
            {
                return Mathf.Repeat(GetInputValue<float>("floatValue", floatValue), GetInputValue<float>("floatLength", floatLength));
            }
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Math-Operations/Repeat";
        }
    }
}