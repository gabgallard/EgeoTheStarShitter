using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(Key2Pitch))]
    public class Key2PitchEditor : FlowNodeEditor
    {
        NodePort pitchPort;
        NodePort keyNumPort;
        SerializedPropertyTree keyNum;
        SerializedPropertyTree referenceNote;
        SerializedPropertyTree referencePitch;
        SerializedPropertyTree outputPitchType;
        public override void OnCreate()
        {
            base.OnCreate();
            pitchPort = target.GetOutputPort("pitch");
            keyNumPort = target.GetInputPort("keyNumber");
            keyNum = serializedObjectTree.FindProperty("keyNumber");
            referenceNote = serializedObjectTree.FindProperty("referenceNote");
            referencePitch = serializedObjectTree.FindProperty("referencePitch");
            outputPitchType = serializedObjectTree.FindProperty("outputPitchType");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();
            Rect noteRect = layout.DrawLine();
            
            LayersGUIUtilities.BeginNewLabelWidth(35f);
            LayersGUIUtilities.DrawNote(noteRect, keyNum);
            LayersGUIUtilities.EndNewLabelWidth();

            LayersGUIUtilities.BeginNewLabelWidth(45f);
            EditorGUI.PropertyField(layout.DrawLine(), outputPitchType,new GUIContent("Output"));
            LayersGUIUtilities.EndNewLabelWidth();

            LayersGUIUtilities.BeginNewLabelWidth(95f);
            NodeEditorGUIDraw.AddPortToRect(noteRect, keyNumPort);
            LayersGUIUtilities.DrawDropdown(layout.DrawLine(), new GUIContent("Reference Note"), referenceNote);

            if (outputPitchType.enumValueIndex == (int)Key2Pitch.PitchTypes.HZ)
            {
                NodeEditorGUIDraw.PropertyField(layout.DrawLine(), referencePitch);
            }
            else
            {
                var referencePitchPort = target.GetInputPort("referencePitch");
                if (referencePitchPort.IsConnected)
                    referencePitchPort.ClearConnections();
            }

            NodeEditorGUIDraw.PortField(layout.DrawLine(), pitchPort, serializedObjectTree);
            LayersGUIUtilities.EndNewLabelWidth();


            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 160;
        }
    }
}