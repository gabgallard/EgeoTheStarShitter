using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Flow
{
    [Node.CreateNodeMenu("Flow/Do While")]
    public class DoWhile : FlowNode {

        [Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict), SerializeField]
        private LayersEvent enter;
        [SerializeField, Input(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected LayersEvent resetIterations;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected LayersEvent conditionReached;

        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected LayersEvent continueLoop;


        [SerializeField, Output(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.Strict)]
        protected int index;

        [SerializeField]
        private int iterations = 0;

        [Input(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Override, Node.TypeConstraint.Inherited), SerializeField]
        private int maxIterationCount = 5;

        [SerializeField]
        private bool useCustomLogic = false;


        public override void NodeAwake()
        {
            base.NodeAwake();
            iterations = 0;
        }

        private bool isConditionReached { get { return useCustomLogic ? !GetInputValue<bool>("condition") : iterations >= GetInputValue<int>("maxIterationCount", maxIterationCount); } }
        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            HandlePlayCalls(calledBy, time, data, nodesCalledThisFrame);
        }
    
        private void HandlePlayCalls(NodePort calledBy, double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
        

            if (calledBy.fieldName == "enter")
            {

                if (!isConditionReached)
                    StartCoroutine(WaitForDSPTime(time, () =>{iterations = useCustomLogic ? iterations + 1 : Mathf.Clamp(iterations + 1, 0, GetInputValue<int>("maxIterationCount", maxIterationCount));}));
            
            
                if (isConditionReached)
                    CallFunctionOnOutputNodes("conditionReached", time, data, nodesCalledThisFrame);
                else
                    CallFunctionOnOutputNodes("continueLoop", time, data, nodesCalledThisFrame);


            }
            else //then reset was called
            {
                //then reset iterations
            
                StartCoroutine(WaitForDSPTime(time, () => { iterations = 0; }));
            }
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data,nodesCalledThisFrame);
            StopAllCoroutines();
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "index")
                return iterations;
            else return null;
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return GetIncomingEventParameterDefsOnPort("enter", visitedNodes);
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Flow/Do-While";
        }
    }
}