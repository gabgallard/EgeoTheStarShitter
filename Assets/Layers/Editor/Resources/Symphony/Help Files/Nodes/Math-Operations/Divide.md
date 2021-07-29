---
typora-root-url:../
---

# Divide Node

## About

This node performs division on the inputs. The Divide node has multiple modes depending on the selected input type. For example, the Float type can be divide by floats, so the Divide node presents a list of float inputs, where the first gets divided by the second, which gets divided by the third, etc. Vector3s on the other hand can be divided by other Vector3s as well as numeric types. In this case a secondary type selector is shown so the user can select the divisor type.

## Mode 1
![Divide-Node-Mode-1](/IMG/Divide-Node-Mode-1.png)

This mode is used for Float, Double, and Integer types

### Inputs
Input | Type | Description
------------ | ------|-------
Variables | List (Type can vary) | The list of inputs to divide. Each input is divided by the subsequent input

### Outputs
Output | Type| Description
------------ | -------|------
Result | Varies | The result of this operation

### Options

| Option  | Type          | Description                                                  |
| ------- | ------------- | ------------------------------------------------------------ |
| Unnamed | Type Selector | The type of the numerators and denominators for this division operation |

## Mode 2
![Divide-Node-Mode-2](/IMG/Divide-Node-Mode-2.png)

This mode is used for Vector3 types

### Inputs
Input | Type | Description
------------ | ------|-------
Value | Varies | The numerator
Value | Varies | The Denominator

### Outputs
Output | Type| Description
------------ | -------|------
Result | Varies | The result of this operation

### Options
Option | Type | Description
------------ | -------|------
Unnamed | Type Selector | The type of the numerator for this operation.
Unnamed | Type Selector | The type of the denominator for this operation.

