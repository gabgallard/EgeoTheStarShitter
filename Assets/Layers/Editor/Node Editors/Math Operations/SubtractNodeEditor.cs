using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using ABXY.Layers.Runtime.Nodes.Math_Operations;

namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(SubtractNode))]
    public class SubtractNodeEditor : MathOperationNodeEditor
    {
        protected override ValueUtility.ValueFilter GetFilter()
        {
            return ValueUtility.ValueFilter.subtractable;
        }
    }
}
