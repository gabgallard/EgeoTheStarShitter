using UnityEngine;
using System.Collections.Generic;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Midi;
[ExecuteInEditMode]
[AddComponentMenu("Layers / Soundgraphs / SingleObjectSounds")]
public class SingleObjectSounds: SoundGraphPlayer
{
    public delegate void ClickDelegate( );
    public delegate void ClickDelayedDelegate( System.Double time );
    
    public ClickDelegate onClick = null;
    public ClickDelayedDelegate onClickDelayed = null;
    
    public delegate void SpawnDelegate( );
    public delegate void SpawnDelayedDelegate( System.Double time );
    
    public SpawnDelegate onSpawn = null;
    public SpawnDelayedDelegate onSpawnDelayed = null;

    public static SingleObjectSounds instance;


    private void OnClickInternal( double time, Dictionary<string, object> data ){
        StartCoroutine(SymphonyUtils.WaitForDSPTime(time, () => {
            onClick?.Invoke();
        }));
        onClickDelayed?.Invoke(time);
    }
    private void OnSpawnInternal( double time, Dictionary<string, object> data ){
        StartCoroutine(SymphonyUtils.WaitForDSPTime(time, () => {
            onSpawn?.Invoke();
        }));
        onSpawnDelayed?.Invoke(time);
    }
    protected override void Awake( ){
        instance = this;
        base.Awake();
        RegisterEventListener("Click", OnClickInternal);
        RegisterEventListener("Spawn", OnSpawnInternal);
        
    }
    private readonly string graphAssetID = "335a543dbae21444caae2f479095908d";
    protected override void LoadGraph( ){
        #if UNITY_EDITOR
        soundGraph = (SoundGraph)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(graphAssetID), typeof(SoundGraph));
        #endif
    }
    public System.String TypeOfObject{
        get {
            return (System.String)GetVariable("TypeOfObject");
        }
        set {
            SetVariable("TypeOfObject",value);
        }
    }
    public UnityEngine.Transform Location{
        get {
            return (UnityEngine.Transform)GetVariable("Location");
        }
        set {
            SetVariable("Location",value);
        }
    }
    public void Click( ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("Click", AudioSettings.dspTime, parameters);
    }
    public void Click( System.Double startTime ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("Click", startTime, parameters);
    }
    public void Spawn( ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("Spawn", AudioSettings.dspTime, parameters);
    }
    public void Spawn( System.Double startTime ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("Spawn", startTime, parameters);
    }
    public void StopAll( ){
        TriggerEvent("EndAll", AudioSettings.dspTime, new Dictionary<string,object>());
    }
    public void StopAll( System.Double time ){
        TriggerEvent("EndAll", time, new Dictionary<string,object>());
    }
}
