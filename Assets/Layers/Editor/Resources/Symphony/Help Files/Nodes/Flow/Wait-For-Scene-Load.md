---
typora-root-url:../
---

# Wait for Scene Load

![Wait-For-Scene-Load](/IMG/Wait-For-Scene-Load.png)

## About

When a scene loads, it often causes a temporary hitch in the framerate. All Unity audio is scheduled on the main thread, so this hitch can cause problems if audio is scheduled to play at the same time. Rather than playing audio as soon is it loads, it is wise to wait until the scene is loaded and then play audio. This node allows the user to wait for a scene to load before continuing execution.

## Inputs
Input | Type | Description
------------ | ------|-------
Begin Wait | Event | Once this event is triggered, the node will wait for a scene with the given name to load. Once the scene is loaded, the End Wait output will be triggered
Scene Name | String | The scene name this node will wait for before the End event is triggered. The value for this input can be set on the node, be read from the incoming port, or from an incoming event parameter on Begin Wait
Reset | Event | If this node is waiting for a scene to load, triggering this event will reset that wait. End Wait will not be triggered


## Outputs
Output | Type| Description
------------ | -------|------
End Wait | Event | This event is triggered once the Start event is triggered, and then the given scene has been loaded


