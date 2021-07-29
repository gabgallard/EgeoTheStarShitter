using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Runtime.Nodes;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(MIDINoteRangeFilterNode))]
    public class MidiNoteRangeFilterNodeEditor : FlowNodeEditor
    {
        NodePort inputPort;
        SerializedProperty startNoteProp;
        SerializedProperty endNoteProp;
        SerializedProperty midiDataString;

        public override void OnCreate()
        {
            base.OnCreate();
            inputPort = target.GetInputPort("Input");
            startNoteProp = serializedObject.FindProperty("startNote");
            endNoteProp = serializedObject.FindProperty("endNote");
            midiDataString = serializedObject.FindProperty("midiDataSelector");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            NodeEditorGUILayout.PortField(inputPort);

            LayersGUIUtilities.IncomingParameterSelector(target as FlowNode, midiDataString, "Input", typeof(MidiData));

            SerializedProperty startProp = startNoteProp;
            SerializedProperty endProp = endNoteProp;

            Rect startControlRect = EditorGUILayout.GetControlRect();
            Rect startNumberRect = new Rect(startControlRect.x, startControlRect.y, startControlRect.width - 35, startControlRect.height);
            startProp.intValue = EditorGUI.IntField(startNumberRect, new GUIContent("Start Note"), startProp.intValue);

            EditorGUI.BeginDisabledGroup(true);
            Rect startNoteNameRect = new Rect(startNumberRect.x + startNumberRect.width, startNumberRect.y, startControlRect.width - startNumberRect.width, startControlRect.height);
            EditorGUI.TextField(startNoteNameRect, MidiUtils.NoteNumberToName(startProp.intValue));
            EditorGUI.EndDisabledGroup();

            Rect endControlRect = EditorGUILayout.GetControlRect();
            Rect endNumberRect = new Rect(endControlRect.x, endControlRect.y, endControlRect.width - 35, endControlRect.height);
            endProp.intValue = EditorGUI.IntField(endNumberRect, new GUIContent("End Note"), endProp.intValue);
            EditorGUI.BeginDisabledGroup(true);

            Rect endNoteNameRect = new Rect(endNumberRect.x + endNumberRect.width, endNumberRect.y, endControlRect.width - endNumberRect.width, startControlRect.height);
            EditorGUI.TextField(endNoteNameRect, MidiUtils.NoteNumberToName(endProp.intValue));
            EditorGUI.EndDisabledGroup();


            serializedObject.ApplyModifiedProperties();

            for (int index = 0; index < 144; index++)
            {
                string portName = MidiUtils.NoteNumberToName(index);
                if (index >= startProp.intValue && index <= endProp.intValue)
                {
                    if (target.GetOutputPort(portName) == null)
                        target.AddDynamicOutput(typeof(LayersEvent), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, portName);
                    NodeEditorGUILayout.PortField(target.GetOutputPort(portName));
                }
                else
                {
                    if (target.GetOutputPort(portName) != null)
                        target.RemoveDynamicPort(portName);
                }
            }

        }

        public override int GetWidth()
        {
            return base.GetWidth();
        }
    }
}
