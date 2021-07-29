using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using UnityEditor;

namespace ABXY.Layers.Editor.Node_Editors.Signal_Sources
{
    [NodeEditor.CustomNodeEditorAttribute(typeof(EventNode))]
    public class EventNodeEditor : FlowNodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty selectedEventID = serializedObject.FindProperty("eventID");

            List<GraphEvent> events = ((target as EventNode).graph as SoundGraph).GetAllEvents();
            List<string> eventNames = events.Select(x => x.eventName).ToList();
            List<string> eventID = events.Select(x => x.eventID).ToList();

            if (eventNames.Count == 0)
            {
                EditorGUI.HelpBox(layout.DrawLines(2), "No events have been created. Add a Graph Inputs node to make one", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }


            int selectionIndex = eventID.IndexOf(selectedEventID.stringValue);
            if (selectionIndex < 0)
            {
                selectionIndex = 0;
                selectedEventID.stringValue = eventID[selectionIndex];
            }

            LayersGUIUtilities.DrawDropdown(layout.DrawLine(), selectionIndex, eventNames.ToArray(), false, (value) => {
                selectedEventID.stringValue = eventID[value];
                serializedObject.ApplyModifiedProperties();
            });

            //int newSelection = EditorGUILayout.Popup(selectionIndex, eventNames.ToArray());
            //selectedEventName.stringValue = eventNames[newSelection];

            NodeEditorGUIDraw.AddPortToRect(layout.LastRect(), target.GetOutputPort("eventCalled"));

            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 120;
        }
    }
}
