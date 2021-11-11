using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors
{
    [CustomNodeEditor(typeof(Magnitude))]
    public class MagnitudeNodeEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            NodeEditorGUIDraw.PortPair(layout.DrawLine(), target.GetInputPort("input"), 
                target.GetOutputPort("output"), serializedObjectTree);


            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 110;
        }
    }
}