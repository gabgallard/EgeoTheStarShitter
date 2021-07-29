---
typora-root-url:../
---

# Get Random Number Node

![Random-Number-Node](/IMG/Random-Number-Node.png)

## About

When this node is executed, a random number between the From and To inputs is generated. This number can be read on the Random Number output.

## Inputs
Input | Type | Description
------------ | ------|-------
Get Number | Event | When this event is triggered, the node generates a new random number
From | Float or Integer | The smallest number the node can randomly generate. The type of this input changes depending on the selected output Type
To | Float or Integer | The largest number the node can randomly generate. The type of this input changes depending on the selected output Type

## Outputs
Output | Type| Description
------------ | -------|------
On Changed | Event | This event is triggered whenever a new random number is generated
Value | Float or Integer | The random number. The type of this input changes deoending on the selected output Type

## Options
Option | Type | Description
------------ | -------|------
Type | Type Selector | The type of the randomly generated number


