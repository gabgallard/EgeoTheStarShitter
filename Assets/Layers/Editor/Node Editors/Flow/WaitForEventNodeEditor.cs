using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes.Flow;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Flow
{
    [NodeEditor.CustomNodeEditor(typeof(WaitForEvent))]
    public class WaitForEventNodeEditor : FlowNodeEditor
    {
        NodePort startPort;
        NodePort endPort;
        NodePort resetPort;

        public override void OnCreate()
        {
            base.OnCreate();
            startPort = target.GetInputPort("start");
            endPort = target.GetOutputPort("end");
            resetPort = target.GetInputPort("Reset");
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedPropertyTree eventID = serializedObject.FindProperty("eventID");
            NodeEditorGUIDraw.PortPair(layout.DrawLine(), startPort, endPort);
            NodeEditorGUIDraw.PortField(layout.DrawLine(), resetPort);


            List<GraphEvent> events = ((target as WaitForEvent).graph as SoundGraph).GetAllEvents();
            List<string> eventNames = events.Select(x=>x.eventName).ToList();
            List<string> eventIDs = events.Select(x => x.eventID).ToList();

            if (eventNames.Count == 0)
            {
                EditorGUI.HelpBox(layout.DrawLine(), "No events have been created. Add a Graph Inputs node to make one", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }


            int selectionIndex = eventIDs.IndexOf(eventID.stringValue);
            if (selectionIndex < 0)
            {
                selectionIndex = 0;
                eventID.stringValue = eventNames[selectionIndex];
            }

            LayersGUIUtilities.DrawDropdown(layout.DrawLine(), selectionIndex, eventNames.ToArray(), false, (newValue) => {
                eventID.stringValue = eventIDs[newValue];
                eventID.serializedObject.ApplyModifiedProperties();
            });

        


            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 140;
        }
    }
}
