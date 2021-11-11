using UnityEngine;
using System.Collections.Generic;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Midi;
[ExecuteInEditMode]
[AddComponentMenu("Layers / Soundgraphs / BackgroundLoop")]
public class BackgroundLoop: SoundGraphPlayer
{
    public delegate void LoopStartDelegate( );
    public delegate void LoopStartDelayedDelegate( System.Double time );
    
    public LoopStartDelegate onLoopStart = null;
    public LoopStartDelayedDelegate onLoopStartDelayed = null;
    
    private void OnLoopStartInternal( double time, Dictionary<string, object> data ){
        StartCoroutine(SymphonyUtils.WaitForDSPTime(time, () => {
            onLoopStart?.Invoke();
        }));
        onLoopStartDelayed?.Invoke(time);
    }
    protected override void Awake( ){
        base.Awake();
        RegisterEventListener("LoopStart", OnLoopStartInternal);
    }
    private readonly string graphAssetID = "f4a8e77622abe5e4abe43c193b3e2ab8";
    protected override void LoadGraph( ){
        #if UNITY_EDITOR
        soundGraph = (SoundGraph)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(graphAssetID), typeof(SoundGraph));
        #endif
    }
    public void LoopStart( ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("LoopStart", AudioSettings.dspTime, parameters);
    }
    public void LoopStart( System.Double startTime ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("LoopStart", startTime, parameters);
    }
    public void StopAll( ){
        TriggerEvent("EndAll", AudioSettings.dspTime, new Dictionary<string,object>());
    }
    public void StopAll( System.Double time ){
        TriggerEvent("EndAll", time, new Dictionary<string,object>());
    }
}
