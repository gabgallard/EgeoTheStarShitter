using System;
using ABXY.Layers.Runtime.Nodes.Logic;
using UnityEngine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class FloatVariableValue : GraphVariableValue, AddableValue, SubtractableValue, MultipliableValue, DividableValue
    {
        public override Type handlesType => typeof(float);

        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.objectValue == null)
                return null;
            return (float)graphVariable.objectValue;
        }


        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return null;
            return (float)graphVariable.defaultObjectValue;
        }

        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.objectValue = (float)value;
        }

        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.defaultObjectValue = (float)value;
        }

        public override object Deserialize(string serializedObjectValue)
        {
            float result = 0;
            float.TryParse(serializedObjectValue, out result);
            return result;
        }

        public override string Serialize(object objectValue)
        {
            if (objectValue == null)
                return "0";
            return objectValue.ToString();
        }

        public override Color GetDefaultColor()
        {
            return new Color32(158, 104, 87, 255);
        }

        public override Color GetDefaultColorPro()
        {
            return new Color32(168, 48, 10, 255);
        }
        public override bool CompareValues(Comparison.comparisonOperators comparator, object a, object b)
        {
            if (a == null || b == null || !IsNumericType(a) || !IsNumericType(b))
                return false;
            float aCast = (float)System.Convert.ChangeType(a, typeof(float));
            float bCast = (float)System.Convert.ChangeType(b, typeof(float));
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
            return ((float)a + (float)b);
        }

        public object Subtract(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            return ((float)a - (float)b);
        }

        public object Multiply(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            return ((float)a * (float)b);
        }

        public object Divide(object a, object b)
        {
            a = CheckValue(a);
            b = CheckValue(b);
            if (((float)a) == 0)
                return 0;
            return ((float)a / (float)b);
        }

        private float CheckValue(object value)
        {
            float finalValue = 0f;
            if (value != null && value is float)
                finalValue = (float)value;
            return finalValue;
        }

        public override object GetValueOnInitialization()
        {
            return 0f;
        }

        public override string GetValueInitializationString()
        {
            return "0f";
        }
    }
}
