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
    public static SoundGraph GetInstance(SoundGraph graphAsset, SoundGraphPlayer owningMono)
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

            if (runtimeGraphCopy == null)
            { // then there are nulls in the system,  need to truncate
                TruncateNulls();
                // going to rerun the process from scratch
                return GetInstance(graphAsset, owningMono);
            }
            instanceDB[targetID].RemoveAt(0);
        }

        runtimeGraphCopy.owningMono = owningMono;
        runtimeGraphCopy.ResetVariablesToDefaults();

        runtimeGraphCopy.isCurrentlyInPool = false;
        return runtimeGraphCopy;
    }

    /// <summary>
    /// If nulls are encountered, run this to remove nulls
    /// </summary>
    private static void TruncateNulls()
    {
        List<string> keysToRemove = new List<string>();
        foreach (var KV in instanceDB)
        {
            if (KV.Value == null) // then list is null
                keysToRemove.Add(KV.Key);
            else // then going to go through list and look for nulls
            {
                for (int index = 0; index < KV.Value.Count; index++)
                {
                    int removeCount = KV.Value.RemoveAll(x => x == null || (x != null && x.Equals(null)));

                    index += removeCount;
                }
            }

        }

        foreach (string removedKey in keysToRemove)
        {
            instanceDB.Remove(removedKey);
        }
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
