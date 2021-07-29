---
typora-root-url:../
---

# Get Knob Node

![Get-Knob](/IMG/Get-Knob.png)

## About

This node generates an event when a Knob on a MIDI deviced attached to this machine is interacted with.

## Outputs
Output | Type| Description
------------ | -------|------
On Change | Event | This event is triggered when the given knob is interacted with
Knob Value | Float | The value of the given knob

## Options
Option | Type | Description
--- | --- | ---
Channel | Channel Selector | The MIDI channel for this node to listen to
Knob Number | Integer | When this knob is interacted with on the attached MIDI device, this node will execute the On Change event. The Knob number can be entered using this field, or the user can press Listen for Knob and then change the desired control on the MIDI device. This function will also change the MIDI channel to match the device
Starting value | Float | This is the initial value that will be read on the Knob Value output. The value of this output will remain this number until the node receives the first change event from the MIDI device for this knob.

## MIDI in builds

By default, nodes that read MIDI devices are disabled in builds. To turn on MIDI support outside of the editor, go to Edit->Project Settings->Layers Settings and check the  Enable MIDI in builds checkbox

