using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Playback
{
    public abstract class SubGraphBase : FlowNode
    {

        private bool firstFunctionCalled = false;


        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            EventCalledFromLeft(calledBy, time, data, nodesCalledThisFrame);
        }


        public virtual void EventCalledFromLeft(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            //setting up values
            if (!firstFunctionCalled)
                SetInitialVariableValues();
            firstFunctionCalled = true;
        }

        public abstract void EndAllPlayback(double time, Dictionary<string, object> data, int nodesCalledThisFrame);

        public abstract void OnEventCalledFromWithin(SoundGraph sourceSoundGraph, string eventName, double time, Dictionary<string, object> data,int nodesCalledThisFrame);

        protected abstract void SetInitialVariableValues();

        public virtual bool IsVariablePortConnectedByID(SoundGraph soundGraph, string variablePortID)
        {
            NodePort port = GetInputPort(variablePortID + "In");
            if (port == null)
                return false;
            return port.IsConnected;
        }

        public virtual object GetIncomingVariableValueByID(SoundGraph soundGraph, string variablePortID)
        {
            NodePort port = GetInputPort(variablePortID + "In");
            if (port == null)
                return null;
            return port.GetInputValue();
        }

        //public abstract bool GetVariableValue(string variableName, out object value);

        public override void NodeStart()
        {
            base.NodeStart();
            SetInitialVariableValues();
        }

        public abstract void ConvertSubgraphsToRuntime();

    }
}