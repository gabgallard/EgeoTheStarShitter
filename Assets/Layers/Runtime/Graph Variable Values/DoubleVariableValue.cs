using System;
using ABXY.Layers.Runtime.Nodes.Logic;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class DoubleVariableValue : GraphVariableValue, AddableValue, SubtractableValue, DividableValue,MultipliableValue
    {
        public override Type handlesType => typeof(double);

        public override bool CompareValues(Comparison.comparisonOperators comparator, object a, object b)
        {
            if (a == null || b == null || !IsNumericType(a) || !IsNumericType(b))
                return false;
            double aCast = (double)System.Convert.ChangeType(a, typeof(double));
            double bCast = (double)System.Convert.ChangeType(b, typeof(double));
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

        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.objectValue == null)
                return null;

            if (IsNumericType(graphVariable.objectValue))
                return (double)graphVariable.objectValue;
            return 0.0;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return null;

            if (IsNumericType(graphVariable.defaultObjectValue))
                return (double)graphVariable.defaultObjectValue;
            return 0.0;
        }

        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
            if (IsNumericType(value))
                graphVariable.objectValue = (double)value;
        }

        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {
            if (IsNumericType(value))
                graphVariable.defaultObjectValue = (double)value;
        }

        public override string Serialize(object objectValue)
        {
            if (IsNumericType(objectValue))
                return objectValue.ToString();
            return "0";
        }

        public override object Deserialize(string serializedObjectValue)
        {
            double result = 0;
            double.TryParse(serializedObjectValue, out result);
            return result;
        }

        public object Add(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            return ((double)a + (double)b);
        }

        public object Subtract(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            return ((double)a - (double)b);
        }

        public object Multiply(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            return ((double)a * (double)b);
        }

        public object Divide(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            if (((double)a) == 0)
                return 0;
            return ((double)a / (double)b);
        }

        private double CheckValue(object value)
        {
            double finalValue = 0;
            if (value != null && value is double)
                finalValue = (double)value;
            return finalValue;
        }

        public override object GetValueOnInitialization()
        {
            return 0d;
        }

        public override string GetValueInitializationString()
        {
            return "0";
        }
    }
}
