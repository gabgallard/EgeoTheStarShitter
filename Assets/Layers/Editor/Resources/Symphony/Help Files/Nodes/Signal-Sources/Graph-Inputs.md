---
typora-root-url:../
---

# Graph Inputs Node

![Graph-Inputs-Node](/IMG/Graph-Inputs-Node.png)

## About

The Graph Inputs node is the entry point for data and events into the Sound Graph. All named variables and events for the Sound Graph are defined on this node. For more information, see the [Events](/Docs/Layers-Key-Concepts/2_Events) and [Variables](/Docs/Layers-Key-Concepts/3_Variables) reference pages. Only one Graph Input node can be created per graph, and Graph Input nodes cannot be deleted.

## Node Outputs
Input | Type | Description
------------ | ------|-------
Update | Event | This output is triggered every frame, both at runtime and in-editor 

## Node Options
Output | Type| Description
------------ | -------|------
 Globals          | Globals Asset | Globals Assets contain variables and events that are shared across Sound Graphs. See [Globals](/Docs/Layers-Key-Concepts/12_Globals) for details 
Pool Sound Graph | Toggle | Controls whether players generated for this sound Graph are pooled. See [Sound Graph Playback](/Docs/Layers-Key-Concepts/7_Sound-Graph-Playback) for details 

## Variables

![Graph-Inputs-Variables](/IMG/Graph-Inputs-Variables.png)

The Graph Input node defines all of the variables available throughout the Sound Graph, and are displayed in the Variables list. Variables have a default value, which is the initial value of the variable at start, and a current value. Here are the properties available for each variable:


### Variable Outputs

| Output  | Type   | Description                                                  |
| ------- | ------ | ------------------------------------------------------------ |
| Unnamed | Varies | Outputs the current value of the variable. The type is the type of the variable |

### Variable Options

| Option        | Type            | Description                                                  |
| ------------- | --------------- | ------------------------------------------------------------ |
| Unnamed       | String          | The name of the variable                                     |
| Unnamed       | Dropdown (type) | The type of the variable                                     |
| Expose        | Dropdown        | Choose whether this variable is an input, and output, or not exposed. Input variables can be written to or read externally to the Sound Graph, and can be read within the Sound Graph. Output variables can be read externally to the Sound Graph, and read or written within the Sound Graph. Unexposed variables cannot be read outside of the Sound Graph, but can be read and written inside the Sound Graph |
| Default Value | Varies          | The initial value of the variable at start                   |
| Current Value | Varies          | The current value of the variable. This value can be reset to its default by clicking the 3-dot menu to the right of the value |

## Events

![Graph-Inputs-Events](/IMG/Graph-Inputs-Events.png)

The Graph Input node defines all of the events available throughout the Sound Graph, and are displayed in the Events list. Events can be private to the Sound Graph, or exposed and accessible externally. Here are the properties available for events:

### Event Outputs

| Output  | Type   | Description                                                  |
| ------- | ------ | ------------------------------------------------------------ |
| Unnamed | Varies | Outputs the current value of the variable. The type is the type of the variable |

### Event Options

| Option  | Type   | Description                                                  |
| ------- | ------ | ------------------------------------------------------------ |
| Trigger | Button | Click this button to manually trigger the event. If any parameters are required for this event, a dialog will show to enable you to set those values |
| Unnamed | String | The name of the Event                                        |
| Expose  | Toggle | Whether or not this event is externally accessible           |

### Event Parameters

Events can have optional parameters that can be used to pass data that may be read by other nodes in the graph. Parameters are set in the Parameters list. Here are the options available for each parameter:

#### Event Parameter Options

| Option         | Type            | Description                                                  |
| -------------- | --------------- | ------------------------------------------------------------ |
| name           | String          | The name of the parameter. This name can be used to retrieve the value of this parameter from this event |
| Parameter Type | Dropdown (type) | The type of this parameter                                   |