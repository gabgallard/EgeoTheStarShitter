using System;
using System.Collections.Generic;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class AudioMixerGroupEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector, SplittableInspector
    {
        public override Type handlesType => typeof(AudioMixerGroup);



        public object GetDefaultValue()
        {
            return null;
        }


        //Default value in input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, new GUIContent(label), edit.unityObjectValue, typeof(AudioMixerGroup), false);
        }

        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        //Default value in player

        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, new GUIContent(label), edit.unityObjectValue, typeof(AudioMixerGroup), true);
        }
        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("Input", typeof(AudioMixerGroup), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Name", typeof(string), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("AudioMixer", typeof(AudioMixer), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            return ports;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("Input"), data.GetOutputPort("Name"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty("AudioMixer", data.nodeSerializedObject);
        }

        public override string GetPrettyTypeName()
        {
            return "Audio Mixer Group";
        }

      

        public int GetSplitNodeWidth()
        {
            return 208;
        }

    }
}
