using System;
using System.Collections.Generic;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class LayersEventValue : GraphVariableValue, SplittableValue, CombinableValue, SplitValuePlayEvent, CombineValuePlayEvent, CombineNodeParamSource
    {
        public override Type handlesType => typeof(LayersEvent);

        public override bool CompareValues(Comparison.comparisonOperators comparator, object a, object b)
        {
            return false;
        }

        public override object GetValue(GraphVariableBase graphVariable)
        {
            return null;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            return null;
        }

        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
        
        }

        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {

        }

        public override Color GetDefaultColor()
        {
            return new Color32(158, 0, 147, 255);
        }
        public override bool IsNonreferenceableType()
        {
            return true;
        }

        public object GetSplitValue(NodePort targetPort, SplitNode target)
        {
            object value = null;
            target.arbitraryData.TryGetValue(targetPort.fieldName, out value);

            if (value == null)
            {
                GraphVariableValue gvv = ValueUtility.GetVariableValue(targetPort.ValueType.FullName, ValueUtility.ValueFilter.All);
                if (gvv != null)
                    value = gvv.GetValueOnInitialization();
            }

            return value;
        }

        public object GetCombineValue(NodePort targetPort, CombineNode target)
        {
            return null;
        }


        public void PlayAtDSPTime(SplitNode node, NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            foreach(KeyValuePair<string,object> datum in data)
            {
                if (node.arbitraryData.ContainsKey(datum.Key))
                    node.arbitraryData[datum.Key] = datum.Value;
                else
                    node.arbitraryData.Add(datum.Key, datum.Value);
            }
            node.CallFunctionOnOutputNodes("EventOut", time, nodesCalledThisFrame);
        }

        public void PlayAtDSPTime(CombineNode node, NodePort calledBy, double time, Dictionary<string, object> data,int nodesCalledThisFrame)
        {
            Dictionary<string, object> outData = new Dictionary<string, object>(data);
            foreach(GraphVariable variable in node.parameters)
            {
                object value = node.GetInputValue(variable.variableID, variable.Value());

                if (outData.ContainsKey(variable.name))
                    outData[variable.name] = value;
                else
                    outData.Add(variable.name, value);
            }
            node.CallFunctionOnOutputNodes("Output", time, outData, nodesCalledThisFrame);
        }

        public override object GetValueOnInitialization()
        {
            return null;
        }

        public List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPort(CombineNode node, NodePort port, List<Node> visitedNodes)
        {
            List<GraphEvent.EventParameterDef> parameters = new List<GraphEvent.EventParameterDef>(node.GetIncomingEventParameterDefsOnPort("Input", visitedNodes));
            foreach (GraphVariable variable in node.parameters)
                parameters.Add(new GraphEvent.EventParameterDef(variable.name, variable.typeName));

            return parameters;
        }
        public override string GetValueInitializationString()
        {
            return "null";
        }
    }
}
