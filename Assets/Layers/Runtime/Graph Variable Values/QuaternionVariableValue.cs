using System;
using System.Collections;
using System.Collections.Generic;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;
namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class QuaternionVariableValue : GraphVariableValue, SplittableValue
    {
        public override Type handlesType => typeof(Quaternion);

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

        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.objectValue == null)
                return null;

            return (Quaternion)graphVariable.objectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return null;

            return (Quaternion)graphVariable.defaultObjectValue;
        }

        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.objectValue = value;
        }

        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.defaultObjectValue = value;
        }

        public object GetSplitValue(NodePort targetPort, SplitNode target)
        {
            if (targetPort == null)
                return null;

            Quaternion quaternion = target.GetInputValue<Quaternion>("quaternion");

            if (targetPort.fieldName == "eulerAngles")
                return quaternion.eulerAngles;
            else if (targetPort.fieldName == "normalized")
                return quaternion.normalized;
            else if (targetPort.fieldName == "x")
                return quaternion.x;
            else if (targetPort.fieldName == "y")
                return quaternion.y;
            else if (targetPort.fieldName == "z")
                return quaternion.z;
            else if (targetPort.fieldName == "w")
                return quaternion.w;

            return null;
        }

        public override string Serialize(object objectValue)
        {

            if (objectValue == null || !(objectValue is Quaternion))
                objectValue = Quaternion.identity;

            Quaternion quaternion = (Quaternion)objectValue;

            return string.Format("{0}|{1}|{2}|{3}", quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        public override object Deserialize(string serializedObjectValue)
        {
            if (string.IsNullOrEmpty(serializedObjectValue))
                return Quaternion.identity;

            string[] uncastElements = serializedObjectValue.Split('|');

            if (uncastElements.Length != 4)
                return Quaternion.identity;

            float x = 0;
            float y = 0;
            float z = 0;
            float w = 0;

            float.TryParse(uncastElements[0], out x);
            float.TryParse(uncastElements[1], out y);
            float.TryParse(uncastElements[2], out z);
            float.TryParse(uncastElements[3], out w);

            return new Quaternion(x,y,z,w);
        }

        public override object GetValueOnInitialization()
        {
            return Quaternion.identity;
        }

        public override string GetValueInitializationString()
        {
            return "Quaternion.identity";
        }
    }
}