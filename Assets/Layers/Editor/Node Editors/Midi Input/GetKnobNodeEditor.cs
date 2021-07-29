using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.ThirdParty.MidiJack;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Midi_Input;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Midi_Input
{
    [NodeEditor.CustomNodeEditor(typeof(GetKnobNode))]
    public class GetKnobNodeEditor : FlowNodeEditor
    {


        NodePort onChangePort;

        NodePort knobValuePort;

        private bool listeningForNextKey = false;
        private bool wasListeningForNextKey = false;

        public override void OnCreate()
        {
            base.OnCreate();
            onChangePort = target.GetOutputPort("onChange");
            knobValuePort = target.GetOutputPort("knobValue");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedPropertyTree channelProp = serializedObject.FindProperty("channel");
            SerializedPropertyTree knobNumberProp = serializedObject.FindProperty("knobNumber");
            SerializedPropertyTree startingValueProp = serializedObject.FindProperty("startingValue");

            LayersGUIUtilities.DrawExpandableProperty(layout, onChangePort,serializedObject);
            LayersGUIUtilities.DrawExpandableProperty(layout, knobValuePort,serializedObject);

            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => {
                LayersGUIUtilities.FastPropertyField(layout.DrawLine(), channelProp);
            });

            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => { 
                LayersGUIUtilities.FastPropertyField(layout.DrawLine(), knobNumberProp);
            });


            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => {
                startingValueProp.floatValue = EditorGUI.Slider(layout.DrawLine(), new GUIContent("Starting value"), startingValueProp.floatValue, 0f, 1f);
            });

            bool drew = false;
            LayersGUIUtilities.DrawExpandableProperty(serializedObject, () => {
                listeningForNextKey = GUI.Toggle(layout.DrawLine(), listeningForNextKey, "Listen for knob", "Button");
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
                MidiMaster.knobDelegate += OnKnobChanged;
            }
            else if (wasListeningForNextKey != listeningForNextKey && !listeningForNextKey)
            {
                MidiMaster.knobDelegate -= OnKnobChanged;
            }

            wasListeningForNextKey = listeningForNextKey;

            serializedObject.ApplyModifiedProperties();
        }

        protected override bool CanExpand()
        {
            return true;
        }

        private void OnKnobChanged(MidiChannel channel, int knobNumber, float velocity)
        {
            SerializedPropertyTree channelProp = serializedObject.FindProperty("channel");
            SerializedPropertyTree knobNumberProp = serializedObject.FindProperty("knobNumber");

            MidiMaster.noteOnDelegate -= OnKnobChanged;
            listeningForNextKey = false;
            wasListeningForNextKey = false;
            channelProp.enumValueIndex = (int)channel;
            knobNumberProp.intValue = knobNumber;
            serializedObject.ApplyModifiedProperties();
        }

    }
}
