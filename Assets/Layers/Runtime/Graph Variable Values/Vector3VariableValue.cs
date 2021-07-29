using System;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using UnityEngine;
using System.Collections.Generic;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class Vector3VariableValue : GraphVariableValue, AddableValue, SubtractableValue, SecondaryMultipliableValue, SecondaryDividableValue, SplittableValue, CombinableValue
    {
        public override Type handlesType => typeof(Vector3);


        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.objectValue == null)
                return null;
            return (Vector3)graphVariable.objectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return null;
            return (Vector3)graphVariable.defaultObjectValue;
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
            if (values.Length != 3)
                return null;
            return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }

        public override string Serialize(object objectValue)
        {
            if (objectValue != null && !typeof(Vector3).IsAssignableFrom(objectValue.GetType()))
                return "";
            if (objectValue == null)
                return "";
            Vector3 castValue = (Vector3)objectValue;
            return string.Format("{0}|{1}|{2}", castValue.x, castValue.y, castValue.z);
        }

        public object GetSplitValue(NodePort targetPort, SplitNode target)
        {
            Vector3 value = target.GetInputValue<Vector3>("vector3", Vector3.zero);
            if (targetPort.fieldName == "x")
                return value.x;
            if (targetPort.fieldName == "y")
                return value.y;
            if (targetPort.fieldName == "z")
                return value.z;
            return 0f;
        }

        public object GetCombineValue(NodePort targetPort, CombineNode target)
        {
            return new Vector3(
                target.GetInputValue<float>("x", target.GetDefaultValueTyped<float>("x", 0f)),
                target.GetInputValue<float>("y", target.GetDefaultValueTyped<float>("y", 0f)),
                target.GetInputValue<float>("z", target.GetDefaultValueTyped<float>("z", 0f)));
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
            a = CheckValue<Vector3>(a);
            b = CheckValue<Vector3>(b);
            return ((Vector3)a + (Vector3)b);
        }

        public object Subtract(object a, object b)
        {
            a = CheckValue<Vector3>(a);
            b = CheckValue<Vector3>(b);
            return ((Vector3)a + (Vector3)b);
        }

        public object Multiply(object a, string secondType, object b)
        {
            a = CheckValue<Vector3>(a);
            if (secondType == typeof(Vector3).FullName)
            {

                b = CheckValue<Vector3>(b);
                Vector3 castA = (Vector3)a;
                Vector3 castB = (Vector3)b;
                return new Vector3(castA.x * castB.x, castA.y * castB.y, castA.z * castB.z);
            }
            else if (secondType == typeof(int).FullName)
            {
                b = CheckValue<int>(b);

                Vector3 castA = (Vector3)a;
                int castB = (int)b;
                return castA * castB;
            }
            else if (secondType == typeof(float).FullName)
            {
                b = CheckValue<float>(b);
                Vector3 castA = (Vector3)a;
                float castB = (float)b;
                return castA * castB;
            }
            else if (secondType == typeof(double).FullName)
            {
                b = CheckValue<double>(b);
                Vector3 castA = (Vector3)a;
                double castB = (double)b;
                return castA * (float)castB;
            }
            return null;
        }

        public object Divide(object a, string secondType, object b)
        {
            a = CheckValue<Vector3>(a);
            if (secondType == typeof(Vector3).FullName)
            {
                b = CheckValue<Vector3>(b);
                Vector3 castA = (Vector3)a;
                Vector3 castB = (Vector3)b;
                Vector3 result = new Vector3(
                    castB.x == 0 ? 0 : castA.x / castB.x,
                    castB.y == 0 ? 0 : castA.y / castB.y,
                    castB.z == 0 ? 0 : castA.z / castB.z
                );
                return result;
            }
            else if (secondType == typeof(int).FullName)
            {
                b = CheckValue<int>(b);
                Vector3 castA = (Vector3)a;
                int castB = (int)b;
                if (castB == 0)
                    return Vector3.zero;
                return castA / castB;
            }
            else if (secondType == typeof(float).FullName)
            {
                b = CheckValue<float>(b);
                Vector3 castA = (Vector3)a;
                float castB = (float)b;
                if (castB == 0)
                    return Vector3.zero;
                return castA / castB;
            }
            else if (secondType == typeof(double).FullName)
            {
                b = CheckValue<double>(b);
                Vector3 castA = (Vector3)a;
                float castB = (float)(double)b;
                if (castB == 0)
                    return Vector3.zero;
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
            typeof(Vector3).FullName,
            typeof(float).FullName,
            typeof(int).FullName,
            typeof(double).FullName
        };
        public string[] GetSecondaryMultiplyTypes()
        {
            return multiplyTypes;
        }

        private string[] divideTypes = new string[] {
            typeof(Vector3).FullName,
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
            return Vector3.zero;
        }

        public override string GetValueInitializationString()
        {
            return "Vector3.zero";
        }
    }
}
