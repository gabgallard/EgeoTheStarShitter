using UnityEngine;
using System.Collections.Generic;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Midi;
[ExecuteInEditMode]
[AddComponentMenu("Layers / Soundgraphs / PlanetSounds")]
public class PlanetSounds: SoundGraphPlayer
{
    public delegate void ClickPlanetDelegate( );
    public delegate void ClickPlanetDelayedDelegate( System.Double time );
    
    public ClickPlanetDelegate onClickPlanet = null;
    public ClickPlanetDelayedDelegate onClickPlanetDelayed = null;
    
    public delegate void SpawnPlanetDelegate( );
    public delegate void SpawnPlanetDelayedDelegate( System.Double time );
    
    public SpawnPlanetDelegate onSpawnPlanet = null;
    public SpawnPlanetDelayedDelegate onSpawnPlanetDelayed = null;
    
    private void OnClickPlanetInternal( double time, Dictionary<string, object> data ){
        StartCoroutine(SymphonyUtils.WaitForDSPTime(time, () => {
            onClickPlanet?.Invoke();
        }));
        onClickPlanetDelayed?.Invoke(time);
    }
    private void OnSpawnPlanetInternal( double time, Dictionary<string, object> data ){
        StartCoroutine(SymphonyUtils.WaitForDSPTime(time, () => {
            onSpawnPlanet?.Invoke();
        }));
        onSpawnPlanetDelayed?.Invoke(time);
    }
    protected override void Awake( ){
        base.Awake();
        RegisterEventListener("ClickPlanet", OnClickPlanetInternal);
        RegisterEventListener("SpawnPlanet", OnSpawnPlanetInternal);
    }
    private readonly string graphAssetID = "335a543dbae21444caae2f479095908d";
    protected override void LoadGraph( ){
        #if UNITY_EDITOR
        soundGraph = (SoundGraph)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(graphAssetID), typeof(SoundGraph));
        #endif
    }
    public void ClickPlanet( ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("ClickPlanet", AudioSettings.dspTime, parameters);
    }
    public void ClickPlanet( System.Double startTime ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("ClickPlanet", startTime, parameters);
    }
    public void SpawnPlanet( ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("SpawnPlanet", AudioSettings.dspTime, parameters);
    }
    public void SpawnPlanet( System.Double startTime ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("SpawnPlanet", startTime, parameters);
    }
    public void StopAll( ){
        TriggerEvent("EndAll", AudioSettings.dspTime, new Dictionary<string,object>());
    }
    public void StopAll( System.Double time ){
        TriggerEvent("EndAll", time, new Dictionary<string,object>());
    }
}
