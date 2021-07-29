---
typora-root-url:../
---

# Set Mixer Parameter Node

![Set-Mixer-Parameter-Node](/IMG/Set-Mixer-Parameter-Node.png)

## About

Sets the value of the given Unity Mixer parameter

## Inputs
Input | Type | Description
------------ | ------|-------
Write | Event | This event triggers the write operation
Mixer | Audio Mixer| The Unity Mixer the parameter belongs to
Name | String | The name of the parameter
Value | Float | The value to write to the parameter

## Outputs
Output | Type| Description
------------ | -------|------
Write Finished | Event | This event is triggered when the write has completed


