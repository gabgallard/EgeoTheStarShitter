
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Playback;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersAnalyzerEvent
{
    public System.Guid eventID;
    public string playerName;
    public SoundGraphPlayer playerObject;
    public System.Guid playerID;

    public string soundGraphName;
    public SoundGraph soundGraphObject;
    public string soundGraphID;

    public string nodeName;
    public System.Guid nodeID;

    public double time;
    public string audioOutNodeName;

    public string prettyName;

    public bool wasLate = false;

    public enum LayersEventTypes { AudioScheduled, AudioStarted, AudioFinished,AudioEnded, GraphEvent, EndAll, NodeEntered, NodeExited, SoundGraphCreated,SoundGraphDestroyed, SoundGraphPlayerCreated, SoundGraphPlayerDestroyed}

    public LayersEventTypes eventType = LayersEventTypes.AudioStarted;

    public LayersAnalyzerEvent(System.Guid eventID, SoundGraph soundGraphObject, FlowNode targetFlowNode, AudioOut audioOut, double time, LayersEventTypes eventType, string prettyName, bool wasLate = false)
    {

        this.nodeName = targetFlowNode != null ? targetFlowNode.name : "";
        nodeID = targetFlowNode != null ? targetFlowNode.nodeID : System.Guid.Empty;

        this.soundGraphObject = soundGraphObject;
        this.soundGraphName = soundGraphObject != null ? soundGraphObject.name : "";
        this.soundGraphID = soundGraphObject != null ? soundGraphObject.graphID : System.Guid.Empty.ToString();

        this.playerObject = soundGraphObject != null ? soundGraphObject.owningMono : null;
        this.playerName = playerObject != null ? playerObject.name : "Playing in preview";
        this.playerID = playerObject != null ? playerObject.playerID : System.Guid.Empty;

        this.audioOutNodeName = audioOut != null ? audioOut.name : "No Audio Out";

        this.eventID = eventID;
        this.time = time;
        this.eventType = eventType;
        this.prettyName = prettyName;
        this.wasLate = wasLate;
    }


}
