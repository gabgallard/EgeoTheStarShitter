using System;
using System.Collections.Generic;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class AudioClipVariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector, SplittableInspector
    {
        public override Type handlesType => typeof(AudioClip);

        public object GetDefaultValue()
        {
            return null;
        }


        // Input  value
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, new GUIContent(label), edit.unityObjectValue, typeof(AudioClip), edit.canAccessSceneAssets);
        }

        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        //Player Inspector
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, new GUIContent(label), edit.unityObjectValue, typeof(AudioClip), edit.canAccessSceneAssets);
        }

        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("AudioClip", typeof(AudioClip), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Name", typeof(string), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Ambisonic", typeof(bool), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Channels", typeof(int), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Frequency", typeof(int), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Length", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            return ports;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty (data.GetInputPort("AudioClip"), data.GetOutputPort("Name"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("Ambisonic"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("Channels"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("Frequency"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("Length"), data.nodeSerializedObject);
        }

        public override string GetPrettyTypeName()
        {
            return "Audio Clip";
        }



        public int GetSplitNodeWidth()
        {
            return 208;
        }

        
    }
}
