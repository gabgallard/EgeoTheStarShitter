using ABXY.Layers.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface InputNodeInspector
{
    void DrawInputNodeValue(Rect position, string label, VariableEdit variable);

    float CalculateInputNodeValueHeight(VariableEdit variable, string label);

    object GetDefaultValue();

    bool descendsFromUnityObject { get; }
}
