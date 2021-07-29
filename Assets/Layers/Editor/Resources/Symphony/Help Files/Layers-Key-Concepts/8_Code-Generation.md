---
typora-root-url:../
---

# Code Generation

## About

Layers uses code generation to create custom Sound Graph players for every Sound Graph. To learn more about custom players, see [here](/Docs/Layers-Key-Concepts/7_Sound-Graph-Playback). Generated code is written to the directory specified by the Generated Code Directory setting. This directory is specified when Layers is first installed to a project. To change the directory after installation, open Project Settings, enter the Layers Settings tab, and change the Generate Code Directory setting.

Certain changes in a Sound Graph require code regeneration. Generally speaking, changes to named events and variables in the Graph Input node most often require code regeneration. Whenever code generation is required for a soundgraph in the project, a prompt will appear in the bottom left of the Sound Graph Editor Window.

![CodeGenerationExample.png](/IMG/CodeGenerationExample.png)
