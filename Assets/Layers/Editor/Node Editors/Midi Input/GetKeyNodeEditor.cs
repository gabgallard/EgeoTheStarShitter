using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.ThirdParty.MidiJack;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Midi_Input;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Midi_Input
{
    [NodeEditor.CustomNodeEditor(typeof(GetKeyNode))]
    public class GetKeyNodeEditor : FlowNodeEditor
    {


        private NodePort onTriggerPort;
        //private NodePort velocityPort;

        private bool listeningForNextKey = false;
        private bool wasListeningForNextKey = false;

        public override void OnCreate()
        {
            base.OnCreate();
            onTriggerPort = target.GetOutputPort("OnTrigger");
            //velocityPort = target.GetOutputPort("velocity");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedPropertyTree channelProp = serializedObject.FindProperty("channel");
            SerializedPropertyTree triggerTypeProp = serializedObject.FindProperty("triggerType");
            SerializedPropertyTree noteNumberProp = serializedObject.FindProperty("noteNumber");


            LayersGUIUtilities.DrawExpandableProperty(layout, onTriggerPort,serializedObject);
            //LayersGUIUtilities.DrawExpandableProperty(velocityPort,serializedObject);

            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => { 
                LayersGUIUtilities.DrawDropdown(layout.DrawLine(), new GUIContent("Channel"), channelProp);
            });

            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => { 
                LayersGUIUtilities.DrawDropdown(layout.DrawLine(), new GUIContent("Trigger Type"), triggerTypeProp);
            });

            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => {
                /*string noteLetter = MidiUtils.NoteNumberToName(noteNumberProp.intValue);
                int noteNumber = MidiUtils.NameToNoteNumber(noteLetter);

                Rect controlRect = EditorGUILayout.GetControlRect();
                Rect numberRect = new Rect(controlRect.x, controlRect.y, controlRect.width - 35, controlRect.height);
                noteNumberProp.intValue = EditorGUI.IntField(numberRect, new GUIContent("Note"), noteNumberProp.intValue);

                EditorGUI.BeginDisabledGroup(true);
                Rect noteRect = new Rect(controlRect.x + controlRect.width - 35, controlRect.y, 35, controlRect.height);
                EditorGUI.TextField(noteRect, noteLetter);
                EditorGUI.EndDisabledGroup();*/
                LayersGUIUtilities.DrawNote(layout.DrawLine(), noteNumberProp);
            });

            bool drew = false;
            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => {
                listeningForNextKey = GUI.Toggle(layout.DrawLine(), listeningForNextKey, "Listen for key", "Button");
                drew = true;
            });
            if (!drew)
                listeningForNextKey = false;

            if (listeningForNextKey)
            {
                int increment = MidiDriver.Instance.TotalMessageCount;//Just forcing an update
            }

            if (wasListeningForNextKey != listeningForNextKey && listeningForNextKey)
            {
                MidiMaster.noteOnDelegate += OnKeyPressed;
            }
            else if (wasListeningForNextKey != listeningForNextKey && !listeningForNextKey)
            {
                MidiMaster.noteOnDelegate -= OnKeyPressed;
            }

            wasListeningForNextKey = listeningForNextKey;

            serializedObject.ApplyModifiedProperties();
        }


        private void OnKeyPressed(MidiChannel channel, int noteNumber, float velocity)
        {
            SerializedPropertyTree channelProp = serializedObject.FindProperty("channel");
            SerializedPropertyTree noteNumberProp = serializedObject.FindProperty("noteNumber");
            MidiMaster.noteOnDelegate -= OnKeyPressed;
            listeningForNextKey = false;
            wasListeningForNextKey = false;
            channelProp.enumValueIndex = (int)channel;
            noteNumberProp.intValue = noteNumber;
            serializedObject.ApplyModifiedProperties();
        }

        protected override bool CanExpand()
        {
            return true;
        }
    }
}
