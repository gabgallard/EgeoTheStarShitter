using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Utilities
{
    [Node.CreateNodeMenu("Utilities/Debug Log")]
    public class DebugLog : FlowNode
    {
        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Multiple, TypeConstraint.Inherited)]
        private LayersEvent print;

        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private string message = null;

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            StartCoroutine(Layers.Runtime.SymphonyUtils.WaitForDSPTime(time, () => {
                Debug.Log(GetInputValue<string>("message", message));
            }));
        }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }
        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
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

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Utilities/Debug-Log";
        }
    }
}