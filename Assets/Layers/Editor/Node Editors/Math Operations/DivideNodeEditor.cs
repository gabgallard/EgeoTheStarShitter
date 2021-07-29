using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using ABXY.Layers.Runtime.Nodes.Math_Operations;

namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(DivideNode))]
    public class DivideNodeEditor : MathOperationNodeEditor
    {
        protected override bool AllowSecondaryTypes()
        {
            return true;
        }

        protected override ValueUtility.ValueFilter GetFilter()
        {
            return ValueUtility.ValueFilter.dividable | ValueUtility.ValueFilter.secondaryDividable;
        }

        protected override string[] GetSecondaryTypes(GraphVariableValue value)
        {
            return (value as SecondaryDividableValue) .GetSecondaryDivideTypes();
        }
    }
}
