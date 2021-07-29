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

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private bool bypassListener = false;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private bool bypassReverb = false;

        [SerializeField, Range(0f, 1f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float volume = 1f;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private AudioMixerGroup audioMixerGroup = null;

        [SerializeField, Range(0, 256), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private int priority = 128;

        [SerializeField, Range(0f, 3f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float pitch = 1f;

        [SerializeField, Range(-1f, 1f), Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited)]
        private float stereoPan = 0f;

    

    

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private Vector3 worldPosition = Vector3.zero;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Override, Node.TypeConstraint.Strict)]
        private Transform playAtTransform = null;

        public enum SettingsSources { Node, Input}

        public enum ShareSettings { ShareSettings, InstanceSettings}

        [SerializeField, NodeEnum]
        private SettingsSources settingsSource = SettingsSources.Node;

        [SerializeField, NodeEnum]
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

            if (shareSettings == ShareSettings.ShareSettings)
            {
                switch (settingsSource)
                {
                    case SettingsSources.Node:

                        UpdateAudiosettings(sharedData, parameters);
                        return sharedData;
                    case SettingsSources.Input:
                        AudioSettingsData presupply = GetInputValue<AudioSettingsData>("audioSettings", audioSettings);
                        if (presupply == null)
                            presupply = AudioSettingsData.defaultAudioSettings;
                        return presupply;
                }
            }
            else
            {
                switch (settingsSource)
                {
                    case SettingsSources.Node:

                        AudioSettingsData dataInstance = null;
                        if (!instancedData.TryGetValue(eventID, out dataInstance))
                        {
                            dataInstance = new AudioSettingsData();
                            UpdateAudiosettings(dataInstance, parameters);
                            instancedData.Add(eventID, dataInstance);
                        }
                        return dataInstance;
                    case SettingsSources.Input:
                        AudioSettingsData presupply = GetInputValue<AudioSettingsData>("audioSettings", audioSettings);
                        if (presupply == null)
                            presupply = AudioSettingsData.defaultAudioSettings;
                        return presupply;
                }
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
            if (shareSettings == ShareSettings.ShareSettings)
            {
                UpdateAudiosettings(sharedData, lastParameters);
            }
        }

        private void UpdateAudiosettings(AudioSettingsData data, Dictionary<string, object> parameters)
        {
            data.bypassEffects = GetInputOrParameterValue<bool>("bypassEffects", bypassEffects, parameters);
            data.bypassListenerEffects = GetInputOrParameterValue<bool>("bypassListener", bypassListener, parameters);
            data.bypassReverbSettings = GetInputOrParameterValue<bool>("bypassReverb", bypassReverb, parameters);
            data.volume = GetInputOrParameterValue<float>("volume", volume, parameters);
            data.priority = GetInputOrParameterValue<int>("priority", priority, parameters);
            data.pitch = GetInputOrParameterValue<float>("pitch", pitch, parameters);
            data.stereoPan = GetInputOrParameterValue<float>("stereoPan", stereoPan, parameters);
            //data.spatialBlend = GetInputOrParameterValue<float>("spatialBlend", spatialBlend, parameters);
            //data.reverbZoneMix = GetInputOrParameterValue<float>("reverbZoneMix", reverbZoneMix, parameters);
            data.targetMixerGroup = GetInputOrParameterValue<AudioMixerGroup>("audioMixerGroup", audioMixerGroup, parameters);

            Transform targetTransform = GetInputOrParameterValue<Transform>("playAtTransform", playAtTransform, parameters);
            if (targetTransform != null)
                data.worldPosition = targetTransform.position;
            else
                data.worldPosition = GetInputOrParameterValue<Vector3>("worldPosition", worldPosition, parameters);

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