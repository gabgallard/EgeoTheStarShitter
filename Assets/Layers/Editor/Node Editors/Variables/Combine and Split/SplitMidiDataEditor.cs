using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
/*
namespace ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split
{
    [CustomNodeEditor(typeof(SplitMidiData))]
    public class SplitMidiDataEditor : FlowNodeEditor
    {
        NodePort input;

        NodePort noteNumber;

        NodePort channelNumber;

        NodePort velocity;

        public override void OnCreate()
        {
            base.OnCreate();
            input = target.GetInputPort("input");
            noteNumber = target.GetOutputPort("noteNumber");
            channelNumber = target.GetOutputPort("channelNumber");
            velocity = target.GetOutputPort("velocity");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            LayersGUIUtilities.DrawExpandableProperty(input, noteNumber,serializedObject);
            LayersGUIUtilities.DrawExpandableProperty(channelNumber,serializedObject);
            LayersGUIUtilities.DrawExpandableProperty(velocity,serializedObject);
        }

        protected override bool CanExpand()
        {
            return true;
        }

        public override int GetWidth()
        {
            return 195;
        }
    }
}
*/