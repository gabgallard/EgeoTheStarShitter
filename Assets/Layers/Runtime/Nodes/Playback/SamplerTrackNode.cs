using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Playback
{
    [Node.CreateNodeMenu("Playback/Sampler Track")]
    public class SamplerTrackNode : FlowNode {

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent MidiIn;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private AudioFlow AudioOut;

        [SerializeField]
        private string audioOutSendID = "";

        [SerializeField]
        private string parameterName = "";

        [SerializeField]
        private List<KeyNumToSound> samples = new List<KeyNumToSound>();

        // Use this for initialization
        protected override void Init() {
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            return null; // Replace this
        }

        [System.Serializable]
        private struct KeyNumToSound
        {
#pragma warning disable CS0649
            public int keyNum;
            public AudioClip audioClip;
#pragma warning restore CS0649
        }

        int currentPlayingCount = 0;

        public override bool isActive => currentPlayingCount != 0;

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            object midiOBJ = null;
            if(data.TryGetValue(parameterName, out midiOBJ)){
                if (midiOBJ is MidiData)
                {
                    MidiData midiData = midiOBJ as MidiData;
                    List<KeyNumToSound> selectedKeys = samples.Where(x => x.keyNum == midiData.noteNumber).ToList();
                    foreach (KeyNumToSound key in selectedKeys)
                    {
                        if (key.audioClip != null)
                            StartCoroutine(PlaySample(key.audioClip, time, midiData.velocity, data));
                    }
                }
            }

            
        
        }

        private IEnumerator PlaySample(AudioClip clip, double time, float velocity, Dictionary<string, object> parameters)
        {
            System.Guid eventID = System.Guid.NewGuid() ;
            AudioOut[] audioOuts = GetAudioOuts(GetOutputPort("AudioOut"), audioOutSendID);
            AudioSettingsData[] audioSettingsData = audioOuts.Select(x=>x.GetAudioSettings(eventID, parameters)).ToArray();


            AudioSource[] audioSources = AudioPool.audioPoolInstance.Checkout(audioSettingsData.Length, name);

            for (int index = 0; index < audioSettingsData.Length; index++)
            {
                audioSources[index].clip = clip;
                audioSettingsData[index].ApplyToAudioSource(audioSources[index], null, velocity, 0f, this);
            }
            //audioSource.volume = Mathf.Clamp01( velocity * GetInputValue<float>("volume", volume));



            while (AudioSettings.dspTime + 0.1 < time)
            {
                for (int index = 0; index < audioSettingsData.Length; index++)
                {
                    audioSources[index].clip = clip;
                    audioSettingsData[index].ApplyToAudioSource(audioSources[index], null, velocity, 0f, this);
                }
                yield return null;
            }

            double offsetTime = time - AudioSettings.dspTime;
            if (offsetTime < 0 && -offsetTime > clip.length)
            {
                AudioPool.audioPoolInstance.Return(audioSources);
                foreach (AudioOut audioOut in audioOuts)
                    audioOut.ReturnAudioSettings(eventID);
                yield break;
            }
            currentPlayingCount++;
            foreach (AudioSource audiosource in audioSources)
                audiosource.PlayScheduled(time);

            while (AudioSettings.dspTime < time + clip.length)
                yield return null;


            currentPlayingCount--;

            AudioPool.audioPoolInstance.Return(audioSources);
            foreach (AudioOut audioOut in audioOuts)
                audioOut.ReturnAudioSettings(eventID);
        }
        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
        }


        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("MidiIn", visitedNodes);
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Playback/Sampler-Track";
        }
    }
}