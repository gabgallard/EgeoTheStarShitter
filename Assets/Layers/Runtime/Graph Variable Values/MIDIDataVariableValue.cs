using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class MIDIDataVariableValue : GraphVariableValue, SplittableValue, CombinableValue
    {
        public override Type handlesType => typeof(MidiData);

        public override bool CompareValues(Comparison.comparisonOperators comparator, object a, object b)
        {
            throw new NotImplementedException();
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            return graphVariable.defaultObjectValue;
        }
        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.defaultObjectValue = value;
        }

        public override object GetValue(GraphVariableBase graphVariable)
        {
            return graphVariable.objectValue;
        }


        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.objectValue = value;
        }

        public override string Serialize(object objectValue)
        {
            string serializedValue = "";
            if (objectValue != null && objectValue is MidiData)
            {
                MidiData castObject = (MidiData)objectValue;
                serializedValue = string.Format("{0}|{1}|{2}", castObject.noteNumber, castObject.velocity, castObject.channelNumber);
            }
            return serializedValue;
        }

        public override object Deserialize(string serializedObjectValue)
        {
            MidiData data = MidiData.defaultMidiFlowInfo;

            if (!string.IsNullOrEmpty(serializedObjectValue))
            {
                string[] components = serializedObjectValue.Split('|');

                if (components.Length == 3)
                {
                    int noteNumber = 0;
                    bool noteSuccessful = int.TryParse(components[0], out noteNumber);

                    float velocity = 0f;
                    bool velSuccessful = float.TryParse(components[1], out velocity);

                    MidiData.MidiChannel channel = MidiData.MidiChannel.All;
                    bool channelSuccessful = Enum.TryParse(components[2], out channel);

                    if (noteSuccessful && velSuccessful && channelSuccessful)
                        data = new MidiData(noteNumber, channel, velocity);
                }
            }

            return data;
        }

        public object GetSplitValue(NodePort targetPort, SplitNode target)
        {
            MidiData inputData = target.GetInputValue<MidiData>("Input");

            if (targetPort.fieldName == "Channel")
            {
                return inputData == null ? MidiData.MidiChannel.All : inputData.channelNumber;
            }else if (targetPort.fieldName == "NoteNumber")
            {
                return inputData == null ? 0 : inputData.noteNumber;
            }
            else if (targetPort.fieldName == "Velocity")
            {
                return inputData == null ? 0f : inputData.velocity;
            }
            return null;
        }

        public object GetCombineValue(NodePort targetPort, CombineNode target)
        {
            return new MidiData(
                target.GetInputValue<int>("NoteNumber", target.GetDefaultValueTyped<int>("NoteNumber", 1)),
                target.GetInputValue<MidiData.MidiChannel>("Channel", target.GetDefaultValueTyped<MidiData.MidiChannel>("Channel", 0f)),
                target.GetInputValue<float>("Velocity", target.GetDefaultValueTyped<float>("Velocity", 0f)));
        }

        public override object GetValueOnInitialization()
        {
            return new MidiData();
        }

        public override Color GetDefaultColorPro()
        {
            return new Color(0.8313726f, 0.9019608f, 0.6470588f, 1f);
        }

        public override string GetValueInitializationString()
        {
            return "MidiData.defaultMidiFlowInfo";
        }

    }
}