---
typora-root-url:../
---

# Multiply Node

## About

This node performs multiplication on the inputs. The Multiply node has multiple modes depending on the selected input type. For example, the Float type can be multiplied by floats, so the Multiply node presents a list of float inputs, where the first gets Multiplied by the second, which gets multiplied by the third, etc. Vector3s on the other hand can be multiplied by other Vector3s as well as numeric types. In this case a secondary type selector is shown so the user can select the operand type.

## Mode 1
![Multiply-Node-Mode-1](/IMG/Multiply-Node-Mode-1.png)

This mode is used for Float, Double, and Integer types

### Inputs
Input | Type | Description
------------ | ------|-------
Variables | List (Type can vary) | The list of inputs to multiply. Each input is multiplied by the subsequent input

### Outputs
Output | Type| Description
------------ | -------|------
Result | Varies | The result of this operation

### Options
Option | Type | Description
------------ | -------|------
Unnamed | Type Selector | The type of the operands for this multiplication operation.


## Mode 2
![Multiply-Node-Mode-2](/IMG/Multiply-Node-Mode-2.png)

This mode is used for Vector3 types

### Inputs
Input | Type | Description
------------ | ------|-------
Value | Varies | The first item to multiply
Value | Varies | The second item to multiply

### Outputs
Output | Type| Description
------------ | -------|------
Result | Varies | The result of this operation

### Options
Option | Type | Description
------------ | -------|------
Unnamed | Type Selector | The type of the first number to multiply for this operation.
Unnamed | Type Selector | The type of the second number to multiply for this operation.


