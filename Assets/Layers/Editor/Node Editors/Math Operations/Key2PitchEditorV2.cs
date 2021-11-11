using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Nodes.Math_Operations;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ABXY.Layers.Editor.Node_Editors.Math_Operations
{
    [NodeEditor.CustomNodeEditor(typeof(Key2PitchV2))]
    public class Key2PitchEditorV2 : FlowNodeEditor
    {
        NodePort pitchPort;
        NodePort keyNumPort;
        SerializedPropertyTree keyNum;
        SerializedPropertyTree referenceNote;
        SerializedPropertyTree referenceNoteNumber;

        SerializedPropertyTree referencePitch;
        SerializedPropertyTree outputPitchType;
        public override void OnCreate()
        {
            base.OnCreate();
            pitchPort = target.GetOutputPort("pitch");
            keyNumPort = target.GetInputPort("keyNumber");
            keyNum = serializedObjectTree.FindProperty("keyNumber");
            referenceNote = serializedObjectTree.FindProperty("referenceNote");
            referenceNoteNumber = serializedObjectTree.FindProperty("referenceNote");
            referencePitch = serializedObjectTree.FindProperty("referencePitch");
            outputPitchType = serializedObjectTree.FindProperty("outputPitchType");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();
            LayersGUIUtilities.BeginNewLabelWidth(95f);
            Rect noteRect = layout.DrawLine();

            LayersGUIUtilities.DrawNote(noteRect, keyNum);

            EditorGUI.PropertyField(layout.DrawLine(), outputPitchType,new GUIContent("Output"));


            NodeEditorGUIDraw.AddPortToRect(noteRect, keyNumPort);




            LayersGUIUtilities.DrawNote(layout.DrawLine(), referenceNoteNumber);



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
            return 200;
        }
    }
}