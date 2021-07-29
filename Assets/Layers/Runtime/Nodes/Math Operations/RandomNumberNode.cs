using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts.Attributes;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
    [Node.CreateNodeMenu("Math operations/Get Random Number")]
    public class RandomNumberNode : FlowNode {

        [Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent getNewNumber;

        [Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent changed;


        public enum RandomNumberTypes { Integer, Float}

        [SerializeField, NodeEnum]
        private RandomNumberTypes randomNumberType = RandomNumberTypes.Integer;


        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private float floatFrom = 0f;

        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private float floatTo = 0f;

        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private int intFrom = 0;

        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private int intTo = 0;

        private float CurrentValue = 0;

        // Use this for initialization
        protected override void Init() {
            base.Init();

            //GetNewNumber();
            
        }

        public override void NodeAwake()
        {
            GetNewNumber();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {

            if (port.fieldName == "value")
            {
                switch (randomNumberType)
                {
                    case RandomNumberTypes.Integer:
                        return (int)CurrentValue;
                    case RandomNumberTypes.Float:
                        return (float)CurrentValue;
                }
                return 0;
            }
            return null;

        }



        private void GetNewNumber()
        {
            switch (randomNumberType)
            {
                case RandomNumberTypes.Integer:
                    int intStart = GetInputValue<int>("intFrom", intFrom);
                    int intEnd = GetInputValue<int>("intTo", intTo);
                    CurrentValue = Random.Range(intStart, intEnd);
                    break;
                case RandomNumberTypes.Float:
                    float floatStart = GetInputValue<float>("floatFrom", floatFrom);
                    float floatEnd = GetInputValue<float>("floatTo", floatTo);
                    CurrentValue = Random.Range(floatStart, floatEnd);
                    break;
            }
        }

    

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            StartCoroutine(WaitForDSPTime(time - 2f* Time.deltaTime, () =>
            {
                GetNewNumber();
            }));
            CallFunctionOnOutputNodes("changed", time, data, nodesCalledThisFrame);
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
        }


        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("getNewNumber", visitedNodes);
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Math-Operations/Random-Number";
        }
    }
}