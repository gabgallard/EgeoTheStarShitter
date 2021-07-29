using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CombineValuePlayEvent
{
    void PlayAtDSPTime(CombineNode node, NodePort calledBy, double time, Dictionary<string, object> data,int nodesCalledThisFrame);
}
