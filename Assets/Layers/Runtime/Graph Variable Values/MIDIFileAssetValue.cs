using System;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Midi;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class MIDIFileAssetValue : GraphVariableValue, SplittableValue
    {
        public override Type handlesType => typeof(MidiFileAsset);

        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.unityObjectValue == null)
                return null;
            return (MidiFileAsset)graphVariable.unityObjectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultUnityObjectValue == null)
                return null;
            return (MidiFileAsset)graphVariable.defaultUnityObjectValue;
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
            MidiFileAsset midiFileAsset = target.GetInputValue<MidiFileAsset>("MIDI File");

            if (targetPort.fieldName == "Name")
            {
                return midiFileAsset == null ? "" : midiFileAsset.name;
            }
            else if (targetPort.fieldName == "endTimeSeconds")
            {
                return midiFileAsset == null ? 0.0 : midiFileAsset.endTimeSeconds;
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
