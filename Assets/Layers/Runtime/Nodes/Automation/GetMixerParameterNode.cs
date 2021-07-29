using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;
using UnityEngine.Audio;

namespace ABXY.Layers.Runtime.Nodes.Automation
{
    [Node.CreateNodeMenu("Automation/Get mixer parameter")]
    public class GetMixerParameterNode : FlowNode {


#pragma warning disable CS0414
        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AudioMixer mixer = null;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private string parameterName = "";

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited)]
        private float value = 0f;

    
#pragma warning restore CS0414



        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            AudioMixer selectedMixer = GetInputValue<AudioMixer>("mixer", mixer);
            if (selectedMixer != null)
            {
                float value = 0;
                selectedMixer.GetFloat(GetInputValue<string>("parameterName", parameterName), out value);
                return value;
            }
            return 0;
        }


        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Automation/Get-Mixer-Parameter";
        }
    }
}