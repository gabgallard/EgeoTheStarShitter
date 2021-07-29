using ABXY.Layers.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface PlayerInspector
{

    

    void DrawInPlayerInspector(Rect position, string label, VariableEdit variable);

    float CalculateHeightInPlayerInspector(VariableEdit variable, string label);

    object GetDefaultValue();

    bool descendsFromUnityObject { get; }
}
