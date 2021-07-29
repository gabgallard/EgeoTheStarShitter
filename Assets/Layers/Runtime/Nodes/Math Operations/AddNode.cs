using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Graph_Variable_Values;

namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
    [Node.CreateNodeMenu("Math operations/Add")]
    public class AddNode : MathOperationNode {



        protected override object DoOperation(object number1, object number2)
        {
            return (ValueUtility.GetVariableValue(outputType, ValueUtility.ValueFilter.addable) as AddableValue).Add(number1, number2);
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Math-Operations/Add";
        }
    }
}