---
typora-root-url:../


---

# Switch Value Selector



![Switch Value Selector](/IMG/Switch-Value-Selector.png)

## About

Given an input Selector, this node chooses a value from a list

## Inputs

| Input    | Type   | Description                                            |
| -------- | ------ | ------------------------------------------------------ |
| Selector | Varies | Values are chosen depending on the value of this input |


## Outputs

| Output | Type   | Description        |
| ------ | ------ | ------------------ |
| Output | Varies | The selected value |

## Options

| Option        | Type            | Description                                  |
| ------------- | --------------- | -------------------------------------------- |
| Selector Type | Dropdown (type) | The type of the Selector input               |
| Value Type    | Dropdown (type) | The type of the values the node chooses from |

## The Switch Values List

Each potential value the node chooses from has the following properties:

## Switch Values List Inputs

| Input    | Type   | Description                                                  |
| -------- | ------ | ------------------------------------------------------------ |
| Selector | Varies | The value compared to the Selector input on the node. If the two values are equal, this value gets selected |
| Value    | Varies | The value                                                    |
