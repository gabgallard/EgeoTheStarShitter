using System;
using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using System.Linq;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class ArrayVariableValue : GraphVariableValue, SplittableValue
    {
        public override Type handlesType => typeof(List<GraphVariable>);



        public override bool CompareValues(Comparison.comparisonOperators comparator, object a, object b)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable is GraphVariable)
            {
                /*GraphVariable castVar = ((GraphVariable)graphVariable);
                System.Array array = System.Array.CreateInstance(ReflectionUtils.FindType(castVar.arrayType), castVar.arrayElements.Count);

                for (int index = 0; index < castVar.arrayElements.Count; index++)
                    array.SetValue(castVar.arrayElements[index].Value(), index);

                return array;*/
                return (graphVariable as GraphVariable).ToGraphArray(GraphVariable.RetrievalTypes.ActualValue);
            }
            return null;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable is GraphVariable)
            {
                /*GraphVariable castVar = ((GraphVariable)graphVariable);
                System.Array array = System.Array.CreateInstance(ReflectionUtils.FindType(castVar.arrayType), castVar.arrayElements.Count);

                for (int index = 0; index < castVar.defaultArrayElements.Count; index++)
                    array.SetValue(castVar.arrayElements[index].Value(), index);

                return array;*/

                return (graphVariable as GraphVariable).ToGraphArray(GraphVariable.RetrievalTypes.DefaultValue);
            }
            return null;
        }

        

        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
        
        }

        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {

        }

        public object GetSplitValue(NodePort targetPort, SplitNode target)
        {
            GraphArrayBase variables = target.GetInputValue<GraphArrayBase>("arrayIn", null);
            int index = target.GetInputValue<int>("index", 1)-1;
            if (targetPort.fieldName == "value")
            {
                if (variables != null && index >= 0 && index < variables.Count)
                    return variables[index];
                return null;
            }
            if (targetPort.fieldName == "length")
            {
                if (variables != null)
                    return variables.Count;
                return 0;
            }
            return null;
        }

        public override object GetValueOnInitialization()
        {
            return new object[0];
        }

        public override string GetValueInitializationString()
        {
            return "null";
        }
    }
}
