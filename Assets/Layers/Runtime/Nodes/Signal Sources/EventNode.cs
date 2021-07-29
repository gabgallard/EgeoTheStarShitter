using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Signal_Sources
{
    [Node.CreateNodeMenu("Signal sources/Event")]
    public class EventNode : FlowNode {

        [Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent eventCalled;

        [SerializeField]
        public string eventID;




        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            CallFunctionOnOutputNodes("eventCalled", time,data, nodesCalledThisFrame);
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            GraphEvent graphEvent = soundGraph.GetEventByID(eventID);
            if (graphEvent == null)
                return new List<GraphEvent.EventParameterDef>();
            return graphEvent.parameters;
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Signal-Sources/Event";
        }
    }
}