using System;
using System.Collections.Generic;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Midi;
using ABXY.Layers.Runtime.Nodes;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class MIDIFileVariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector, SplittableInspector
    {
        public override Type handlesType => typeof(MidiFileAsset);


        public object GetDefaultValue()
        {
            return null;
        }


        // value Input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, label, edit.unityObjectValue, typeof(MidiFileAsset), false);
        }

        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        


        // Default Player Value
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, label, edit.unityObjectValue, typeof(MidiFileAsset), false);
        }
        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }




        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("MIDI File", typeof(MidiFileAsset), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Name", typeof(string), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("End Time", typeof(double), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            return ports;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("MIDI File"), data.GetOutputPort("Name"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("End Time"), data.nodeSerializedObject);
        }

        public override string GetPrettyTypeName()
        {
            return "MIDI File";
        }

       

        public int GetSplitNodeWidth()
        {
            return 208;
        }

    }
}
