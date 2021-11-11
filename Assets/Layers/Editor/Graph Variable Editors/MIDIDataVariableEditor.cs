using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class MIDIDataVariableEditor : GraphVariableEditor, InputNodeInspector, PlayerInspector, SplittableInspector, CombinableInspector
    {
        public override Type handlesType => typeof(MidiData);

        public object GetDefaultValue()
        {
            return MidiData.defaultMidiFlowInfo;
        }

        public override string GetPrettyTypeName()
        {
            return "MIDI Data";
        }

        // Default in input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            Rect controlPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(controlPosition, label + ":");


            controlPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel++;
            controlPosition = EditorGUI.IndentedRect(controlPosition);
            EditorGUI.indentLevel--;

            float currentLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 84;

            if (edit.objectValue != null && edit.objectValue is MidiData)
            {

                MidiData data = edit.objectValue as MidiData;
                LayersGUIUtilities.DrawNote(controlPosition, data.noteNumber, (change)=> {
                    data.noteNumber = change;
                });

                controlPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                data.velocity = EditorGUI.Slider(controlPosition, "Velocity", data.velocity, 0f, 1f);

                controlPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                LayersGUIUtilities.DrawDropdown(controlPosition, new GUIContent("Channel"), data.channelNumber, (newSelection) => {
                    data.channelNumber = (MidiData.MidiChannel)newSelection;
                });
            }
            EditorGUIUtility.labelWidth = currentLabelWidth;
        }

        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight * 4f + EditorGUIUtility.standardVerticalSpacing * 5f;
        }


       


        // Default in Player
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            DrawInputNodeValue(position, label, edit);
        }

        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight * 4f + EditorGUIUtility.standardVerticalSpacing * 5f;
        }



        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("Input", typeof(MidiData), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("Channel", typeof(MidiData.MidiChannel), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("NoteNumber", typeof(int), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("Velocity", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            return ports;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            NodeEditorGUILayout.PortPair(data.GetInputPort("Input"), data.GetOutputPort("Channel"));
            NodeEditorGUILayout.PortField(data.GetOutputPort("NoteNumber"));
            NodeEditorGUILayout.PortField(data.GetOutputPort("Velocity"));
        }


        public List<PortDefinition> GetCombinePorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("Channel", typeof(MidiData.MidiChannel), NodePort.IO.Input, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("NoteNumber", typeof(int), NodePort.IO.Input, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("Velocity", typeof(float), NodePort.IO.Input, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("Output", typeof(MidiData), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            return ports;
        }


        public void DrawCombineGUI(CombineSplitData data)
        {
            DrawPortWithDefaults(data.GetInputPort("Channel"), (defaultObj, onNewValue) => {
                LayersGUIUtilities.DrawDropdown(new GUIContent("Channel"), (System.Enum)defaultObj, onNewValue,searcheable:false);
            });


            DrawPortWithDefaults(data.GetInputPort("NoteNumber"), (defaultObj, onChange) => {
                 LayersGUIUtilities.DrawNote((int)defaultObj, (newValue)=> {
                     onChange?.Invoke(newValue);
                });
            });

            DrawPortWithDefaults(data.GetInputPort("Velocity"), (defaultObj) => {
                return EditorGUILayout.FloatField("Velocity", (float)defaultObj);
            });

            NodeEditorGUILayout.PortField(data.GetOutputPort("Output"));
        }

        public int GetSplitNodeWidth()
        {
            return 208;
        }

        public int GetCombineNodeWidth()
        {
            return 208;
        }

    }
}