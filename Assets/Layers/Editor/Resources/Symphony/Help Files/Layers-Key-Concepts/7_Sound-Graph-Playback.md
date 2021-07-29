---
typora-root-url:../
---

# Sound Graph Playback

## About

Player components are how Sound Graphs are played in a scene. There are two different kinds of player components, a Sound Graph Player component, and a Pooled Sound Graph Player Component. Both take an input Sound Graph and use it to play back audio in a scene. The Sound Graph defines the logic behind how audio plays, and the Player component provides interfaces in code and the editor for triggering events and accessing variables.

## Sound Graph Players and Pooled Sound Graph Players

Each player component is most useful in certain situations, and have different options exposed in the editor.

### Sound Graph Player Components
#### When to Use Sound Graph Player Components
Sound Graph Players each make an individual copy of its source Sound Graph asset. This makes them slightly less complex to use than Pooled Sound Graph Players, but since each player has its own copy of the Sound Graph, making many instances of a Sound Graph Player can be memory-inefficient. Sound Graph Player components are best when only a few instances of that player are needed - for example, playing menu music.

#### Editor Options

![SoundGraphPlayerOverview.png](/IMG/SoundGraphPlayerOverview.png)

1. The Edit Button - Click this button to open up the source Sound Graph Asset for editing
2. Don't Destroy on Load - When checked, this player won't get destroyed when a new scene is loaded
3. Play On Awake - When checked, the events in the Starting Events list will be triggered immediately when the player is enabled
4. Starting Events - These are the events that are called by Play On Awake. Note that this list only appears when Play On Awake is checked. Only events that are exposed in the sound graph may be called here.
5. Variables - All variables that are exposed as Inputs in the sound graph are listed here. The starting values of the variables may be overwritten from the default values set in the Sound Graph.
6. Events - All events that are exposed in the Sound Graph are displayed here. Functions on other scripts can be added here as a Unity Event, and will be called when the given event is called in the Sound Graph. Each function should take in three values - A Double representing the time the event is called, a Dictionary<string,Object> representing the values of the event's parameters (the string is the event name, and the Object is the value), and MidiFlowInfo, which contains any MIDI data contained in the event. Note that in most circumstances, functions called here are called *before* the event's scheduled time. Use the time parameter to precisely schedule any actions based on an event.

### Pooled Sound Graph Player Components

#### When to Use Pooled Sound Graph Player Components
Pooled Sound Graph Players share a pool of instances of their source Sound Graph asset. When many instances of a sound graph are needed (for example, a sound effect for interacting with objects), it's best to use a pooled Sound Graph Player. Sound Graph instances are only applied to a Pooled Sound Graph Player when it's needed, and are returned when they are no longer needed. Thus, Pooled Sound Graph Players can be used all over a scene without much performance impact, since the number of Sound Graphs actually running logic and audio on those Pooled Sound Graph Players is actually much smaller than the number of players.

#### Editor Options
![PooledSoundGraphPlayerOverview.png](/IMG/PooledSoundGraphPlayerOverview.png)

1. The Edit Button - Click this button to open up the source Sound Graph Asset for editing
2. Don't Destroy on Load - When checked, this player won't get destroyed when a new scene is loaded
3. Variables - All variables that are exposed As Inputs in the sound graph are listed here. The starting values of the variables may be overrode from the default values set in the Sound Graph.
4. Events - All events that are exposed in the Sound Graph are displayed here. Functions on other scripts can be added here as a Unity Event, and will be called when the given event is called in the Sound Graph. Each function should take in three values - A Double representing the time the event is called, a Dictionary<string,Object> representing the values of the event's parameters (the string is the event name, and the Object is the value), and MidiFlowInfo, which contains any MIDI data contained in the event. Note that in most circumstances, functions called here are called *before* the event's scheduled time. Use the time parameter to precisely schedule any actions based on an event.


## Creating Player Components
Custom player components are generated for every Sound Graph for easier programming. Each generated player is named after their Sound Graph. Note that a Sound Graph can either be used in a pooled player or a regular player, but can't be used in both. To choose which kind of player is generated for a Sound Graph, check or uncheck Pool Sound Graph in that graph's [Graph Input Node](/Docs/Nodes/Signal-Sources/Graph-Inputs). To learn more about code Generation, see the [code generation page](/Docs/Layers-Key-Concepts/8_Code-Generation)
