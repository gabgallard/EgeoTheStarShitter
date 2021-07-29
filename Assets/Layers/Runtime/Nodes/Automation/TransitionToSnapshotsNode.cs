using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;
using UnityEngine.Audio;

namespace ABXY.Layers.Runtime.Nodes.Automation
{
    [Node.CreateNodeMenu("Automation/Transition to snapshots")]
    public class TransitionToSnapshotsNode : FlowNode {

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent startTransition;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent endTransition;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[0];

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private float[] weights = new float[0];

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AudioMixer audioMixer = null;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private float timeToReach = 1f;

#pragma warning disable CS0414
        private bool running = false;
#pragma warning restore CS0414
   
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
            DoTransition(time, nodesCalledThisFrame);
        }

        private void DoTransition(double time,int nodesCalledThisFrame)
        {
            StartCoroutine(WaitForDSPTime(time, () => {

                AudioMixer selectedMixer = GetInputValue<AudioMixer>("audioMixer", audioMixer);

                if (selectedMixer == null)
                    return;

                running = true;

                selectedMixer.TransitionToSnapshots(GetInputValue<AudioMixerSnapshot[]>("snapshots", new AudioMixerSnapshot[0]), GetInputValue<float[]>("weights", new float[0]), GetInputValue<float>("timeToReach", timeToReach));

                StartCoroutine(WaitForDSPTime(time + timeToReach, () => {
                    running = false;
                }));

                StartCoroutine(WaitForDSPTime(time + timeToReach - 0.1, () => {
                    CallFunctionOnOutputNodes("endTransition", time + timeToReach, nodesCalledThisFrame);
                }));

            }));
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("startTransition", visitedNodes);
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Automation/Transition-To-Snapshots";
        }
    }
}