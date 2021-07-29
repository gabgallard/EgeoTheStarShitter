using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Playback
{
    [Node.CreateNodeMenu("Playback/Sub graph")]
    public class SubGraph : SubGraphBase {


        //[SerializeField]
        //private SymphonyPlayer subPlayer;

        [SerializeField]
        public SoundGraph subGraph = null;

        private SoundGraph _runtimeSoundGraph;
        public SoundGraph runtimeSoundGraph
        {
            get
            {
                if (_runtimeSoundGraph == null)
                    ConvertSubgraphsToRuntime();
                return _runtimeSoundGraph;
            }
        }

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent EndAll;


        public override bool isActive {
            get
            {
                if (runtimeSoundGraph != null)
                    return runtimeSoundGraph.isActive;
                else
                    return false;
            }
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {

            if (port.ValueType != typeof(LayersEvent))
            {
                string variableID = port.fieldName.Substring(0, port.fieldName.Length - 3);
                return runtimeSoundGraph.GetVariableValueByID(variableID);
            }

            return null; 
        }
    

        private void OnDestroy()
        {
            /*if (Application.isPlaying)
            Destroy(subPlayer.gameObject);
        else
            DestroyImmediate(subPlayer.gameObject);*/
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            //Debug.Log("[" + name + "]" + "[Subgraph][PlayAtDSPTime] playback scheduled for " + time + ", current time is " + AudioSettings.dspTime);
            EventCalledFromLeft(calledBy, time,data, nodesCalledThisFrame);
        }


        public override void EventCalledFromLeft(NodePort calledBy, double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            base.EventCalledFromLeft(calledBy, time, data, nodesCalledThisFrame);

            string eventName = calledBy.fieldName;
            if (eventName.EndsWith("In"))
            {
                eventName = eventName.Substring(0, eventName.Length - 2);
                runtimeSoundGraph.subgraphNode = this;
                runtimeSoundGraph.CallEventByID(eventName, time,data, nodesCalledThisFrame);
                return;
            }


            runtimeSoundGraph.CallEvent(eventName, time, data, nodesCalledThisFrame);
        }

        public override void EndAllPlayback(double time, Dictionary<string, object> data,int  nodesCalledThisFrame)
        {
            runtimeSoundGraph?.CallEvent("EndAll", time, data, nodesCalledThisFrame);
        }

        public override void OnEventCalledFromWithin(SoundGraph sourceSoundGraph, string eventName, double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {

            GraphEvent graphEvent = runtimeSoundGraph.GetEvent(eventName);

            if (graphEvent == null)
                return;


            string eventNameToUse = graphEvent.eventID + "Out";
            this.CallFunctionOnOutputNodes(eventNameToUse, time, data, nodesCalledThisFrame);
        
        }

        protected override void SetInitialVariableValues()
        {
            foreach (NodePort port in DynamicInputs)
            {
                if (port.ValueType != typeof(LayersEvent) && port.IsConnected)
                {
                    GraphVariable variable = runtimeSoundGraph.GetGraphVariableByID(port.fieldName.Substring(0, port.fieldName.Length -2));
                    variable?.SetValue(port.GetInputValue());
                }
            }
        }


        /*
        public override bool GetVariableValue(string variableName, out object value)
        {
            GraphVariable variable = subGraph.GetGraphVariable(variableName,true);
            NodePort port = GetInputPort(variable.variableID + "In");
            bool hasValue = port != null && port.IsConnected;

            if (hasValue)
                value = port.GetInputValue();
            else
                value = null;
            return hasValue;
        }*/

        public override void NodeUpdate()
        {
            base.NodeUpdate();
            runtimeSoundGraph?.GraphUpdate();
        }

        public override void NodeAwake()
        {
            base.NodeAwake();
            runtimeSoundGraph?.GraphAwake();
            //subGraph?.RegisterEventListener(OnEventCalledFromWithin);
        }

        public override void NodeStart()
        {
            base.NodeStart();
            runtimeSoundGraph?.GraphStart();
            SetInitialVariableValues();
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            if (port.ValueType == typeof(LayersEvent) && runtimeSoundGraph != null)
            {
                GraphEvent gevent = runtimeSoundGraph.GetEventByID(port.fieldName.Substring(0, port.fieldName.Length - 3));
                if (gevent != null)
                    return gevent.parameters;
            }
            return GetIncomingEventParameterDefsOnPort("Start", visitedNodes);//TODO: probably need to do dynamic ports
        }

        public override void ConvertSubgraphsToRuntime()
        {
            if (subGraph != null)
                _runtimeSoundGraph= (SoundGraph)(Application.isPlaying ? subGraph.RuntimeCopy() : subGraph.Copy());
        }

        public override void OnNodeOpenedInGraphEditor()
        {
            if (!Application.isPlaying)
                ConvertSubgraphsToRuntime();

        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Playback/Sub-Graph";
        }
    }
}