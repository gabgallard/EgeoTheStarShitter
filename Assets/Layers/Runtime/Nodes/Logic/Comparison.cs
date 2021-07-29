using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts.Attributes;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Logic
{
    [Node.CreateNodeMenu("Logic/Comparison")]
    public class Comparison : FlowNode {
        public enum comparisonOperators { Equal, NotEqual, LessThan,GreaterThan,LessThanOrEqualTo, GreaterThanOrEqualTo}

        public readonly string[] comparisonOperatorPrettyNames = new string[] { "=", "≠", "<", ">" , "≤", "≥" };

#pragma warning disable CS0414
        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private bool result = false;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private object value1 = 0f;

        [SerializeField, NodeEnum]
        private comparisonOperators comparisonOperator = comparisonOperators.GreaterThan;


        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private object value2 = 0f;

#pragma warning restore CS0414

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


        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            object value1 = GetInputValue("value1");
            object value2 = GetInputValue("value2");
            if (value1 == null || value2 == null)
                return false;
            GraphVariableValue valueComparer;
            if (typeName2Value.TryGetValue(value1.GetType().FullName, out valueComparer))
            {
                return valueComparer.CompareValues(comparisonOperator, value1, value2);
            }
            return false;
        }

        private static void LoadGraphVarValues()
        {
            if (_typeName2Value.Count != 0) // Then already loaded
                return;

            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in assembly.GetTypes())
                {
                    if (type.BaseType == typeof(GraphVariableValue))
                    {
                        GraphVariableValue instance = (GraphVariableValue)System.Activator.CreateInstance(type);
                        _typeName2Value.Add(instance.handlesType.FullName, instance);
                    }
                }
            }
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Logic/Comparison";
        }
    }
}