using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts.Attributes;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/Midi Channel Filter")]
    public class MIDIChannelFilter : FlowNode {

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent input;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent output;

        public enum filterTypes { Include, Exclude }

        [SerializeField, NodeEnum]
        private filterTypes filterType = filterTypes.Include;

        [SerializeField]
        private List<MidiData.MidiChannel> conditions = new List<MidiData.MidiChannel>();

        [SerializeField]
        private string midiParameterSelector = "";

        //[SerializeField]
        //private long noteNumber = 0;

        //[SerializeField]
        //private MidiChannel channelNumber = MidiChannel.All;

        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            return null; // Replace this
        }
        
        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            object midiDataObject = null;

            if (data.TryGetValue(midiParameterSelector, out midiDataObject))
            {
                MidiData noteInfo = midiDataObject as MidiData;
                if (noteInfo != null)
                {
                    if ((filterType == filterTypes.Include && (conditions.Contains(noteInfo.channelNumber) || conditions.Contains(MidiData.MidiChannel.All)))
                        || (filterType == filterTypes.Exclude && !conditions.Contains(noteInfo.channelNumber) && !conditions.Contains(MidiData.MidiChannel.All)))
                    {
                        CallFunctionOnOutputNodes("output", time, data, nodesCalledThisFrame);
                    }
                }
            }


           
        }
        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/MIDI-Channel-Filter";
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("input", visitedNodes);
        }
    }
}