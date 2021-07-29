---
typora-root-url:../
---

# Variables

## About

Variables are how data is passed around a Sound Graph. There are two types of events, named Sound Graph variables, and anonymous variables. Sound Graph variables are defined in the [Graph Input](/Docs/Nodes/Signal-Sources/Graph-Inputs) node, can be written to using a [Write](/Docs/Nodes/Variables/Write) node, read using a [Variable](/Docs/Nodes/Variables/Variable) node, and used externally.

Variables can be Input , Output, or Unexposed. Input variables can be written to or read externally to the Sound Graph, and can be read within the Sound Graph. Output variables can be read externally to the Sound Graph, and read or written within the Sound Graph. Unexposed variables cannot be read outside of the Sound Graph, but can be read and written inside the Sound Graph

![Named VS Anonymous Variables](/IMG/NamedVSAnonymousVariables.png)

Many nodes use and produce anonymous variables as inputs and outputs. Of particular note is the [Variable](/Docs/Nodes/Variables/Variable) node, which can both read named Sound Graph variables, or output a constant anonymous variable.
