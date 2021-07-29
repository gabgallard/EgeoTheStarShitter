using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/If")]
    public class IfNode : FlowNode {
        [Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent evaluate;

        [Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private bool condition;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected LayersEvent onTrue;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected LayersEvent onFalse;




        private void Evaluate(double dspTime, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            bool condition = GetInputValue<bool>("condition", false);
            if (condition)
                CallFunctionOnOutputNodes("onTrue", dspTime, data, nodesCalledThisFrame);
            else
                CallFunctionOnOutputNodes("onFalse", dspTime, data, nodesCalledThisFrame);

        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            StartCoroutine(WaitForDSPTime(time, () => {
                Evaluate(time, data, nodesCalledThisFrame);
            }));
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("evaluate", visitedNodes);
        }
        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/If";
        }
    }
}