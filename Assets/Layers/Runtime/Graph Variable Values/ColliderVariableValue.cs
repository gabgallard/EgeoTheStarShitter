using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderVariableValue : GraphVariableValue, SplittableValue
{
    public override Type handlesType => typeof(Collider);

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
        return (Collider)graphVariable.unityObjectValue;
    }

    public override object GetDefaultValue(GraphVariableBase graphVariable)
    {
        if (graphVariable.defaultUnityObjectValue == null)
            return null;
        return (Collider)graphVariable.defaultUnityObjectValue;
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

        Collider collider = target.GetInputValue<Collider>("collider");
        if (collider == null)
            return null;

        if (targetPort.fieldName == "attachedRigidbody")
            return collider.attachedRigidbody;
        else if (targetPort.fieldName == "bounds")
            return collider.bounds;
        else if (targetPort.fieldName == "contactOffset")
            return collider.contactOffset;
        else if (targetPort.fieldName == "enabled")
            return collider.enabled;
        else if (targetPort.fieldName == "isTrigger")
            return collider.isTrigger;
        else if (targetPort.fieldName == "material")
            return collider.material;
        else if (targetPort.fieldName == "sharedMaterial")
            return collider.sharedMaterial;

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
