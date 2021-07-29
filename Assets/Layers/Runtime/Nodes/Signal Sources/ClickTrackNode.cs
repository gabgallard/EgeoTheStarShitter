using System.Collections;
using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Signal_Sources
{
    [Node.CreateNodeMenu("Signal sources/Click Track")]
    public class ClickTrackNode : FlowNode {

        [Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent enter;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected LayersEvent ClickTrackFinished;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected LayersEvent onBar;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected LayersEvent onBeat;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected LayersEvent onClick;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private float BPM = 60f;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private int numberOfBars = 1;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private int beatsPerBar = 4;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private int clicksPerBeat = 1;

        [SerializeField]
        private bool playClick = false;

        private AudioSource barSource = null;
        private AudioSource beatSource = null;
        private AudioSource clickSource = null;
        public override void NodeAwake()
        {
            Setup();
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            StartCoroutine( RunClickTrack(time,data, nodesCalledThisFrame));
        }



        private IEnumerator RunClickTrack(double time, Dictionary<string,object> data,int nodesCalledThisFrame)
        {
            double targetTime = 0;
            for (int index = 0; index < GetInputValue<int>("numberOfBars", numberOfBars)  * GetInputValue<int>("beatsPerBar", beatsPerBar) * GetInputValue<int>( "clicksPerBeat", clicksPerBeat); index++)
            {
                double secondsPerBeat = 60.0 / GetInputValue<float>("BPM", BPM)/ GetInputValue<int>("clicksPerBeat", clicksPerBeat);
                targetTime = time + index * secondsPerBeat;

                //Waiting for beat
                while (AudioSettings.dspTime + (Time.deltaTime * 2f) < targetTime)
                {

                    yield return null;
                }



                CallFunctionOnOutputNodes("onClick", targetTime, data, nodesCalledThisFrame);


                bool isBeat = (index) % (GetInputValue<int>("clicksPerBeat", clicksPerBeat)) == 0;
                bool isBar = (index) % (GetInputValue<int>("beatsPerBar", beatsPerBar) * GetInputValue<int>("clicksPerBeat", clicksPerBeat)) == 0;


                if (playClick && !isBeat && !isBar)
                    clickSource?.PlayScheduled(targetTime);

            

                if (isBeat)
                {
                    CallFunctionOnOutputNodes("onBeat", targetTime, data, nodesCalledThisFrame);
                    if (playClick && !isBar)
                        beatSource?.PlayScheduled(targetTime);
                }

                if (isBar)
                {
                    CallFunctionOnOutputNodes("onBar", targetTime,data, nodesCalledThisFrame);
                    if (playClick)
                        barSource?.PlayScheduled(targetTime);
                }

            }
            targetTime += 60.0 / GetInputValue<float>("BPM", BPM) / GetInputValue<int>("clicksPerBeat", clicksPerBeat);
            CallFunctionOnOutputNodes("ClickTrackFinished", targetTime,data, nodesCalledThisFrame);
        }

        public override void NodeUpdate()
        {
            Setup();
        }

        private void Setup()
        {
            if (playClick)
            {
                
                


                if (barSource == null)
                {
                    barSource = AudioPool.audioPoolInstance.Checkout("Bar");
                    barSource.playOnAwake = false;
                    barSource.clip = Resources.Load<AudioClip>("Symphony/Bar");
                }

                if (beatSource == null)
                {
                    beatSource = AudioPool.audioPoolInstance.Checkout("Beat");
                    beatSource.playOnAwake = false;
                    beatSource.clip = Resources.Load<AudioClip>("Symphony/Beat");
                }

                if (clickSource == null)
                {
                    clickSource = AudioPool.audioPoolInstance.Checkout("Click");
                    clickSource.playOnAwake = false;
                    clickSource.clip = Resources.Load<AudioClip>("Symphony/Click");
                }
            }
            else
            {

                if (barSource != null)
                {
                    AudioPool.audioPoolInstance.Return(barSource);
                    barSource = null;
                }

                if (beatSource != null)
                {
                    AudioPool.audioPoolInstance.Return(beatSource);
                    beatSource = null;
                }

                if (clickSource != null)
                {
                    AudioPool.audioPoolInstance.Return(clickSource);
                    clickSource = null;
                }

            }
        }
        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
        }


        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("enter",visitedNodes);
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Signal-Sources/Click-Track";
        }
    }
}