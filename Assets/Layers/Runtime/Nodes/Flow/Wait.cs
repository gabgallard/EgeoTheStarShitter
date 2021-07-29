using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/Wait")]
    public class Wait : FlowNode {

        [Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent enter;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected LayersEvent exit;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float waitTime = 1f;

        bool waiting = false;

        public override bool isActive => waiting;

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            CallNext(time, data);
        }



        private void CallNext(double time, Dictionary<string, object> data)
        {

            float timeToWait = GetInputValue<float>("waitTime", waitTime);

            StartCoroutine(WaitForDSPTime(time - 0.1, () => {
                waiting = true;
                CallFunctionOnOutputNodes("exit", time + timeToWait, data,0);

                StartCoroutine(WaitForDSPTime(time + timeToWait, () => {
                    waiting = false;
                }));
            }));

        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
            waiting = false;
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("enter", visitedNodes);
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/Wait";
        }
    }
}