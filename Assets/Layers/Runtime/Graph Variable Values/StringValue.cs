using System;
using ABXY.Layers.Runtime.Nodes.Logic;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class StringValue : GraphVariableValue
    {
        public override Type handlesType => typeof(string);

        public override bool CompareValues(Comparison.comparisonOperators comparator, object a, object b)
        {
            string castA = (string)a;
            string castB = (string)b;
            if (a == null || b == null || castA == null || castB == null)
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
                return null;
            return graphVariable.objectValue.ToString();
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return null;
            return graphVariable.defaultObjectValue.ToString();
        }

        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.objectValue = value;
        }

        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {
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
            return serializedObjectValue;
        }

        public override object GetValueOnInitialization()
        {
            return "";
        }

        public override string GetValueInitializationString()
        {
            return "\"\"";
        }
    }
}
