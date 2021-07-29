
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors.Utilities
{
    [CustomNodeEditor(typeof(DebugLog))]
    public class DebugLogEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            NodeEditorGUIDraw.PortField(layout.DrawLine(), target.GetInputPort("print"));
            NodeEditorGUIDraw.PropertyField(layout.DrawLine(), serializedObject.FindProperty("message"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}