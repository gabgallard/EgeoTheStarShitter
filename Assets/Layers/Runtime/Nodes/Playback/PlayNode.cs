using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Timeline;
using ABXY.Layers.Runtime.Timeline.Playnode;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using ABXY.Layers.ThirdParty.MidiJack;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Playback
{
    [Node.CreateNodeMenu("Playback/Play")]
    public class PlayNode : FlowNode
    {

        //[SerializeField]
        //public List<TimeLineChange> timeLine = new List<TimeLineChange>();

        [SerializeField]
        public List<PlaynodeDataItem> timelineData = new List<PlaynodeDataItem>();

        //[SerializeField]
        //public List<PlayNodeTrack> tracks = new List<PlayNodeTrack>();

        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("actualTracks")]
        public List<PlaynodeTrackItem> tracks = new List<PlaynodeTrackItem>();

        [SerializeField]
        public List<BPMDataItem> bpms = new List<BPMDataItem>();

        [SerializeField]
        public BPMDataItem defaultBPM = new BPMDataItem();

        [SerializeField]
        public List<TimeSignatureDataItem> timeSignatures = new List<TimeSignatureDataItem>();

        public TimeSignatureDataItem defaultTimeSignature = new TimeSignatureDataItem();

        public enum AudioPreviewTypes { PlayAllAudio, PlayAudioUsingNodeSettings }
        public AudioPreviewTypes previewAudioType = AudioPreviewTypes.PlayAllAudio;

        public enum EventPreviewTypes { PlayOnNode, DoNotPlayOnNode }
        public EventPreviewTypes eventPreviewType = EventPreviewTypes.DoNotPlayOnNode;

        [SerializeField, Range(0f, 1f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        public float combinedVolume = 1f;

        [SerializeField, Range(-1f, 1f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        public float combinedPan = 0f;

        public enum EndTimeStyles { AtSpecifiedTime, WhenAllClipsAreFinished }

        public EndTimeStyles playFinishedWhen = EndTimeStyles.AtSpecifiedTime;

        /// <summary>
        /// in microseconds
        /// </summary>
        [SerializeField]
        public double endTime = 0;

#pragma warning disable CS0414
        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent play = null;



        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent midiOut = null;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private AudioFlow audioOut;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent pause = null;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent resume = null;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent stop = null;

#pragma warning restore CS0414

        [SerializeField]
        public string audioFlowOutID = "";





        public enum states { Playing, Paused, Stopped }

        public states currentState = states.Stopped;


        PlaybackSystem _playback = null;

        PlaybackSystem playback
        {
            get
            {
                if (_playback == null)
                {
                    _playback = new PlaybackSystem(
                        () => { return timelineData; },
                        StartCoroutine,
                        () => { return GetInputValue<float>("combinedVolume", combinedVolume); },
                        () => { return GetInputValue<float>("combinedPan", combinedPan); },
                        () => {
                            if (playFinishedWhen == EndTimeStyles.AtSpecifiedTime)
                                return endTime;
                            else
                                return CalculateLastClipEnd();
                        },
                        () => { return name; },
                        CallFunctionOnOutputNodes,
                        () => { return GetAudioOuts(GetOutputPort("audioOut"), audioFlowOutID); },
                        (timeLineChange) => { return GetAudioOuts(GetOutputPort(tracks[(int)timeLineChange.rowNumber].audioOutNodeName), tracks[(int)timeLineChange.rowNumber].audioOutSendName); },
                        () => { return tracks.Select(x => (TimeLineRowDataItem)x).ToList(); }, this);
                }
                return _playback;
            }
        }

        public override bool isActive => playback.runningSounds;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            playback.QueueAudio(time, 0, data, nodesCalledThisFrame);
        }



        public override void Pause(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            playback.Pause(time);
        }

        public override void ResumePlay(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            playback.ResumePlay(time);
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            //Debug.Log("[" + name + "]" + "[PlayNode][Stop] Stopped");
            playback.Stop(time);
            StartCoroutine(WaitForDSPTime(time, () => {
                //StopAllCoroutines();
            }));
        }

        private double CalculateLastClipEnd()
        {
            double endTime = 0;
            foreach (PlaynodeDataItem item in timelineData)
            {
                double thisItemEndTime = item.startTime + item.length;
                if (thisItemEndTime > endTime)
                    endTime = thisItemEndTime;
            }
            return endTime;
        }


        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            List<GraphEvent.EventParameterDef> parameters = new List<GraphEvent.EventParameterDef>();

            int trackNum = tracks.FindIndex(x => x.midiOutNodeName == port.fieldName);
            if (trackNum >= 0)
            {
                List<PlaynodeDataItem> data = timelineData.Select(x => x as PlaynodeDataItem).Where(x => x.rowNumber == trackNum).ToList();
                foreach (PlaynodeDataItem item in data)
                {
                    foreach (GraphVariable variable in item.eventParameters)
                        parameters.Add(new GraphEvent.EventParameterDef(variable.name, variable.typeName));

                    if (item.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.MIDI)
                    {
                        parameters.Add(new GraphEvent.EventParameterDef("NoteInfo", typeof(MidiData).FullName));
                    }
                }
            }
            else if (port.fieldName == "midiOut")
            {
                List<PlaynodeDataItem> data = timelineData.Select(x => x as PlaynodeDataItem).ToList();
                foreach (PlaynodeDataItem item in data)
                {
                    foreach (GraphVariable variable in item.eventParameters)
                        parameters.Add(new GraphEvent.EventParameterDef(variable.name, variable.typeName));

                    if (item.playnodeDataItemType == PlaynodeDataItem.PlaynodeDataItemTypes.MIDI)
                    {
                        parameters.Add(new GraphEvent.EventParameterDef("NoteInfo", typeof(MidiData).FullName));
                    }
                }
            }
            else
            {
                parameters.AddRange(GetIncomingEventParameterDefsOnPort("play", visitedNodes));
                parameters.AddRange(GetIncomingEventParameterDefsOnPort("stop", visitedNodes));

            }

            return parameters;
        }

        public override void NodeUpdate()
        {
            base.NodeUpdate();
        }
        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Playback/Play";
        }

        private void OnDestroy()
        {
            List<string> sends = new List<string>();
            sends.Add(audioFlowOutID);
            foreach (PlaynodeTrackItem track in tracks)
            {
                sends.Add(track.audioOutSendName);
            }

            List<AudioOut> selectedAudioOuts = new List<AudioOut>();

            if (soundGraph != null)
            {
                AudioOut[] audioOuts = soundGraph.GetNodesOfType<AudioOut>();
                foreach (string id in sends)
                {
                    AudioOut selectedOut = audioOuts.Where(x => x.nodeID.ToString() == id).FirstOrDefault();
                    if (selectedOut != null)
                        selectedAudioOuts.Add(selectedOut);
                }
            }

#if UNITY_EDITOR
            UnityEditor.Undo.RecordObjects(selectedAudioOuts.ToArray(), "Removed deleted node audio sends");
#endif

            foreach (AudioOut selectedOut in selectedAudioOuts)
                selectedOut.sendList.Remove(this);

        }

        public List<AudioSource> GetAudioSourcesInUse()
        {
            return playback.GetAudiosourcesInUse();
        }

        private class PlayToken
        {


            private double originalStartTime;
            public double startTime { get; private set; }

            private double lastPauseTime = 0;

            private double trailTime = 0.1;

            private double nodeEndTime = 0;

            private PlayNode player;

            public bool wasStopped { get; private set; }

            public List<AudioSource> audioSources = new List<AudioSource>();

            public bool forceStopped = false;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expectedStartTime">The expected start time for the time segment this token represents</param>
            /// <param name="nodeEndTime">The expected end time for the ENTIRE node</param>
            /// <param name="trailTime">The amount of buffer time used to schedule events in advance. Keep this larger than the deltatime</param>
            /// <param name="player">Reference to the owning node</param>
            public PlayToken(double expectedStartTime, double nodeEndTime, double trailTime, PlayNode player)
            {
                this.originalStartTime = expectedStartTime;
                this.startTime = expectedStartTime;
                this.trailTime = trailTime;
                this.player = player;
                this.nodeEndTime = nodeEndTime;
            }



            public void Pause(double time)
            {
                if (lastPauseTime == 0)
                    lastPauseTime = time;
                player.StartCoroutine(FlowNode.WaitForDSPTime(time, () => {
                    foreach (AudioSource audioSource in audioSources)
                        audioSource?.Pause();
                }));
            }

            public void Resume(double time)
            {
                if (lastPauseTime != 0)
                {
                    startTime += time - lastPauseTime;
                    lastPauseTime = 0;
                }
                player.StartCoroutine(FlowNode.WaitForDSPTime(time, () => {
                    foreach (AudioSource audioSource in audioSources)
                        audioSource?.UnPause();
                }));
            }

            public IEnumerator DoPause()
            {
                while (shouldYield)
                {
                    if (player.currentState == states.Stopped)
                        wasStopped = true;
                    yield return null;
                }
            }

            public void Stop()
            {
                foreach (AudioSource audioSource in audioSources)
                    audioSource?.Stop();
                forceStopped = true;
            }

            public bool shouldYield
            {
                get
                {
                    return AudioSettings.dspTime + trailTime + Time.deltaTime < startTime || player.currentState == states.Paused;
                }
            }


            /// <summary>
            /// True if playback is past the end time for the play node
            /// </summary>
            public bool reachedEndTime
            {
                get
                {
                    return AudioSettings.dspTime > nodeEndTime + CalculatePauseDelay() || player.currentState == states.Stopped;
                }
            }

            public double CalculatePauseDelay()
            {
                return startTime - originalStartTime;
            }

        }
    }

    [System.Serializable]
    public class PlayNodeTrack
    {
        public string trackLabel = "Track";
        public float volume = 1f;
        public float stereoPan = 0f;


        [SerializeField]
        public string audioOutNodeName = "";

        [SerializeField]
        public string midiOutNodeName = "";


        public bool exposed = false;

        public PlayNodeTrack(PlayNode node)
        {
            NodePort audioOutNode = node.AddDynamicOutput(typeof(AudioFlow), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict);
            this.audioOutNodeName = audioOutNode.fieldName;

            NodePort midiOut = node.AddDynamicOutput(typeof(LayersEvent), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict);
            this.midiOutNodeName = midiOut.fieldName;
        }

        public void ClearConnections(PlayNode node)
        {
            node.GetOutputPort(audioOutNodeName).ClearConnections();
            node.GetOutputPort(midiOutNodeName).ClearConnections();
        }


    }
}