using System;
using System.Collections;
using System.Collections.Generic;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEngine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values {
    public class CollisionVariableValue : GraphVariableValue, SplittableValue
    {
        public override Type handlesType => typeof(Collision);

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
            return (CollisionVariableValue)graphVariable.objectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return null;
            return (CollisionVariableValue)graphVariable.defaultObjectValue;
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
            Collision input = target.GetInputValue<Collision>("collision");
            if (input == null)
                return null;

            if (targetPort.fieldName == "collider")
                return input.collider;
            else if (targetPort.fieldName == "contactCount")
                return input.contactCount;
            else if (targetPort.fieldName == "contacts")
                return input.contacts;
            else if (targetPort.fieldName == "gameObject")
                return input.gameObject;
            else if (targetPort.fieldName == "impulse")
                return input.impulse;
            else if (targetPort.fieldName == "relativeVelocity")
                return input.relativeVelocity;
            else if (targetPort.fieldName == "rigidbody")
                return input.rigidbody;
            else if (targetPort.fieldName == "transform")
                return input.transform;

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