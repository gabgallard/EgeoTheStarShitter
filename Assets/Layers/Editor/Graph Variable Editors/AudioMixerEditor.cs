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
    public class AudioMixerEditor : GraphVariableEditor, InputNodeInspector, PlayerInspector, SplittableInspector
    {
        public override Type handlesType => typeof(AudioMixer);


        public object GetDefaultValue()
        {
            return null;
        }


        //Default in input
        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, new GUIContent(label), edit.unityObjectValue, typeof(AudioMixer), false);
        }


        // Draw in player
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, new GUIContent(label), edit.unityObjectValue, typeof(AudioMixer), true);
        }
        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> nodes = new List<PortDefinition>();
            nodes.Add(new PortDefinition("Input", typeof(AudioMixer), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            nodes.Add(new PortDefinition("Name", typeof(string), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            nodes.Add(new PortDefinition("OutputAudioMixerGroup", typeof(AudioMixerGroup), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            return nodes;
        }
        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("Input"), data.GetOutputPort("Name"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("OutputAudioMixerGroup"), data.nodeSerializedObject);
        }

        public override string GetPrettyTypeName()
        {
            return "Audio Mixer";
        }

        

        public int GetSplitNodeWidth()
        {
            return 208;
        }

    }
}
