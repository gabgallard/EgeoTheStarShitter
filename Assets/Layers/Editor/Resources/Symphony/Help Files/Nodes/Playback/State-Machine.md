---
typora-root-url:../

---

# State Machine

![State-Machine-Node-Header](/IMG/State-Machine-Node-Header.png)

## About

The State Machine node simplifies the process of creating stateful music and audio systems. The state machine node, for examples, can be used to create a dynamic music system with various levels of intensity that transitions between intensity levels in response to game play.

The State Machine node works by defining a series of states and the possible transitions between states. Each state links to a different Sound Graph that plays when the state is active. Only one state can be active at a time, and the transitions define the rules by which the states can change. 

The State Machine node has two different transition modes - Fade and Sequential. In Fade mode, when a transition is triggered the currently active state fades out in volume and the new state simultaneously fades in. In Sequential mode, the node waits for the currently active state to finish playing and then immediately triggers the next state.

## Fade mode
![State Machine Fade Node](/IMG/State-Machine-Fade-Node.png)

In Fade mode, the State Machine node has the following outputs and options:

### Node Outputs

| Output          | Type        | Description                                                  |
| --------------- | ----------- | ------------------------------------------------------------ |
| On State Change | Event       | This event is triggered whenever a state change is initiated |
| Current State   | Enumeration | This output contains the current state running on the node. This enumeration is created when code is generated for the Sound Graph, so this output will not be available when the node is created. It will appear once code is regenerated for the Sound Graph |

### Node Options

| Option              | Type     | Description                                                  |
| ------------------- | -------- | ------------------------------------------------------------ |
| State Machine Name  | String   | The name of this state machine, used to name the state machine for code generation |
| State Machine Style | Dropdown | Defines how transitions are executed on this node. In Fade mode, when a transition is triggered the currently active state fades out in volume and the new state simultaneously fades in. In Sequential mode, the node waits for the currently active state to finish playing and then immediately triggers the next state. |

### States

![State-Machine-State-Fade](/IMG/State-Machine-State-Fade.png)

In addition to the above properties, the State Machine node contains a list of states. Here are the properties for each state in Fade mode:

#### State Inputs

| Input   | Type  | Description                                                  |
| ------- | ----- | ------------------------------------------------------------ |
| Unnamed | Event | Each state has an event input next to the state name. Sending an event to that input triggers the transition to that state |

#### State Options

| Option          | Type        | Description                                                  |
| --------------- | ----------- | ------------------------------------------------------------ |
| State Name      | String      | The name of the state. Used for defining transitions, as well as code generation |
| State Graph     | Sound Graph | The Sound Graph that plays when this state is active         |
| Start Event     | Dropdown    | Selects an event from the specified State Graph. When this state becomes active, the selected event will be triggered to initiate playback |
| Volume Variable | Dropdown    | Selects a variable with the type Float from the specified State Graph. During transitions, the State Machine will write a number between 0 and 1 to the variable representing the current volume of the state. In your State Graph, you'll need to read that variable value and use it to change the volume of the audio you are playing back |

If the State Graph contains exposed variables and events, they will appear as inputs and outputs on the state in addition to the above inputs and options, excluding events that are consumed by the Start Event or variables that are consumed by the Volume Variable options.

#### Transitions

Each state also contains a list of transitions. One state can only transition to another if it has a transition defined for that state. Here are the properties for each transition in Fade mode:

##### Transition Options

| Option         | Type     | Description                                                  |
| -------------- | -------- | ------------------------------------------------------------ |
| Unnamed        | Dropdown | Selects the State that gets activated when this transition occurs |
| Fade Time (s)  | Float    | The amount of time in seconds it takes for fade to complete  |
| Wait for Event | Dropdown | Selects an event from the active state's State Graph. When this transition is triggered, it will wait for this event to trigger in the active State Graph before beginning the fade to the new state. |

## Sequential mode

![State-Machine-Node-Sequential](/IMG/State-Machine-Node-Sequential.png)

In Sequential mode, the State Machine node has the following outputs and options:

### Node Outputs

| Output          | Type        | Description                                                  |
| --------------- | ----------- | ------------------------------------------------------------ |
| On State Change | Event       | This event is triggered whenever a state change is initiated |
| Current State   | Enumeration | This output contains the current state running on the node. This enumeration is created when code is generated for the Sound Graph, so this output will not be available when the node is created. It will appear once code is regenerated for the Sound Graph |

### Node Options

| Option              | Type     | Description                                                  |
| ------------------- | -------- | ------------------------------------------------------------ |
| State Machine Name  | String   | The name of this state machine, used to name the state machine for code generation |
| State Machine Style | Dropdown | Defines how transitions are executed on this node. In Fade mode, when a transition is triggered the currently active state fades out in volume and the new state simultaneously fades in. In Sequential mode, the node waits for the currently active state to finish playing and then immediately triggers the next state. |

### States

![State-Machine-State-Sequential](/IMG/State-Machine-State-Sequential.png)

In addition to the above properties, the State Machine node contains a list of states. Here are the properties for each state in Sequential mode:

#### State Inputs

| Input   | Type  | Description                                                  |
| ------- | ----- | ------------------------------------------------------------ |
| Unnamed | Event | Each state has an event input next to the state name. Sending an event to that input triggers the transition to that state |

#### State Options

| Option      | Type        | Description                                                  |
| ----------- | ----------- | ------------------------------------------------------------ |
| State Name  | String      | The name of the state. Used for defining transitions, as well as code generation |
| State Graph | Sound Graph | The Sound Graph that plays when this state is active         |
| Start Event | Dropdown    | Selects an event from the specified State Graph. When this state becomes active, the selected event will be triggered to initiate playback |

If the State Graph contains exposed variables and events, they will appear as inputs and outputs on the state in addition to the above inputs and options, excluding events that are consumed by the Start Event option.

#### Transitions

Each state also contains a list of transitions. One state can only transition to another if it has a transition defined for that state. Here are the properties for each transition in Sequential mode:

##### Transition Options

| Option           | Type        | Description                                                  |
| ---------------- | ----------- | ------------------------------------------------------------ |
| Unnamed          | Dropdown    | Selects the State that gets activated when this transition occurs |
| Transition Graph | Sound Graph | Optional. When this transition begins, this Transition Graph gets played after the current state completes playback, and before the next state begins playback. |
| Start Event      | Dropdown    | If a Transition Graph has been specified, this option appears. This option selects an event from the Transition Graph. It is used to begin playback on the transition graph. |
| End Event        | Dropdown    | If a Transition Graph has been specified, this option appears. This option selects an event from the Transition Graph. Once playback in the transition graph has begun via Start Event, the transition waits for this event to be called from within the transition graph. Once this event has been called, the State Machine will begin playback on the next state. |
| Wait for Event   | Dropdown    | Selects an event from the active state's State Graph. When this transition is triggered, it will wait for this event to trigger in the active State Graph before beginning the fade to the new state. |

If the Transition Graph has exposed events and variables that are not consumed by the Start Event or End Event options, they will appear as inputs and outputs on the transition.