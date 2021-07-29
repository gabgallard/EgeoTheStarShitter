---
typora-root-url:../
---

# Transition To Snapshots Node

![Transition-To-Snapshots](/IMG/Transition-To-Snapshots.png)

## About

Transitions the Audio Mixer to a weighted mix of snapshots over time

## Inputs
Input | Type | Description
------------ | ------|-------
Start Transition | Event | This event triggers the node to begin the transition
Snapshots | Snapshot Array| The array of snapshots to transition the Audio Mixer towards
Weights | Float Array | The array of weights for the given snapshots. Each weight corresponds to the snapshot with the same index. Note, the array of weights must have the same number of elements as the array of snapshots. 
Audio Mixer | Audio Mixer | The Audio Mixer to perform the transition on. 
Time To Reach | Float | The amount of time it takes to transition the Audio Mixer to the given weighted snapshots

## Outputs
Output | Type| Description
------------ | -------|------
End Transition | Event | This event is triggered when the audio mixer has finished transitioning to the given weighted snapshots


