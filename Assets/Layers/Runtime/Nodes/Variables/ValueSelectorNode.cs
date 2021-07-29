using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes
{

    [CreateNodeMenu("Variables/Value Selector")]
    public class ValueSelectorNode : FlowNode
    {
#pragma warning disable CS0414
        [SerializeField]
        private string variableType = "";

        [SerializeField]
        private string arrayType = "";
#pragma warning restore CS0414

        [SerializeField]
        List<GraphVariable> selectionValues = new List<GraphVariable>();

        [SerializeField]
        private GraphVariable output = new GraphVariable();

        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private int selectedBranch = 1;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            int index = GetInputValue<int>("selectedBranch", selectedBranch) - 1;

            if (selectionValues.Count == 0)
                return null; // Replace this

            index = Mathf.Clamp(index, 0, selectionValues.Count - 1);

            return GetInputValue(selectionValues[index].variableID, selectionValues[index].Value());
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Variables/Value-Selector";
        }
    }
}