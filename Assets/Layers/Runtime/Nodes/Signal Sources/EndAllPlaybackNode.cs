using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Signal_Sources
{
    [Node.CreateNodeMenu("Signal sources/End All Playback")]
    public class EndAllPlaybackNode : FlowNode
    {

        [SerializeField, Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)]
        private LayersEvent endPlayback;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            soundGraph.CallEvent("EndAll", time, data, nodesCalledThisFrame);
        }
        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Signal-Sources/End-All-Playback";
        }
    }
}
