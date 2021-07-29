using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SplittableInspector
{
    List<PortDefinition> GetSplitPorts(CombineSplitData data);

    void DrawSplitGUI(CombineSplitData data);

    int GetSplitNodeWidth();
}
