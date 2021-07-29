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
    public class Vector2VariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector, SplittableInspector, CombinableInspector
    {
        public override Type handlesType => typeof(Vector2);

        public object GetDefaultValue()
        {
            return Vector2.zero;
        }

        //value input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = EditorGUI.Vector2Field(position, label, (Vector2)edit.objectValue);
        }

        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            if (string.IsNullOrEmpty(label))
                return (EditorGUIUtility.singleLineHeight * 1f) + (EditorGUIUtility.standardVerticalSpacing * 2f);
            return (EditorGUIUtility.singleLineHeight * 2f) + (EditorGUIUtility.standardVerticalSpacing * 3f);
        }




        // Player value

        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = EditorGUI.Vector2Field(position, label, (Vector2)edit.objectValue);
        }
        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            if (string.IsNullOrEmpty(label))
                return (EditorGUIUtility.singleLineHeight * 1f) + (EditorGUIUtility.standardVerticalSpacing * 2f);
            return (EditorGUIUtility.singleLineHeight * 2f) + (EditorGUIUtility.standardVerticalSpacing * 3f);
        }

        public List<PortDefinition> GetSplitPorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("Vector2", typeof(Vector2), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("x", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("y", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            return ports;
        }

        public List<PortDefinition> GetCombinePorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("x", typeof(float), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("y", typeof(float), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("Output", typeof(Vector2), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            return ports;
        }


        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("Vector2"), data.GetOutputPort("x"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("y"), data.nodeSerializedObject);
        }

        public void DrawCombineGUI(CombineSplitData data)
        {
            DrawPortWithDefaults(data.GetInputPort("x"), (defaultObj) => {
                return EditorGUILayout.FloatField("x", (float)defaultObj);
            });


            DrawPortWithDefaults(data.GetInputPort("y"), (defaultObj) => {
                return EditorGUILayout.FloatField("y", (float)defaultObj);
            });


            NodeEditorGUILayout.PortField(data.GetOutputPort("Output"));
        }

        public override string GetPrettyTypeName()
        {
            return "Vector 2";
        }


        public int GetSplitNodeWidth()
        {
            return 208;
        }

        public int GetCombineNodeWidth()
        {
            return 208;
        }

    }
}
