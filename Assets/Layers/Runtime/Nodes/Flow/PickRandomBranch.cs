using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/Pick Random Branch")]
    public class PickRandomBranch : FlowNode {

        [Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent play;

        [SerializeField]
        private bool dontRepeat = false;


        private int lastRandomNumber = -1;

        [SerializeField]
        public List<NodePort> branches = new List<NodePort>();

        // Use this for initialization
        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            PlayRandom(time, data, nodesCalledThisFrame);
        }

    

        private void PlayRandom(double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            int index = GetRandom();
            NodePort selectedPort = GetDynamicOutputPort(index);
            CallFunctionOnOutputNodes(selectedPort, time, data, nodesCalledThisFrame);
        }

        private int GetRandom()
        {
            int index = Random.Range(0, NumberDynamicOutputs());

            if (dontRepeat && index == lastRandomNumber && NumberDynamicOutputs() > 1)
                index = GetRandom();
            lastRandomNumber = index;
            return index;

        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("play", visitedNodes);
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/Pick-Random-Branch";
        }
    }
}