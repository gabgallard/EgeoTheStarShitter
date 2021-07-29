---
typora-root-url:../
---

# Move Towards Node

![Get-Mixer-Parameter](/IMG/Get-Mixer-Parameter.png)

## About

The Move Towards node shifts an input value towards a target value over time. The node can either output to a graph variable or to an output port - the output mode is changed using the Output Mode property.  The Move Towards node works with Float, Double, Vector3, and Quaternion input types



## Output Mode: Variable

![Move-Towards-Variable-Mode](/IMG/Move-Towards-Variable-Mode.png)

The Move Towards Node outputs values to a graph variable in this mode.

### Inputs

| Input        | Type                            | Description                                                  |
| ------------ | ------------------------------- | ------------------------------------------------------------ |
| Target Value | Float/Double/Vector3/Quaternion | The target value this node will move the variable towards. This input changes type depending on the type of the variable |
| Max Delta    | Float                           | The amount the value can change per second                   |

### Options

| Option      | Type           | Description                                                  |
| ----------- | -------------- | ------------------------------------------------------------ |
| Output Mode | Mode selector  | A dropdown picker to choose whether the node outputs to a Graph variable or an output Port |
| Unnamed     | Graph Variable | A dropdown picker for selecting the graph variable this node outputs to. |

## Output Mode: Port

![Move-Towards-Port-Mode](/IMG/Move-Towards-Port-Mode.png)

The Move Towards Node outputs values to a port in this mode. The output value smoothly moves towards the target value

### Inputs

| Input        | Type                            | Description                                                  |
| ------------ | ------------------------------- | ------------------------------------------------------------ |
| Target Value | Float/Double/Vector3/Quaternion | The target value. The type of this port changes depending on the Type property |
| Max Delta    | Float                           | The amount the value can change per second                   |

### Outputs

| Output | Type                            | Description                                                  |
| ------ | ------------------------------- | ------------------------------------------------------------ |
| Output | Float/Double/Vector3/Quaternion | The output value of the node, targeting the Target Value input and dampened by Max Delta. The type of this port changes depending on the Type property. |

### Options

| Option      | Type                   | Description                                                  |
| ----------- | ---------------------- | ------------------------------------------------------------ |
| Output Mode | Mode selector          | A dropdown picker to choose whether the node outputs to a Graph variable or an output Port |
| Type        | Variable Type Selector | Selects the input and output type. Can be set to Float, Double, Vector3, and Quaternion |

