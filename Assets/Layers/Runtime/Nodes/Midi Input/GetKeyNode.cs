using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Settings;
using ABXY.Layers.ThirdParty.MidiJack;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Midi_Input
{
    [Node.CreateNodeMenu("MIDI Input/Get Key")]
    public class GetKeyNode : FlowNode {
#pragma warning disable CS0414
        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent OnTrigger = null;
#pragma warning restore CS0414

        //[SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited)]
        //private float velocity = 0f;

        [SerializeField]
        private MidiChannel channel = MidiChannel.All;


        private enum triggerTypes { KeyDown, KeyUp}

        [SerializeField]
        private triggerTypes triggerType = triggerTypes.KeyDown;

        [SerializeField]
        private int noteNumber = 0;


        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            return null;
        }

        public override void NodeUpdate()
        {
            if ((Application.platform != RuntimePlatform.LinuxEditor
                 || Application.platform != RuntimePlatform.OSXEditor
                 || Application.platform != RuntimePlatform.WindowsEditor)
                && !LayersSettings.GetOrCreateSettings().enableMIDIInBuilds)
                return;

            if (triggerType == triggerTypes.KeyDown && MidiMaster.GetKeyDown(channel, noteNumber))
            {
                MidiData.MidiChannel castChannel = (MidiData.MidiChannel)(int)channel;
                float velocity = MidiMaster.GetKey(channel, noteNumber);
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("NoteInfo", new MidiData(noteNumber, castChannel, velocity));
                CallFunctionOnOutputNodes("OnTrigger", AudioSettings.dspTime, data,0);

            }
            /*else if (triggerType == triggerTypes.KeyUp && MidiMaster.GetKeyUp(channel, noteNumber))
        {
            velocity = MidiMaster.GetKey(channel, noteNumber);
            CallFunctionOnOutputNodes("OnTrigger", AudioSettings.dspTime);
            lastAccessDSPTime = AudioSettings.dspTime;
        }*/
            /*else
            {
                velocity = 0f;
            }*/
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            List<GraphEvent.EventParameterDef> parameters = GetIncomingEventParameterDefsOnPort("OnTrigger", visitedNodes);
            parameters.Add(new GraphEvent.EventParameterDef("NoteInfo", typeof(MidiData).FullName));
            return parameters;
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/MIDI-Input/Get-Key";
        }
    }
}