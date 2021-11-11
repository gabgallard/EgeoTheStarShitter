using UnityEngine;
using System.Collections.Generic;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Midi;
[ExecuteInEditMode]
[AddComponentMenu("Layers / Soundgraphs / EatingSounds")]
public class EatingSounds: SoundGraphPlayer
{
    public delegate void ChewDelegate( );
    public delegate void ChewDelayedDelegate( System.Double time );
    
    public ChewDelegate onChew = null;
    public ChewDelayedDelegate onChewDelayed = null;
    
    private void OnChewInternal( double time, Dictionary<string, object> data ){
        StartCoroutine(SymphonyUtils.WaitForDSPTime(time, () => {
            onChew?.Invoke();
        }));
        onChewDelayed?.Invoke(time);
    }
    protected override void Awake( ){
        base.Awake();
        RegisterEventListener("Chew", OnChewInternal);
    }
    private readonly string graphAssetID = "3e2ba1d60fd0fb8438b57133ee471e4a";
    protected override void LoadGraph( ){
        #if UNITY_EDITOR
        soundGraph = (SoundGraph)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(graphAssetID), typeof(SoundGraph));
        #endif
    }
    public void Chew( ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("Chew", AudioSettings.dspTime, parameters);
    }
    public void Chew( System.Double startTime ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("Chew", startTime, parameters);
    }
    public void StopAll( ){
        TriggerEvent("EndAll", AudioSettings.dspTime, new Dictionary<string,object>());
    }
    public void StopAll( System.Double time ){
        TriggerEvent("EndAll", time, new Dictionary<string,object>());
    }
}
