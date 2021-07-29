using System.Collections;
using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes
{

    [CreateNodeMenu("Variables/Boolean Value Selector")]
    public class BooleanValueSelectorNode : FlowNode
    {
        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private bool condition = false;

        [SerializeField]
        GraphVariable value1 = new GraphVariable();

        [SerializeField]
        GraphVariable value2 = new GraphVariable();

        [SerializeField]
        GraphVariable output = new GraphVariable();

#pragma warning disable CS0414
        [SerializeField]
        private string typeName = "";


        [SerializeField]
        private string arrayTypeName = "";
#pragma warning restore CS0414

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            bool actualCondition = GetInputValue<bool>("condition", condition);

            if (actualCondition)
            {
                return GetInputValue("value1", value1.Value());
            }
            else
            {
                return GetInputValue("value2", value2.Value());

            }
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Variables/Boolean-Value-Selector";
        }
    }
}