using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;
using UnityEngine.Audio;

namespace ABXY.Layers.Runtime.Nodes.Automation
{
    [Node.CreateNodeMenu("Automation/Set mixer parameter")]
    public class SetMixerParameterNode : FlowNode {

#pragma warning disable CS0414

        [SerializeField, Node.InputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent write = null;

        [SerializeField, Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent writeFinished = null;

        [SerializeField, Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AudioMixer mixer = null;

        [SerializeField, Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private string parameterName = "";

        [SerializeField, Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float value = 0f;

#pragma warning restore CS0414


        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            return null; // Replace this
        }


        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            OnSetVar(time, data, nodesCalledThisFrame);
        }


        private void OnSetVar(double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            StartCoroutine(WaitForDSPTime(time, () =>
            {
                GetInputValue<AudioMixer>("mixer", mixer)?.SetFloat(GetInputValue<string>("parameterName", parameterName), GetInputValue<float>("value", value));
            }));
            CallFunctionOnOutputNodes("writeFinished", time, data, nodesCalledThisFrame);
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("write", visitedNodes);
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Automation/Set-Mixer-Parameter";
        }
    }
}