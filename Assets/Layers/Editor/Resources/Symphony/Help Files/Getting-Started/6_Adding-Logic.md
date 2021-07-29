---
typora-root-url:../
---

# Adding Logic

Currently, all of our audio plays simultaneously, and they only play once. Let's make things more interesting! First, let's make our audio loop.

## Looping audio
There are several ways to loop audio in Layers, but the easiest is to connect the Play Finished output port of the Play node to the Play input of the Play node. Recall that we set the End of Playback marker to the end of our Audio Clips in the timeline. This means that the Play Finished output will be triggered when the Audio Clips finish playing, and since the Play Finished output is connected to the Play input, the Play node will begin playback again.

So let's do that! Drag a line from the Play Finished output to the Play input. As you're dragging, you can middle-click to drop pins that reroute the line. When you're done, the Sound Graph should look like this:

![Tutorial-LoopingThePlayNode](/IMG/Tutorial-LoopingThePlayNode.png)

Now when you manually Trigger the Begin Play Event, the Play node will keep looping. To stop execution, press the Stop Playback button in the top right of the Sound Graph editor.

## Layering audio
Now that the audio is looping, let's work on layering audio. To do this, we are going to create an intensity variable, and then use Nodes to control the volume of each track in the Play Node.

To keep from cluttering up Sound Graphs, the volume control of a Play node's track is not initially accessible to other nodes. The track must be exposed. Open the Play node's timeline. To the right of each track's name is an icon - an eye with a line crossing it out. Click on the icon for each track. The icons will no longer be crossed, indicating that the track is exposed. 

![The expose track button](/IMG/ExposeTrackButton.png)

Close the timeline, and take a look at the Play node. Each track exposed track is now listed in the Play node with several new outputs and inputs, including track volume.

![Tutorial-ExposingTracks.png](/IMG/Tutorial-ExposingTracks.png)

Now that the Volume inputs for each track are exposed, let's create the variable that will control them.

Click the "+" icon on the Variables List in Graph Inputs to create a new variable. Every variable has a name, and next to that name is the variable's type. Set the name to "Intensity", and the type to Number (Float).

![Tutorial-AddingIntensityVariable.png](/IMG/Tutorial-AddingIntensityVariable.png)

Now that the Intensity variable is set up, let's do something with it.

## Controlling the track volume with nodes
Our goal is to always keep the Low Intensity track playing, to layer in the Medium Intensity track when the Intensity variable is equal to 2, and layer in the High Intensity track when the Intensity variable is equal to 3. The easiest way to accomplish this effect is with the Inverse Lerp node.

The [Inverse Lerp](/Docs/Nodes/Math-Operations/Inverse-Lerp) node takes in 3 numeric inputs, a Value, a From, and a To. It takes a Value, a number between From and To, and creates an output from 0 to 1. Let's walk through setting up two Inverse Lerp nodes in the Sound Graph, and then discuss how it works in detail.

To add an Inverse Lerp node, right click anywhere in the Sound Graph editor window and select Math Operations -> Inverse Lerp. Go ahead and add two to your Sound Graph.

For the first Inverse Lerp node, connect the output from the Intensity Variable in the Graph Inputs node to the Value input. Then type 1 into the From input, and 2 for the To input. Then connect the Result output on the Inverse Lerp Node to the Volume input on the Medium Intensity Track in the Play Node.

For the second Inverse Lerp node, connect the output from the Intensity Variable in the Graph Inputs node to the Value input. Then type 2 into the From input, and 3 for the To input. Then connect the Result output on the Inverse Lerp Node to the Volume input on the High Intensity Track in the Play Node. The Sound Graph should now look like this:

![Tutorial-SettingUpInverseLerp.png](/IMG/Tutorial-SettingUpInverseLerp.png)

Press the Trigger button on the Event in the Graph Input Node, and then play with the value of the Intensity variable. See how the Medium Intensity and High Intensity tracks fade in, depending on its value? Press the Stop Playback button in the top right of the Sound Graph editor window when you are done.

So what's going on here? The Low Intensity track is easy - we're not feeding in a volume from a node, so it's always at its max volume, 1. Examine the Inverse Lerp node for the Medium Intensity track. When the Intensity is equal to the From Value (1), the output volume is 0. When the Intensity is equal to the To value (2), the output volume is 1. The output is clamped to always be between 0 and 1, so an Intensity greater than 2 will still produce a volume of 1, and an intensity less than 1 will still produce a volume of 0. In other words, as we increase the Intensity from 1 to 2, the volume of the Medium Intensity Track approaches 1, and when the Intensity increases beyond 2, the volume remains at 1. The same goes for the Inverse Lerp node for the High Intensity track, just over a different intensity range.

Now we have a Sound Graph that defines a music system capable of dynamically layering music based on an intensity level. Let's next discuss how to play that Sound Graph in a Scene.

