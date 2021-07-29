using ABXY.Layers.Editor.Node_Editor_Window;
using ABXY.Layers.Editor.Node_Editors;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes.Playback;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.ThirdParty.Malee.List;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using ABXY.Layers.Editor;
namespace ABXY.Layers.Editor.Node_Editors.Playback
{

    public class TransitionGUIContainer
    {
        public SerializedPropertyTree guid { get { return transitionProperty.FindPropertyRelative("guid"); } }
        public SerializedPropertyTree targetStateGUID { get { return transitionProperty.FindPropertyRelative("targetStateGUID"); } }
        public SerializedPropertyTree fadeTime { get { return transitionProperty.FindPropertyRelative("fadeTime"); } }
        public SerializedPropertyTree transitionGraph { get { return transitionProperty.FindPropertyRelative("transitionGraph"); } }
        public SerializedPropertyTree startEventID { get { return transitionProperty.FindPropertyRelative("startEventID"); } }
        public SerializedPropertyTree endEventID { get { return transitionProperty.FindPropertyRelative("endEventID"); } }
        public SerializedPropertyTree delayEventID { get { return transitionProperty.FindPropertyRelative("delayEventID"); } }
        public SerializedPropertyTree expanded { get { return transitionProperty.FindPropertyRelative("expanded"); } }

        public static SoundgraphCombinedStyle style = new SoundgraphCombinedStyle();

        public SerializedPropertyTree isInTransition { get { return transitionProperty.FindPropertyRelative("isInTransition"); } }
        public SerializedPropertyTree timeOfLastActivation { get { return transitionProperty.FindPropertyRelative("timeOfLastActivation"); } }

        public SerializedPropertyTree stateProperty { get; private set; }
        public SerializedPropertyTree transitionProperty { get; private set; }

        private StateGUIContainer parent;

        public SerializedObjectTree stateMachineNode { get; private set; }


        private Dictionary<string, bool> portVisibility = new Dictionary<string, bool>();

        private Transition transitionObject;

        public TransitionGUIContainer(SerializedPropertyTree transition, SerializedPropertyTree stateProperty, StateGUIContainer parent)
        {
            stateMachineNode = transition.serializedObject;
            transitionObject = (Transition)SerializedPropertyUtils.GetPropertyObject(transition);
            this.parent = parent;

            this.transitionProperty = transition;
            this.stateProperty = stateProperty;
        }

        public void ResetAState()
        {
            targetStateGUID.stringValue = "";
            fadeTime.floatValue = 2f;
            transitionGraph.objectReferenceValue = null;
            startEventID.stringValue = "";
            endEventID.stringValue = "";
            delayEventID.stringValue = "";
            isInTransition.boolValue = false;
            timeOfLastActivation.floatValue = 0f;
        }

        public void DrawTransition(Rect rect, bool selected)
        {
            PreCalculateVisibility(false);
            EditorApplication.delayCall += () =>
            {
                portVisibility.Clear();
            };

            Color highlightColor = style.nodeHighlightBackground;
            Color normalbg = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0f);

            Color backgroundColor = Color.Lerp(normalbg, highlightColor,
                this.isInTransition.boolValue ? 1 : Mathf.Clamp01(1f - ((float)AudioSettings.dspTime - (float)this.timeOfLastActivation.doubleValue)));

            EditorGUI.DrawRect(rect, backgroundColor);

            //getting names and ids of states
            List<string> names = new List<string>();
            List<string> ids = new List<string>();

            names.Add("Any State");
            ids.Add("Any State");

            SerializedPropertyTree statesList = this.stateMachineNode.FindProperty("states");
            for (int index = 0; index < statesList.arraySize; index++)
            {
                names.Add(statesList.GetArrayElementAtIndex(index).FindPropertyRelative("stateName").stringValue);
                ids.Add(statesList.GetArrayElementAtIndex(index).FindPropertyRelative("guid").stringValue);
            }


            int selectedIndex = ids.IndexOf(this.targetStateGUID.stringValue);
            if (selectedIndex == -1)
            {
                selectedIndex = 0;
                this.targetStateGUID.stringValue = ids[selectedIndex];
                this.stateMachineNode.ApplyModifiedProperties();
            }



            float yPosition = rect.y;

            Color headerColor = 0.8f * (Color)(selected ? new Color32(89, 137, 207, 255) : style.nodeBackgroundColor);
            Rect headerPosition = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing);
            this.expanded.boolValue = LayersGUIUtilities.ExpandHeader(headerPosition, this.GetPrettyName(), this.expanded.boolValue, headerColor);

            yPosition += EditorGUIUtility.singleLineHeight + 3f * EditorGUIUtility.standardVerticalSpacing;

            if (this.expanded.boolValue)
            {
                Rect targetState = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);

                LayersGUIUtilities.DrawDropdown(targetState, selectedIndex, names.ToArray(), false, (newIndex) =>
                {
                    this.targetStateGUID.stringValue = ids[newIndex];
                    this.stateMachineNode.ApplyModifiedProperties();
                }, true);

                yPosition += targetState.height + EditorGUIUtility.standardVerticalSpacing;

                if (this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Fade)
                {
                    Rect fadeTimeRect = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fadeTimeRect, this.fadeTime, new GUIContent("Fade Time (s)", "The time over which this state is faded into the next"));

                    yPosition += targetState.height + EditorGUIUtility.standardVerticalSpacing;
                }

                if (this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Sequential)
                {
                    Rect transitionGraphRect = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(transitionGraphRect, this.transitionGraph, new GUIContent("Transition Graph", "When a transition is triggered, this graph will play this SoundGraph"));

                    yPosition += targetState.height + EditorGUIUtility.standardVerticalSpacing;

                    if (this.transitionGraph.objectReferenceValue != null)
                    {

                        Rect startEventSelector = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                        LayersGUIUtilities.DrawEventSelector(startEventSelector, new GUIContent("Start Event", "The event used to trigger the transition SoundGraph"), this.startEventID,
                            (SoundGraph)this.transitionGraph.objectReferenceValue, false);

                        yPosition += targetState.height + EditorGUIUtility.standardVerticalSpacing;

                        Rect endEventSelector = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                        LayersGUIUtilities.DrawEventSelector(endEventSelector, new GUIContent("End Event", "The event used to signal that playback in the transition SoundGraph is complete." +
                            " Call it to continue transitioning to the next state"), this.endEventID,
                            (SoundGraph)this.transitionGraph.objectReferenceValue, false);
                        yPosition += targetState.height + EditorGUIUtility.standardVerticalSpacing;
                    }
                }


                Rect waitForEvent = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                LayersGUIUtilities.DrawEventSelector(waitForEvent, new GUIContent("Wait for Event", "Wait for this event to trigger before continuing with the transition"), this.delayEventID,
                            (SoundGraph)this.stateProperty.FindPropertyRelative("subGraph").objectReferenceValue, true);

                yPosition += waitForEvent.height + EditorGUIUtility.standardVerticalSpacing;
            }

            yPosition += DrawEventsAndVariables(rect, yPosition, -58, 11);
        }

        public float GetTransitionHeight()
        {
            PreCalculateVisibility(false);

            bool isFade = this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Fade;

            float height = 0f;

            if (this.expanded.boolValue)
            {
                int numLines = isFade ? 3 : 5;

                if (this.transitionGraph.objectReferenceValue == null)
                    numLines = 3;
                height = numLines * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);



            }

            if (this.transitionGraph.objectReferenceValue != null && this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Sequential)
            {
                List<GraphEvent> gevents = (this.transitionGraph.objectReferenceValue as SoundGraph).GetAllEvents()
                .Where(x => GetVisibility(x)).ToList();

                List<GraphVariable> variables = (this.transitionGraph.objectReferenceValue as SoundGraph).GetAllVariables().Where(x => GetVisibility(x)).ToList();

                height += gevents.Count == 0 ? 0 : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                height += variables.Count == 0 ? 0 : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                height += gevents.Count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                height += variables.Count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }

            height += (EditorGUIUtility.singleLineHeight + 3f * EditorGUIUtility.standardVerticalSpacing); // header
            return height;
        }


        private float DrawEventsAndVariables(Rect rect, float yPosition, float leftPortOffset, float rightPortOffset)
        {
            if (this.transitionGraph.objectReferenceValue != null && this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Sequential)
            {

                List<GraphEvent> gevents = transitionObject.transitionGraphPlaybackGraph.GetAllEvents()
                    .Where(x => GetVisibility(x)).ToList();


                string prettyName = this.GetPrettyName();

                if (gevents.Count != 0)
                {
                    Rect eventsTitleRect = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(eventsTitleRect, prettyName + ": Events", EditorStyles.boldLabel);
                    yPosition += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                foreach (GraphEvent gevent in gevents)
                {

                    Rect port = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    LayersGUIUtilities.DrawOrCreatePort(port, this.stateMachineNode.targetObject as Node
                        , gevent.eventName, leftPortOffset, typeof(LayersEvent), Node.ConnectionType.Override, Node.TypeConstraint.Strict, this.guid.stringValue + gevent.eventID + "In",
                        rightPortOffset, typeof(LayersEvent), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, this.guid.stringValue + gevent.eventID + "Out");
                    yPosition += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                }

                List<GraphVariable> variables = transitionObject.transitionGraphPlaybackGraph.GetAllVariables()
                    .Where(x => GetVisibility(x)).ToList();

                if (variables.Count != 0)
                {
                    Rect variablesTitleRect = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(variablesTitleRect, prettyName + ": Variables", EditorStyles.boldLabel);

                    Rect dropdownRect = new Rect(variablesTitleRect.x + variablesTitleRect.width - EditorGUIUtility.singleLineHeight, variablesTitleRect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                    LayersGUIUtilities.DrawThreeDotDropdown(dropdownRect, "", new string[] { "Reset all variables" }, true, (selection) =>
                    {
                        foreach (GraphVariable variable in this.transitionObject.transitionGraphPlaybackGraph.GetAllVariables())
                            variable.ResetToDefaultValue();
                    });

                    yPosition += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                foreach (GraphVariable variable in variables)
                {

                    Rect port = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    string variableNameAppend = variable.expose == GraphVariableBase.ExposureTypes.AsInput ? "In" : "Out";
                    string portID = this.guid.stringValue + variable.variableID + variableNameAppend;
                    object value = variable.expose == GraphVariableBase.ExposureTypes.AsInput ? parent.target.GetInputValue(portID, variable.Value()) : variable.Value();
                    LayersGUIUtilities.DrawOrCreatePort(port, variable.expose == GraphVariableBase.ExposureTypes.AsInput ? leftPortOffset : rightPortOffset, this.stateMachineNode.targetObject as Node,
                        variable.expose == GraphVariableBase.ExposureTypes.AsInput ? NodePort.IO.Input : NodePort.IO.Output,
                        ReflectionUtils.FindType(variable.typeName), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, portID,
                        string.Format("{0} ({1})   ", variable.name, value));


                    if (variable.expose != GraphVariableBase.ExposureTypes.AsInput)
                    {
                        Rect dropdownRect = new Rect(port.x + port.width - EditorGUIUtility.singleLineHeight, port.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                        LayersGUIUtilities.DrawThreeDotDropdown(dropdownRect, "", new string[] { "Reset" }, true, (selection) =>
                        {
                            variable.ResetToDefaultValue();
                        });
                    }
                    yPosition += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                }
            }

            return yPosition;
        }



        public string GetPrettyName()
        {
            List<string> names = new List<string>();
            List<string> ids = new List<string>();

            names.Add("Any State");
            ids.Add("Any State");

            SerializedProperty statesList = stateMachineNode.FindProperty("states");
            for (int index = 0; index < statesList.arraySize; index++)
            {
                names.Add(statesList.GetArrayElementAtIndex(index).FindPropertyRelative("stateName").stringValue);
                ids.Add(statesList.GetArrayElementAtIndex(index).FindPropertyRelative("guid").stringValue);
            }


            int selectedIndex = ids.IndexOf(targetStateGUID.stringValue);
            if (selectedIndex == -1)
            {
                selectedIndex = 0;
                targetStateGUID.stringValue = ids[selectedIndex];
                stateMachineNode.ApplyModifiedProperties();
            }

            return "Transition to " + names[selectedIndex];
        }

        public float CalculateMinimizedEventAndVariableViewHeight(float height)
        {
            PreCalculateVisibility(true);
            if (this.transitionGraph.objectReferenceValue != null && this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Sequential)
            {
                List<GraphEvent> gevents = (this.transitionGraph.objectReferenceValue as SoundGraph).GetAllEvents()
                .Where(x => GetVisibility(x)).ToList();

                List<GraphVariable> variables = (this.transitionGraph.objectReferenceValue as SoundGraph).GetAllVariables()
                .Where(x => GetVisibility(x)).ToList();

                height += gevents.Count == 0 ? 0 : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                height += variables.Count == 0 ? 0 : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                height += gevents.Count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                height += variables.Count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }
            return height;
        }

        public float DrawMinimizedEventAndVariableView(Rect rect, float yPosition)
        {
            PreCalculateVisibility(true);
            EditorApplication.delayCall += () =>
            {
                portVisibility.Clear();
            };
            return DrawEventsAndVariables(rect, yPosition, -29, 6);
        }

        private bool GetVisibility(GraphEvent gevent)
        {
            string inPortID = GetInPortID( gevent);
            bool visible = false;
            portVisibility.TryGetValue(inPortID, out visible);
            return visible;
        }

        private bool GetVisibility(GraphVariable variable)
        {
            string portID = GetPortID( variable);
            bool visible = false;
            portVisibility.TryGetValue(portID, out visible);
            return visible;
        }

        public void PreCalculateVisibility(bool onlyShowConnected)
        {
            if (portVisibility.Count() != 0)
                return;
            

            if (this.transitionGraph.objectReferenceValue == null)
                return;

            List<GraphEvent> gevents = (this.transitionGraph.objectReferenceValue as SoundGraph).GetAllEvents().Where(x => x.expose).ToList() ;

            foreach (GraphEvent gevent in gevents)
            {
                string inPortID = GetInPortID(gevent);
                if (!portVisibility.ContainsKey(inPortID))
                    portVisibility.Add(inPortID, PreCalculateVisibility(gevent, onlyShowConnected));
            }

            List<GraphVariable> variables = (this.transitionGraph.objectReferenceValue as SoundGraph).GetAllVariables().Where(x=>x.expose != GraphVariableBase.ExposureTypes.DoNotExpose).ToList();
            foreach (GraphVariable variable in variables)
            {
                string portID = GetPortID( variable);
                if (!portVisibility.ContainsKey(portID))
                    portVisibility.Add(portID, PreCalculateVisibility(variable, onlyShowConnected));
            }
            
        }

        private bool PreCalculateVisibility(GraphEvent gevent, bool onlyShowConnected)
        {
            StateMachineNode target = this.stateMachineNode.targetObject as StateMachineNode;
            if (this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Sequential)
            {
                if (gevent.eventID == this.startEventID.stringValue)
                    return false;

                if (gevent.eventID == this.endEventID.stringValue)
                    return false;
            }

            if (onlyShowConnected || !this.expanded.boolValue)
            {
                string inPortID = GetInPortID( gevent);
                string outPortID = GetOutPortID(gevent);
                bool shouldShowIn = target.GetPort(inPortID) == null || target.GetPort(inPortID).IsConnected;
                bool shouldShowOut = target.GetPort(outPortID) == null || target.GetPort(outPortID).IsConnected;
                return shouldShowIn || shouldShowOut;
            }

            return true;
        }

        private bool PreCalculateVisibility(GraphVariable variable, bool onlyShowConnected)
        {
            StateMachineNode target = this.stateMachineNode.targetObject as StateMachineNode;


            if (onlyShowConnected || !this.expanded.boolValue)
            {
                string portID = GetPortID( variable);
                bool shouldShowIn = target.GetPort(portID) == null || target.GetPort(portID).IsConnected;
                return shouldShowIn;
            }

            return true;
        }

        public List<StateMachineNodeEditor.ExpectedPortInfo> GetPortIDSInUse()
        {
            List<StateMachineNodeEditor.ExpectedPortInfo> portIDSInUse = new List<StateMachineNodeEditor.ExpectedPortInfo>();
            if (this.transitionGraph.objectReferenceValue != null && this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Sequential)
            {
                List<GraphVariable> variables = (this.transitionGraph.objectReferenceValue as SoundGraph).GetAllVariables().Where(x=>x.expose != GraphVariableBase.ExposureTypes.DoNotExpose).ToList();
                List<GraphEvent> gevents = (this.transitionGraph.objectReferenceValue as SoundGraph).GetAllEvents().Where(x=>x.expose).ToList();

                foreach (GraphEvent gevent in gevents)
                {
                    if (gevent.eventID != this.startEventID.stringValue && gevent.eventID != this.endEventID.stringValue)
                    {
                        string inPortID = GetInPortID(gevent);
                        string outPortID = GetOutPortID(gevent);
                        portIDSInUse.Add(new StateMachineNodeEditor.ExpectedPortInfo(NodePort.IO.Input, inPortID, typeof(LayersEvent).FullName));
                        portIDSInUse.Add(new StateMachineNodeEditor.ExpectedPortInfo(NodePort.IO.Output, outPortID, typeof(LayersEvent).FullName));
                    }
                }

                foreach (GraphVariable variable in variables)
                {
                    string portID = GetPortID(variable);
                    portIDSInUse.Add(new StateMachineNodeEditor.ExpectedPortInfo(variable.expose == GraphVariableBase.ExposureTypes.AsInput? NodePort.IO.Input : NodePort.IO.Output, portID, variable.typeName));
                }
            }
            return portIDSInUse;
        }

        private string GetPortID(GraphVariable variable)
        {
            return this.guid.stringValue + variable.variableID + (variable.expose == GraphVariableBase.ExposureTypes.AsInput ? "In" : "Out");
        }

        private string GetInPortID(GraphEvent gevent)
        {
            return this.guid.stringValue + gevent.eventID + "In";
        }

        private string GetOutPortID(GraphEvent gevent)
        {
            return this.guid.stringValue + gevent.eventID + "Out";
        }

    }
}
