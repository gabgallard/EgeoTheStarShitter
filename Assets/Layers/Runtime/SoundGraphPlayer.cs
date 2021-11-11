using System.Collections.Generic;
using ABXY.Layers.Runtime.Sound_graph_players;
using UnityEngine;

namespace ABXY.Layers.Runtime
{
    [ExecuteInEditMode]
    public abstract class SoundGraphPlayer : PlayerBase
    {

        public SoundGraph runtimeGraphCopy { get; private set; }



        protected override void Awake()
        {
            // loading the soundgraph from asset if necessary




        }


        protected override void Start()
        {

            if (Application.isPlaying)
                runtimeGraphCopy?.GraphStart();


        }
        protected override void OnEnable()
        {
            base.OnEnable();

            // setup
            ConnectToSoundGraph();

            // running starting events
            if (Application.isPlaying && playOnAwake)
                RunStartingEvents();


        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DisconnectFromSoundGraph();

        }

        private void Update()
        {
            // If I don't have a reference to the soundgraph, I load it from disk. I don't need to do this in built players
            LoadGraphIfNeeded();

            /*
            if (soundGraph == null)
                LoadGraph();

            if (Application.isPlaying)
            {
                runtimeGraphCopy.GraphUpdate();
            }
            else
            {
#if UNITY_EDITOR
                if (runtimeGraphCopy != null)
                {
                    runtimeGraphCopy?.GraphUpdate();
                    UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                }
#endif
            }*/
        }

        private void LoadGraphIfNeeded()
        {
#if UNITY_EDITOR
            if (soundGraph == null)
                LoadGraph();
#endif
        }

        /// <summary>
        /// Connects the player to a soundgraph instance at runtime, or the sound graph asset at edit time
        /// </summary>
        private void ConnectToSoundGraph()
        {
            if (Application.isPlaying) // Then need to set up runtime graph
            {
                // loading asset if editor and necessary
                LoadGraphIfNeeded();

                // Creating the runtime graph
                runtimeGraphCopy = SoundgraphPool.GetInstance(soundGraph, this);

                //copying values to the runtime graph
                CopyPlayerVarValuesToRuntimeGraph();

                // setting up the player so i'ts operating on the same variable objects
                LoadVariablesFromRuntimeGraphIntoPlayer();

                // Loading events from sound graph
                SyncEvents();

                // Setting up starting event parameters, if any
                SyncStartingEventParameters();

                // Running awake on the runtime graph
                runtimeGraphCopy?.GraphAwake();

                // Connecting events so code listening to events on this player gets called
                SetupEvents();

                if (dontDestroyOnLoad && Application.isPlaying)
                    DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                //loading values from assets
                CopyVariableValuesFromAssetGraph();
            }
        }

        public void DisconnectFromSoundGraph()
        {
            if (Application.isPlaying && runtimeGraphCopy != null)
            {
                //disconnecting from events
                UnSetupEvents();

                // Making sure the graph has stopped
                TriggerEvent("EndAll", AudioSettings.dspTime, new Dictionary<string, object>());

                // saving variable values
                for (int index = 0; index < localVariablesList.Count; index++)
                {
                    localVariablesList[index] = localVariablesList[index].FullCopy();
                    localVariablesList[index].defaultObjectValue = localVariablesList[index].objectValue;
                    localVariablesList[index].defaultArrayElements = localVariablesList[index].arrayElements;
                }

                //returning the instance
                SoundgraphPool.ReturnSoundGraph(runtimeGraphCopy);
            }

            runtimeGraphCopy = null;
        }

        public List<AudioSource> GetAudioSourcesInUse()
        {
            if (runtimeGraphCopy != null)
                return runtimeGraphCopy.GetAudioSourcesInUse();
            else
                return soundGraph.GetAudioSourcesInUse();
        }

        /// <summary>
        /// At runtime, we want to copy the variables list from the graph to the player so they are operating on
        /// the same objects
        /// </summary>
        private void LoadVariablesFromRuntimeGraphIntoPlayer()
        {
            localVariablesList = runtimeGraphCopy.GetAllVariables();
        }


        /// <summary>
        /// Loads values from the asset graph, not overwriting modified values
        /// </summary>
        private void CopyVariableValuesFromAssetGraph()
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
        /// At runtime and before synchronizing so player and graph are using the same object,
        /// we need to copy the default values over to the asset graph
        /// </summary>
        private void CopyPlayerVarValuesToRuntimeGraph()
        {
            if (_soundGraph != null)
            {
                List<GraphVariable> assetVariables = runtimeGraphCopy.GetAllVariables();
                foreach (var playerVariable in localVariablesList)
                {
                    GraphVariable targetAssetVariable = assetVariables.Find(x => x.variableID == playerVariable.variableID);
                    if (targetAssetVariable != null)
                    {
                        targetAssetVariable.unityObjectValue = playerVariable.defaultUnityObjectValue;
                        targetAssetVariable.objectValue = playerVariable.defaultObjectValue;
                        targetAssetVariable.defaultUnityObjectValue = playerVariable.defaultUnityObjectValue;
                        targetAssetVariable.defaultObjectValue = playerVariable.defaultObjectValue;
                        targetAssetVariable.arrayElements = playerVariable.defaultArrayElements;
                        targetAssetVariable.defaultArrayElements = playerVariable.defaultArrayElements;
                        targetAssetVariable.arrayType = playerVariable.arrayType;
                    }
                }
            }
        }


















        private void OnDestroy()
        {
            //if (runtimeGraphCopy != null && soundGraphCopies.ContainsKey(runtimeGraphCopy.graphID))
            //soundGraphCopies.Remove(runtimeGraphCopy.graphID);
            DisconnectFromSoundGraph();
        }
        /*
        private void Prepare()
        {
            if (_soundGraph == null)
                return;

            runtimeGraphCopy = (SoundGraph)_soundGraph.RuntimeCopy();
            //soundGraphCopies.Add(runtimeGraphCopy.graphID, runtimeGraphCopy);
            runtimeGraphCopy.owningMono = this;

            runtimeGraphCopy.ResetVariablesToDefaults();

            foreach (GraphVariable variable in localVariablesList)
            {
                GraphVariable targetVar = runtimeGraphCopy.GetGraphVariable(variable.name, false);
                targetVar.SetValue(variable.DefaultValue());

                targetVar.arrayElements.Clear();
                foreach (var element in variable.defaultArrayElements)
                    targetVar.arrayElements.Add(element.Copy());

                targetVar.arrayType = variable.arrayType;
                //runtimeGraphCopy.SetVariable(variable.name, variable.Value());
            }

            runtimeGraphCopy.GraphAwake();

            if (dontDestroyOnLoad && Application.isPlaying)
                DontDestroyOnLoad(this.gameObject);
        }*/

        private void RunStartingEvents()
        {
            if (runtimeGraphCopy == null)
                return;

            foreach (StartingEvent startingEvent in startingEvents)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                foreach (GraphVariable variable in startingEvent.parameters)
                    data.Add(variable.name, variable.Value());
                runtimeGraphCopy.CallEvent(startingEvent.eventName, AudioSettings.dspTime, data, 0);
            }

        }

        /*
        private void WriteLocalVariableToUnderlyingGraph(string variableName)
        {
            if (runtimeGraphCopy == null)
                return;

            GraphVariable localvariable = localVariablesList.Find(x => x.name == variableName);
            GraphVariable graphVariable = runtimeGraphCopy.GetGraphVariable(variableName, false);
            if (localvariable != null && graphVariable != null)
            {
                graphVariable.SetValue(localvariable.Value());
            }
        }*/

        /// <summary>
        /// Sets the value of the variable with the given name
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        protected void SetVariable(string variableName, object value)
        {
            if (runtimeGraphCopy == null)
                return;

            GraphVariable localvariable = localVariablesList.Find(x => x.name == variableName);
            // GraphVariable graphVariable = runtimeGraphCopy.GetGraphVariable(variableName, true);
            if (localvariable != null/* && graphVariable != null*/)
            {
                localvariable.SetValue(value);
                //graphVariable.SetValue(value);
            }
        }

        /// <summary>
        /// Gets the value of the given variable, identfied by name
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        protected object GetVariable(string variableName)
        {
            SoundGraph targetSoundgraph = soundGraph;
            if (runtimeGraphCopy != null)
                targetSoundgraph = runtimeGraphCopy;

            GraphVariable graphVariable = targetSoundgraph.GetGraphVariable(variableName, true);
            if (graphVariable != null)
            {
                return graphVariable.Value();
            }
            return null;
        }

        protected void TriggerEvent(string eventName, double time)
        {
            TriggerEvent(eventName, time, new Dictionary<string, object>());
        }

        protected void TriggerEvent(string eventName, double time, Dictionary<string, object> data)
        {
            runtimeGraphCopy?.CallEvent(eventName, time, data, 0);
        }

        public void RegisterEventListener(string eventName, UnityEngine.Events.UnityAction<double, Dictionary<string, object>> onEventCalled)
        {
            runtimeGraphCopy?.RegisterEventListener(eventName, onEventCalled);
        }

        public void UnRegisterEventListener(string eventName, UnityEngine.Events.UnityAction<double, Dictionary<string, object>> onEventCalled)
        {
            runtimeGraphCopy?.UnregisterEventListener(eventName, onEventCalled);
        }

        public void ClearEventListeners()
        {
            runtimeGraphCopy?.ClearAllEvents();
        }

        //TODO: Figure out if I still need this
        public void ResetVariables()
        {

            if (!EfficientResetVariables())
                InefficientResetVariables();
        }

        /// <summary>
        /// Resets the variables in runtimeCopy to their original values. This 
        /// may not be possible if the runtime graph has been changed. Returns false
        /// if ineffecient reset is needed
        /// </summary>
        /// <returns></returns>
        private bool EfficientResetVariables()
        {
            List<GraphVariable> originalVariables = soundGraph.GetAllVariables();
            List<GraphVariable> copiedVariables = soundGraph.GetAllVariables();
            if (copiedVariables.Count != originalVariables.Count)
                return false;

            for (int index = 0; index < originalVariables.Count; index++)
            {
                GraphVariable originalVar = originalVariables[index];
                GraphVariable copiedVar = copiedVariables[index];
                if (originalVar.name != copiedVar.name || originalVar.typeName != copiedVar.typeName)
                    return false;
                copiedVar.SetValue(originalVar.Value());
            }
            return true;
        }

        /// <summary>
        /// Used if EfficientResetVariables fails
        /// </summary>
        private void InefficientResetVariables()
        {
            List<GraphVariable> originalVariables = soundGraph.GetAllVariables();
            List<GraphVariable> copiedVariables = soundGraph.GetAllVariables();
            foreach (GraphVariable copiedVar in copiedVariables)
            {
                GraphVariable originalVar = originalVariables.Find(x => x.name == copiedVar.name && x.typeName == copiedVar.typeName);
                if (originalVar != null)
                    copiedVar.SetValue(originalVar.Value());
            }
        }

        /// <summary>
        /// Attaches the player events to the runtime graph so when it fires in the graph,
        /// it fires in the player
        /// </summary>
        private void SetupEvents()
        {
            if (Application.isPlaying && runtimeGraphCopy != null)
            {
                foreach (GraphEvent graphEvent in localEventsList)
                {
                    runtimeGraphCopy.RegisterEventListenerById(graphEvent.eventID, graphEvent.CallEvents);
                }
            }
        }

        /// <summary>
        /// Detatches the player events from the runtime graph
        /// </summary>
        private void UnSetupEvents()
        {
            if (Application.isPlaying && runtimeGraphCopy != null)
            {
                foreach (GraphEvent graphEvent in localEventsList)
                {
                    runtimeGraphCopy.UnregisterEventListenerByID(graphEvent.eventID, graphEvent.CallEvents);
                }
            }
        }

        internal override SoundGraphPlayer GetSoundGraphPlayer()
        {
            return this;
        }

        internal override void FinishedWithSoundGraphPlayer(SoundGraphPlayer player)
        {

        }
    }
}
