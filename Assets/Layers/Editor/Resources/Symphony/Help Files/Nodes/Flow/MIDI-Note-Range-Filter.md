---
typora-root-url:../
---

# MIDI Note Range Filter Node

![Midi-Note-Range-Filter](/IMG/Midi-Note-Range-Filter.png)

## About

This node provides a simple way to break out an incoming event by its MIDI note number. The node will read the specified MIDI Data parameter on the incoming event, and trigger the appropriate output. See [Events](/Layers-In-Detail/2_Events.md) for more information.

## Inputs
Input | Type | Description
------------ | ------|-------
Input | Event | The incoming event. This event must contain a MIDI Data parameter. 

## Outputs
Output | Type| Description
------------ | -------|------
Note name | Event | The outgoing event for the given note. The number of these events vary depending on the Start Note and End Note parameters. When an incoming event arrives on Input, if its MIDI note number matches this output this event will be triggered

## Options
Option | Type | Description
------------ | -------|------
Unnamed | Dropdown | Dropdown to choose which MIDI Data parameter to use 
Start Note | Integer | The first note in the range of notes this node will display an event for
End Note | Integer | The last note in the range of notes this node will display an event for


