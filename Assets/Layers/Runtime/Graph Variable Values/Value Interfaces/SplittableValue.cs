using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SplittableValue
{
    object GetSplitValue(NodePort targetPort, SplitNode target);
}
