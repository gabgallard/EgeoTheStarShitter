using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/Probability Gate")]
    public class ProbabilityGate : FlowNode
    {


#pragma warning disable CS0414
        [SerializeField, Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        LayersEvent input = null;

        [SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        LayersEvent output = null;

        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        float probability = 0f;
#pragma warning restore CS0414

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            float randomNumber = Random.Range(0f, 100f);
            float probabilityValue = Mathf.Clamp( GetInputValue<float>("probability", probability),0f,100f);
            if (probabilityValue >= randomNumber)
                CallFunctionOnOutputNodes(GetOutputPort("output"), time, data, nodesCalledThisFrame);
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

    }
}