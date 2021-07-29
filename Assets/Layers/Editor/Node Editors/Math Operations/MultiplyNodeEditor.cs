using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using ABXY.Layers.Runtime.Nodes.Math_Operations;

namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(MultiplyNode))]
    public class MultiplyNodeEditor : MathOperationNodeEditor
    {
        protected override bool AllowSecondaryTypes()
        {
            return true;
        }

        protected override ValueUtility.ValueFilter GetFilter()
        {
            return ValueUtility.ValueFilter.multipliable | ValueUtility.ValueFilter.secondaryMultipliable;
        }

        protected override string[] GetSecondaryTypes(GraphVariableValue value)
        {
            return (value as SecondaryMultipliableValue).GetSecondaryMultiplyTypes();
        }
    }
}
