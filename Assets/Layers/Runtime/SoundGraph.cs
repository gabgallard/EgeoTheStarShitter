using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Flow;
using ABXY.Layers.Runtime.Nodes.Playback;
using ABXY.Layers.Runtime.Nodes.Signal_Sources;
using UnityEngine;
using System.Collections;
using System;

namespace ABXY.Layers.Runtime
{
    [CreateAssetMenu]
    public class SoundGraph : NodeGraph
    {
        //private List<Track> tracks = new List<Track>();

        //public AudioSourceController audioSourceController { get; private set; }


        /// <summary>
        /// The player running this soundgraph
        /// </summary>
        [SerializeField]
        private SoundGraphPlayer _owningMono;

        public bool isRunningSoundGraph { get; private set; }


        public bool isCurrentlyInPool;

        public SoundGraphPlayer owningMono
        {
            get
            {
                if (!isSubgraph)
                    return _owningMono;
                else
                {
                    SoundGraph currentGraph = this;
                    while (currentGraph.subgraphNode != null && currentGraph.subgraphNode.soundGraph != null)
                        currentGraph = currentGraph.subgraphNode.soundGraph;
                    return currentGraph.owningMono;
                }
            }
            set
            {
                _owningMono = value;
            }
        }

        // subgraph Things
        //[SerializeField]
        //private SubGraph owningNode;
        //public SubGraph owningSubgraphNode { get { return owningNode; } }

        public bool isSubgraph { get { return subgraphNode != null; } }


        /// <summary>
        /// The subgraphNode owning this graph
        /// </summary>
        public SubGraphBase subgraphNode;



        /// <summary>
        /// Called whenever an event is called in this graph
        /// </summary>
        private System.Action<string, double, Dictionary<string, object>> onEventCalled;


    
        private GraphInputs _graphInput = null;

        /// <summary>
        /// The graph input node for this graph, if it exists
        /// </summary>
        public GraphInputs graphInput
        {
            get
            {
                if (_graphInput == null)
                    _graphInput = GetNodeOfType<GraphInputs>();
                if (_graphInput == null)
                {
                    _graphInput = AddNode<GraphInputs>();
                    _graphInput.name = "Graph Input";
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.AddObjectToAsset(_graphInput, this);
                    UnityEditor.AssetDatabase.SaveAssets();
#endif
                }
                return _graphInput;
            }
        }

        [SerializeField, TextArea(5, int.MaxValue)]
        private string comment;

        public double lastAccessDSPTime = 0;

        [SerializeField]
        private string _graphID = "";
        public string graphID
        {
            get
            {
                if (_graphID == "")
                    ResetID();
                return _graphID;
            }
        }

        //doing caching since this is expensive to calculate
        double lastCalculatedActiveFrame = 0;
        private bool _isActive = false;
        public bool isActive { 
            get { 
                if (lastCalculatedActiveFrame != currentFrame)
                {
                    _isActive = false;
                    foreach(Node node in nodes)
                    {
                        if (node is FlowNode && !(node is GraphInputs) && (node as FlowNode).isActive )
                            _isActive = true;
                    }
                }
                return _isActive;
            } 
        }


        /// <summary>
        /// Used to keep track the current frame, even in edit mode. Time.frameCount does not work in edit mode, so this is necessary
        /// </summary>
        double currentFrame = 0;



        private bool _stopped = true;

        /// <summary>
        /// True if stop all has been called, and no events have restarted the graph
        /// </summary>
        public bool stopped { get { return _stopped; } private set { _stopped = value; } }

        internal void ResetID()
        {
            _graphID = System.Guid.NewGuid().ToString();
        }

        internal void SetID(string id)
        {
            _graphID = id;
        }

        public override Node CopyNode(Node original)
        {
            Node newNode =  base.CopyNode(original);
            (newNode as FlowNode).ResetID();
            return newNode;
        }

        public void GraphAwake()
        {
            foreach (Node node in nodes)
                if (node != null && (node as FlowNode) != null)
                    (node as FlowNode).NodeAwake();

        }

        public void GraphStart()
        {
            foreach (Node node in nodes)
                if (node != null && (node as FlowNode) != null)
                    (node as FlowNode).NodeStart();
        }

        public void GraphUpdate()
        {
            currentFrame++;
            foreach (Node node in nodes)
                if (node != null && (node as FlowNode) != null)
                    (node as FlowNode).NodeUpdate();

            CallEvent("Update", AudioSettings.dspTime,0);
        }

        Dictionary<System.Type, System.Array> nodeTypeCache = new Dictionary<System.Type, System.Array>();
        private void LoadNodeCache(bool force = false)
        {
            if (nodeTypeCache.Count != 0 && !force)
                return;
            nodeTypeCache.Clear();
            Dictionary<System.Type, List<Node>> tempTypeCache = new Dictionary<System.Type, List<Node>>();
            foreach (Node node in nodes)
            {
                if (node != null) {
                    System.Type type = node.GetType();
                    if (!tempTypeCache.ContainsKey(type))
                        tempTypeCache.Add(type, new List<Node>());
                    tempTypeCache[type].Add(node);
                }
            }

            foreach(KeyValuePair<System.Type, List<Node>> kv in tempTypeCache)
            {
                System.Array array = System.Array.CreateInstance(kv.Key, kv.Value.Count);
                for (int index = 0; index < kv.Value.Count; index++)
                    array.SetValue(kv.Value[index], index);
                if (!nodeTypeCache.ContainsKey(kv.Key))
                    nodeTypeCache.Add(kv.Key, array);
            }
        }


        public T[] GetNodesOfType<T> () where T : Node
        {
            LoadNodeCache();
            T[] selectedNodes = new T[0];
            System.Array outValue;
            if (nodeTypeCache.TryGetValue(typeof(T), out outValue))
                selectedNodes = (T[])outValue;
            return selectedNodes;
        }

        public T GetNodeOfType<T>() where T : Node
        {
            LoadNodeCache();
            T selected = GetNodesOfType<T>().FirstOrDefault();
            return selected;
        }

        public Node[] GetNodesOfType(System.Type type)
        {
            LoadNodeCache();
            System.Array outValue;
            if (nodeTypeCache.TryGetValue(type, out outValue))
                return (Node[])outValue;
            return new Node[0];
        }

        public override Node AddNode(Type type)
        {
            Node node = base.AddNode(type);
            LoadNodeCache(true);
            return node;
        }

        public override void RemoveNode(Node node)
        {
            base.RemoveNode(node);
            LoadNodeCache(true);
        }

        public List<string> GetVariableNames(GraphVariable.ExposureTypes exposure)
        {
            List<string> names = new List<string>();
            GraphInputs[] inputNodes = GetNodesOfType<GraphInputs>();
            foreach (GraphInputs input in inputNodes)
            {
                names.AddRange(input.variables.Where(x => exposure.HasFlag(x.expose)).Select(x => x.name));
                if (input.globals != null)
                    names.AddRange(input.globals.GetVariableNames());
            }
            return names;
        }

        public List<string> GetVariableNames()
        {
            List<string> names = new List<string>();
            GraphInputs[] inputNodes = GetNodesOfType<GraphInputs>();
            foreach (GraphInputs input in inputNodes)
            {
                names.AddRange(input.variables.Select(x => x.name));
                if (input.globals != null)
                    names.AddRange(input.globals.GetVariableNames());
            }
            return names;
        }

        public List<GraphVariable> GetAllVariables()
        {
            List<GraphVariable> variables = new List<GraphVariable>();
            GraphInputs[] inputNodes = GetNodesOfType<GraphInputs>();
            foreach (GraphInputs input in inputNodes)
            {
                variables.AddRange(input.variables);
                if (input.globals != null)
                    variables.AddRange(input.globals.GetAllVariables());
            }
            return variables;
        }


        public List<string> GetEventNames()
        {
            List<string> names = new List<string>();
            //names.Add("Start");
            names.Add("Update");
            GraphInputs[] inputNodes = GetNodesOfType<GraphInputs>();
            foreach (GraphInputs input in inputNodes)
            {
                names.AddRange(input.events.Select(x => x.eventName));
                if (input.globals != null)
                    names.AddRange(input.globals.GetEventNames());
            }
            return names;
        }

        public List<GraphEvent> GetAllEvents()
        {
            List<GraphEvent> names = new List<GraphEvent>();
            GraphInputs[] inputNodes = GetNodesOfType<GraphInputs>();
            foreach (GraphInputs input in inputNodes)
            {
                names.AddRange(input.events);
                if (input.globals != null)
                    names.AddRange(input.globals.GetAllEvents());
            }
            return names;
        }

        public void CallEventByID(string id, double dspTime,int nodesCalledThisFrame)
        {
            CallEventByID(id, dspTime, new Dictionary<string, object>(), nodesCalledThisFrame);
        }

        public void CallEventByID(string id, double dspTime, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            GraphEvent gevent = GetEventByID(id);
            if (gevent != null)
                CallEvent(gevent.eventName, dspTime, data, nodesCalledThisFrame);
        }

        public void CallEvent(string eventName, double dspTime, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            // if an event is called, the graph is no longer stopped (as long as it's not an update event)
            if (eventName != "Update")
                stopped = false;

            double startTime = AudioSettings.dspTime;
            onEventCalled?.Invoke(eventName, dspTime, data);

            //EventNode[] foundNodes = GetNodesOfType<EventNode>().Where(x => x.eventName == eventName).ToArray();

            double eventNodesTime = AudioSettings.dspTime;

        

            // calling on connections to graph input
            graphInput.CallEvent(eventName, dspTime, data, nodesCalledThisFrame);


            //Call graph events in input node
            List<GraphEvent> graphEvents = GetAllEvents().Where(x => x.eventName == eventName).ToList();
            foreach (GraphEvent graphEvent in graphEvents)
            {
                graphEvent?.onGraphEventCalled?.Invoke(dspTime, data);
                graphEvent?.InvokeEphemerals(dspTime, data);
            }


            //Calling on event nodes
            GraphEvent targetEvent = graphEvents.Find(x => x.eventName == eventName);
            if (targetEvent != null)
            {
                double createArrayTime = AudioSettings.dspTime;
                EventNode[] foundNodes = GetNodesOfType<EventNode>().Where(x => x.eventID == targetEvent.eventID).ToArray();
                createArrayTime = AudioSettings.dspTime - createArrayTime;


                foreach (EventNode node in foundNodes)
                    node.PlayAtDSPTime(null, dspTime, data, nodesCalledThisFrame);
            }


            if (targetEvent != null)
            {
                // Doing wait fors
                WaitForEvent[] selectedWaitFors = GetNodesOfType<WaitForEvent>().Where(x => x.eventID == targetEvent.eventID).ToArray();
                foreach (WaitForEvent waitForEvent in selectedWaitFors)
                {
                    waitForEvent.OnEventCalled(dspTime, nodesCalledThisFrame);
                }
            }


            // Doing end all
            if (eventName == "EndAll")
            {
                stopped = true;
                foreach (Node node in nodes)
                {
                    FlowNode castNode = (FlowNode)node;
                    if (castNode != null)
                    {
                        castNode.Stop(null, dspTime, data, nodesCalledThisFrame);
                    }
                    if (castNode is SubGraphBase)
                    {
                        (castNode as SubGraphBase).EndAllPlayback(dspTime, data, nodesCalledThisFrame);
                    }
                }
                LayersEventBus.RaiseEvent(new LayersAnalyzerEvent(System.Guid.NewGuid(),this, null, null, dspTime, LayersAnalyzerEvent.LayersEventTypes.GraphEvent, "End all"));
            }else if (eventName != "Update")
                LayersEventBus.RaiseEvent(new LayersAnalyzerEvent(System.Guid.NewGuid(), this, null, null, dspTime, LayersAnalyzerEvent.LayersEventTypes.GraphEvent, eventName));

        }

        public void CallEvent(string eventName, double dspTime,int nodesCalledThisFrame)
        {
            CallEvent(eventName, dspTime, new Dictionary<string, object>(), nodesCalledThisFrame);
        }

        public void RegisterEventListener(string eventName, UnityEngine.Events.UnityAction<double, Dictionary<string, object>> onEventQueue)
        {
            GraphEvent graphEvent = GetEvent(eventName);
            if (graphEvent == null)
            {
                Debug.LogError($"Event named {eventName} doesn't exist. If you're seeing this, you may need to regenerate code for {name}");
                return;
            }
            graphEvent.onGraphEventCalled.AddListener(onEventQueue);
        }

        public void RegisterEventListenerById(string eventID, UnityEngine.Events.UnityAction<double, Dictionary<string, object>> onEventQueue)
        {
            GraphEvent graphEvent = GetEventByID(eventID);
            if (graphEvent == null)
            {
                Debug.LogError("Event with ID " + eventID + " doesn't exist");
                return;
            }
            graphEvent.onGraphEventCalled.AddListener(onEventQueue);
        }

        public void ClearAllEvents()
        {
            List<GraphEvent> allEvents = GetAllEvents();
            foreach (GraphEvent gevent in allEvents)
                gevent.onGraphEventCalled.RemoveAllListeners();
        }

        public void UnregisterEventListener(string eventName, UnityEngine.Events.UnityAction<double, Dictionary<string, object>> onEventQueue)
        {
            GraphEvent graphEvent = GetEvent(eventName);
            if (graphEvent == null)
            {
                Debug.LogError("Event named " + eventName + " doesn't exist");
                return;
            }
            graphEvent.onGraphEventCalled.RemoveListener(onEventQueue);
        }

        public void UnregisterEventListenerByID(string eventID, UnityEngine.Events.UnityAction<double, Dictionary<string, object>> onEventQueue)
        {
            GraphEvent graphEvent = GetEventByID(eventID);
            if (graphEvent == null)
            {
                Debug.LogError("Event with ID " + eventID + " doesn't exist");
                return;
            }
            graphEvent.onGraphEventCalled.RemoveListener(onEventQueue);
        }

        public void RegisterEventListener(System.Action<string, double, Dictionary<string, object>> onEventQueue)
        {
            onEventCalled -= onEventQueue;
            onEventCalled += onEventQueue;
        }


        public void UnregisterEventListener(System.Action<string, double, Dictionary<string, object>> onEventQueue)
        {
            onEventCalled -= onEventQueue;
        }

        public GraphEvent GetEvent(string eventName)
        {
            List<GraphEvent> graphEvents = new List<GraphEvent>();
            GraphInputs[] inputNodes = GetNodesOfType<GraphInputs>();
            foreach (GraphInputs input in inputNodes)
            {
                graphEvents.AddRange(input.events.Where(x => x.eventName == eventName));
                if (input.globals != null)
                    graphEvents.Add(input.globals.GetEvent(eventName));
            }
            return graphEvents.FirstOrDefault();
        }

        public GraphEvent GetEventByID(string eventID)
        {
            List<GraphEvent> graphEvents = new List<GraphEvent>();
            GraphInputs[] inputNodes = GetNodesOfType<GraphInputs>();
            foreach (GraphInputs input in inputNodes)
            {
                graphEvents.AddRange(input.events.Where(x => x.eventID == eventID));
                if (input.globals != null)
                    graphEvents.Add(input.globals.GetEventByID(eventID));
            }
            return graphEvents.FirstOrDefault();
        }

        public bool HasEventWithID(string eventID)
        {
            return GetEventByID(eventID) != null;
        }

        public T GetVariable<T>(string name)
        {
            GraphVariable graphVar = GetGraphVariable(name,false);
            if (graphVar == null)
            {
                Debug.Log("Variable named " + name + " does not exist");
                return default(T);
            }


            object value = graphVar.Value();
            if (value != null && typeof(T).IsAssignableFrom(value.GetType()))
                return (T)value;
            else
            {
                Debug.LogError("Attempted to get the value of " + name + " as type " + typeof(T).Name + ", but " + name + " is of type " + value.GetType());
                return default(T);
            }
        }

        public object GetVariableValue(string name)
        {
        
            GraphVariable graphVar = GetGraphVariable(name,true);

            if (graphVar == null)
            {
                Debug.Log("Variable named " + name + " does not exist");
                return null;
            }

            if (subgraphNode != null && subgraphNode.IsVariablePortConnectedByID(this, graphVar.variableID))
            {
                return subgraphNode.GetIncomingVariableValueByID(this, graphVar.variableID);
            }

            return graphVar.Value();
        }

        public object GetVariableValueByID(string name)
        {
        

            GraphVariable graphVar = GetGraphVariableByID(name);
            if (graphVar == null)
            {
                Debug.Log("Variable named " + name + " does not exist");
                return null;
            }

            if (subgraphNode != null && subgraphNode.IsVariablePortConnectedByID(this, graphVar.variableID))
            {
                return subgraphNode.GetIncomingVariableValueByID(this, graphVar.variableID);
            }

            return graphVar.Value();

        }

        public void SetVariable<T>(string name, T value) {

            GraphVariable graphVar = GetGraphVariable(name,false);
            if (graphVar == null)
            {
                Debug.Log("Variable named " + name + " does not exist");
                return;
            }

            graphVar.SetValue(value);
        }

        public void SetVariable(string name, object value)
        {
            GraphVariable graphVar = GetGraphVariable(name, false);
            if (graphVar == null)
            {
                Debug.Log("Variable named " + name + " does not exist");
                return;
            }

            graphVar.SetValue(value);
        }

        public void SetVariableByID(string id, object value)
        {
            GraphVariable graphVar = GetGraphVariableByID(id);
            if (graphVar == null)
            {
                Debug.Log("Variable with id " + id + " does not exist");
                return;
            }

            graphVar.SetValue(value);
        }

        public GraphVariable GetGraphVariable(string name, bool includePrivate)
        {
            
            //GraphVariable var = variables.Find(x => x.name == name);
            //return GetNodesOfType<GraphInputs>().Select(x => x.variables.Find(x => x.name == name)).FirstOrDefault();
            if (includePrivate)
                return GetAllVariables().Where(y => y.name == name).FirstOrDefault();
            else
                return GetAllVariables().Where(y => y.name == name && y.expose != GraphVariable.ExposureTypes.DoNotExpose).FirstOrDefault();
        }

        public GraphVariable GetGraphVariableByID(string id)
        {
            //GraphVariable var = variables.Find(x => x.name == name);
            //return GetNodesOfType<GraphInputs>().Select(x => x.variables.Find(x => x.name == name)).FirstOrDefault();
            return GetAllVariables().Where(y => y.variableID == id).FirstOrDefault();
        }

        public bool HasGraphVariableWithID(string id)
        {
            return GetGraphVariableByID(id) != null;
        }

  
        public void SetupAsRegularGraph()
        {
            //nodes.Add(CreateInstance<GraphInputs>());

        }

        public override NodeGraph Copy()
        {
            SoundGraph newGraph =  (SoundGraph)base.Copy();
            newGraph._graphID = _graphID;

            SubGraphBase[] subGraphs = newGraph.GetNodesOfType<SubGraphBase>();
            foreach (SubGraphBase subgraph in subGraphs)
            {
                subgraph.ConvertSubgraphsToRuntime();
            }

            return newGraph;
        }

        public NodeGraph RuntimeCopy()
        {
            NodeGraph graph = Copy();
            (graph as SoundGraph).isRunningSoundGraph = true;
            return graph;
        }

        public void ResetVariablesToDefaults()
        {
            foreach(GraphVariable variable in GetAllVariables())
            {
                variable.ResetToDefaultValue();
            }
        }

    }
}