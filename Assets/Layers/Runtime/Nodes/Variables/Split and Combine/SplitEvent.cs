using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;
/*
namespace ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine
{
    [CreateNodeMenu("Variables/Combine and split/Split Event")]
    public class SplitEvent : FlowNode {

        [SerializeField, Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        private LayersEvent input;

        [SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        private LayersEvent output;



        private Dictionary<string, object> lastParameters = new Dictionary<string, object>();

        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            if (lastParameters.ContainsKey(port.fieldName))
                return lastParameters[port.fieldName];

            return null; // Replace this
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data)
        {
        

            StartCoroutine(WaitForDSPTime(time - 0.1, () => {

                if (data != null)
                    lastParameters = data;
                else
                    lastParameters.Clear();

                CallFunctionOnOutputNodes("output", time);
            }));
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("input", visitedNodes);

        }
    }
}*/