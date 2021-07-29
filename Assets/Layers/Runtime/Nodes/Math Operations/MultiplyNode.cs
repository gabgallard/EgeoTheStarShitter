using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Graph_Variable_Values;

namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
    [Node.CreateNodeMenu("Math operations/Multiply")]
    public class MultiplyNode : MathOperationNode {

    

        protected override object DoOperation(object number1, object number2)
        {
            return (ValueUtility.GetVariableValue(outputType, ValueUtility.ValueFilter.multipliable) as MultipliableValue).Multiply(number1, number2);
        }

        protected override object DoSecondaryOperation(object a, string secondaryType, object b)
        {
            return (ValueUtility.GetVariableValue(outputType, ValueUtility.ValueFilter.secondaryMultipliable) as SecondaryMultipliableValue).Multiply(a, secondaryType, b); 
        }

        protected override string[] GetSecondaryTypes(GraphVariableValue value)
        {
            if (value is SecondaryMultipliableValue)
                return (value as SecondaryMultipliableValue).GetSecondaryMultiplyTypes();
            return new string[0];
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Math-Operations/Multiply";
        }
    }
}