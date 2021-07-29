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
    public class QuaternionVariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector, SplittableInspector
    {
        public override Type handlesType => typeof(Quaternion);

        public object GetDefaultValue()
        {
            return Quaternion.identity;
        }

        public override string GetPrettyTypeName()
        {
            return "Quaternion";
        }

        // Default value in input
        public void DrawInputNodeValue(Rect position, string label,VariableEdit edit)
        {
            edit.objectValue = Quaternion.Euler(EditorGUI.Vector3Field(position, label, ((Quaternion)edit.objectValue).eulerAngles));
        }
        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 2f;
        }

        

        // Player inspector
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = Quaternion.Euler(EditorGUI.Vector3Field(position, label, ((Quaternion)edit.objectValue).eulerAngles));
        }


        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 2f;
        }

        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> outputs = new List<PortDefinition>();
            outputs.Add(new PortDefinition("quaternion", typeof(Quaternion), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Strict));
            outputs.Add(new PortDefinition("eulerAngles", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            outputs.Add(new PortDefinition("normalized", typeof(Quaternion), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict));
            outputs.Add(new PortDefinition("x", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            outputs.Add(new PortDefinition("y", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            outputs.Add(new PortDefinition("z", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            outputs.Add(new PortDefinition("w", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            return outputs;
        }

        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("quaternion"), data.GetOutputPort("eulerAngles"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("normalized"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("x"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("y"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("z"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("w"), data.nodeSerializedObject);
        }

        public int GetSplitNodeWidth()
        {
            return 208;
        }

    }
}