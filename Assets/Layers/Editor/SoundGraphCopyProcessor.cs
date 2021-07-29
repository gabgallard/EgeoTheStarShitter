//C#
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ABXY.Layers.Runtime;
class DetectDuplicatesPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var str in importedAssets)
        {
            SoundGraph thisGraph = AssetDatabase.LoadAssetAtPath(str, typeof(SoundGraph)) as SoundGraph;
            if (thisGraph != null)
            {
                string guid = AssetDatabase.AssetPathToGUID(str);

                
                typeof(SoundGraph).GetMethod("SetID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(thisGraph, new object[] { guid });
                

            }
        }

    }
}