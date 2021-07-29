---
typora-root-url:../
---

# Playing Audio

Lets add and play some audio!

## Adding a Play node
First, lets add a [Play node](/Docs/Layers-Key-Concepts/4_Play-Nodes) to the graph. As the name suggests, Play nodes are responsible for playing back Audio in Layers. They can also playback other types of assets, but that's beyond the scope of this tutorial - check out the [Play node documentation](/Docs/Layers-Key-Concepts/4_Play-Nodes) for more information.

To add a Play node to the Sound Graph, right click anywhere in the Sound Graph, and click Playback -> Play. A play node will appear in your Sound Graph.

![Tutorial-CreatingAPlayNode](/IMG/Tutorial-CreatingAPlayNode.png)

Let's add some tracks to this Play node. Click the Open button on the Play node to open up the Play node Timeline.

![Tutorial-PlayNodeTimelineIntro.png](/IMG/Tutorial-PlayNodeTimelineIntro.png)

The timeline works exactly like the timeline in a Digital Audio Workstation. In the center are a series of tracks that we can add Audio Clips to. Each timeline starts with one track, but you can add another simply by dragging a clip in the timeline downwards.

## Adding audio
Let's add some audio! To add audio, drag it onto the timeline, or right click in the timeline and select Add Audio Clip. From the House Vibes pack we downloaded previously, let's add *06_House_Vibes_Snare in track 1*, *01_House_Vibes_Bassdrum* in track 2, and *09_House_Vibes_Piano* in track 3. Drag each clip their start aligns with the beginning of the timeline.

For organization purposes, let's name each of our tracks. On the left hand side of each track is the track name (by default, each track is named Track). Name track 1 Low Intensity, name track 2 Medium Intensity, and name track 3 High Intensity.

Finally, lets drag the End of Playback marker to the end of the Audio Clips. The End of Playback marker is a vertical gray bar with a little tab on it in the timeline. We won't need this until later in the tutorial, but the marker defines what time in playback to trigger subsequent nodes connected to the Play Finished output of the play node. Drag the marker to the end of playback (use the ctrl key to snap it to the end of your Audio Clips).

Your timeline should now look like this:

![Tutorial-AddedAudioClips.png](/IMG/Tutorial-AddedAudioClips.png)

## Adding events and playing  audio
Go ahead and close the timeline. Our Play node now has audio to play, but nothing is triggering the Play node to play. Let's make an event to do that!

On the Graph Inputs node, click the "+" in the Events list. A new event will appear in the list. Every event has a name, let's set ours to "Begin Play". Then, connect the output port of the event to the Play input port in the Play node. Your graph should look like this:

![Tutorial-BeginPlayEvent.png](/IMG/Tutorial-BeginPlayEvent.png)

On the Begin Play event is a button labeled "Trigger". This button is used to manually trigger the event. Currently if we trigger the event, the Play node is activated, but no audio will play. This is because we need to tell the Play node how to play the audio - to do this, we'll use an [Audio Out](Docs/Nodes/Playback/Audio-Out) node.

## Adding the Audio Out
To add an Audio out node, right click anywhere in the Sound Graph Editor window and select Playback -> Audio Out. Audio Out nodes define how audio is played, including location, volume, panning, spatialization, etc. For more information, see the [Audio Out documentation page](Docs/Nodes/Playback/Audio-Out).

For our purposes, the default settings on the Audio Out node will be fine. We just need to send the audio from the Play node to the Audio Out by connecting the Audio Out output port of the Play Node to the Audio In input port of the Audio Out node. Your Sound Graph should now look like this:

![Tutorial-ConnectingAudioOut.png](/IMG/Tutorial-ConnectingAudioOut.png)

Now, press the Trigger button on the event in the Graph Inputs node. A small window should pop up - click the Trigger button in that window. Your Play node should start playing your audio!

Right now, all of our audio plays once, and then playback ends. Let's fix that!
