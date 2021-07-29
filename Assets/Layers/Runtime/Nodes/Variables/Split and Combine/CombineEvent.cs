using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;
/*
namespace ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine
{
    [Node.CreateNodeMenu("Variables/Combine and split/Combine Event")]
    public class CombineEvent : FlowNode {

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent input;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent output;


        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private MidiData MIDIData;

        [SerializeField]
        private List<GraphVariable> parameters = new List<GraphVariable>();


        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            return null; // Replace this
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data)
        {

            MidiData newMidiData = GetInputValue<MidiData>("MIDIData", MidiData.defaultMidiFlowInfo);

            Dictionary<string, object> newData = new Dictionary<string, object>();
            foreach (GraphVariable variable in parameters)
            {
                if (!newData.ContainsKey(variable.name))
                    newData.Add(variable.name, GetInputValue(variable.variableID, variable.Value()));
            }

            CallFunctionOnOutputNodes("output", time, newData);
       
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            List<GraphEvent.EventParameterDef> parameterDefs = new List<GraphEvent.EventParameterDef>();
            foreach(GraphVariable variable in parameters)
            {
                parameterDefs.Add(new GraphEvent.EventParameterDef(variable.name, variable.typeName));
            }
            return parameterDefs;
        }



    }
}*/