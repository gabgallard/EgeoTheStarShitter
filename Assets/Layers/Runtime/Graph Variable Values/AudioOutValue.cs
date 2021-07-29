using System;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes.Logic;
using UnityEngine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class AudioOutValue : GraphVariableValue
    {
        public override Type handlesType => typeof(AudioFlow);

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
            return new Color32(0, 95, 158, 255);
        }
        public override bool IsNonreferenceableType()
        {
            return true;
        }

        public override object GetValueOnInitialization()
        {
            return null;
        }

        public override string GetValueInitializationString()
        {
            return "null";
        }
    }
}
