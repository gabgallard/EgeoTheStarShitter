using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes.Playback;
using UnityEngine;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using Andeart.EditorCoroutines.Unity.Coroutines;
using Andeart.EditorCoroutines.Unity;
#endif

namespace ABXY.Layers.Runtime.Nodes
{
    public abstract class FlowNode : Node
    {
        //public enum States { Queued, Inactive, Running}
        [SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        protected LayersEvent playFinished;


        public SoundGraph soundGraph { get { return graph as SoundGraph; } }


        public enum connectionTypes { Play, Pause, Stop, Resume, Unknown };

        public List<VariableDrivenByParameter> variablesDrivenByParameters = new List<VariableDrivenByParameter>();

        private double _lastAccessDSPTime;
        public double lastAccessDSPTime
        {
            get { return _lastAccessDSPTime; }
            private set
            {
                _lastAccessDSPTime = value;
                if (soundGraph != null)
                    soundGraph.lastAccessDSPTime = value;
            }
        }

#pragma warning disable CS0414
        [SerializeField]
        private bool expanded = true;
#pragma warning restore CS0414

        private System.Guid _nodeID;
        public System.Guid nodeID
        {
            get
            {
                if (_nodeID == System.Guid.Empty)
                {
                    if (!System.Guid.TryParse(serializedID, out _nodeID))
                    {
                        ResetID();
                    }
                }
                return _nodeID;
            }
        }

        [SerializeField]
        private string serializedID;

        public void ResetID()
        {
            _nodeID = System.Guid.NewGuid();
            serializedID = _nodeID.ToString();
        }


        protected override void Init()
        {
            base.Init();
            SetVisibility();
        }


        private void SetVisibility()
        {
#if !SYMPHONY_DEV && UNITY_EDITOR
            if (hideFlags != HideFlags.HideInHierarchy)
            {
                hideFlags = HideFlags.HideInHierarchy;
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
#elif SYMPHONY_DEV && UNITY_EDITOR
            if (hideFlags != HideFlags.None)
            {
                hideFlags = HideFlags.None;
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
#endif
        }

        public virtual void NodeAwake()
        {

        }

        public virtual void NodeStart()
        {

        }

        public virtual void NodeUpdate()
        {

        }

        public virtual void OnNodeOpenedInGraphEditor()
        {

        }

        public virtual void LoadFlowNode(NodePort calledBy, double time, Dictionary<string, object> data)
        {

        }

        public virtual void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {

        }

        public virtual void ResumePlay(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {

        }

        public virtual void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {

        }

        public virtual void Pause(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {

        }

        private List<IEnumerator> runtimeRoutines = new List<IEnumerator>();

#if UNITY_EDITOR
        private List<IEnumerator> editorRoutines = new List<IEnumerator>();
        //private Dictionary<IEnumerator, List<Unity.EditorCoroutines.Editor.EditorCoroutine>> editorRoutines = new Dictionary<IEnumerator, List<Unity.EditorCoroutines.Editor.EditorCoroutine>>();
#endif

        public LayersCoroutine StartCoroutine(IEnumerator routine)
        {

            LayersCoroutine output = null;

            if (Application.isPlaying)
            {
                if (soundGraph == null || soundGraph.owningMono == null || soundGraph.owningMono.gameObject == null)
                    return null;

                if (!soundGraph.owningMono.gameObject.activeInHierarchy)
                    return null;
                Coroutine unityCoroutine = soundGraph.owningMono.StartCoroutine(routine);
                output = new LayersCoroutine(routine, unityCoroutine);
                runtimeRoutines.Add(routine);
            }
            else
            {
#if UNITY_EDITOR
                EditorCoroutine editorCoroutine = EditorCoroutineService.StartCoroutine(routine);
                output = new LayersCoroutine(routine, editorCoroutine);

                editorRoutines.Add(routine);
#endif
            }
            return output;
        }

        public void StopCoroutine(IEnumerator routine)
        {
            if (routine == null)
                return;

            if (Application.isPlaying)
            {
                soundGraph.owningMono.StopCoroutine(routine);
                runtimeRoutines.Remove(routine);
            }
            else
            {
#if UNITY_EDITOR
                EditorCoroutineService.StopCoroutine(routine);
                editorRoutines.Remove(routine);
#endif
            }
        }

        public void StopCoroutine(LayersCoroutine coroutine)
        {
            if (Application.isPlaying)
            {
                if (coroutine.regularCoroutine != null)
                    soundGraph.owningMono.StopCoroutine(coroutine.regularCoroutine);
                if (coroutine.ienumerator != null)
                    runtimeRoutines.Remove(coroutine.ienumerator);
            }
            else
            {
#if UNITY_EDITOR
                if (coroutine.editorCoroutine != null)
                    EditorCoroutineService.StopCoroutine(coroutine.editorCoroutine);
                if (coroutine.ienumerator != null)
                    editorRoutines.Remove(coroutine.ienumerator);
#endif
            }
        }

        public void StopAllCoroutines()
        {
            if (Application.isPlaying)
            {
                for (int index = 0; index < runtimeRoutines.Count; index++)
                    StopCoroutine(runtimeRoutines[index]);
                runtimeRoutines.Clear();
            }
            else
            {
#if UNITY_EDITOR
                foreach (IEnumerator routine in editorRoutines)
                    EditorCoroutineService.StopCoroutine(routine);
                editorRoutines.Clear();
#endif
            }
        }

        public class LayersCoroutine
        {
            public IEnumerator ienumerator { get; private set; }

#if UNITY_EDITOR
            public EditorCoroutine editorCoroutine { get; private set; }
#endif

            public Coroutine regularCoroutine { get; private set; }

#if UNITY_EDITOR
            public LayersCoroutine(IEnumerator ienumerator, EditorCoroutine editorCoroutine)
            {
                this.ienumerator = ienumerator;
                this.editorCoroutine = editorCoroutine;
            }
#endif

            public LayersCoroutine(IEnumerator ienumerator, Coroutine regularCoroutine)
            {
                this.ienumerator = ienumerator;
                this.regularCoroutine = regularCoroutine;
            }
        }

        public struct FlowNodeInfo
        {
            public FlowNode flownode { get; private set; }
            public connectionTypes connectionType { get; private set; }

            public NodePort remoteNodePort { get; private set; }

            public FlowNodeInfo(FlowNode flownode, NodePort remoteNodePort, connectionTypes connectionType)
            {
                this.flownode = flownode;
                this.connectionType = connectionType;
                this.remoteNodePort = remoteNodePort;
            }

            public override string ToString()
            {
                return "FlowNodeInfo: {" + (flownode != null ? flownode.name : "null") + ", " + connectionType + "}";
            }
        }


        public virtual bool isActive { get { return false; } }


        public void CallFunctionOnOutputNodes(string name, double dspStartTime, int nodesCalledThisFrame)
        {
            CallFunctionOnOutputNodes(name, dspStartTime, new Dictionary<string, object>(), nodesCalledThisFrame);
        }

        public void CallFunctionOnOutputNodes(string name, double dspStartTime, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            List<FlowNodeInfo> targets = GetOutputNodes(name);

            CallFunctionOnTargets(name, targets, dspStartTime, data, nodesCalledThisFrame);
        }

        public void CallFunctionOnOutputNodes(NodePort port, double dspStartTime, int nodesCalledThisFrame)
        {
            CallFunctionOnOutputNodes(port, dspStartTime, new Dictionary<string, object>(), nodesCalledThisFrame);
        }

        public void CallFunctionOnOutputNodes(NodePort port, double dspStartTime, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            List<FlowNodeInfo> targets = GetOutputNodes(port);
            CallFunctionOnTargets("", targets, dspStartTime, data, nodesCalledThisFrame);
        }

        private void CallFunctionOnTargets(string eventName, List<FlowNodeInfo> targets, double dspStartTime, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            nodesCalledThisFrame++;

            if (soundGraph.stopped)
                return;

            // preventing loops from running away in a frame;
            int loopDetectionThreshold = 8;
            if (nodesCalledThisFrame > loopDetectionThreshold)
            {
                StartCoroutine(SkipOneFrame(() =>
                {
                    nodesCalledThisFrame = 0;
                    CompleteCallFunctionOnTargets(eventName, targets, dspStartTime, data, nodesCalledThisFrame);
                }));
            }
            else
            {
                CompleteCallFunctionOnTargets(eventName, targets, dspStartTime, data, nodesCalledThisFrame);
            }
            

        }

        private IEnumerator SkipOneFrame(System.Action action)
        {
            yield return null;
            action?.Invoke();
        }

        /// <summary>
        /// In the first half of this operation (CallFunctionOnTargets) we check if execution needs to get paused for a frame to prevent
        /// runaway loops and a stackoverflow. This method completes the operation after the check
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="targets"></param>
        /// <param name="dspStartTime"></param>
        /// <param name="data"></param>
        /// <param name="nodesCalledThisFrame"></param>
        private void CompleteCallFunctionOnTargets(string eventName, List<FlowNodeInfo> targets, double dspStartTime, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            foreach (FlowNodeInfo target in targets)
            {
                if (target.flownode != null)
                {
                    if (target.connectionType == connectionTypes.Play || target.connectionType == connectionTypes.Unknown)
                    {
                        target.flownode.PlayAtDSPTime(target.remoteNodePort, dspStartTime, data, nodesCalledThisFrame);
                    }
                    else if (target.connectionType == connectionTypes.Resume)
                    {
                        target.flownode.ResumePlay(target.remoteNodePort, dspStartTime, data, nodesCalledThisFrame);
                    }
                    else if (target.connectionType == connectionTypes.Pause)
                    {
                        target.flownode.Pause(target.remoteNodePort, dspStartTime, data, nodesCalledThisFrame);
                    }
                    else if (target.connectionType == connectionTypes.Stop)
                    {
                        target.flownode.Stop(target.remoteNodePort, dspStartTime, data, nodesCalledThisFrame);
                    }
                }
                target.flownode.SetLastAccessTime(dspStartTime);
            }

            if (eventName != "update")
                SetLastAccessTime(dspStartTime);
        }

        private void SetLastAccessTime(double dspStartTime)
        {
            if (lastAccessDSPTime < dspStartTime)
                lastAccessDSPTime = dspStartTime;
        }


        public static IEnumerator WaitForDSPTime(double waitTime, System.Action onReady)
        {
            while (waitTime > AudioSettings.dspTime)
                yield return null;
            onReady?.Invoke();
        }


        public FlowNodeInfo GetOutputNode(string name)
        {
            NodePort outputPort = GetOutputPort(name);
            FlowNode flowNode = outputPort != null && outputPort.Connection != null ? (FlowNode)outputPort.Connection.node : null;
            string otherPortName = outputPort != null && outputPort.Connection != null ? outputPort.Connection.fieldName : "";
            FlowNode.connectionTypes connectionType = FlowNode.connectionTypes.Unknown;
            if (otherPortName == "play")
            {
                connectionType = FlowNode.connectionTypes.Play;
            }
            else if (otherPortName == "pause")
            {
                connectionType = FlowNode.connectionTypes.Pause;
            }
            else if (otherPortName == "resume")
            {
                connectionType = connectionTypes.Resume;
            }
            else if (otherPortName == "stop")
            {
                connectionType = FlowNode.connectionTypes.Stop;
            }
            return new FlowNodeInfo(flowNode, outputPort.Connection, connectionType);
        }

        public List<FlowNodeInfo> GetOutputNodes(string name)
        {
            NodePort localPort = GetOutputPort(name);
            return GetOutputNodes(localPort);
        }

        public List<FlowNodeInfo> GetOutputNodes(NodePort localPort)
        {
            List<NodePort> ports = localPort != null && localPort.Connection != null ? localPort.GetConnections() : new List<NodePort>();

            List<FlowNodeInfo> flowNodeInfos = new List<FlowNodeInfo>();
            foreach (NodePort port in ports)
            {
                FlowNode flowNode = port != null && port != null ? (FlowNode)port.node : null;
                string otherPortName = port != null && port != null ? port.fieldName : "";
                FlowNode.connectionTypes connectionType = FlowNode.connectionTypes.Unknown;
                if (otherPortName == "play")
                {
                    connectionType = FlowNode.connectionTypes.Play;
                }
                else if (otherPortName == "resume")
                    connectionType = connectionTypes.Resume;
                else if (otherPortName == "pause")
                {
                    connectionType = FlowNode.connectionTypes.Pause;
                }
                else if (otherPortName == "stop")
                {
                    connectionType = FlowNode.connectionTypes.Stop;
                }
                flowNodeInfos.Add(new FlowNodeInfo(flowNode, port, connectionType));
            }

            return flowNodeInfos;
        }

        public int NumberDynamicOutputs()
        {
            return DynamicOutputs.ToArray().Length;
        }

        public int NumberDynamicInputs()
        {
            return DynamicInputs.ToArray().Length;
        }


        public NodePort GetDynamicOutputPort(int index)
        {
            if (DynamicOutputs.Count() <= index || index < -1)
                return null;

            NodePort port = DynamicOutputs.ToArray()[index];
            return port != null ? port : null;
        }

        public NodePort GetDynamicInputPort(int index)
        {
            NodePort port = DynamicInputs.ToArray()[index];
            return port != null ? port : null;
        }

        public List<GraphEvent.EventParameterDef> GetIncomingEventParameterDefsOnPort(string portName, List<Node> visitedNodes)
        {
            List<GraphEvent.EventParameterDef> parameters = new List<GraphEvent.EventParameterDef>();

            NodePort port = GetInputPort(portName);
            if (port != null)
            {
                if (port.ConnectionCount == 1 && port.Connection != null)
                {
                    NodePort otherPort = port.Connection;
                    parameters.AddRange((otherPort.node as FlowNode).GetOutGoingEventParameterDefsOnPort(otherPort, visitedNodes));
                }
                else
                {
                    foreach (NodePort otherPort in port.GetConnections())
                    {
                        if ((otherPort.node as FlowNode) != null)
                            parameters.AddRange((otherPort.node as FlowNode).GetOutGoingEventParameterDefsOnPort(otherPort, new List<Node>(visitedNodes)));
                    }
                }
            }

            return parameters;
        }


        public List<GraphEvent.EventParameterDef> GetOutGoingEventParameterDefsOnPort(NodePort port, List<Node> visitedNodes)
        {
            if (visitedNodes.Contains(this))
                return new List<GraphEvent.EventParameterDef>();
            visitedNodes.Add(this);
            return GetOutGoingEventParametersOnPortInternal(port, visitedNodes);
        }
        public AudioOut[] GetAudioOuts(NodePort port, string fallbackNodeGUID)
        {
            if (port.IsConnected)
                return port.GetConnections().Select(x => (x.node as AudioOut)).ToArray();
            else
            {
                AudioOut audioOut = soundGraph.GetNodesOfType<AudioOut>().Where(x => x.nodeID.ToString() == fallbackNodeGUID).FirstOrDefault();
                if (audioOut == null)
                    return new AudioOut[0];
                return new AudioOut[] { audioOut };
            }
        }

        public T GetInputOrParameterValue<T>(string propertyName, T defaultValue, Dictionary<string, object> parameters)
        {
            //if (variablesDrivenByParameters.

            //string parameterName = "";
            VariableDrivenByParameter param = variablesDrivenByParameters.Find(x => x.serializedPropertyPath == propertyName);
            if (param != null)
            {
                object parameterObject = null;
                if (parameters.TryGetValue(param.parameterName, out parameterObject))
                {
                    if (parameterObject != null && parameterObject.GetType() == typeof(T))
                        return (T)parameterObject;
                }
            }
            return GetInputValue<T>(propertyName, defaultValue);
        }


        protected virtual string GetHelpFileResourcePath()
        {
            string fileName = this.GetType().Name;
            fileName = fileName.Replace("Node", "");
            fileName = Regex.Replace(fileName, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

            return string.Format("Nodes/{0}", fileName);
        }

        /// <summary>
        /// Don't call this
        /// </summary>
        /// <param name="port"></param>
        /// <param name="visitedNodes"></param>
        /// <returns></returns>
        protected abstract List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes);

        [System.Serializable]
        public class VariableDrivenByParameter
        {
            public string serializedPropertyPath;
            public string parameterName;

            public VariableDrivenByParameter(string serializedPropertyPath, string parameterName)
            {
                this.serializedPropertyPath = serializedPropertyPath;
                this.parameterName = parameterName;
            }

            public override bool Equals(object obj)
            {
                return obj != null
                       && obj is VariableDrivenByParameter
                       && this.serializedPropertyPath == ((VariableDrivenByParameter)obj).serializedPropertyPath
                       && this.parameterName == ((VariableDrivenByParameter)obj).parameterName;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

    }
}