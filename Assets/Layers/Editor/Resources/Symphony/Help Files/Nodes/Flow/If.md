---
typora-root-url:../
---

# If Node

![If-Node](/IMG/If-Node.png)

## About

When triggered, this node chooses one output event or the other depending on an input condition

## Inputs
Input | Type | Description
------------ | ------|-------
Evaluate | Event | Triggers evaluation of the condition. Depending on the condition's value, On True or On False is triggered
Condition | Boolean| The value read when the Evaluate input is triggered. If True, On True is triggered. If False, On False is triggered

## Outputs
Output | Type| Description
------------ | -------|------
On True | Event | This event is triggered when the Evaluate input is triggered and the condition is True
On False | Event | This event is triggered when the Evaluate input is triggered and the condition is False


