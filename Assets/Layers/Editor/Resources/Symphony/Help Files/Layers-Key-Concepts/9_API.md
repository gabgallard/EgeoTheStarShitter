---
typora-root-url:../
---

# API

## About

The primary way of interacting with Layers via code are through the generated Player Component API's. In short, player components are responsible for executing Sound Graphs in a scene. There are two kinds, Sound Graph Player Components and Pooled Sound Graph Player Components. The difference between the two is Pooled Sound Graph Player Components share instances of the Sound Graph to save resources, while each Sound Graph Player Component is backed by it's own copy of its Sound Graph Asset. See [here](Sound-Graph-Playback) for more information on the different kinds of Player Components.

Each Sound Graph has a custom Player Component generated for it. This means that each Sound Graph's events and variables are accessible directly through code. This also means that code that incorrectly references a Sound Graph (for example, referencing an event that has since been renamed) can be identified at compile time, rather than as a runtime bug. Below are the API references for the two kinds of Player Components.



## Sound Graph Player Component API

### public VariableType InputVariableName {get; set;}

All graph variables that are set to As Input are available as a property with a setter, where VariableType is the type of the variable in the Sound Graph, and InputVariableName is the name.



### public VariableType OutputVariableName {get;}

All graph variables that are set to As Output are available as a property with a getter, where VariableType is the type of the variable in the Sound Graph, and OutputVariableName is the name.



### public void EventName( )

When this method is called, the exposed event sharing a name with this method is triggered. The event will be triggered immediately. If the event has parameters, they will be additional parameters to this method.



### public void EventName( Double startTime )

When this method is called, the exposed event sharing a name with this method is triggered. The event will be scheduled to trigger at the given [DSP Time](https://docs.unity3d.com/ScriptReference/AudioSettings-dspTime.html)

Parameter Name | Type | Description
------------ | -------|------
startTime | Double | The [DSP Time](https://docs.unity3d.com/ScriptReference/AudioSettings-dspTime.html) to schedule the event for

If the event has parameters, they will be additional parameters to this method.



### public void StopAll( )

When called, this method stops all playback on player immediately



### public void StopAll( Double time )

When called, this method stops all playback on the player at the given [DSP Time](https://docs.unity3d.com/ScriptReference/AudioSettings-dspTime.html)



Parameter Name | Type | Description
------------ | -------|------
time | Double | The [DSP Time](https://docs.unity3d.com/ScriptReference/AudioSettings-dspTime.html) to schedule the stopping all playback



## Globals API

The following APIs are generated in a static class with the same name as the Globals asset.

### public VariableType VariableName {get; set;}

All graph variables that are set to As Input are available as a property with a setter, where VariableType is the type of the variable in the Sound Graph, and InputVariableName is the name.



### public void EventName( )

When this method is called, the exposed event sharing a name with this method is triggered. The event will be triggered immediately. If the event has parameters, they will be additional parameters to this method.



### public void EventName( Double startTime )

When this method is called, the exposed event sharing a name with this method is triggered. The event will be scheduled to trigger at the given [DSP Time](https://docs.unity3d.com/ScriptReference/AudioSettings-dspTime.html)

| Parameter Name | Type   | Description                                                  |
| -------------- | ------ | ------------------------------------------------------------ |
| startTime      | Double | The [DSP Time](https://docs.unity3d.com/ScriptReference/AudioSettings-dspTime.html) to schedule the event for |

If the event has parameters, they will be additional parameters to this method.
