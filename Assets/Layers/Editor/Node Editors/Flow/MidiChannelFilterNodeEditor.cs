using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.ThirdParty.Malee.List;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEditor;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(MIDIChannelFilter))]
    public class MidiChannelFilterNodeEditor : FlowNodeEditor
    {
        ReorderableList channelFilters;
        NodePort inputPort;
        NodePort outputPort;
        //SerializedProperty filterTypeProp;
        //SerializedProperty midiEventselector;
        public override void OnCreate()
        {
            base.OnCreate();
            channelFilters = new ReorderableList(serializedObjectTree.FindProperty("conditions"));
            inputPort = target.GetInputPort("input");
            outputPort = target.GetOutputPort("output");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedPropertyTree filterTypeProp = serializedObjectTree.FindProperty("filterType");
            SerializedPropertyTree midiEventselector = serializedObjectTree.FindProperty("midiParameterSelector");
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), inputPort, outputPort, serializedObjectTree);
            LayersGUIUtilities.IncomingParameterSelector(layout.DrawLine(), target as FlowNode, midiEventselector, "input", typeof(MidiData));
            LayersGUIUtilities.DrawDropdown(layout.DrawLine(), filterTypeProp);
            channelFilters.DoList(layout.Draw(channelFilters.GetHeight()), new UnityEngine.GUIContent("Conditions"));


            serializedObject.ApplyModifiedProperties();
        }

    }
}
