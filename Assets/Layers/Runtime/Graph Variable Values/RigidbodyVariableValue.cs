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
    public class RigidbodyVariableValue : GraphVariableValue, SplittableValue
    {
        public override Type handlesType => typeof(Rigidbody);

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
            if (graphVariable.unityObjectValue == null)
                return null;
            return (Rigidbody)graphVariable.unityObjectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultUnityObjectValue == null)
                return null;
            return (Rigidbody)graphVariable.defaultUnityObjectValue;
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
            if (targetPort == null)
                return null;

            Rigidbody rb = target.GetInputValue<Rigidbody>("rigidbody");
            if (rb == null)
                return null;

            if (targetPort.fieldName == "angularDrag")
                return rb.angularDrag;
            else if (targetPort.fieldName == "angularVelocity")
                return rb.angularVelocity;
            else if (targetPort.fieldName == "centerOfMass")
                return rb.centerOfMass;
            else if (targetPort.fieldName == "mass")
                return rb.mass;
            else if (targetPort.fieldName == "position")
                return rb.position;
            else if (targetPort.fieldName == "rotation")
                return rb.rotation;
            else if (targetPort.fieldName == "velocity")
                return rb.velocity;
            else if (targetPort.fieldName == "drag")
                return rb.drag;

            return null;
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