using System;
using ABXY.Layers.Runtime.Nodes.Logic;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class IntVariableValue : GraphVariableValue, AddableValue,SubtractableValue,MultipliableValue,DividableValue
    {
        public override Type handlesType => typeof(int);

        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.objectValue == null)
                return null;
            return (int)graphVariable.objectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return null;
            return (int)graphVariable.defaultObjectValue;
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
            int value = 0;
            int.TryParse(serializedObjectValue, out value);
            return value;
        }


        public override bool CompareValues(Comparison.comparisonOperators comparator, object a, object b)
        {
            if (a == null || b == null || !IsNumericType(a) || !IsNumericType(b))
                return false;
            int aCast = (int)System.Convert.ChangeType(a, typeof(int));
            int bCast = (int)System.Convert.ChangeType(b, typeof(int));
            switch (comparator)
            {
                case Comparison.comparisonOperators.Equal:
                    return aCast == bCast;
                case Comparison.comparisonOperators.NotEqual:
                    return aCast != bCast;
                case Comparison.comparisonOperators.LessThan:
                    return aCast < bCast;
                case Comparison.comparisonOperators.GreaterThan:
                    return aCast > bCast;
                case Comparison.comparisonOperators.LessThanOrEqualTo:
                    return aCast <= bCast;
                case Comparison.comparisonOperators.GreaterThanOrEqualTo:
                    return aCast >= bCast;
            }

            return false;
        }


        public object Add(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            return ((int)a + (int)b);
        }

        public object Subtract(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            return ((int)a - (int)b);
        }

        public object Multiply(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            return ((int)a * (int)b);
        }

        public object Divide(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            if (((int)a) == 0)
                return 0;
            return ((int)a / (int)b);
        }

        private int CheckValue(object value)
        {
            int finalValue = 0;
            if (value != null && value is int)
                finalValue = (int)value;
            return finalValue;
        }

        public override object GetValueOnInitialization()
        {
            return 0;
        }

        public override string GetValueInitializationString()
        {
            return "0";
        }
    }
}
