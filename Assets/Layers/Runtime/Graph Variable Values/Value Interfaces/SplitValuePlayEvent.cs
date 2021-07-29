using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SplitValuePlayEvent
{
    void PlayAtDSPTime(SplitNode node, NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame );
}
