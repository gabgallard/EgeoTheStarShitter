using System;
using ABXY.Layers.Runtime.Nodes.Logic;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class BoolValue : GraphVariableValue
    {
        public override Type handlesType => typeof(bool);

        public override bool CompareValues(Comparison.comparisonOperators comparator, object a, object b)
        {
            bool castA = (bool)a;
            bool castB = (bool)b;
            if (a == null || b == null)
                return false;

            switch (comparator)
            {
                case Comparison.comparisonOperators.Equal:
                    return castA == castB;
                case Comparison.comparisonOperators.NotEqual:
                    return castA != castB;
            }
            return false;
        }

        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.objectValue == null)
                return false;

            return (bool)graphVariable.objectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return false;

            return (bool)graphVariable.defaultObjectValue;
        }

        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
            if (value == null)
                graphVariable.objectValue = false;
            graphVariable.objectValue = value;
        }

        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {
            if (value == null)
                graphVariable.defaultObjectValue = false;
            graphVariable.defaultObjectValue = value;
        }

        public override string Serialize(object objectValue)
        {
            if (objectValue == null)
                return "";
            return objectValue.ToString();
        }

        public override object Deserialize(string serializedObjectValue)
        {
            return serializedObjectValue == "True";
        }

        public override object GetValueOnInitialization()
        {
            return false;
        }

        public override string GetValueInitializationString()
        {
            return "false";
        }
    }
}
