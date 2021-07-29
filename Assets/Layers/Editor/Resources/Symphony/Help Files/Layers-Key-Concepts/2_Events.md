---
typora-root-url:../
---

# Events

## About

Events are how nodes are sequenced in a Sound Graph. Events can carry MIDI data and arbitrary parameters. There are two types of events, named Sound Graph events and anonymous events. Sound Graph events are defined in the [Graph Input](/Docs/Nodes/Signal-Sources/Graph-Inputs) node, and can be triggered within the sound graph using the [Trigger](/Docs/Nodes/Signal-Sources/Trigger-Event) node, and externally when the event is exposed. Anonymous events are produced by nodes.

![Named VS Anonymous Events](/IMG/NamedVSAnonymousEvents.png)

Most nodes produce anonymous events. See the documentation to learn when and why these events are produced, as well as the data that accompanies them. Events may be split into their component parts using a [Split](/Docs/Nodes/Variables/Split) node. MIDI data and parameters can be added to an event using the [Combine](/Docs/Nodes/Variables/Combine) node.
