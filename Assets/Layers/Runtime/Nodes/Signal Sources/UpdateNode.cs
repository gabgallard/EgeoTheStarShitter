using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Signal_Sources
{
    [Node.CreateNodeMenu("Signal sources/Update")]
    public class UpdateNode : FlowNode
    {

        [SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        private LayersEvent update;

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

        public override void NodeUpdate()
        {
            CallFunctionOnOutputNodes(GetOutputPort("update"), AudioSettings.dspTime,0);
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Signal-Sources/Update";
        }
    }
}