---
typora-root-url:../
---

# Playing in the Scene

We've now created a Sound Graph Asset, but we still need to get that Sound Graph in a scene and playing. To play a Sound Graph in a scene, a [Sound Graph Player](/Docs/Layers-Key-Concepts/7_Sound-Graph-Playback) is used. Layers generates a unique Sound Graph Player for every Sound Graph (which is why we set a code generation directory when we installed Layers). This makes the Sound Graph Player easier to control from code. For more information, see the [Code Generation](/Docs/Layers-Key-Concepts/8_Code-Generation) and [API](/Docs/Layers-Key-Concepts/9_API) documentation.

By now, you've probably spotted the following message in the bottom left corner of the Sound Graph editor window:

![Tutorial-RegenerateCode.png](/IMG/Tutorial-RegenerateCode.png)

It will have appeared when you created your first event on the Graph Input node. It appears whenever a change has been made to a Sound Graph that warrants code regeneration. Go ahead and click Regenerate Code to apply the changes. Note - if you have already pressed the button to get the dialog out of the way in previous steps, that's fine! Just click it if you see it now.

Now that code is generated, we can add a player in the scene.

## Adding a Sound Graph Player

Add an empty Game Object to your scene, and give it a name (whatever you like; this tutorial will call it Music Player). Select that object, and then click the Add Component menu. Select Layers->Soundgraphs->The name of your graph, or search for the name of your Sound Graph. Add that component to your Game Object.

![Tutorial-TheSoundGraphComponent.png](/IMG/Tutorial-TheSoundGraphComponent.png)

There are several options on the player we are interested in the first is the Play On Awake setting. When true, the Player will trigger the events specified in the Starting Events list when it loads. Currently, that list is empty. Press the "+" icon on the list to add an event from the Sound Graph to the list.

Here, we've run into a problem - the list item says "No events are currently exposed", yet we definitely created an event in our Sound Graph. This is because events can either be internal to a Sound Graph, or exposed so external systems can trigger them. This is a handy way of preventing others from triggering the internal workings of your Sound Graph and causing it to malfunction. We need to go back into our Sound Graph and edit the Begin Play event so it is exposed. Click on the edit button in the Sound Graph Player - this will open the Sound Graph that created the player.

Take another look at the Begin Play event in the Graph Input node. There is checkbox called "Expose", go ahead and set it to true.

While we're back in the Sound Graph editor, lets take a look at the Intensity variable we created. We're going to want to access the variable from the player as well, and it turns out we have the same problem here as with events - they need to be exposed to be accessed outside of the Sound Graph. Set the variable from "Do Not Expose" to "As Input". For more on what that means, see the [Variables](/Docs/Layers-Key-Concepts/3_Variables) documentation.

Regenerate your code by clicking the dialog at the bottom left of the Sound Graph editor window. Then, close the editor, and go back to the Game Object you created. In the item you created in the Starting Events List, now exists, and the selected item is the Begin Play event. Notice also, the Intensity variable is now editable in the Player.

![Tutorial-PlayerWithExposure.png](/IMG/Tutorial-PlayerWithExposure.png)

We're done! Enter Play Mode in Unity - your Sound Graph Player Component will begin playing.

## Next Steps

Take a look at the [API](/Docs/Layers-Key-Concepts/9_API) documentation to see how the events and variables you create are accessible from code. Then, take a look at the [Layers Key Concepts](/Docs/Layers-Key-Concepts/0_Layers-Key-Concepts) page to read some deep-dives on the different systems that comprise Layers. Thanks for reading!
