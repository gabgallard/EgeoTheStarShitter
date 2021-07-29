using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;
using UnityEngine.Audio;

namespace ABXY.Layers.Runtime.Nodes.Automation
{
    [Node.CreateNodeMenu("Automation/Find snapshot node")]
    public class FindSnapshotNode : FlowNode {

        [SerializeField, Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        public string snapshotName;

        [SerializeField, Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        public AudioMixer audioMixer;

        [SerializeField, Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        public AudioMixerSnapshot snapshot;

        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            AudioMixer selectedMixer = GetInputValue<AudioMixer>("audioMixer", audioMixer);
            if (selectedMixer != null)
            {
                return selectedMixer.FindSnapshot(GetInputValue<string>("snapshotName", snapshotName));
            }
            return null;

        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Automation/Find-Snapshot";
        }
    }
}