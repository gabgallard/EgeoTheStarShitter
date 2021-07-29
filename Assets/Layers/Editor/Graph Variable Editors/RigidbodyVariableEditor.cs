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
    public class RigidbodyVariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector, SplittableInspector
    {
        public override Type handlesType => typeof(Rigidbody);


        public object GetDefaultValue()
        {
            return null;
        }

        public override string GetPrettyTypeName()
        {
            return "Rigidbody";
        }

        //Input default value
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            VariableInspectorDrawFunctions.InputNodeFNs.DrawReadonly(position, "Writeable in Player");
        }

        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return VariableInspectorDrawFunctions.InputNodeFNs.ReadOnlyHeight();
        }



        // Player inspector
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.unityObjectValue = EditorGUI.ObjectField(position, label, edit.unityObjectValue, typeof(Rigidbody), true);
        }

        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("rigidbody", typeof(Rigidbody), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("angularDrag", typeof(float), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("angularVelocity", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("centerOfMass", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("mass", typeof(float), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("position", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("rotation", typeof(Quaternion), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("velocity", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            ports.Add(new PortDefinition("drag", typeof(float), NodePort.IO.Output, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            return ports;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("rigidbody"), data.GetOutputPort("angularDrag"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("angularVelocity"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("centerOfMass"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("mass"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("position"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("rotation"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("velocity"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("drag"), data.nodeSerializedObject);
        }

        

        public int GetSplitNodeWidth()
        {
            return 208;
        }
    }
}
