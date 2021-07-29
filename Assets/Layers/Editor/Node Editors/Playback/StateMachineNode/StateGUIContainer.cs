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
namespace ABXY.Layers.Editor.Node_Editors.Playback
{
    public class StateGUIContainer
    {
        public static SoundgraphCombinedStyle style = new SoundgraphCombinedStyle();

        public SerializedPropertyTree stateName { get { return stateProp.FindPropertyRelative("stateName"); } }

        public ReorderableList transitionsList { get; private set; }
        public SerializedPropertyTree guid { get { return stateProp.FindPropertyRelative("guid"); } }

        public SerializedPropertyTree subGraph { get { return stateProp.FindPropertyRelative("subGraph"); } }

        public SerializedPropertyTree startGraphEventID { get { return stateProp.FindPropertyRelative("startGraphEventID"); } }

        public SerializedPropertyTree volumeVariableID { get { return stateProp.FindPropertyRelative("volumeVariableID"); } }

        public SerializedPropertyTree expanded { get { return stateProp.FindPropertyRelative("expanded"); } }

        public SerializedObjectTree stateMachineNode { get; private set; }

        private SerializedPropertyTree stateProp;


        public SerializedPropertyTree volume { get { return stateProp.FindPropertyRelative("volume"); } }


        private Dictionary<string, bool> portVisibility = new Dictionary<string, bool>();

        public Node target { get; private set; }

        private StateMachineNodeEditor parentEditor;

        private State stateObject;

        public StateGUIContainer(SerializedPropertyTree state, StateMachineNodeEditor parentEditor)
        {
            this.parentEditor = parentEditor;
            transitionsList = new ReorderableList(state.FindPropertyRelative("transitions"));
            transitionsList.drawElementCallback += OnDrawTransition;
            transitionsList.getElementHeightCallback += GetTransitionHeight;
            transitionsList.onAddCallback += OnAddTransition;
            transitionsList.onRemoveCallback += OnRemoveTransition;
            transitionsList.onReorderCallback += OnReorder;
            transitionsList.expandable = false;

            this.stateMachineNode = state.serializedObject;
            this.stateProp = state;

            this.target = state.serializedObject.targetObject as Node;

            stateObject = (State)SerializedPropertyUtils.GetPropertyObject(state);


            for (int index = 0; index < transitionsList.List.arraySize; index++)
            {
                GetTransitionContainer(transitionsList.List.GetArrayElementAtIndex(index), stateProp);
            }


        }

        private void OnRemoveTransition(ReorderableList list)
        {
            foreach (int selection in list.Selected)
            {
                SerializedPropertyTree stateProp = list.List.GetArrayElementAtIndex(selection);
                _transitions.Remove(stateProp.FindPropertyRelative("guid").stringValue);
            }
            list.Remove(list.Selected);
        }

        public void ResetValues()
        {
            this.startGraphEventID.stringValue = "";
            this.stateName.stringValue = "New State " + Random.Range(0,9999);
            this.volumeVariableID.stringValue = "";
            this.volume.floatValue = 0f;
            this.subGraph.objectReferenceValue = null;
            this.transitionsList.List.ClearArray();
            this._transitions.Clear();
        }


        public void DrawStateElement(Rect rect, bool selected, Node target)
        {

            PreCalculateVisibility();
            EditorApplication.delayCall += () => { portVisibility.Clear(); };

            Color highlightColor = style.nodeHighlightBackground;
            Color normalbg = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0f);
            Color backgroundColor = normalbg;
            if (this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Fade)
                backgroundColor = Color.Lerp(normalbg, highlightColor, this.volume.floatValue);
            else if (GetCurrentStateGUID() == this.guid.stringValue)
                backgroundColor = highlightColor;


            EditorGUI.DrawRect(rect, backgroundColor);

            float yPosition = rect.y;


            Color headerColor = 0.8f * (Color)(selected ? new Color32(89, 137, 207, 255) : style.nodeBackgroundColor);
            Rect expandHeader = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing);

            Vector2 portPosition = new Vector2(expandHeader.x - 45, expandHeader.y);
            LayersGUIUtilities.DrawOrCreatePort(portPosition, target, Runtime.ThirdParty.XNode.Scripts.NodePort.IO.Input, typeof(LayersEvent), Runtime.ThirdParty.XNode.Scripts.Node.ConnectionType.Multiple,
                 Runtime.ThirdParty.XNode.Scripts.Node.TypeConstraint.Strict, this.guid.stringValue);

            this.expanded.boolValue = LayersGUIUtilities.ExpandHeader(expandHeader, this.stateName.stringValue, this.expanded.boolValue, headerColor);


            yPosition += 3f * EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            if (this.expanded.boolValue)
            {


                //Drawing state name field
                EditorGUI.BeginChangeCheck();
                Rect stateNameRect = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(stateNameRect, this.stateName, new GUIContent("State Name", "The name of this state (used for identification purposes only)"));
                if (EditorGUI.EndChangeCheck())
                    parentEditor.MarkNeedsCodeRegen();

                yPosition += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;

                // Drawing the sound graph selector for this state
                Rect subgraphRect = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(subgraphRect, this.subGraph, new GUIContent("State Graph", "The SoundGraph this state runs"));

                yPosition += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;

                // Drawing the selector for the start event
                Rect eventSelectorRect = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                LayersGUIUtilities.DrawEventSelector(eventSelectorRect, new GUIContent("Start Event", "The event called on this state's SubGraph when this graph is started"), this.startGraphEventID, (SoundGraph)this.subGraph.objectReferenceValue, false);


                yPosition += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;

                // Drawing the volume variable selector for fade type state machines
                if (this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Fade)
                {

                    Rect variableSelectorRect = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    LayersGUIUtilities.DrawVariableSelector(variableSelectorRect, new GUIContent( "Volume Variable", "The variable controlling the volume in this state's SoundGraph"), this.volumeVariableID, (SoundGraph)this.subGraph.objectReferenceValue, (variable) =>
                    {
                        return variable.expose == GraphVariableBase.ExposureTypes.AsInput;
                    });

                    yPosition += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;

                }


            }

            

            if (this.subGraph.objectReferenceValue != null && this.stateObject.subGraphRuntime != null)
            {


                // draw state events
                List<GraphEvent> gevents = this.stateObject.subGraphRuntime.GetAllEvents()
                    .Where(x => GetVisibility(x)).ToList();

                if (gevents.Count != 0)
                {
                    Rect eventsTitleRect = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(eventsTitleRect, this.stateName.stringValue + ": Events", EditorStyles.boldLabel);
                    yPosition += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                foreach (GraphEvent gevent in gevents)
                {
                    if (!gevent.expose)
                        continue;
                    Rect port = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    LayersGUIUtilities.DrawOrCreatePort(port, target, gevent.eventName, -29, typeof(LayersEvent), Node.ConnectionType.Override, Node.TypeConstraint.Strict, this.guid.stringValue + gevent.eventID + "In",
                        7f, typeof(LayersEvent), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict, this.guid.stringValue + gevent.eventID + "Out");
                    yPosition += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                }


                // Draw state variables
                List<GraphVariable> variables = this.stateObject.subGraphRuntime.GetAllVariables()
                    .Where(x => GetVisibility(x)).ToList();

                //header
                if (variables.Count != 0)
                {
                    Rect variablesTitleRect = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(variablesTitleRect, this.stateName.stringValue + ": Variables", EditorStyles.boldLabel);


                    Rect dropdownRect = new Rect(variablesTitleRect.x + variablesTitleRect.width - EditorGUIUtility.singleLineHeight, variablesTitleRect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                    LayersGUIUtilities.DrawThreeDotDropdown(dropdownRect, "", new string[] { "Reset all variables" }, true, (selection) =>
                    {
                        foreach (GraphVariable variable in this.stateObject.subGraphRuntime.GetAllVariables())
                            variable.ResetToDefaultValue();
                    });


                    yPosition += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                foreach (GraphVariable variable in variables)
                {
                    if (variable.expose == GraphVariableBase.ExposureTypes.DoNotExpose)
                        continue;

                    Rect port = new Rect(rect.x, yPosition, rect.width, EditorGUIUtility.singleLineHeight);
                    string variableNameAppend = variable.expose == GraphVariableBase.ExposureTypes.AsInput ? "In" : "Out";
                    string portID = this.guid.stringValue + variable.variableID + variableNameAppend;
                    object value = variable.expose == GraphVariableBase.ExposureTypes.AsInput ? target.GetInputValue(portID, variable.Value()) : variable.Value();
                    LayersGUIUtilities.DrawOrCreatePort(port, variable.expose == GraphVariableBase.ExposureTypes.AsInput ? -29 : 7, target,
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

            if (this.expanded.boolValue)
            {
                // drawing the transitions
                float listHeight = this.transitionsList.GetHeight();
                Rect listRect = new Rect(rect.x, yPosition, rect.width, listHeight);
                this.transitionsList.DoList(listRect, new GUIContent("Transitions", "The ways this state can transition to other states"));
                yPosition += listHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                //yPosition += (EditorGUIUtility.singleLineHeight + 3f * EditorGUIUtility.standardVerticalSpacing);
                foreach (TransitionGUIContainer transition in this.transitions)
                {
                    yPosition = transition.DrawMinimizedEventAndVariableView(rect, yPosition);
                }
            }

        }

        public float GetStateHeight()
        {
            PreCalculateVisibility();

            bool isFade = this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Fade;
            float height = 0;

            if (this.expanded.boolValue)
            {
                int numLines = isFade ? 4 : 3;
                height = numLines * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                height += this.transitionsList.GetHeight();
            }
            else
            {
                foreach (TransitionGUIContainer transition in this.transitions)
                {
                    height = transition.CalculateMinimizedEventAndVariableViewHeight(height);
                }
            }

            if (this.subGraph.objectReferenceValue != null)
            {
                List<GraphEvent> gevents = (this.subGraph.objectReferenceValue as SoundGraph).GetAllEvents()
                    .Where(x => GetVisibility(x)).ToList();

                List<GraphVariable> variables = (this.subGraph.objectReferenceValue as SoundGraph).GetAllVariables()
                    .Where(x => GetVisibility(x)).ToList();

                height += gevents.Count == 0 ? 0 : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                height += variables.Count == 0 ? 0 : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                height += gevents.Count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                height += variables.Count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }

            height += (EditorGUIUtility.singleLineHeight + 3f * EditorGUIUtility.standardVerticalSpacing); // header
            return height;
        }

        private string GetCurrentStateGUID()
        {
            return (string)typeof(StateMachineNode).GetMethod("GetCurrentState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(target, null);
        }

        private bool GetVisibility(GraphEvent gevent)
        {
            string inPortID = GetInPortID(gevent);
            bool visible = false;
            portVisibility.TryGetValue(inPortID, out visible);
            return visible;
        }

        private bool GetVisibility(GraphVariable variable)
        {
            string portID = GetPortID(variable);
            bool visible = false;
            portVisibility.TryGetValue(portID, out visible);
            return visible;
        }

        private void PreCalculateVisibility()
        {
            if (portVisibility.Count != 0)
                return;

            if (this.subGraph.objectReferenceValue == null)
                return;

            List<GraphVariable> variables = (this.subGraph.objectReferenceValue as SoundGraph).GetAllVariables().Where(x => x.expose != GraphVariableBase.ExposureTypes.DoNotExpose).ToList();
            List<GraphEvent> gevents = (this.subGraph.objectReferenceValue as SoundGraph).GetAllEvents().Where(x => x.expose).ToList();

            foreach (GraphEvent gevent in gevents)
            {
                string inPortID = GetInPortID(gevent);
                if (!portVisibility.ContainsKey(inPortID))
                    portVisibility.Add(inPortID, PreCalculateVisibility(gevent));
            }

            foreach (GraphVariable variable in variables)
            {
                string portID = GetPortID(variable);
                if (!portVisibility.ContainsKey(portID))
                    portVisibility.Add(portID, PreCalculateVisibility(variable));
            }
        }

        private bool PreCalculateVisibility(GraphVariable variable)
        {
            string portID = GetPortID(variable);

            bool shouldShowPort = target.GetPort(portID) == null || target.GetPort(portID).IsConnected;

            if (this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Fade
                && this.volumeVariableID.stringValue == variable.variableID)
                return false;

            return this.expanded.boolValue || shouldShowPort;
        }

        private bool PreCalculateVisibility(GraphEvent gevent)
        {

            if (gevent.eventID == this.startGraphEventID.stringValue)
                return false;

            if (!gevent.expose)
                return false;

            foreach (TransitionGUIContainer transition in this.transitions)
            {
                if (transition.delayEventID.stringValue == gevent.eventID)
                    return false;

            }
            string inPortID = GetInPortID( gevent);
            string outPortID = GetOutPortID( gevent);
            bool shouldShowInPort = target.GetPort(inPortID) == null || target.GetPort(inPortID).IsConnected;
            bool shouldShowOutPort = target.GetPort(outPortID) == null || target.GetPort(outPortID).IsConnected;


            return expanded.boolValue || shouldShowInPort || shouldShowOutPort;
        }

        private string GetPortID( GraphVariable variable)
        {
            string variableNameAppend = variable.expose == GraphVariableBase.ExposureTypes.AsInput ? "In" : "Out";
            return this.guid.stringValue + variable.variableID + variableNameAppend;
        }

        private string GetInPortID(GraphEvent gevent)
        {
            return this.guid.stringValue + gevent.eventID + "In";
        }

        private string GetOutPortID(GraphEvent gevent)
        {
            return this.guid.stringValue + gevent.eventID + "Out";
        }


        public List<StateMachineNodeEditor.ExpectedPortInfo> GetPortIDSInUse()
        {
            List<StateMachineNodeEditor.ExpectedPortInfo> portIDSInUse = new List<StateMachineNodeEditor.ExpectedPortInfo>();

            portIDSInUse.Add(new StateMachineNodeEditor.ExpectedPortInfo(NodePort.IO.Input, this.guid.stringValue, typeof(LayersEvent).FullName));

            if (this.subGraph.objectReferenceValue != null)
            {
                List<GraphVariable> variables = (this.subGraph.objectReferenceValue as SoundGraph).GetAllVariables().Where(x => x.expose != GraphVariableBase.ExposureTypes.DoNotExpose).ToList();
                List<GraphEvent> gevents = (this.subGraph.objectReferenceValue as SoundGraph).GetAllEvents().Where(x => x.expose).ToList();

                foreach (GraphEvent gevent in gevents)
                {
                    bool inUse = true;
                    foreach (TransitionGUIContainer transition in this.transitions)
                    {
                        if (transition.delayEventID.stringValue == gevent.eventID)
                            inUse = false;

                    }

                    if(inUse){
                        string inPortID = GetInPortID(gevent);
                        string outPortID = GetOutPortID(gevent);
                        portIDSInUse.Add(new StateMachineNodeEditor.ExpectedPortInfo(NodePort.IO.Input, inPortID, typeof(LayersEvent).FullName));
                        portIDSInUse.Add(new StateMachineNodeEditor.ExpectedPortInfo(NodePort.IO.Output, outPortID, typeof(LayersEvent).FullName));
                    }
                }

                foreach (GraphVariable variable in variables)
                {
                    bool shouldntShow = this.stateMachineNode.FindProperty("stateMachineStyle").enumValueIndex == (int)StateMachineNode.StateMachineStyle.Fade
                && this.volumeVariableID.stringValue == variable.variableID;

                    if (!shouldntShow)
                    {
                        string portID = GetPortID(variable);
                        portIDSInUse.Add(new StateMachineNodeEditor.ExpectedPortInfo(variable.expose == GraphVariableBase.ExposureTypes.AsInput ? NodePort.IO.Input : NodePort.IO.Output, portID, variable.typeName));

                    }
                }
            }

            foreach (TransitionGUIContainer transition in this.transitions)
            {
                portIDSInUse.AddRange(transition.GetPortIDSInUse());
            }

            return portIDSInUse;
        }


        private void OnReorder(ReorderableList list)
        {
            _transitions.Clear();
        }

        private void OnAddTransition(ReorderableList list)
        {
            SerializedPropertyTree newItem = list.AddItem();
            list.List.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            string newGUIDString = System.Guid.NewGuid().ToString();
            newItem.FindPropertyRelative("guid").stringValue = newGUIDString;
            list.List.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            TransitionGUIContainer transitionGUIContainer = GetTransitionContainer(newItem, stateProp);


            list.List.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            transitionGUIContainer.ResetAState();

            list.List.serializedObject.ApplyModifiedProperties();
            
        }

        private float GetTransitionHeight(SerializedPropertyTree element)
        {
            TransitionGUIContainer guiContainer = GetTransitionContainer(element, stateProp);

            return guiContainer.GetTransitionHeight();
        }

        private void OnDrawTransition(Rect rect, SerializedPropertyTree element, GUIContent label, bool selected, bool focused)
        {
            

            TransitionGUIContainer transitionContainer = GetTransitionContainer(element, stateProp);

            transitionContainer?.DrawTransition(rect, selected);

        }

        

        private Dictionary<string, TransitionGUIContainer> _transitions = new Dictionary<string, TransitionGUIContainer>();

        public List<TransitionGUIContainer> transitions { get { return _transitions.Values.ToList(); } }

        private TransitionGUIContainer GetTransitionContainer(SerializedPropertyTree transition, SerializedPropertyTree stateProp)
        {
            SerializedPropertyTree guid = transition.FindPropertyRelative("guid");
            if (!_transitions.ContainsKey(guid.stringValue))
                _transitions.Add(guid.stringValue, new TransitionGUIContainer(transition, stateProp,this));
            return _transitions[guid.stringValue];
        }
    }

}