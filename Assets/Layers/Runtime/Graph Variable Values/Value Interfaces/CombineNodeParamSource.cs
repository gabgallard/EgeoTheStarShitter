using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CombineNodeParamSource
{
    List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPort(CombineNode node, NodePort port, List<Node> visitedNodes);
}
