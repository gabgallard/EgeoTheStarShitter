using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine
{
    [Node.CreateNodeMenu("Variables/Split")]
    public class SplitNode : CombineSplitBase {

        [SerializeField]
        private string typeName = "";


        private static Dictionary<string, GraphVariableValue> _typeName2Value = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> typeName2Value
        {
            get
            {
                if (_typeName2Value.Count == 0)
                    LoadGraphVarValues();
                return _typeName2Value;
            }
        }

       

        private static void LoadGraphVarValues()
        {
            if (_typeName2Value.Count != 0) // Then already loaded
                return;

            _typeName2Value = ValueUtility.GetVariableValues(ValueUtility.ValueFilter.splittable);
        }

        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {

            GraphVariableValue value = null;
            if (typeName2Value.TryGetValue(typeName, out value))
            {
                return (value as SplittableValue).GetSplitValue(port, this);
            }
            return null;
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            GraphVariableValue value = null;
            if (typeName2Value.TryGetValue(typeName, out value))
            {
                if (value is SplitValuePlayEvent)
                    (value as SplitValuePlayEvent).PlayAtDSPTime(this, calledBy, time, data, nodesCalledThisFrame);
            }
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Variables/Split";
        }

    }
}