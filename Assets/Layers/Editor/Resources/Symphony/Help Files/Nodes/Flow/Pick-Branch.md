---
typora-root-url:../
---

# Pick Branch

![Pick-Branch](/IMG/Pick-Branch.png)

## About

This node picks a branch to trigger when executed. The chosen branch depends on the Selected Branch input

## Inputs
Input | Type | Description
------------ | ------|-------
Input | Event | Executes this node. When triggered, the node picks an output to trigger, depending on the Selected Branch input
Selected Branch | Integer | When Input is triggered, the output branch with this number is triggered. Note that branches are numbered starting at 1. The value for this input can be set on the node, be read from the incoming port, or from an incoming event parameter on Input. Click the dropdown next to the input to choose where the value is sourced from. 

## Outputs
Output | Type| Description
------------ | -------|------
Outputs | List of Events | When Input is triggered, an event is selected and triggered from this list using the Selected Branch input



