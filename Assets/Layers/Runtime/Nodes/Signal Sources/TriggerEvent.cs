using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Signal_Sources
{
    [Node.CreateNodeMenu("Signal sources/Trigger Event")]
    public class TriggerEvent : FlowNode {

        [Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent triggerEvent;

        [Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent eventTriggered;

        [SerializeField]
        public string eventID;

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            TriggerTheEvent(time, data, nodesCalledThisFrame);
        }

        private void TriggerTheEvent(double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            CallFunctionOnOutputNodes("eventTriggered", time, data, nodesCalledThisFrame);
            soundGraph.CallEventByID(eventID, time, data, nodesCalledThisFrame);
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("triggerEvent", visitedNodes);
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Signal-Sources/Trigger-Event";
        }
    }
}