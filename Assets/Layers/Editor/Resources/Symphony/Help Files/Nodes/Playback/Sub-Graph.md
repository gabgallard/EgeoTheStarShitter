---
typora-root-url:../
---

# Sub Graph Node

![SubGraph](/IMG/SubGraph.png)

## About

The Sub Graph node enables the user to embed Sound Graphs within Sound Graphs. This is useful for reusing functions and processes across Sound Graphs and keeping individial graphs from getting too complex. To get started with a Sub Graph, select a Sound Graph using the Sub Graph field on the Sub Graph node, or click the New SubGraph button to create a new one.

## Exposed Subgraph Variables.
If any variables in the subgraph are set to be exposed As Input or As Output, they will appear in the subgraph node. Variables set As Output are readable on the node, and variables set As Input will be writeable. Variables set to Do Not Expose will not appear in the Subgraph node. When a variable set As Input has a connected input in the subgraph node, it overrides the default value set on that variable.

## Exposed Subgraph Events
IF any events in the subgraph are set to exposed, they will appear in the subgraph node. Each exposed subgraph event will show an input event and output event in the Subgraph node. Trigger the input to call the event in the subgraph, or connect to the output to listen for when the event is triggered.

## End All
The End All input is a special utility event. Triggering the event stops all playback in the subgraph


