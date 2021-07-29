using System;
using System.Collections;
using System.Collections.Generic;
using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class ContactPointVariableEditor : GraphVariableEditor, InputNodeInspector, PlayerInspector, SplittableInspector
    {
        public override Type handlesType => typeof(ContactPoint);

        public object GetDefaultValue()
        {
            return new ContactPoint(); 
        }


        public override string GetPrettyTypeName()
        {
            return "Contact Point";
        }

        public void DrawInputNodeValue(Rect position, string label, VariableEdit variable)
        {
            VariableInspectorDrawFunctions.InputNodeFNs.DrawReadonly(position);
        }

        public float CalculateInputNodeValueHeight(VariableEdit variable, string label)
        {
            return VariableInspectorDrawFunctions.InputNodeFNs.ReadOnlyHeight();
        }

        public void DrawInPlayerInspector(Rect position, string label, VariableEdit variable)
        {
            VariableInspectorDrawFunctions.InputNodeFNs.DrawReadonly(position);
        }

        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return VariableInspectorDrawFunctions.InputNodeFNs.ReadOnlyHeight();
        }


        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("contactPoint", typeof(ContactPoint), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("normal", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("otherCollider", typeof(Collider), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("point", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("separation", typeof(float), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("thisCollider", typeof(Collider), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            return ports;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("contactPoint"), data.GetOutputPort("normal"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("otherCollider"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("point"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("separation"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("thisCollider"), data.nodeSerializedObject);
        }

        public int GetSplitNodeWidth()
        {
            return 208;
        }

        
    }
}