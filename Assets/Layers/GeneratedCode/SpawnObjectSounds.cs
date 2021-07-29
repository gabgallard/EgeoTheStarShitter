using UnityEngine;
using System.Collections.Generic;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Midi;
[ExecuteInEditMode]
[AddComponentMenu("Layers / Soundgraphs / SpawnObjectSounds")]
public class SpawnObjectSounds: SoundGraphPlayer
{
    private readonly string graphAssetID = "2c0a24d6de1409a4c8bee146a1dd8385";
    protected override void LoadGraph( ){
        #if UNITY_EDITOR
        soundGraph = (SoundGraph)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(graphAssetID), typeof(SoundGraph));
        #endif
    }
    public void StopAll( ){
        TriggerEvent("EndAll", AudioSettings.dspTime, new Dictionary<string,object>());
    }
    public void StopAll( System.Double time ){
        TriggerEvent("EndAll", time, new Dictionary<string,object>());
    }
}
