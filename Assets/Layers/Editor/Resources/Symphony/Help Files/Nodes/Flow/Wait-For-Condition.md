---
typora-root-url:../
---

# Wait for Condition Node

![Wait-Condition-Node](/IMG/Wait-Condition-Node.png)

## About

When the Start input is triggered, this node waits until the Condition input is true. Once the condition is True, the End Event is triggered

## Inputs
Input | Type | Description
------------ | ------|-------
Start | Event | Once this event is triggered, the node begins waiting
Condition | Boolean | When this node is triggered, it waits for this input to be True before triggering End
Reset | Event | If this node is waiting for a condition, triggering this event will reset that wait. End will not be triggered

## Outputs
Output | Type| Description
------------ | -------|------
End | Event | This event is triggered when the Start input is triggered and the Condition input is True


