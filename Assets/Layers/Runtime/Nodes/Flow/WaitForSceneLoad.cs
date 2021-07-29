using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/Wait For Scene Load")]
    public class WaitForSceneLoad : FlowNode {

        private bool primed = false;

        [SerializeField, Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private string sceneName= "";

#pragma warning disable CS0414
        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent beginWait = null;

        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent reset = null;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        private LayersEvent endWait = null;
#pragma warning restore CS0414

        private string lastSceneName = "";
        Dictionary<string, object> lastData = new Dictionary<string, object>();

        public override bool isActive => primed;

        // Use this for initialization
        protected override void Init() {
            base.Init();
		
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port) {
            return null; // Replace this
        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            if (calledBy.fieldName == "beginWait")
            {
                lastSceneName = GetSceneName(data);
                lastData = data;
                if (!Application.isPlaying || SceneManager.GetSceneByName(lastSceneName).isLoaded)
                    CallFunctionOnOutputNodes("endWait", time, data, nodesCalledThisFrame);
                else
                    primed = true;
            }
            else
            {
                primed = false;
            }
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
            primed = false;
        }

        public override void NodeUpdate()
        {
            if (primed)
            {
                if (SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    CallFunctionOnOutputNodes("endWait", AudioSettings.dspTime, lastData,0);
                    primed = false;
                }
            }
        }

        private string GetSceneName(Dictionary<string, object> parameters)
        {
            return GetInputOrParameterValue<string>("sceneName", GetInputValue<string>("sceneName", sceneName), parameters);
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/Wait-For-Scene-Load";
        }
    }
}