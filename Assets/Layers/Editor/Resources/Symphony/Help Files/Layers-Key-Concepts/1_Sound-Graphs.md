---
typora-root-url:../
---

# Sound Graph

## About

Sound Graphs are the heart of Layers. Sound Graphs are assets that define the logic that governs how audio is played. Sound Graphs can be thought of like a flow chart. One node of the Sound Graph triggers the next. Sometimes a node may perform an action, like playing audio, or making a computation. Other times, it may decide what branch to take. In this way, Sound Graphs enable you to create incredibly dynamic audio systems from very simple components.

![SoundGraphExample.png](/IMG/SoundGraphExample.png)

## Creating Sound Graphs
To create a sound graph, right click in the Project browser and select Create->Sound Graph. Then, click "Open" in the Sound Graph's inspector to open the graph editor window.

## Working with Sound Graphs
The first node in the Sound Graph is the Graph Input node. It is the only node that is required in a Sound Graph. All graph-wide [Events](/Docs/Layers-Key-Concepts/2_Events) and [Variables](/Docs/Layers-Key-Concepts/3_Variables) are defined on the Graph Input node. The Graph Input node is the starting point for all logic on the graph.

To add nodes to the Sound Graph, right click in the Graph editor to view all available nodes. To connect nodes together, just drag from the output of one node to the input of another. Note that the output and input must match type - for example, an event can only connect to an event, and a boolean can only connect to a boolean. Input and outputs are color coded, so compatibility can be seen at a glance.

To move nodes, simply drag them by their header. Some nodes that are especially tall have a expand / contract symbol in the right side of their header. When contracted, all unused Inputs and Outputs will be hidden so as not to clutter the graph.

![MinimizeMaximize.png](/IMG/MinimizeMaximize.png)

## Previewing Playback
Sound Graph playback can be previewed both in the Editor and at runtime by manually triggering events on the Graph Input node. Playback can be stopped by clicking the Stop Playback button at the top right of the Sound Graph editor window.

## Playing Sound Graphs.
Sound graphs are played using Sound Graph Players. See the [Sound Graph Player page](/Docs/Layers-Key-Concepts/7_Sound-Graph-Playback) for more information.

## Runtime changes
Note that [Sound Graph Players](/Docs/Layers-Key-Concepts/7_Sound-Graph-Playback) will make a copy of their target Sound Graph at runtime. If you plan on editing a Sound Graph asset while your game is playing, be mindful you are not editing a runtime copy, otherwise your changes will be lost when playback ends. Likewise, if you are intending to make temporary changes to a sound graph for testing purposes at runtime, make sure you are editing a runtime copy and not the original asset, otherwise your changes will be preserved when playback ends.

The Sound Graph editor window provides an indicator in the bottom right of the window at runtime so you can check if you are viewing a runtime copy or an original asset.

![RuntimeVsAssetGraph.png](/IMG/RuntimeVsAssetGraph.png)
