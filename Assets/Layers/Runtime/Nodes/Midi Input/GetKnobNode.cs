using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Settings;
using ABXY.Layers.ThirdParty.MidiJack;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Midi_Input
{
    [Node.CreateNodeMenu("MIDI Input/Get Knob")]
    public class GetKnobNode : FlowNode {

#pragma warning disable CS0414
        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent onChange = null;
#pragma warning restore CS0414

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited)]
        private float knobValue = 0f;

#pragma warning disable CS0414
        [SerializeField]
        private MidiChannel channel = MidiChannel.All;

        [SerializeField]
        private int knobNumber = 0;
#pragma warning restore CS0414

        [SerializeField]
        private float startingValue = 0;

        public override void NodeAwake()
        {
            base.NodeAwake();
            if ((Application.platform != RuntimePlatform.LinuxEditor
                 || Application.platform != RuntimePlatform.OSXEditor
                 || Application.platform != RuntimePlatform.WindowsEditor)
                && !LayersSettings.GetOrCreateSettings().enableMIDIInBuilds)
                return;

            MidiMaster.knobDelegate += OnKnobChanged;
            knobValue = startingValue;
            CallFunctionOnOutputNodes("onChange", AudioSettings.dspTime,0);
        }
        public override object GetValue(NodePort port)
        {
            return knobValue;
        }


        private void OnKnobChanged(MidiChannel channel, int knobNumber, float velocity)
        {
            knobValue = MidiMaster.GetKnob(channel, knobNumber);
            CallFunctionOnOutputNodes("onChange", AudioSettings.dspTime,0);
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/MIDI-Input/Get-knob";
        }
    }
}