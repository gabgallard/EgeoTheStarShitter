using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [CustomNodeEditor(typeof(RemapToCurve))]
    public class RemapToCurveEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            LayersGUIUtilities.BeginNewLabelWidth(50f);
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), target.GetInputPort("input"), target.GetOutputPort("output"), serializedObjectTree);
            LayersGUIUtilities.FastPropertyField(layout.DrawLine(), serializedObject.FindProperty("curve"));
            LayersGUIUtilities.EndNewLabelWidth();
            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 150;
        }
    }
}