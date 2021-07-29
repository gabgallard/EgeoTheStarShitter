using System;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using UnityEngine;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public class TransformVariableValue : GraphVariableValue, SplittableValue
    {
        public override Type handlesType => typeof(Transform);

        public override object GetValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.unityObjectValue == null)
                return null;

            if (graphVariable.unityObjectValue is Transform)
                return (Transform)graphVariable.unityObjectValue;
            return null;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultUnityObjectValue == null)
                return null;

            if (graphVariable.defaultUnityObjectValue is Transform)
                return (Transform)graphVariable.defaultUnityObjectValue;
            return null;
        }

        public override void SetValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.unityObjectValue = (UnityEngine.Object)value;

        }

        public override void SetDefaultValue(GraphVariableBase graphVariable, object value)
        {
            graphVariable.defaultUnityObjectValue = (UnityEngine.Object)value;
        }

        public object GetSplitValue(NodePort targetPort, SplitNode target)
        {
            Transform transform = target.GetInputValue<Transform>("transform");

            if (targetPort.fieldName == "Name")
            {
                return transform == null ? "" : transform.name;
            }
            else if (targetPort.fieldName == "Position")
            {
                return transform == null ? Vector3.zero : transform.position;
            }
            else if (targetPort.fieldName == "LocalPosition")
            {
                return transform == null ? Vector3.zero : transform.localPosition;
            }
            else if (targetPort.fieldName == "Scale")
            {
                return transform == null ? Vector3.one : transform.lossyScale;
            }
            else if (targetPort.fieldName == "LocalScale")
            {
                return transform == null ? Vector3.one : transform.localScale;
            }
            else if (targetPort.fieldName == "Rotation")
            {
                return transform == null ? Quaternion.identity : transform.rotation;
            }
            else if (targetPort.fieldName == "LocalRotation")
            {
                return transform == null ? Quaternion.identity : transform.localRotation;
            }
            else if (targetPort.fieldName == "Parent")
            {
                return transform == null ? transform : transform.parent;
            }
            return null;
        }

        public override Color GetDefaultColor()
        {
            return new Color32(166, 145, 125, 255);
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
