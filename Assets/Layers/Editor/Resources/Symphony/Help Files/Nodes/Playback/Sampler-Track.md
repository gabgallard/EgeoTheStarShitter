---
typora-root-url:../
---

# Sampler Track Node

![Sampler-Track-Node](/IMG/Sampler-Track-Node.png)

## About

This node takes in events with MIDI parameters, and plays a sample based on that parameter's note number

## Inputs
Input | Type | Description
------------ | ------|-------
MIDI In | Event | The event triggering sample playback. The sample will be chosen based on the selected MIDI Parameter on the event. 

## Outputs
Output | Type| Description
------------ | -------|------
Audio Out | Audio | All audio output from this node flows through this output. Note that this output can be connected directly to an Audio Out, or audio can be "sent" by clicking the dropdown next to this output and selecting a target Audio Out. This function helps unclutter the Sound Graph.

## Options
Option | Type | Description
------------ | -------|------
Unnamed | Dropdown (Parameter selector) | Dropdown to choose which MIDI Data parameter to read from the event 
Samples | List of Samples | The samples for this sample node. Each sample has a MIDI note number and an associated Audio Clip. When an event with the given MIDI note number triggers the MIDI In input, the associated Audio Clip will play. Each item in the samples list has a Listen for Key function to make setup of samples more convenient. When Listen for Key is active, the sample will set it's note number to the next key that is pressed on an attached MIDI device.

## Load Folder Function
This node can load in an entire folder of samples at once for convenience. Click the Load Folder button, pick a folder, and all audio in that folder will be loaded into the sampler. If the names of the clips have note numbers in it, the sampler will try to parse them and automatically configure the note number in the list. For example, a file name containing the number 30 will be matched to F#1 in the sampler. 


