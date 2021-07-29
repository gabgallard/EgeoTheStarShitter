using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Autodoc;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Automation
{
    [Node.CreateNodeMenu("Automation/Animate"), NodeDoc("Animates an output value over time, according to an animation curve")]
    public class Animate : FlowNode {

        [SerializeField, PortDoc("The animation curve representing the output value of the node over time.")]
        private AnimationCurve animationCurve = null;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), PortDoc("Begins animating the output value")]
        private LayersEvent start;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), PortDoc("Stops animating the output value, and resets the time on the curve to zero")]
        private LayersEvent stop;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), PortDoc("The resulting value of the animation curve over time")]
        private float value;

        private bool playing;
        private double startTime;

        public override bool isActive => playing;

        // Use this for initialization
        protected override void Init() {
            base.Init();
        }



        private void DoAnimation (NodePort source, double time)
        {
            if (source.fieldName == "start")
            {
                StartCoroutine(WaitForDSPTime(time, ()=>{
                    playing = true;
                    startTime = time;
                }));
            }
            else if (source.fieldName == "stop")
            {
                StopAllCoroutines();
                playing = false;
            }
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            StopAllCoroutines();
            playing = false;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            if (playing)
            {
                float currentTime = (float)(AudioSettings.dspTime - startTime);
                if (animationCurve.length > 0 && currentTime < animationCurve.keys[animationCurve.length - 1].time)
                {
                    return animationCurve.Evaluate(currentTime);
                }
                else
                    playing = false;
            }
            return 0f;
        
        }

        public override void NodeUpdate()
        {
            base.NodeUpdate();
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            DoAnimation(calledBy, time);//TODO: This probably needs a delay
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("start", visitedNodes);
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Automation/Animate";
        }
    }
}