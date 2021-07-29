using ABXY.Layers.Editor.Node_Editors;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [CustomNodeEditor(typeof(RemapNode))]
    public class RemapNodeEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), target.GetInputPort("inputValue"), target.GetOutputPort("outputValue"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("inputMin"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("inputMax"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("outputMin"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("outputMax"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}