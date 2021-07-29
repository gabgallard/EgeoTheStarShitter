using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors.Signal_Sources
{
    [NodeEditor.CustomNodeEditorAttribute(typeof(TriggerEvent))]
    public class TriggerEventNodeEditor : FlowNodeEditor
    {
        private bool hasErrors = false;
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObject.UpdateIfRequiredOrScript();
        
            //NodeEditorGUILayout.PortPair(target.GetInputPort("triggerEvent"), target.GetOutputPort("eventTriggered"));

            SerializedProperty selectedEventID = serializedObject.FindProperty("eventID");

            List<GraphEvent> events = ((target as TriggerEvent).graph as SoundGraph).GetAllEvents();
            List<string> eventNames = events.Select(x => x.eventName).ToList();
            List<string> eventIDs = events.Select(x => x.eventID).ToList();

            if (eventNames.Count == 0)
            {
                EditorGUI.HelpBox(layout.DrawLine(), "No events have been created. Add a Graph Inputs node to make one", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }


            int selectionIndex = eventIDs.IndexOf(selectedEventID.stringValue);
            if (selectionIndex < 0)
            {
                selectionIndex = 0;
                selectedEventID.stringValue = eventIDs[selectionIndex];
            }

            LayersGUIUtilities.DrawDropdown(layout.DrawLine(), selectionIndex, eventNames.ToArray(), false, (value) => {
                selectedEventID.stringValue = eventIDs[value];
                serializedObject.ApplyModifiedProperties();
            });

            //int newSelection = EditorGUILayout.Popup(new GUIContent(""), selectionIndex, eventNames.ToArray());
        

            Rect lastRect = layout.LastRect();
            //NodeEditorGUILayout.PortField(new Vector2()
            NodeEditorGUIDraw.AddPortToRect(lastRect,target.GetInputPort("triggerEvent"));
            NodeEditorGUIDraw.AddPortToRect(lastRect, target.GetOutputPort("eventTriggered"));

            hasErrors = false;

            serializedObject.ApplyModifiedProperties();

            List<GraphEvent.EventParameterDef> incomingParameters = (target as FlowNode).GetIncomingEventParameterDefsOnPort("triggerEvent", new List<Node>());
            GraphEvent graphEvent = ((target as TriggerEvent).graph as SoundGraph).GetEventByID(eventIDs[selectionIndex]);
            if (graphEvent == null)
                return;

            foreach(GraphEvent.EventParameterDef parameter in graphEvent.parameters)
            {
                if (incomingParameters.FindAll(x => x.parameterName == parameter.parameterName && x.parameterTypeName == parameter.parameterTypeName).Count == 0)
                {
                    hasErrors = true;
                    EditorGUILayout.HelpBox(string.Format("Parameter {0} of type {1} does not exist in incoming flow", parameter.parameterName, parameter.parameterTypeName), MessageType.Error);
                }
            }
        }

        public override int GetWidth()
        {
            GraphEvent graphEvent = ((target as TriggerEvent).graph as SoundGraph).GetEvent(serializedObject.FindProperty("eventID").stringValue);
            if ((graphEvent == null || graphEvent.parameters.Count == 0) && hasErrors == false)
                return 120;
            return 240;
        }
    }
}
