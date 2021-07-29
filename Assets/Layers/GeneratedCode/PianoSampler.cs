using UnityEngine;
using System.Collections.Generic;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Midi;
[ExecuteInEditMode]
[AddComponentMenu("Layers / Soundgraphs / Piano Sampler")]
public class PianoSampler: SoundGraphPlayer
{
    public delegate void Event925Delegate( ABXY.Layers.Runtime.MidiData NoteInfo );
    public delegate void Event925DelayedDelegate( System.Double time, ABXY.Layers.Runtime.MidiData NoteInfo );
    
    public Event925Delegate onEvent925 = null;
    public Event925DelayedDelegate onEvent925Delayed = null;
    
    private void OnEvent925Internal( double time, Dictionary<string, object> data ){
        object NoteInfo = MidiData.defaultMidiFlowInfo;
        data.TryGetValue("NoteInfo", out NoteInfo);
        StartCoroutine(SymphonyUtils.WaitForDSPTime(time, () => {
            onEvent925?.Invoke((ABXY.Layers.Runtime.MidiData)NoteInfo);
        }));
        onEvent925Delayed?.Invoke(time, (ABXY.Layers.Runtime.MidiData)NoteInfo);
    }
    protected override void Awake( ){
        base.Awake();
        RegisterEventListener("Event-925", OnEvent925Internal);
    }
    private readonly string graphAssetID = "bef6f8cd8e567ca4c8047920ca80aaf1";
    protected override void LoadGraph( ){
        #if UNITY_EDITOR
        soundGraph = (SoundGraph)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(graphAssetID), typeof(SoundGraph));
        #endif
    }
    public void Event925( System.Double startTime, ABXY.Layers.Runtime.MidiData NoteInfo ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("NoteInfo",NoteInfo);
        TriggerEvent("Event-925", startTime, parameters);
    }
    public void Event925( ABXY.Layers.Runtime.MidiData NoteInfo ){
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("NoteInfo",NoteInfo);
        TriggerEvent("Event-925", AudioSettings.dspTime, parameters);
    }
    public void StopAll( ){
        TriggerEvent("EndAll", AudioSettings.dspTime, new Dictionary<string,object>());
    }
    public void StopAll( System.Double time ){
        TriggerEvent("EndAll", time, new Dictionary<string,object>());
    }
}
