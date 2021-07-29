using ABXY.Layers.Editor.Node_Editors.Variables.Combine_and_Split;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CombinableInspector
{
    List<PortDefinition> GetCombinePorts(CombineSplitData data);


    void DrawCombineGUI(CombineSplitData data);

    int GetCombineNodeWidth();
}
