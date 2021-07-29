using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ABXY.Layers.Runtime.Sound_graph_players
{
    public abstract class PlayerBase: MonoBehaviour
    {
        [SerializeField]
        protected SoundGraph _soundGraph = null;
        public SoundGraph soundGraph
        {
            get { return _soundGraph; }
            set
            {
                _soundGraph = value;
                if (_soundGraph != null)
                {
                    Awake();
                    Start();
                    OnEnable();
                }
            }
        }

        [SerializeField]
        protected bool playOnAwake = true;


        [SerializeField]
        protected bool dontDestroyOnLoad = false;

        //protected static Dictionary<string, SoundGraph> soundGraphCopies = new Dictionary<string, SoundGraph>();

        [SerializeField]
        protected List<GraphEvent> localEventsList = new List<GraphEvent>();

        [SerializeField]
        protected List<GraphVariable> localVariablesList = new List<GraphVariable>();

        [SerializeField]
        protected List<StartingEvent> startingEvents = new List<StartingEvent>();

#pragma warning disable CS0414
        [SerializeField]
        protected bool variablesExpanded = true;

        [SerializeField]
        protected bool eventsExpanded = true;
#pragma warning restore CS0414

        protected System.Guid _playerID = System.Guid.NewGuid();
        public System.Guid playerID { get { return _playerID; } }


        /// <summary>
        /// Called when the sound graph is null and needs to be loaded from disk
        /// </summary>
        protected virtual void LoadGraph()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void Awake()
        {

        }

        protected virtual void OnEnable()
        {
            LayersEventBus.RaiseEvent(new LayersAnalyzerEvent(_playerID,soundGraph, null, null,AudioSettings.dspTime,LayersAnalyzerEvent.LayersEventTypes.SoundGraphPlayerCreated, "Player created"));
        }

        protected virtual void OnDisable()
        {
            LayersEventBus.RaiseEvent(new LayersAnalyzerEvent(_playerID, soundGraph, null, null, AudioSettings.dspTime, LayersAnalyzerEvent.LayersEventTypes.SoundGraphPlayerDestroyed, "Player destroyed"));
        }


        protected void SyncVariables()
        {

            if (_soundGraph != null)
            {
                List<GraphVariable> graphVariables = _soundGraph.GetAllVariables();
                for (int index = 0; index < localVariablesList.Count; index++)
                {
                    GraphVariable partnerVarInGraph = graphVariables.Find(x => x.variableID == localVariablesList[index].variableID);
                    if (partnerVarInGraph == null || partnerVarInGraph.expose != GraphVariable.ExposureTypes.AsInput || partnerVarInGraph.typeName != localVariablesList[index].typeName)
                        localVariablesList.Remove(localVariablesList[index]);
                    else
                    {
                        localVariablesList[index].typeName = partnerVarInGraph.typeName;
                        localVariablesList[index].name = partnerVarInGraph.name;
                        localVariablesList[index].arrayType = partnerVarInGraph.arrayType;
                        if (localVariablesList[index].synchronizeWithGraphVariable)
                        {
                            

                            //localVariablesList[index].arrayElements = partnerVarInGraph.arrayElements.Select(x => x).ToList();
                            GraphVariable.CopyValue(partnerVarInGraph, localVariablesList[index]);
                        }
                    }
                }

                foreach (GraphVariable graphVar in graphVariables)
                    if (graphVar.expose == GraphVariable.ExposureTypes.AsInput && localVariablesList.Find(x => x.variableID == graphVar.variableID) == null)
                    {
                        localVariablesList.Add(graphVar.FullCopy());
                    }
            }
        }

        /// <summary>
        /// Copies the events from the asset graph to the player
        /// </summary>
        protected void SyncEvents()
        {
            if (_soundGraph != null)
            {
                List<GraphEvent> graphEvents = _soundGraph.GetAllEvents();
                for (int index = 0; index < localEventsList.Count; index++)
                {
                    GraphEvent partnerEventInGraph = graphEvents.Find(x => x.eventID == localEventsList[index].eventID);
                    if (partnerEventInGraph == null || !partnerEventInGraph.expose)
                        localEventsList.Remove(localEventsList[index]);
                    else
                    {
                        GraphEvent.CopyEvent(partnerEventInGraph, localEventsList[index]);
                    }
                }

                foreach (GraphEvent graphVar in graphEvents)
                    if (graphVar.expose && localEventsList.Find(x => x.eventID == graphVar.eventID) == null)
                    {
                        localEventsList.Add(graphVar.Copy());
                    }
            }
        }

        /// <summary>
        /// Setting up parameters so I can call the starting event
        /// </summary>
        protected void SyncStartingEventParameters()
        {
            if (_soundGraph == null)
                return;
            foreach (StartingEvent startingEvent in startingEvents)
            {
                GraphEvent targetEvent = _soundGraph.GetEvent(startingEvent.eventName);
                if (targetEvent == null)
                    continue;
                foreach (GraphEvent.EventParameterDef parameterDef in targetEvent.parameters)
                {
                    if (startingEvent.parameters.Find(x => x.name == parameterDef.parameterName && x.typeName == parameterDef.parameterTypeName) == null)
                        startingEvent.parameters.Add(new GraphVariable(parameterDef.parameterName, parameterDef.parameterTypeName));
                }
                for (int index = 0; index < startingEvent.parameters.Count; index++)
                {
                    GraphVariable currentParameterValue = startingEvent.parameters[index];
                    if (targetEvent.parameters.FindAll(x => x.parameterName == currentParameterValue.name && x.parameterTypeName == currentParameterValue.typeName).Count == 0)
                        startingEvent.parameters.RemoveAt(index);
                }
            }
        }

        internal abstract SoundGraphPlayer GetSoundGraphPlayer();

        internal abstract void FinishedWithSoundGraphPlayer(SoundGraphPlayer player);


        [System.Serializable]
        protected class StartingEvent
        {
            public string eventName;


            //[SerializeField]
            //protected string eventID = System.Guid.Empty.ToString();

            [SerializeField]
            public List<GraphVariable> parameters = new List<GraphVariable>();
        }
    }
}
