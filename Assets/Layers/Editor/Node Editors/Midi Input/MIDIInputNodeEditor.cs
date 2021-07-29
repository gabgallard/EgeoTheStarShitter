using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Midi_Input;

namespace ABXY.Layers.Editor.Node_Editors.Midi_Input
{
    [NodeEditor.CustomNodeEditor(typeof(MIDIInput))]
    public class MIDIInputNodeEditor : FlowNodeEditor
    {
        NodePort midiOutput;

        public override void OnCreate()
        {
            base.OnCreate();
            midiOutput = target.GetOutputPort("MIDIOutput");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUILayout.PortField(midiOutput);
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 120;
        }
    }
}
