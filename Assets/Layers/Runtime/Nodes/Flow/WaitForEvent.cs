using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/Wait for Event")]
    public class WaitForEvent : FlowNode {

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent start;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent end;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent Reset;

        [SerializeField]
        public string eventID;

        private bool entered;


        private Dictionary<string, object> lastData = new Dictionary<string, object>();

        public override bool isActive => entered;
        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            if (calledBy.fieldName == "start")
            {
                entered = true;
                lastData = data;
            }
            else
                entered = false;
        }


        public void OnEventCalled(double time,int nodesCalledThisFrame)
        {
            if (entered)
            {
                entered = false;
                CallFunctionOnOutputNodes("end", time,lastData, nodesCalledThisFrame);

            }
        }


        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            entered = false;
            StopAllCoroutines();
        }


        public override void NodeUpdate()
        {

        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("start", visitedNodes);
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/Wait-For-Event";
        }

    }
}