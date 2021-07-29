---
typora-root-url:../
---

# Do While Node

![Do-While](/IMG/Do-While.png)

## About

This node is used to create loops. It evaluates a condition when triggered. If that condition is met, the Condition Reached output is triggered. Otherwise, Continue Loop is triggered. That condition may be an iteration count, or custom logic.

## Inputs
Input | Type | Description
------------ | ------|-------
Enter | Event | Triggers evaluation of the condition. Depending on the condition's value, Continue Loop or Condition Reached may be triggered
Max Iteration Count | Integer | (Displayed when Use Custom Logic is unchecked) The number of times this node can be executed before Condition Reached will be triggered
Condition | Boolean| (Displayed when Use Custom Logic is checked) The value read when the Enter input is triggered. If True, Condition reached is triggered. When False, Continue Loop is triggered
Reset Iterations | Event | Resets this node's iteration count

## Outputs
Output | Type| Description
------------ | -------|------
Continue Loop | Event | This event is triggered when the Enter input is triggered and the condition is false
Continue Reached | Event | This event is triggered when the Enter input is triggered and the condition is true
Index | Integer | The current iteration count for this node. Note that this shows even when using custom logic, in case you want to know how many times the node has been called

## Options
Option | Type | Description
------------ | -------|------
Use Custom Logic | Boolean | If true, iteration is controlled using the condition input. If false, iteration is controlled using the Max Iteration Count Input

