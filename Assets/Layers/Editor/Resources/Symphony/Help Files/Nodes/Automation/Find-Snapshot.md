---
typora-root-url:../
---

# Find Snapshot Node

![Find-Snapshot](/IMG/Find-Snapshot.png)

## About

Gets a reference to a Unity Audio Mixer snapshot

## Inputs
Input | Type | Description
------------ | -------|------
Snapshot name | String | The name of the snapshot to search for
Audio mixer | AudioMixer| The Unity Audio Mixer the snapshot belongs to

## Outputs
Output | Type | Description
------------ | -----|--------
Snapshot | AudioMixerSnapshot | A reference to the snapshot, or null if a snapshot with the given name doesn't exist in the audio mixer

