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
    public class PhysicMaterialVariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector, SplittableInspector
    {
        public override Type handlesType => typeof(PhysicMaterial);


        public object GetDefaultValue()
        {
            return null ;
        }

        public override string GetPrettyTypeName()
        {
            return "Physics Material";
        }

        //Default in input

        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, label, edit.unityObjectValue, typeof(PhysicMaterial), false);
        }
        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        //Player value
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, label, edit.unityObjectValue, typeof(PhysicMaterial), false);

        }
        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("physicsMaterial", typeof(PhysicMaterial), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("name", typeof(string), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("bounciness", typeof(float), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("dynamicFriction", typeof(float), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("staticFriction", typeof(float), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            return ports;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("physicsMaterial"), data.GetOutputPort("name"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("bounciness"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("dynamicFriction"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("staticFriction"), data.nodeSerializedObject);
        }

        public int GetSplitNodeWidth()
        {
            return 240;
        }

    }
}