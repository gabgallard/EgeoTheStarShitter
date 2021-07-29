using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Playback
{
    [CreateNodeMenu("Playback/State Machine (BETA)")]
    public class StateMachineNode : SubGraphBase
    {
        [SerializeField]
        private List<State> states = new List<State>();

        public State[] States { get { return states.ToArray(); } }

        public enum StateMachineStyle { Fade, Sequential }

        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("transitionStyle")]
        private StateMachineStyle stateMachineStyle = StateMachineStyle.Fade;

        public enum TransitionTypes { Event, Conditional }

        public TransitionTypes transitionType = TransitionTypes.Event;

        private string currentStateID = "";
        private float currentFadeSpeed = 0.00000000000001f;

        [SerializeField]
        private string _stateMachineName = "";
        public string stateMachineName { get { return _stateMachineName; } }

        [SerializeField]
        private string statesEnumTypeName = "";
        private System.Type cachedStatesEnumType;

        [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        private LayersEvent onStateChange;

#pragma warning disable CS0414
        [SerializeField]
        private bool created = false;
#pragma warning restore CS0414

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.PlayAtDSPTime(calledBy, time, data, nodesCalledThisFrame);

            if (calledBy.fieldName.EndsWith("In") || calledBy.fieldName.EndsWith("Out"))
                return;

            ResetCoroutines();

            if (stateMachineStyle == StateMachineStyle.Fade)
            {
                if (string.IsNullOrEmpty(currentStateID))
                {
                    foreach (State state in states)
                    {
                        if (state.subGraphRuntime != null)
                        {
                            GraphEvent gevent = state.subGraphRuntime.GetEventByID(state.startGraphEventID);
                            if (gevent != null)
                                state.subGraphRuntime.CallEventByID(gevent.eventID, time, nodesCalledThisFrame);
                        }
                    }
                }
                currentRunningProcess = StartCoroutine(ProcessFadeEvent(calledBy.fieldName, time, nodesCalledThisFrame));
            }
            else
            {
                currentRunningProcess = StartCoroutine(ProcessSequentialEvent(calledBy.fieldName, time, nodesCalledThisFrame));
            }
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            currentStateID = "";
            CallFunctionOnOutputNodes("onStateChange", time, data, nodesCalledThisFrame);

            foreach (State state in states)
            {
                if (state.subGraphRuntime != null)
                {
                    state.subGraphRuntime.CallEvent("EndAll", time, nodesCalledThisFrame);
                }

                foreach (Transition transition in state.transitions)
                {
                    transition.isInTransition = false;
                }
            }
            ResetCoroutines();
        }

        private LayersCoroutine currentRunningProcess;

        private IEnumerator ProcessFadeEvent(string targetState, double time, int nodesCalledThisFrame)
        {
            while (time > AudioSettings.dspTime)
                yield return null;


            State currentStateOBJ = states.Find(x => x.guid == currentStateID);
            Transition targetTransition = null;
            if (currentStateOBJ != null)
            {
                targetTransition = currentStateOBJ.transitions.Find(x => x.targetStateGUID == targetState);

                if (targetTransition == null)
                    targetTransition = currentStateOBJ.transitions.Find(x => x.targetStateGUID == "Any State");
            }


            if (targetTransition != null || currentStateOBJ == null)
            { // Either transition is allowed, or this is the first state in the state machine

                State targetStateOBJ = states.Find(x => x.guid == targetState);

                //waiting for event if necessary
                if (currentStateOBJ != null)
                {
                    if (targetTransition != null && currentStateOBJ.subGraphRuntime != null && !string.IsNullOrEmpty(targetTransition.delayEventID))
                    {
                        GraphEvent waitEvent = currentStateOBJ.subGraphRuntime.GetEventByID(targetTransition.delayEventID);
                        if (waitEvent != null)
                        {
                            bool ready = false;
                            double outputEventTime = AudioSettings.dspTime;
                            waitEvent.onGraphEventCalledEphemeral.AddListener((eventTime, data) => {
                                outputEventTime = eventTime;
                                ready = true;
                            });

                            while (!ready) // waiting for event to be called
                                yield return null;

                            while (outputEventTime > AudioSettings.dspTime) // Waiting for event time
                                yield return null;

                        }
                    }
                }


                if (targetStateOBJ != null)
                {
                    this.currentStateID = targetStateOBJ.guid;
                    CallFunctionOnOutputNodes("onStateChange", time, nodesCalledThisFrame);
                }

                if (targetTransition != null)
                    currentFadeSpeed = targetTransition.fadeTime;
                else
                    currentFadeSpeed = 0.00000000000001f;
            }
        }

        private IEnumerator ProcessSequentialEvent(string targetState, double time, int nodesCalledThisFrame)
        {
            while (time > AudioSettings.dspTime)
                yield return null;


            State currentStateOBJ = states.Find(x => x.guid == currentStateID);
            Transition targetTransition = null;
            if (currentStateOBJ != null)
            {
                targetTransition = currentStateOBJ.transitions.Find(x => x.targetStateGUID == targetState);

                if (targetTransition == null)
                    targetTransition = currentStateOBJ.transitions.Find(x => x.targetStateGUID == "Any State");
            }

            if (targetTransition != null || currentStateOBJ == null)
            { // Either transition is allowed, or this is the first state in the state machine

                State targetStateOBJ = states.Find(x => x.guid == targetState);

                //waiting for event if necessary
                if (currentStateOBJ != null)
                {
                    if (targetTransition != null && currentStateOBJ.subGraphRuntime != null && !string.IsNullOrEmpty(targetTransition.delayEventID))
                    {
                        GraphEvent waitEvent = currentStateOBJ.subGraphRuntime.GetEventByID(targetTransition.delayEventID);
                        if (waitEvent != null)
                        {
                            bool ready = false;
                            double outputEventTime = AudioSettings.dspTime;
                            waitEvent.onGraphEventCalledEphemeral.AddListener((eventTime, data) =>
                            {
                                outputEventTime = eventTime;
                                ready = true;
                            });

                            while (!ready) // waiting for event to be called
                                yield return null;

                            //while (outputEventTime > AudioSettings.dspTime) // Waiting for event time
                            //yield return null;

                            time = outputEventTime;

                        }
                    }
                }


                foreach (State state in states)
                {
                    if (state.subGraphRuntime != null)
                    {
                        bool isTargetState = state == targetStateOBJ;
                        bool wasTargetstate = state.guid == currentStateID;
                        if (wasTargetstate && isTargetState)
                            state.subGraphRuntime.CallEvent("EndAll", time - deltaTime, nodesCalledThisFrame);// The wiggle room is to prevent race conditions when stopping and restarting the same graph
                        else if (!isTargetState)
                            state.subGraphRuntime.CallEvent("EndAll", time, nodesCalledThisFrame);
                    }
                }

                if (targetTransition != null)
                    targetTransition.timeOfLastActivation = AudioSettings.dspTime;

                if (targetTransition != null && targetTransition.transitionGraphPlaybackGraph != null)
                {
                    GraphEvent startEvent = targetTransition.transitionGraphPlaybackGraph.GetEventByID(targetTransition.startEventID);
                    GraphEvent endEvent = targetTransition.transitionGraphPlaybackGraph.GetEventByID(targetTransition.endEventID);
                    if (startEvent != null && endEvent != null)
                    {
                        targetTransition.isInTransition = true;
                        targetTransition.transitionGraphPlaybackGraph.CallEventByID(startEvent.eventID, time, nodesCalledThisFrame);


                        bool endEventFinished = false;
                        double endEventTime = AudioSettings.dspTime;
                        endEvent.onGraphEventCalledEphemeral.AddListener((endeventfinishtime, data) => {
                            endEventFinished = true;
                            endEventTime = endeventfinishtime;
                        });

                        while (!endEventFinished)
                            yield return null;

                        time = endEventTime;

                        StartCoroutine(WaitForDSPTime(time, () => {
                            targetTransition.isInTransition = false;
                            targetTransition.timeOfLastActivation = AudioSettings.dspTime;
                        }));
                    }

                }



                if (targetStateOBJ != null && targetStateOBJ.subGraphRuntime != null)
                {
                    this.currentStateID = targetStateOBJ.guid;
                    CallFunctionOnOutputNodes("onStateChange", time, nodesCalledThisFrame);
                    targetStateOBJ.subGraphRuntime.CallEventByID(targetStateOBJ.startGraphEventID, time, nodesCalledThisFrame);
                }
            }
        }

        private void ResetCoroutines()
        {
            if (currentRunningProcess != null)
                StopCoroutine(currentRunningProcess);
        }


        float lastTime;
        float deltaTime;
        public override void NodeUpdate()
        {
            foreach (SoundGraph subgraph in GetSubGraphs())
            {
                subgraph.GraphUpdate();
            }
            deltaTime = Time.realtimeSinceStartup - lastTime;
            lastTime = Time.realtimeSinceStartup;

            if (stateMachineStyle == StateMachineStyle.Fade)
            {
                foreach (State state in states)
                {
                    if (state.subGraphRuntime != null)
                    {
                        GraphVariable variable = state.subGraphRuntime.GetGraphVariableByID(state.volumeVariableID);
                        if (variable != null && variable.typeName == typeof(float).FullName)
                        {
                            bool isSelected = state.guid == currentStateID;
                            state.volume = Mathf.Clamp01(Mathf.MoveTowards(state.volume, isSelected ? 1f : 0f, (1f / currentFadeSpeed) * deltaTime));
                            variable.SetValue(CalcLogarithmVolume(state.volume));
                        }
                    }
                }
            }
        }

        private float CalcLogarithmVolume(float volume)
        {
            return Mathf.Clamp01(Mathf.Log10(volume) + 1f);
        }

        public override void NodeAwake()
        {
            base.NodeAwake();
            foreach (SoundGraph subgraph in GetSubGraphs())
            {
                subgraph.GraphAwake();
            }
            //subGraph?.RegisterEventListener(OnEventCalledFromWithin);
        }

        public override void NodeStart()
        {
            base.NodeStart();
            foreach (SoundGraph subgraph in GetSubGraphs())
            {
                subgraph.GraphStart();
            }
            lastTime = Time.realtimeSinceStartup;
        }

        private List<SoundGraph> GetSubGraphs()
        {
            List<SoundGraph> subGraphs = new List<SoundGraph>();
            foreach (State state in states)
            {
                if (state.subGraphRuntime != null)
                    subGraphs.Add(state.subGraphRuntime);

                if (stateMachineStyle == StateMachineStyle.Sequential)
                {
                    foreach (Transition transition in state.transitions)
                    {
                        if (transition.transitionGraphPlaybackGraph != null)
                            subGraphs.Add(transition.transitionGraphPlaybackGraph);
                    }
                }
            }
            return subGraphs;
        }

        private string GetCurrentState()
        {
            return currentStateID;
        }

        public override void EndAllPlayback(double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            List<SoundGraph> soundGraphs = GetSubGraphs();
            foreach (SoundGraph graph in soundGraphs)
                graph.CallEvent("EndAll", time, nodesCalledThisFrame);
        }

        private List<string> GetStateAndTransitionIDsUsingGraph(SoundGraph soundGraph)
        {
            List<string> ids = new List<string>();
            foreach (State state in states)
            {
                if (state.subGraphRuntime != null && state.subGraphRuntime == soundGraph)
                    ids.Add(state.guid);

                foreach (Transition transition in state.transitions)
                {
                    if (stateMachineStyle == StateMachineStyle.Sequential && transition.transitionGraphPlaybackGraph != null && transition.transitionGraphPlaybackGraph == soundGraph)
                        ids.Add(transition.guid);
                }
            }
            return ids;
        }

        private SoundGraph GetSoundGraphFromStateOrTransitionID(string id)
        {
            foreach (State state in states)
            {
                if (state.guid == id)
                    return state.subGraphRuntime;

                if (stateMachineStyle == StateMachineStyle.Sequential)
                {
                    foreach (Transition transition in state.transitions)
                    {
                        if (transition.guid == id)
                            return transition.transitionGraphPlaybackGraph;
                    }
                }
            }
            return null;
        }

        public override void OnEventCalledFromWithin(SoundGraph sourceSoundGraph, string eventName, double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            GraphEvent graphEvent = sourceSoundGraph.GetEvent(eventName);

            if (graphEvent == null)
                return;


            List<string> ids = GetStateAndTransitionIDsUsingGraph(sourceSoundGraph);
            foreach (string id in ids)
            {
                string eventNameToUse = id + graphEvent.eventID + "Out";
                this.CallFunctionOnOutputNodes(eventNameToUse, time, data, nodesCalledThisFrame);
            }
        }

        public override void EventCalledFromLeft(NodePort calledBy, double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            base.EventCalledFromLeft(calledBy, time, data, nodesCalledThisFrame);


            List<SoundGraph> soundGraphs = GetSubGraphs();

            foreach (SoundGraph soundGraph in soundGraphs)
                soundGraph.subgraphNode = this;


            if (calledBy.fieldName.EndsWith("In"))
            {

                string graphID = calledBy.fieldName.Substring(0, 36);
                string eventName = calledBy.fieldName.Substring(36, calledBy.fieldName.Length - 2 - 36);

                SoundGraph targetSoundGraph = GetSoundGraphFromStateOrTransitionID(graphID);


                if (targetSoundGraph != null && targetSoundGraph.HasEventWithID(eventName))
                {
                    targetSoundGraph.CallEventByID(eventName, time, data, nodesCalledThisFrame);
                    return;
                }

            }


        }

        protected override void SetInitialVariableValues()
        {
            foreach (NodePort port in DynamicInputs)
            {
                if (port.ValueType != typeof(LayersEvent) && port.IsConnected)
                {
                    string graphID = port.fieldName.Substring(0, 36);
                    string varID = port.fieldName.Substring(36, port.fieldName.Length - 2 - 36);

                    SoundGraph soundgraph = GetSoundGraphFromStateOrTransitionID(graphID);
                    if (soundgraph != null && soundgraph.HasGraphVariableWithID(varID))
                    {
                        GraphVariable variable = soundgraph.GetGraphVariableByID(varID);
                        variable?.SetValue(port.GetInputValue());
                    }

                }
            }
        }

        public override bool IsVariablePortConnectedByID(SoundGraph soundGraph, string variablePortID)
        {
            string id = GetStateAndTransitionIDsUsingGraph(soundGraph).FirstOrDefault();

            NodePort port = GetInputPort(id + variablePortID + "In");
            if (port == null)
                return false;
            return port.IsConnected;
        }

        public override object GetIncomingVariableValueByID(SoundGraph soundGraph, string variablePortID)
        {

            string id = GetStateAndTransitionIDsUsingGraph(soundGraph).First();
            NodePort port = GetInputPort(id + variablePortID + "In");
            if (port == null)
                return null;
            return port.GetInputValue();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (port.ValueType != typeof(LayersEvent) && port.fieldName.EndsWith("Out"))
            {
                string stateOrTransitionID = port.fieldName.Substring(0, 36);
                string variableID = port.fieldName.Substring(36, port.fieldName.Length - 3 - 36);

                SoundGraph targetGraph = GetSoundGraphFromStateOrTransitionID(stateOrTransitionID);
                if (targetGraph != null && targetGraph.HasGraphVariableWithID(variableID))
                    return targetGraph.GetVariableValueByID(variableID);

            }
            else if (port.fieldName == "CurrentState")
            {
                if (cachedStatesEnumType == null || cachedStatesEnumType.FullName != statesEnumTypeName)
                    cachedStatesEnumType = ReflectionUtils.FindType(statesEnumTypeName);
                if (cachedStatesEnumType != null)
                {
                    string parseString = "NotActive";
                    if (!string.IsNullOrEmpty(currentStateID))
                    {
                        State currentState = states.Find(x => x.guid == currentStateID);

                        if (currentState != null)
                        {
                            parseString = ReflectionUtils.RemoveSpecialCharacters(currentState.stateName);
                        }
                    }

                    try
                    {
                        return System.Enum.Parse(cachedStatesEnumType, parseString);
                    }
                    catch (System.Exception) { };
                }
            }
            return null; // Replace this
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        public override void ConvertSubgraphsToRuntime()
        {
            foreach (State state in states)
            {
                if (state.subGraphRuntime != null)
                    state.MakeRuntimeCopy();


                foreach (Transition transition in state.transitions)
                {
                    if (transition.transitionGraphPlaybackGraph != null)
                        transition.MakeRuntimeCopy();
                }

            }
        }

        public override void OnNodeOpenedInGraphEditor()
        {
            if (!Application.isPlaying)
                ConvertSubgraphsToRuntime();

        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Playback/State-Machine";
        }
    }
}