using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/Wait for Condition")]
    public class WaitForCondition : FlowNode {

#pragma warning disable CS0414
        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent start= null;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent reset = null;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent end = null;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private bool condition = false;

#pragma warning restore CS0414

        private bool primed = false;

        private Dictionary<string, object> lastData;

        public override bool isActive => primed;

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            if (calledBy.fieldName == "start")
                Begin(time, data);
            else
                primed = false;
        }

        private void Begin(double time, Dictionary<string, object> data)
        {
            StartCoroutine(WaitForDSPTime(time, () => {
                lastData = data;
                primed = true;
            }));
        }

        public override void NodeUpdate()
        {
            if (primed == true && GetInputValue<bool>("condition",condition))
            {
                primed = false;
                CallFunctionOnOutputNodes("end", AudioSettings.dspTime, lastData,0);
            }
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
            primed = false;
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("start", visitedNodes);
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/Wait-For-Condition";
        }
    }
}