---
typora-root-url:../
---

# Get Key Node

![Get-Key](/IMG/Get-Key.png)

## About

This node generates an event when a note on a MIDI deviced attached to this machine is interacted with.

## Outputs
Output | Type| Description
------------ | -------|------
On Trigger | Event | This event is triggered when the given note is interacted with
Velocity | Float | The velocity of the note when On Trigger is triggered, ranging from 0 to 1

## Options
Option | Type | Description
--- | --- | ---
Channel | Channel Selector | The MIDI channel for this node to listen to
Trigger type | Trigger Type Selector | If Note Down is selected, this node will execute On Trigger when the given note is first pressed. If Note Up is selected, it will trigger when the note is released
Note | Integer | When this note is interacted with on the attached MIDI device, this node will execute the On Trigger event. The note can be entered using this field, or the user can press Listen for Key and then press the desired key on the MIDI device. This function will also change the MIDI channel to match the device

## MIDI in builds

By default, nodes that read MIDI devices are disabled in builds. To turn on MIDI support outside of the editor, go to Edit->Project Settings->Layers Settings and check the  Enable MIDI in builds checkbox
