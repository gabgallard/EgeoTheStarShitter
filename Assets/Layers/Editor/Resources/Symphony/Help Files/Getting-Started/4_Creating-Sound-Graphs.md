---
typora-root-url:../
---

# Creating Sound Graphs

Let's make your first Sound Graph! Sound Graphs define the logic for how audio is played in your scenes. Sound graphs are assets that are located in your project, and can be reused as many times as you like, in any scene. You can make as many Sound Graphs as you like - for example, you may have one Sound Graph running your background music, and another for controlling sound effects for a player controller.

To create a Sound Graph, right click in your project window and click Create->Sound graph. Name it Tutorial Graph for this tutorial.

## Editing a Sound Graph
To open a Sound Graph for editing, click on the Sound Graph in the Project Window, and then click Open in the Inspector Window. You can also double click on the asset to open it. The Sound Graph Editor will appear. You will notice that your Sound Graph is already populated with a node, the [Graph Input](/Docs/Nodes/Signal-Sources/Graph-Inputs) Node.

![Tutorial-GraphInputNode.png](/IMG/Tutorial-GraphInputNode.png)

Nodes define the logic of the Sound Graph. Sound Graphs are like flow charts. One node of the Sound Graph triggers the next. Sometimes a node may perform an action, like playing audio, or making a computation. Other times, it may decide what branch to take. The Graph Input node is the most important - everything else in the Sound Graph depends on it. The Graph Inputs node defines variables that may be used throughout the Sound Graph. It also defines the events used to trigger logic throughout the Sound Graph, such as starting or changing playback. For more information on the Graph Input node, see the node's [documentation](/Docs/Nodes/Signal-Sources/Graph-Inputs).

Now that the Sound Graph is created and open for editing, let's add some audio!
