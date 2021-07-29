---
typora-root-url:../
---

# Wait Node

![Wait-Node](/IMG/Wait-Node.png)

## About

When the Enter input is triggered, this node waits a set amount of time. After that time, the Exit output is triggered

## Inputs
Input | Type | Description
------------ | ------|-------
Enter | Event | Triggers this node to begin waiting
Wait Time | Float | The amount of time this node waits once the Enter input is triggered

## Outputs
Output | Type| Description
------------ | -------|------
Exit | Event | This event is triggered once the Enter input has been triggered, and the given time period has elapsed


