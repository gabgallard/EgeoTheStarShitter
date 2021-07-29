using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Signal_Sources
{
    [Node.CreateNodeMenu("Signal sources/Graph Data"),DisallowMultipleNodes]
    public class GraphInputs : FlowNode {

        [SerializeField]
        public List<GraphVariable> variables = new List<GraphVariable>();

        [SerializeField]
        public List<GraphEvent> events = new List<GraphEvent>();

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent update;


        [SerializeField]
        public int poolSize = 1;

        [SerializeField]
        public GlobalsAsset globals = null;

        public override bool isActive { 
            get {
                return soundGraph.isActive;
            } 
        }

        public override void NodeStart()
        {
            base.NodeStart();
            if (globals != null)
            {
                globals.RegisterEventListener((eventName, time, data) => {
                    soundGraph.CallEvent(eventName, time, data,0);
                });
            }
        }

        public void CallEvent(string name, double dspTime, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            GraphEvent selectedEvent = events.Find(x => x.eventName == name);
            if (selectedEvent != null)
                CallFunctionOnOutputNodes(selectedEvent.eventID, dspTime,data, nodesCalledThisFrame);
        
            if (name == "Update")
                CallFunctionOnOutputNodes("update", dspTime, data, nodesCalledThisFrame);

            if (soundGraph.subgraphNode != null)
            {
                soundGraph.subgraphNode.OnEventCalledFromWithin(soundGraph, name, dspTime, data, nodesCalledThisFrame);
            }
        }

        public override object GetValue(NodePort port)
        {
            if (soundGraph.subgraphNode != null && soundGraph.subgraphNode.IsVariablePortConnectedByID(soundGraph, port.fieldName))
            {
                return soundGraph.subgraphNode.GetIncomingVariableValueByID(soundGraph, port.fieldName);
            }
            GraphVariable variable =  variables.Find(x => x.variableID == port.fieldName);
            if (variable != null)
                return variable.Value();
            return null;
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            GraphEvent gevent = soundGraph.GetEventByID(port.fieldName);
            if (gevent != null)
            {
                return gevent.parameters;
            }

            return new List<GraphEvent.EventParameterDef>(); //TODO: implement
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Signal-Sources/Graph-Inputs";
        }
    }
}