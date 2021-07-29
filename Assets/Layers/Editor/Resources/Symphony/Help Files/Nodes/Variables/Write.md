---
typora-root-url:../
---

# Write Node

![Write-Node](/IMG/Write-Node.png)

## About

The Write node writes an incoming value to the given named variable in the Graph Input node. 

## Inputs
Input | Type | Description
------------ | ------|-------
Start Write | Event | When this input is triggered, the incoming value is read and written to the specified graph variable.
Unnamed | Varies | The value to be written to the variable

## Outputs
Output | Type| Description
------------ | -------|------
Write Ended | Event | Once the value has been written, this output is triggered

## Options
Option | Type | Description
------------ | -------|------
Variable Name | Variable Name Selector | The variable on the Graph Input to write to


