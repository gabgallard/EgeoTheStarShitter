using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Utilities
{
    //[Node.CreateNodeMenu("Utilities/Comment")]
    [Node.CreateNodeMenu("")]
    public class CommentNode : FlowNode
    {
        [SerializeField]
        private Vector2 commentDimensions = new Vector2(500, 500);

#pragma warning disable CS0414
        [SerializeField, TextArea(1,200)]
        private string comment = "";
#pragma warning restore CS0414

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
            throw new System.NotImplementedException();
        }

        public override bool DoNotCull()
        {
            return true;
        }
    }
}