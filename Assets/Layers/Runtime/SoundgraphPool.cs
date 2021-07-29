using ABXY.Layers.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundgraphPool
{
    private static Dictionary<string, List<SoundGraph>> instanceDB = new Dictionary<string, List<SoundGraph>>();

    /// <summary>
    /// Takes in an asset version of a graph, and returns an instance from the pool
    /// </summary>
    /// <param name="graphAsset"></param>
    /// <returns></returns>
    public static SoundGraph GetInstance( SoundGraph graphAsset, SoundGraphPlayer owningMono)
    {
        if (graphAsset == null)
            return null;

        string targetID = graphAsset.graphID;

        if (!instanceDB.ContainsKey(targetID))
            instanceDB.Add(targetID, new List<SoundGraph>());

        SoundGraph runtimeGraphCopy = null;

        if (instanceDB[targetID].Count == 0)
        {
            runtimeGraphCopy = (SoundGraph)graphAsset.RuntimeCopy();
            //soundGraphCopies.Add(runtimeGraphCopy.graphID, runtimeGraphCopy);
        }
        else
        {
            runtimeGraphCopy = instanceDB[targetID].First();
            instanceDB[targetID].RemoveAt(0);
        }

        runtimeGraphCopy.owningMono = owningMono;
        runtimeGraphCopy.ResetVariablesToDefaults();

        runtimeGraphCopy.isCurrentlyInPool = false;
        return runtimeGraphCopy;
    }

    public static void ReturnSoundGraph(SoundGraph instance)
    {
        if (instance == null)
            return;

        instance.CallEvent("EndAll", AudioSettings.dspTime, new Dictionary<string, object>(), 0);


        string targetID = instance.graphID;

        if (!instanceDB.ContainsKey(targetID))
            instanceDB.Add(targetID, new List<SoundGraph>());

        instance.isCurrentlyInPool = true;

        instanceDB[targetID].Add(instance);

    }
}
