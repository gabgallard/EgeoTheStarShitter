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
    public class Vector3VariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector, SplittableInspector, CombinableInspector
    {
        public override Type handlesType => typeof(Vector3);

        public object GetDefaultValue()
        {
            return Vector3.zero;
        }

        //Default value input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = EditorGUI.Vector3Field(position, label, (Vector3)edit.objectValue);
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
            edit.objectValue = EditorGUI.Vector3Field(position, label, (Vector3)edit.objectValue);
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
            ports.Add(new PortDefinition("vector3", typeof(Vector3), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("x", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("y", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("z", typeof(float), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            return ports;
        }

        public List<PortDefinition> GetCombinePorts(CombineSplitData data)
        {
            List<PortDefinition> ports = new List<PortDefinition>();
            ports.Add(new PortDefinition("x", typeof(float), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("y", typeof(float), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("z", typeof(float), NodePort.IO.Input, Node.ConnectionType.Override, Node.TypeConstraint.Inherited));
            ports.Add(new PortDefinition("Output", typeof(Vector3), NodePort.IO.Output, Node.ConnectionType.Multiple, Node.TypeConstraint.Inherited));
            return ports;
        }


        public void DrawSplitGUI(CombineSplitData data)
        {
            LayersGUIUtilities.DrawExpandableProperty(data.GetInputPort("vector3"), data.GetOutputPort("x"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("y"), data.nodeSerializedObject);
            LayersGUIUtilities.DrawExpandableProperty(data.GetOutputPort("z"), data.nodeSerializedObject);
        }

        public void DrawCombineGUI(CombineSplitData data)
        {        
            DrawPortWithDefaults(data.GetInputPort("x"), (defaultObj) => {
                return EditorGUILayout.FloatField("x", (float)defaultObj);
            });


            DrawPortWithDefaults(data.GetInputPort("y"), (defaultObj) => {
                return EditorGUILayout.FloatField("y", (float)defaultObj);
            });

            DrawPortWithDefaults(data.GetInputPort("z"), (defaultObj)=>{
                return EditorGUILayout.FloatField("z", (float)defaultObj);
            });

            NodeEditorGUILayout.PortField(data.GetOutputPort("Output"));
        }

        public override string GetPrettyTypeName()
        {
            return "Vector 3";
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
