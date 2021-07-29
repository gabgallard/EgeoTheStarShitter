using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes
{
    [CreateNodeMenu("Automation/Move Towards")]
    public class MoveTowardsNode : FlowNode
    {


        [SerializeField]
        private string variableID = "";

        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private float maxDelta = 1f;

        public enum Styles { Variable, Port }

        [SerializeField]
        private Styles style = Styles.Variable;

        private double lastActivationTime = -1;


        // used just to draw ports in variable mode, but actually stores the value in flow mode
        [SerializeField]
        public GraphVariable targetValue = new GraphVariable();

        //Used to draw output for flow mode
        [SerializeField]
        public GraphVariable targetOutput = new GraphVariable();




        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        

        public override void NodeUpdate()
        {
            if (style == Styles.Variable)
            {
                double deltaTime = Time.deltaTime;

                if (lastActivationTime != -1)
                    deltaTime = Time.time - lastActivationTime;

                lastActivationTime = Time.time;

                GraphVariable graphVariable = soundGraph.GetGraphVariableByID(variableID);

                if (graphVariable == null)
                    return;

                float actualDeltaTime = GetInputValue<float>("maxDelta", maxDelta) * (float)deltaTime;

                if (graphVariable.typeName == typeof(float).FullName)
                {
                    float targetFloat = GetInputValue<float>("targetValue", (float)targetValue.Value());
                    float currentValue = (float)graphVariable.Value();
                    float newFloat = Mathf.MoveTowards((float)currentValue, (float)targetFloat, actualDeltaTime);
                    graphVariable.SetValue(newFloat);
                }
                else if (graphVariable.typeName == typeof(double).FullName)
                {
                    double targetDouble = GetInputValue<double>("targetValue", (double)targetValue.Value());
                    double currentValue = (double)graphVariable.Value();
                    double newDouble = Mathf.MoveTowards((float)currentValue, (float)targetDouble, actualDeltaTime);
                    graphVariable.SetValue(newDouble);

                }
                else if (graphVariable.typeName == typeof(Vector3).FullName)
                {
                    Vector3 targetVector = GetInputValue<Vector3>("targetValue", (Vector3)targetValue.Value());
                    Vector3 currentValue = (Vector3)graphVariable.Value();
                    Vector3 newVector = Vector3.MoveTowards(currentValue, targetVector, actualDeltaTime);
                    graphVariable.SetValue(newVector);
                }
                else if (graphVariable.typeName == typeof(Quaternion).FullName)
                {
                    Quaternion targetQuaternion = GetInputValue<Quaternion>("targetValue", (Quaternion)targetValue.Value());
                    Quaternion currentValue = (Quaternion)graphVariable.Value();
                    Quaternion newQuaternion = Quaternion.RotateTowards(currentValue, targetQuaternion, actualDeltaTime);
                    graphVariable.SetValue(newQuaternion);
                }
            }
            else
            {
                double deltaTime = Time.deltaTime;

                if (lastActivationTime != -1)
                    deltaTime = Time.time - lastActivationTime;

                lastActivationTime = Time.time;


                float actualDeltaTime = GetInputValue<float>("maxDelta", maxDelta) * (float)deltaTime;

                if (targetValue.typeName == typeof(float).FullName)
                {
                    float targetFloat = GetInputValue<float>("targetValue", (float)targetValue.Value());
                    float currentValue = (float)targetValue.Value();
                    float newFloat = Mathf.MoveTowards((float)currentValue, (float)targetFloat, actualDeltaTime);
                    targetValue.SetValue(newFloat);
                }
                else if (targetValue.typeName == typeof(double).FullName)
                {
                    double targetDouble = GetInputValue<double>("targetValue", (double)targetValue.Value());
                    double currentValue = (double)targetValue.Value();
                    double newDouble = Mathf.MoveTowards((float)currentValue, (float)targetDouble, actualDeltaTime);
                    targetValue.SetValue(newDouble);

                }
                else if (targetValue.typeName == typeof(Vector3).FullName)
                {
                    Vector3 targetVector = GetInputValue<Vector3>("targetValue", (Vector3)targetValue.Value());
                    Vector3 currentValue = (Vector3)targetValue.Value();
                    Vector3 newVector = Vector3.MoveTowards(currentValue, targetVector, actualDeltaTime);
                    targetValue.SetValue(newVector);
                }
                else if (targetValue.typeName == typeof(Quaternion).FullName)
                {
                    Quaternion targetQuaternion = GetInputValue<Quaternion>("targetValue", (Quaternion)targetValue.Value());
                    Quaternion currentValue = (Quaternion)targetValue.Value();
                    Quaternion newQuaternion = Quaternion.RotateTowards(currentValue, targetQuaternion, actualDeltaTime);
                    targetValue.SetValue(newQuaternion);
                }
            }
               
            

            
        }


        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (style == Styles.Port)
            {
                return targetValue.Value();
            }
            return null; // Replace this
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Automation/Move-Towards";
        }
    }
}