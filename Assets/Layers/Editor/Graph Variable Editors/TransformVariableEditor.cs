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
    public class TransformVariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector, SplittableInspector
    {
        public override Type handlesType => typeof(Transform);



        public object GetDefaultValue()
        {
            return null;
        }



        // Default Value in Input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            VariableInspectorDrawFunctions.InputNodeFNs.DrawReadonly(position, "Only writeable in Player");
        }
        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        //Player inspector
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, label, edit.unityObjectValue, typeof(Transform),true);
        }
        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("transform", typeof(Transform), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Name", typeof(string), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Position", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("LocalPosition", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Scale", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("LocalScale", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Rotation", typeof(Quaternion), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("LocalRotation", typeof(Quaternion), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("Parent", typeof(Transform), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            return ports;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("transform"), data.GetOutputPort("Name"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("Position"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("LocalPosition"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("Scale"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("LocalScale"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("Rotation"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("LocalRotation"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("Parent"), data.nodeSerializedObject);
        }

        public override string GetPrettyTypeName()
        {
            return "Transform";
        }

        

        

        public int GetSplitNodeWidth()
        {
            return 208;
        }

    }
}
