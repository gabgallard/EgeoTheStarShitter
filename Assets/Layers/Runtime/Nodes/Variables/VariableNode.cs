using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Variables
{
    [Node.CreateNodeMenu("Variables/Variable")]
    public class VariableNode : FlowNode {


        [SerializeField]
        GraphVariable variableObject = new GraphVariable();

        [SerializeField]
        private bool isGraphInput = false;

        [SerializeField]
        private string graphVariableID = "";

        [SerializeField]
        private GraphVariable value = new GraphVariable();

        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            if (isGraphInput)
                return soundGraph.GetVariableValueByID(graphVariableID);
            else
                return variableObject.Value();
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Variables/Variable";
        }
    }
}