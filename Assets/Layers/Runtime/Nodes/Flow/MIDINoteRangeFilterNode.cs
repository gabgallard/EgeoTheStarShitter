using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/MIDI Note Range Filter")]
    public class MIDINoteRangeFilterNode : FlowNode {

        // Use this for initialization
        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent Input;

        [SerializeField]
        private string midiDataSelector = "";

#pragma warning disable CS0414

        [SerializeField]
        private int startNote = 0;

        [SerializeField]
        private int endNote = 12;
#pragma warning restore CS0414

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
            if(data.TryGetValue(midiDataSelector, out midiDataObject)){
                if (midiDataObject is MidiData)
                {
                    MidiData midiData = midiDataObject as MidiData;
                    CallFunctionOnOutputNodes(MidiUtils.NoteNumberToName(midiData.noteNumber), time, data, nodesCalledThisFrame);
                }
            }
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("Input", visitedNodes);
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/MIDI-Note-Range-Filter";
        }
    }
}