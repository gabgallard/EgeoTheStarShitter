using ABXY.Layers.Runtime.Curves;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Timeline.Playnode;
using UnityEngine;
using UnityEngine.Audio;

namespace ABXY.Layers.Runtime
{
    public class AudioSettingsData
    {
        [SerializeField]
        public bool bypassEffects;

        [SerializeField]
        public bool bypassListenerEffects;

        [SerializeField]
        public bool bypassReverbSettings;

        [SerializeField, Range(0, 128)]
        public int priority;

        [SerializeField, Range(0, 3)]
        public float pitch;

        [SerializeField, Range(-1, 1)]
        public float stereoPan;

        //[SerializeField, Range(0, 1)]
        //public float spatialBlend;

        //[SerializeField, Range(0f, 1.1f)]
        //public float reverbZoneMix;

        [SerializeField]
        public AudioMixerGroup targetMixerGroup;

        [SerializeField, Range(0f, 1f)]
        public float volume;

        public AnimationCurve reverbZoneMixCurve;

        public AnimationCurve spatialBlendCurve;

        public AnimationCurve spreadCurve;

        public float volumeRolloffMinDistance;

        public float volumeRolloffMaxDistance;

        public AnimationCurve volumeRolloffCurve;

        public AudioRolloffMode rolloffMode;

        [SerializeField]
        public Vector3 worldPosition;

        public System.Guid testGUID = System.Guid.NewGuid();

        public static AudioSettingsData defaultAudioSettings
        {
            get
            {
                return new AudioSettingsData(false, false, false, 1f, 128, 1f, 0f, 0f, 1f, 0f, 1f, 500f, AudioRolloffMode.Logarithmic, null, Vector3.zero);
            }
        }

        public AudioSettingsData(bool bypassEffects, 
            bool bypassListenerEffect, 
            bool bypassReverbSettings, 
            float volume, 
            int priority, 
            float pitch, 
            float stereoPan, 
            float spatialBlend, 
            float reverbZoneMix, 
            float spread,
            float minDistance,
            float maxDistance,
            AudioRolloffMode rolloffMode,
            AudioMixerGroup mixerGroup, 
            Vector3 worldPosition)
        {
            this.bypassEffects = bypassEffects;
            this.bypassListenerEffects = bypassListenerEffect;
            this.bypassReverbSettings = bypassReverbSettings;
            this.priority = priority;
            this.stereoPan = stereoPan;
            this.spatialBlendCurve = AnimationCurveUtils.HorizontalLine( spatialBlend);
            this.reverbZoneMixCurve = AnimationCurveUtils.HorizontalLine(reverbZoneMix);
            this.spreadCurve = AnimationCurveUtils.HorizontalLine(spread);
            this.volumeRolloffCurve = AnimationCurveUtils.LogarithmicCurve(minDistance / maxDistance, 1f, 1f);
            this.volumeRolloffMinDistance = minDistance;
            this.volumeRolloffMaxDistance = maxDistance;
            this.rolloffMode = rolloffMode;
            this.pitch = pitch;
            this.targetMixerGroup = mixerGroup;
            this.volume = volume;
            this.worldPosition = worldPosition;
        }

        public AudioSettingsData() { }

        public void ApplyToAudioSource(AudioSource audioSource, PlaynodeTrackItem track, float combinedBusVolume, float combinedBusPan, FlowNode flowNode)
        {
            if (audioSource == null)
                return;
            audioSource.bypassEffects = bypassEffects;
            audioSource.bypassListenerEffects = bypassListenerEffects;
            audioSource.bypassReverbZones = bypassReverbSettings;
            audioSource.priority = priority;
            audioSource.panStereo = stereoPan;
            audioSource.pitch = pitch;
            audioSource.outputAudioMixerGroup = targetMixerGroup;
            audioSource.volume = volume;
            audioSource.transform.position = worldPosition;
            audioSource.volume *= combinedBusVolume;
            if (track != null)
            {
                audioSource.volume *= flowNode.GetInputValue<float>(track.volumeInNodeName, track.volume);
                audioSource.panStereo = Mathf.Clamp( audioSource.panStereo + flowNode.GetInputValue<float>(track.panInNodeName, track.stereoPan) + combinedBusPan, -1f, 1f);
            }
            audioSource.minDistance = volumeRolloffMinDistance;
            audioSource.maxDistance = volumeRolloffMaxDistance;
            audioSource.rolloffMode = rolloffMode;
            audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, reverbZoneMixCurve);
            audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, spatialBlendCurve);
            audioSource.SetCustomCurve(AudioSourceCurveType.Spread, spreadCurve);
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, volumeRolloffCurve);
            audioSource.loop = false;
        }

        public override string ToString()
        {
            return string.Format("{{\nbypassEffects: {0},\n bypassListenerEffects: {1},\n bypassReverbSettings: {2},\n priority: {3},\n pitch: {4},\n " +
                                 "stereoPan:{5},\n targetMixerGroup: {6},\n volume: {7},\n worldPosition: {8}\n}}",
                bypassEffects, bypassListenerEffects, bypassReverbSettings, priority, pitch, stereoPan, targetMixerGroup, volume, worldPosition);
        }
    }
}

