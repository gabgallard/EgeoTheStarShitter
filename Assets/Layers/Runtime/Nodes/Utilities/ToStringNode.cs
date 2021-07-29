using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Utilities
{
    [Node.CreateNodeMenu("Utilities/To String")]
    public class ToStringNode : FlowNode
    {

        [SerializeField, Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Inherited)]
        private object input;

        [SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Inherited)]
        private string output;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            object input = GetInputValue<object>("input");
            if (input == null)
                return "null";
            return GetInputValue<object>("input").ToString();
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Utilities/To-String";
        }
    }
}