using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts.Attributes;
using ABXY.Layers.Runtime.Curves;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;
using UnityEngine.Audio;

namespace ABXY.Layers.Runtime.Nodes.Playback
{
    [Node.CreateNodeMenu("Playback/Audio Out")]
    public class AudioOut : FlowNode, AudioOutSource {

#pragma warning disable CS0414
        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private AudioFlow audioIn = null;
#pragma warning restore CS0414

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AudioSettingsData audioSettings = null;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private bool bypassEffects = false;
        [SerializeField]
        private bool bypassEffectsInstance;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private bool bypassListener = false;
        [SerializeField]
        private bool bypassListenerInstance = false;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private bool bypassReverb = false;
        [SerializeField]
        private bool bypassReverbInstance = false;

        [SerializeField, Range(0f, 1f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float volume = 1f;
        [SerializeField]
        private bool volumeInstance;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AudioMixerGroup audioMixerGroup = null;
        [SerializeField]
        private bool audioMixerGroupInstance;

        [SerializeField, Range(0, 256), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private int priority = 128;
        [SerializeField]
        private bool priorityInstance;


        [SerializeField, Range(0f, 3f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float pitch = 1f;
        [SerializeField]
        private bool pitchInstance;

        [SerializeField, Range(-1f, 1f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float stereoPan = 0f;
        [SerializeField]
        private bool stereoPanInstance;

    

    

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private Vector3 worldPosition = Vector3.zero;
        [SerializeField]
        private bool worldPositionInstance;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private Transform playAtTransform = null;
        [SerializeField]
        private bool playAtTransformInstance;

        public enum SettingsSources { Node, Input}

        [System.Obsolete]
        public enum ShareSettings { ShareSettings, InstanceSettings, NoSetting}

        [SerializeField, NodeEnum]
        private SettingsSources settingsSource = SettingsSources.Node;

        //[SerializeField, NodeEnum]

        [System.Obsolete]
        [SerializeField]
        private ShareSettings shareSettings = ShareSettings.ShareSettings;

#pragma warning disable CS0414
        // graph stuff
        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AnimationCurve reverbZoneMixCurve = new AnimationCurve(new Keyframe(0f, 1f, 0f, 0f));
        [SerializeField, Range(0f, 1.1f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float reverbZoneMix = 1f;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AnimationCurve spreadCurve = new AnimationCurve(new Keyframe(0f, 0f));
        [SerializeField, Range(0f, 360f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float spread = 0f;


        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AnimationCurve spatialCurve = new AnimationCurve(new Keyframe(0f, 0f));
        [SerializeField, Range(0f, 1f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float spatialBlend = 0f;

        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Strict), Range(0f,5f)]
        private float dopplerLevel = 1f;
        [SerializeField]
        private bool dopplerLevelInstance;

#pragma warning restore CS0414

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AnimationCurve volumeFalloffCurve = AnimationCurveUtils.LogarithmicCurve(1f / 500f, 1f, 1f);

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float minDistance = 1;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float maxDistance = 500;

        [SerializeField]
        AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

        private AudioSettingsData sharedData = new AudioSettingsData();

        private Dictionary<System.Guid, AudioSettingsData> instancedData = new Dictionary<System.Guid, AudioSettingsData>();

        private Dictionary<string, object> lastParameters = new Dictionary<string, object>();

        [SerializeField]
        public List<FlowNode> sendList = new List<FlowNode>();

        public override bool isActive {
            get
            {
                foreach(NodePort node in GetInputPort("audioIn").GetConnections())
                {
                    if ((node.node as FlowNode).isActive)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventAudioSettings">The audio settings from the flow</param>
        /// <returns></returns>
        public AudioSettingsData GetAudioSettings(System.Guid eventID, Dictionary<string, object> parameters)
        {
            lastParameters = parameters;


            switch (settingsSource)
            {
                case SettingsSources.Node:

                    AudioSettingsData dataInstance = null;
                    if (!instancedData.TryGetValue(eventID, out dataInstance))
                    {
                        dataInstance = new AudioSettingsData();
                        UpdateAudiosettings(dataInstance, parameters,true);
                        instancedData.Add(eventID, dataInstance);
                    }
                    else
                    {
                        UpdateAudiosettings(dataInstance, parameters, false);
                    }
                    return dataInstance;
                case SettingsSources.Input:
                    AudioSettingsData presupply = GetInputValue<AudioSettingsData>("audioSettings", audioSettings);
                    if (presupply == null)
                        presupply = AudioSettingsData.defaultAudioSettings;
                    return presupply;
            }


            return AudioSettingsData.defaultAudioSettings;
        }

        public void ReturnAudioSettings(System.Guid eventID)
        {
            if (instancedData.ContainsKey(eventID))
                instancedData.Remove(eventID);
        }

        public override void NodeUpdate()
        {
            
             UpdateAudiosettings(sharedData, lastParameters,false);
            
        }

        /// <summary>
        /// Retrieves the current settings on the node, and writes it to data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parameters"></param>
        private void UpdateAudiosettings(AudioSettingsData data, Dictionary<string, object> parameters, bool initialCall)
        {
            if(initialCall || !bypassEffectsInstance)
                data.bypassEffects = GetInputOrParameterValue<bool>("bypassEffects", bypassEffects, parameters);

            if (initialCall || !bypassListenerInstance)
                data.bypassListenerEffects = GetInputOrParameterValue<bool>("bypassListener", bypassListener, parameters);


            if (initialCall || !bypassReverbInstance)
                data.bypassReverbSettings = GetInputOrParameterValue<bool>("bypassReverb", bypassReverb, parameters);


            if (initialCall || !volumeInstance)
                data.volume = GetInputOrParameterValue<float>("volume", volume, parameters);


            if (initialCall || !priorityInstance)
                data.priority = GetInputOrParameterValue<int>("priority", priority, parameters);

            if (initialCall || !pitchInstance)
                data.pitch = GetInputOrParameterValue<float>("pitch", pitch, parameters);

            if (initialCall || !stereoPanInstance)
                data.stereoPan = GetInputOrParameterValue<float>("stereoPan", stereoPan, parameters);
            //data.spatialBlend = GetInputOrParameterValue<float>("spatialBlend", spatialBlend, parameters);
            //data.reverbZoneMix = GetInputOrParameterValue<float>("reverbZoneMix", reverbZoneMix, parameters);

            if (initialCall || !audioMixerGroupInstance)
                data.targetMixerGroup = GetInputOrParameterValue<AudioMixerGroup>("audioMixerGroup", audioMixerGroup, parameters);


            //handling playback position
            if (initialCall)
            {
                data.worldPositionAtEventStart = GetInputOrParameterValue<Vector3>("worldPosition", worldPosition, parameters);
                data.targetTransformAtEventStart = GetInputOrParameterValue<Transform>("playAtTransform", playAtTransform, parameters);
            }

            Transform targetTransform = playAtTransformInstance? data.targetTransformAtEventStart: GetInputOrParameterValue<Transform>("playAtTransform", playAtTransform, parameters);
            if(targetTransform != null)
                data.worldPosition = targetTransform.position;
            else
                data.worldPosition = worldPositionInstance? data.worldPositionAtEventStart : GetInputOrParameterValue<Vector3>("worldPosition", worldPosition, parameters);


            if (initialCall || !dopplerLevelInstance)
                data.dopplerLevel = GetInputOrParameterValue<float>("dopplerLevel", dopplerLevel, parameters);

            // curves

            data.reverbZoneMixCurve = GetCurveProperty("reverbZoneMix", "reverbZoneMixCurve", reverbZoneMixCurve, parameters);
            data.spatialBlendCurve = GetCurveProperty("spatialBlend", "spatialCurve", spatialCurve, parameters);
            data.spreadCurve = GetCurveProperty("spread", "spreadCurve", spreadCurve, parameters);
            data.volumeRolloffCurve = volumeFalloffCurve;
            data.volumeRolloffMinDistance = GetInputOrParameterValue<float>("minDistance", minDistance, parameters);
            data.volumeRolloffMaxDistance = GetInputOrParameterValue<float>("maxDistance", maxDistance, parameters);
            data.rolloffMode = rolloffMode;
        }

        private AnimationCurve GetCurveProperty(string floatPropertyPath, string curvePropertyPath, AnimationCurve defaultValue, Dictionary<string, object> parameters)
        {
            AnimationCurve resultCurve = new AnimationCurve();
            if (defaultValue != null)
                resultCurve.keys = defaultValue.keys;

            bool floatParameterPresent = variablesDrivenByParameters.Find(x => x.serializedPropertyPath == floatPropertyPath) != null || GetInputPort(floatPropertyPath).IsConnected;
            bool curveParameterPresent = variablesDrivenByParameters.Find(x => x.serializedPropertyPath == curvePropertyPath) != null || GetInputPort(curvePropertyPath).IsConnected;

            if (floatParameterPresent)
            {
                float floatValue = GetInputOrParameterValue<float>(floatPropertyPath, 0f, parameters);
                resultCurve = AnimationCurveUtils.HorizontalLine(floatValue);
            }

            if (curveParameterPresent)
            {
                resultCurve = GetInputOrParameterValue<AnimationCurve>(curvePropertyPath, resultCurve, parameters);
            }

            return resultCurve;
        }



        // Use this for initialization
        protected override void Init() {
            base.Init();
            Migrate();
        }

        public void Migrate()
        {
            //upgrading
#pragma warning disable CS0612
            if (shareSettings != ShareSettings.NoSetting)
            {
                bool setting = shareSettings == ShareSettings.InstanceSettings;
                audioMixerGroupInstance = setting;
                bypassEffectsInstance = setting;
                bypassListenerInstance = setting;
                bypassReverbInstance = setting;
                dopplerLevelInstance = setting;
                pitchInstance = setting;
                playAtTransformInstance = setting;
                priorityInstance = setting;
                stereoPanInstance = setting;
                volumeInstance = setting;
                worldPositionInstance = setting;
            }
            shareSettings = ShareSettings.NoSetting;
#pragma warning restore CS0612
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            return null; // Replace this
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Playback/Audio-Out";
        }
    }
}