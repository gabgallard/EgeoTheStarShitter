using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values {
    public class ContactPointVariableValue : GraphVariableValue, SplittableValue
    {
        public override Type handlesType => typeof(ContactPoint);

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
            return (ContactPoint)graphVariable.objectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultObjectValue == null)
                return null;

            return (ContactPoint)graphVariable.defaultObjectValue;
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

            ContactPoint contactPoint = target.GetInputValue<ContactPoint>("contactPoint");

            if (targetPort.fieldName == "normal")
                return contactPoint.normal;
            else if (targetPort.fieldName == "otherCollider")
                return contactPoint.otherCollider;
            else if (targetPort.fieldName == "point")
                return contactPoint.point;
            else if (targetPort.fieldName == "separation")
                return contactPoint.separation;
            else if (targetPort.fieldName == "thisCollider")
                return contactPoint.thisCollider;

            return null;
        }

        public override object GetValueOnInitialization()
        {
            return new ContactPoint();
        }

        public override string GetValueInitializationString()
        {
            return "new ContactPoint()";
        }
    }
}