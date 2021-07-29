using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.ThirdParty.MidiJack;
using UnityEngine;
/*
namespace ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine
{
    [Node.CreateNodeMenu("Variables/Combine and split/Combine MIDI Data")]
    public class CombineMidiData : FlowNode {

#pragma warning disable CS0414
        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private MidiData output = null;
#pragma warning restore CS0414

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private int noteNumber = 0;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private MidiData.MidiChannel channelNumber = MidiData.MidiChannel.All;

        [SerializeField, Range(0f, 1f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float velocity = 0f;


        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {

            return new MidiData(
                GetInputValue<int>("noteNumber", noteNumber), 
                GetInputValue<MidiData.MidiChannel>("channelNumber", channelNumber), 
                Mathf.Clamp01( GetInputValue<float>("velocity", velocity)));

        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }
    }
}*/