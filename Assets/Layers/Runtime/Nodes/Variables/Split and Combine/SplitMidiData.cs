using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.ThirdParty.MidiJack;
using UnityEngine;
/*
namespace ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine
{
    [Node.CreateNodeMenu("Variables/Combine and split/Split Midi Data")]
    public class SplitMidiData : FlowNode {

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private MidiData input;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private int noteNumber;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private MidiChannel channelNumber;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private float velocity;

        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            input = GetInputValue<MidiData>("input",MidiData.defaultMidiFlowInfo);

            if (input == null)
                input = MidiData.defaultMidiFlowInfo;

            if (port.fieldName == "noteNumber")
            {
                return input.noteNumber;
            }
            else if(port.fieldName == "channelNumber")
            {
                return input.channelNumber;
            }
            else if (port.fieldName == "velocity")
            {
                return input.velocity;
            }
            return null; // Replace this
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }
    }
}*/