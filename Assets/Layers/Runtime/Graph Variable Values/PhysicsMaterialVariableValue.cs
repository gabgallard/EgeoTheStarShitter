
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
    public class PhysicsMaterialVariableValue : GraphVariableValue, SplittableValue
    {
        public PhysicsMaterialVariableValue()
        {
        }

        public override Type handlesType => typeof(PhysicMaterial);

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
            return (PhysicMaterial)graphVariable.unityObjectValue;
        }

        public override object GetDefaultValue(GraphVariableBase graphVariable)
        {
            if (graphVariable.defaultUnityObjectValue == null)
                return null;
            return (PhysicMaterial)graphVariable.defaultUnityObjectValue;
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

            PhysicMaterial material = target.GetInputValue<PhysicMaterial>("physicsMaterial");

            if (material == null)
                return null;

            if (targetPort.fieldName == "name")
                return material.name;
            else if (targetPort.fieldName == "bounciness")
                return material.bounciness;
            else if (targetPort.fieldName == "dynamicFriction")
                return material.dynamicFriction;
            else if (targetPort.fieldName == "staticFriction")
                return material.staticFriction;

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