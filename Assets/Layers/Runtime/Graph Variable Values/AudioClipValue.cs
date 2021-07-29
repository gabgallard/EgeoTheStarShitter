using System;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using UnityEngine;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class AudioClipValue : GraphVariableValue, SplittableValue
    {
        public override Type handlesType => typeof(AudioClip);

        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.unityObjectValue is AudioClip)
                return (AudioClip)graphVariable.unityObjectValue;
            return null;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultUnityObjectValue is AudioClip)
                return (AudioClip)graphVariable.defaultUnityObjectValue;
            return null;
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
            AudioClip audioClip = target.GetInputValue<AudioClip>("AudioClip");

            if (targetPort.fieldName == "Name")
            {
                return audioClip == null ? "" : audioClip.name;
            }
            else if (targetPort.fieldName == "Ambisonic")
            {
                return audioClip == null ? false : audioClip.ambisonic;
            }
            else if (targetPort.fieldName == "Channels")
            {
                return audioClip == null ? 0 : audioClip.channels;
            }
            else if (targetPort.fieldName == "Frequency")
            {
                return audioClip == null ? 0 : audioClip.frequency;
            }
            else if (targetPort.fieldName == "Length")
            {
                return audioClip == null ? 0 : audioClip.length;
            }
            return null;
        }

        public override Color GetDefaultColor()
        {
            return new Color32(101, 157, 228, 255);
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
