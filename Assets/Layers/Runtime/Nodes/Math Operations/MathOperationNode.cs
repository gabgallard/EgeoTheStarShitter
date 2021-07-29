using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
    public abstract class MathOperationNode : FlowNode
    {
        [SerializeField]
        protected List<string> nodeIDs = new List<string>();
    
        [SerializeField]
        protected List<GraphVariable> variables = new List<GraphVariable>();

        [SerializeField]
        protected GraphVariable primaryVariable = new GraphVariable();

        [SerializeField]
        protected GraphVariable secondVariable = new GraphVariable();

        protected string outputType { get { return primaryVariable.typeName; } }

        public override object GetValue(NodePort port)
        {
            GraphVariableValue value = ValueUtility.GetVariableValue(primaryVariable.typeName, 
                ValueUtility.ValueFilter.addable | ValueUtility.ValueFilter.subtractable | 
                ValueUtility.ValueFilter.multipliable | ValueUtility.ValueFilter.secondaryMultipliable|
                ValueUtility.ValueFilter.dividable | ValueUtility.ValueFilter.secondaryDividable);

            if (value == null || (GetSecondaryTypes(value).Length == 0 && variables.Count == 0))
            {
                System.Type type = ReflectionUtils.FindType(outputType);
                if (type.IsValueType)
                {
                    return System.Activator.CreateInstance(type);
                }
                return null;
            }

            if (GetSecondaryTypes(value).Length == 0)
            {
                object currentValue = GetInputValue(variables[0].variableID, variables[0].Value());
                for (int index = 1; index < variables.Count; index++)
                {
                    currentValue = DoOperation(currentValue, GetInputValue(variables[index].variableID, variables[index].Value()));
                }

                return currentValue;
            }
            else
            {
                return DoSecondaryOperation(GetInputValue(primaryVariable.variableID, primaryVariable.Value()), secondVariable.typeName,  GetInputValue(secondVariable.variableID, secondVariable.Value()));
            }
        }


        protected abstract object DoOperation(object a, object b);

        protected virtual object DoSecondaryOperation(object a, string secondaryType, object b) { return null; }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }
        protected virtual string[] GetSecondaryTypes(GraphVariableValue value)
        {
            return new string[0];
        }
    }
}
