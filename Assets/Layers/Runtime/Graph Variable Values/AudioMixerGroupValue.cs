using System;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using UnityEngine.Audio;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class AudioMixerGroupValue : GraphVariableValue, SplittableValue
    {
        public override Type handlesType => typeof(AudioMixerGroup);

        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.unityObjectValue == null)
                return null ;

            return (AudioMixerGroup)graphVariable.unityObjectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultUnityObjectValue == null)
                return null;
            return (AudioMixerGroup)graphVariable.defaultUnityObjectValue;
        }

        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.unityObjectValue = (UnityEngine.Object)value;

        }

        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.defaultUnityObjectValue = (UnityEngine.Object)value;
        }

        public object GetSplitValue(NodePort targetPort, SplitNode target)
        {
            AudioMixerGroup mixerGroup = target.GetInputValue<AudioMixerGroup>("Input");

            if (targetPort.fieldName == "Name")
            {
                return mixerGroup == null ? "" : mixerGroup.name;
            }
            else if (targetPort.fieldName == "AudioMixer")
            {
                return mixerGroup == null ? null : mixerGroup.audioMixer;
            }
            return null;
        }

        public override bool CompareValues(Comparison.comparisonOperators comparator, object a, object b)
        {
            switch (comparator)
            {
                case Comparison.comparisonOperators.Equal:
                    return a == b;
                case Comparison.comparisonOperators.NotEqual:
                    return a != b;
            
            }
            return false;
        }

        public override object GetValueOnInitialization()
        {
            return null;
        }
        public override string GetValueInitializationString()
        {
            return "null";
        }
    }
}
