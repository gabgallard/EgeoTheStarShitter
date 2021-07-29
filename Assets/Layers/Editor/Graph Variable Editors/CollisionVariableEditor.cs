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

namespace ABXY.Layers.Editor.Graph_Variable_Editors {
    public class CollisionVariableEditor : GraphVariableEditor, InputNodeInspector, PlayerInspector, SplittableInspector
    {
        public override Type handlesType => typeof(Collision);


        public object GetDefaultValue()
        {
            return null;
        }

        public override string GetPrettyTypeName()
        {
            return "Collision";
        }

       

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("collision"), data.GetOutputPort("collider"), data.nodeSerializedObject);
            //NodeEditorGUILayout.PortPair(target.GetInputPort("collision"), target.GetOutputPort("collider"));
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("contactCount"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("contacts"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("gameObject"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("impulse"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("relativeVelocity"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("rigidbody"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("transform"), data.nodeSerializedObject);
        }


        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("collision", typeof(Collision), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("collider", typeof(Collider), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("contactCount", typeof(int), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("contacts", typeof(List<GraphVariable>), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("gameObject", typeof(GameObject), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("impulse", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("relativeVelocity", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("rigidbody", typeof(Rigidbody), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("transform", typeof(Transform), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            return ports;
        }

        public int GetSplitNodeWidth()
        {
            return 208;
        }

        public void DrawInPlayerInspector(Rect position, string label, VariableEdit variable)
        {
            VariableInspectorDrawFunctions.PlayerInspectorFNs.DrawReadonly(position);
        }

        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return VariableInspectorDrawFunctions.PlayerInspectorFNs.ReadOnlyHeight();
        }

        public void DrawInputNodeValue(Rect position, string label, VariableEdit variable)
        {
            VariableInspectorDrawFunctions.InputNodeFNs.DrawReadonly(position);
        }

        public float CalculateInputNodeValueHeight(VariableEdit variable, string label)
        {
            return VariableInspectorDrawFunctions.InputNodeFNs.ReadOnlyHeight();
        }

    }
}