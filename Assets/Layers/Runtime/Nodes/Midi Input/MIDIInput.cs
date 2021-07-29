using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Settings;
using ABXY.Layers.ThirdParty.MidiJack;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Midi_Input
{
    [Node.CreateNodeMenu("MIDI Input/Midi Input")]
    public class MIDIInput : FlowNode
    {

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent MIDIOutput;

        public override void NodeStart()
        {
            base.NodeStart();

            if ((Application.platform != RuntimePlatform.LinuxEditor
                 || Application.platform != RuntimePlatform.OSXEditor
                 || Application.platform != RuntimePlatform.WindowsEditor)
                && !LayersSettings.GetOrCreateSettings().enableMIDIInBuilds)
                return;

            MidiMaster.noteOnDelegate += OnKeyPressed;
        }

        public override void NodeUpdate()
        {
            base.NodeUpdate();
            if ((Application.platform != RuntimePlatform.LinuxEditor
                 || Application.platform != RuntimePlatform.OSXEditor
                 || Application.platform != RuntimePlatform.WindowsEditor)
                && !LayersSettings.GetOrCreateSettings().enableMIDIInBuilds)
                return;
#if UNITY_EDITOR
            int increment = MidiDriver.Instance.TotalMessageCount;
#endif
        }

        private void OnDestroy()
        {
            if ((Application.platform != RuntimePlatform.LinuxEditor
                 || Application.platform != RuntimePlatform.OSXEditor
                 || Application.platform != RuntimePlatform.WindowsEditor)
                && !LayersSettings.GetOrCreateSettings().enableMIDIInBuilds)
                return;


            MidiMaster.noteOnDelegate -= OnKeyPressed;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/MIDI-Input/MIDI-Input";
        }


        private void OnKeyPressed(MidiChannel channel, int noteNumber, float velocity)
        {
            MidiData.MidiChannel castChannel = (MidiData.MidiChannel)(int)channel;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("NoteInfo", new MidiData(noteNumber, castChannel, velocity));
            CallFunctionOnOutputNodes("MIDIOutput", AudioSettings.dspTime, parameters,0);
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            List<GraphEvent.EventParameterDef> parameters = new List<GraphEvent.EventParameterDef>();
            parameters.Add(new GraphEvent.EventParameterDef("NoteInfo", typeof(MidiData).FullName));
            return parameters;
        }

        
    }
}