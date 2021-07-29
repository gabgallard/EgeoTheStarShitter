---
typora-root-url:../
---

# Globals

![Sound-Graph-Globals](/IMG/Sound-Graph-Globals.png)

## About

Globals objects are project assets containing variables and events that can be shared across Sound Graphs

## Using Globals Assets

To create a Globals asset, right-click in the project window and select Create->Sound Graph Globals. Globals must exist within a Resources folder.

To use a Globals Asset in a Sound Graph, open the Sound Graph and drag the Globals asset into the Globals field in the Graph Input node.

## Code Generation

Like Sound Graphs, Globals assets use code generation to make their values accessible in scripts. When code regeneration is required, a button will appear in the Globals Asset Inspector. Click it to generate the code.

![Sound-Graph-Globals-Regenerate](/IMG/Sound-Graph-Globals-Regenerate.png)

Once code has been generated, the properties on the Globals object are accessible via a static class with the same name as the Globals Asset.