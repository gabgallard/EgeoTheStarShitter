using UnityEngine;
using System.Collections.Generic;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Midi;
[ExecuteInEditMode]
[AddComponentMenu("Layers / Soundgraphs / CollisionSounds")]
public class CollisionSounds: SoundGraphPlayer
{
    public delegate void CollisionDelegate( );
    public delegate void CollisionDelayedDelegate( System.Double time );
    
    public CollisionDelegate onCollision = null;
    public CollisionDelayedDelegate onCollisionDelayed = null;
    
    private void OnCollisionInternal( double time, Dictionary<string, object> data ){
        StartCoroutine(SymphonyUtils.WaitForDSPTime(time, () => {
            onCollision?.Invoke();
        }));
        onCollisionDelayed?.Invoke(time);
    }
    protected override void Awake( ){
        base.Awake();
        RegisterEventListener("Collision", OnCollisionInternal);
    }
    private readonly string graphAssetID = "2c0a24d6de1409a4c8bee146a1dd8385";
    protected override void LoadGraph( ){
        #if UNITY_EDITOR
        soundGraph = (SoundGraph)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(graphAssetID), typeof(SoundGraph));
        #endif
    }
    public UnityEngine.Transform Location{
        get {
            return (UnityEngine.Transform)GetVariable("Location");
        }
        set {
            SetVariable("Location",value);
        }
    }
    public System.String TypeOfObject{
        get {
            return (System.String)GetVariable("TypeOfObject");
        }
        set {
            SetVariable("TypeOfObject",value);
        }
    }
    public System.Single CollisionSpeed{
        get {
            return (System.Single)GetVariable("CollisionSpeed");
        }
        set {
            SetVariable("CollisionSpeed",value);
        }
    }
    public void Collision( ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("Collision", AudioSettings.dspTime, parameters);
    }
    public void Collision( System.Double startTime ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        TriggerEvent("Collision", startTime, parameters);
    }
    public void StopAll( ){
        TriggerEvent("EndAll", AudioSettings.dspTime, new Dictionary<string,object>());
    }
    public void StopAll( System.Double time ){
        TriggerEvent("EndAll", time, new Dictionary<string,object>());
    }
}
