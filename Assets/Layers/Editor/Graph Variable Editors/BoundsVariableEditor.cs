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
    public class BoundsVariableEditor : GraphVariableEditor, InputNodeInspector,  SplittableInspector
    {
        public override Type handlesType => typeof(Bounds);

       

        public override string GetPrettyTypeName()
        {
            return "Bounds";
        }

        //default value in input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit variable)
        {
            variable.objectValue = EditorGUI.BoundsField(position, label, (Bounds)variable.objectValue);
        }

        public float CalculateInputNodeValueHeight(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight * 3f;
        }




        public object GetDefaultValue()
        {
            throw new NotImplementedException();
        }

        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> outputs = new List<PortDefinition>();
            outputs.Add(new PortDefinition("bounds", typeof(Bounds), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            outputs.Add(new PortDefinition("center", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            outputs.Add(new PortDefinition("extents", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            outputs.Add(new PortDefinition("size", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            outputs.Add(new PortDefinition("min", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            outputs.Add(new PortDefinition("max", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            return outputs;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty (data.GetInputPort("bounds"), data.GetOutputPort("center"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("extents"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("size"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("min"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("max"), data.nodeSerializedObject);
        }

        public int GetSplitNodeWidth()
        {
            return 208;
        }

        
    }
}