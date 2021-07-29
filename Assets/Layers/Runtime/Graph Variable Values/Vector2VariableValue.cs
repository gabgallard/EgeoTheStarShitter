using System;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using UnityEngine;
using System.Collections.Generic;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class Vector2VariableValue : GraphVariableValue, AddableValue, SubtractableValue, SecondaryMultipliableValue, SecondaryDividableValue, SplittableValue, CombinableValue
    {
        public override Type handlesType => typeof(Vector2);


        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.objectValue == null)
                return null;
            return (Vector2)graphVariable.objectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return null;
            return (Vector2)graphVariable.defaultObjectValue;
        }

        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.objectValue = value;
        }

        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.defaultObjectValue = value;
        }

        public override object Deserialize(string serializedObjectValue)
        {
            string[] values = serializedObjectValue.Split('|');
            if (values.Length != 2)
                return null;
            return new Vector2(float.Parse(values[0]), float.Parse(values[1]));
        }

        public override string Serialize(object objectValue)
        {
            if (objectValue != null && !typeof(Vector2).IsAssignableFrom(objectValue.GetType()))
                return "";
            if (objectValue == null)
                return "";
            Vector2 castValue = (Vector2)objectValue;
            return string.Format("{0}|{1}", castValue.x, castValue.y);
        }

        public object GetSplitValue(NodePort targetPort, SplitNode target)
        {
            Vector2 value = target.GetInputValue<Vector2>("Vector2", Vector2.zero);
            if (targetPort.fieldName == "x")
                return value.x;
            if (targetPort.fieldName == "y")
                return value.y;
            return 0f;
        }

        public object GetCombineValue(NodePort targetPort, CombineNode target)
        {
            return new Vector2(
                target.GetInputValue<float>("x", target.GetDefaultValueTyped<float>("x", 0f)),
                target.GetInputValue<float>("y", target.GetDefaultValueTyped<float>("y", 0f))
                );
        }

        public override bool CompareValues(Comparison.comparisonOperators comparator, object a, object b)
        {
            switch (comparator)
            {
                case Comparison.comparisonOperators.Equal:
                    return a == b;
                case Comparison.comparisonOperators.NotEqual:
                    return a != b;
            }

            return false;
        }


        public object Add(object a, object b)
        {
            a = CheckValue<Vector2>(a);
            b = CheckValue<Vector2>(b);
            return ((Vector2)a + (Vector2)b);
        }

        public object Subtract(object a, object b)
        {
            a = CheckValue<Vector2>(a);
            b = CheckValue<Vector2>(b);
            return ((Vector2)a + (Vector2)b);
        }

        public object Multiply(object a, string secondType, object b)
        {
            a = CheckValue<Vector2>(a);
            if (secondType == typeof(Vector2).FullName)
            {

                b = CheckValue<Vector2>(b);
                Vector2 castA = (Vector2)a;
                Vector2 castB = (Vector2)b;
                return new Vector2(castA.x * castB.x, castA.y * castB.y);
            }
            else if (secondType == typeof(int).FullName)
            {
                b = CheckValue<int>(b);

                Vector2 castA = (Vector2)a;
                int castB = (int)b;
                return castA * castB;
            }
            else if (secondType == typeof(float).FullName)
            {
                b = CheckValue<float>(b);
                Vector2 castA = (Vector2)a;
                float castB = (float)b;
                return castA * castB;
            }
            else if (secondType == typeof(double).FullName)
            {
                b = CheckValue<double>(b);
                Vector2 castA = (Vector2)a;
                double castB = (double)b;
                return castA * (float)castB;
            }
            return null;
        }

        public object Divide(object a, string secondType, object b)
        {
            a = CheckValue<Vector2>(a);
            if (secondType == typeof(Vector2).FullName)
            {
                b = CheckValue<Vector2>(b);
                Vector2 castA = (Vector2)a;
                Vector2 castB = (Vector2)b;
                Vector2 result = new Vector2(
                    castB.x == 0 ? 0 : castA.x / castB.x,
                    castB.y == 0 ? 0 : castA.y / castB.y
                );
                return result;
            }
            else if (secondType == typeof(int).FullName)
            {
                b = CheckValue<int>(b);
                Vector2 castA = (Vector2)a;
                int castB = (int)b;
                if (castB == 0)
                    return Vector2.zero;
                return castA / castB;
            }
            else if (secondType == typeof(float).FullName)
            {
                b = CheckValue<float>(b);
                Vector2 castA = (Vector2)a;
                float castB = (float)b;
                if (castB == 0)
                    return Vector2.zero;
                return castA / castB;
            }
            else if (secondType == typeof(double).FullName)
            {
                b = CheckValue<double>(b);
                Vector2 castA = (Vector2)a;
                float castB = (float)(double)b;
                if (castB == 0)
                    return Vector2.zero;
                return castA / castB;
            }
            return null;
        }

        private ExpectedType CheckValue<ExpectedType>(object value)
        {
            ExpectedType finalValue = default(ExpectedType);
            if (value != null && value is ExpectedType)
                finalValue = (ExpectedType)value;
            return finalValue;
        }


        private string[] multiplyTypes = new string[] {
            typeof(Vector2).FullName,
            typeof(float).FullName,
            typeof(int).FullName,
            typeof(double).FullName
        };
        public string[] GetSecondaryMultiplyTypes()
        {
            return multiplyTypes;
        }

        private string[] divideTypes = new string[] {
            typeof(Vector2).FullName,
            typeof(float).FullName,
            typeof(int).FullName,
            typeof(double).FullName
        };
        public string[] GetSecondaryDivideTypes()
        {
            return divideTypes;
        }

        public override object GetValueOnInitialization()
        {
            return Vector2.zero;
        }

        public override string GetValueInitializationString()
        {
            return "Vector2.zero";
        }
    }
}
