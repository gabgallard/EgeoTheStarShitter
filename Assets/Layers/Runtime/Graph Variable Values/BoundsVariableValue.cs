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
    public class BoundsVariableValue : GraphVariableValue, SplittableValue
    {
        public override Type handlesType => typeof(Bounds);

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
            return (Bounds)graphVariable.objectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return null;
            return (Bounds)graphVariable.defaultObjectValue;
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

            Bounds bounds = target.GetInputValue<Bounds>("bounds");

            if (targetPort.fieldName == "center")
                return bounds.center;
            else if (targetPort.fieldName == "extents")
                return bounds.extents;
            else if (targetPort.fieldName == "size")
                return bounds.size;
            else if (targetPort.fieldName == "min")
                return bounds.min;
            else if (targetPort.fieldName == "max")
                return bounds.max;

            return null;
        }

        public override string Serialize(object objectValue)
        {
            if (objectValue is Bounds)
            {
                Bounds castB = (Bounds)objectValue;
                return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", castB.center.x, castB.center.y, castB.center.z, castB.extents.x, castB.extents.y, castB.extents.z);
            }
            return "";
        }

        public override object Deserialize(string serializedObjectValue)
        {
            string[] splitString = serializedObjectValue.Split('|');
            if (splitString.Length == 6)
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float ex = 0;
                float ey = 0;
                float ez = 0;

                bool xOk = float.TryParse(splitString[0], out x);
                bool yOk = float.TryParse(splitString[1], out y);
                bool zOk = float.TryParse(splitString[2], out z);
                bool exOk = float.TryParse(splitString[3], out ex);
                bool eyOk = float.TryParse(splitString[4], out ey);
                bool ezOk = float.TryParse(splitString[5], out ez);

                if (xOk && yOk && zOk && exOk && eyOk && ezOk)
                    return new Bounds(new Vector3(x, y, z), new Vector3(ex, ey, ez));

            }
            return new Bounds();
        }

        public override object GetValueOnInitialization()
        {
            return new Bounds();
        }

        public override string GetValueInitializationString()
        {
            return "new Bounds()";
        }
    }
}