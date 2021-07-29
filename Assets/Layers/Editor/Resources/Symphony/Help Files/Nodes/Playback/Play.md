---
typora-root-url:../
---

# Play Node



![Play Node](/IMG/Play-Node.png)

## About

The Play node is the primary method of sequencing and playing audio and MIDI. It is also capable of sequencing events.

## The Play Node Timeline
![Play-Node-Timeline](/IMG/Play-Node-Timeline.png)



The Play node timeline is where sequencing for the Play Node takes place. Press the Open button on the Play node to open the timeline.

### The Track Area
In the middle of the Play Node Timeline window is the track area for the Play Node. These tracks behave similarly to tracks in a Digital Audio Workstation. Timeline elements can be dragged around the timeline, resized, and looped.

On the left hand side of each track are the track controls. The volume and pan can be adjusted per track. Tracks can also be named for organization purposes. Clicking on the Eye icon, users can set the track to visible, which displays the track and its audio and event outputs on the Play Node.

### Timeline elements
The timeline window support several different element types.

#### Audio Clip
Audio clips can be dragged into the timeline from the Unity Project browser, or by right clicking on the timeline and selecting Add Audio Clip. Audio Clips play through the Audio Out output in the Play node's Combined Audio Bus, as well as the track's Audio Out if the track is exposed in the Play node. Advanced options for the audio clip can be accessed by right clicking and selecting edit. The following options are available:
* Audio Clip -The audio clip attached to this timeline element. This field lets you swap out the audio clip for this timeline element.
* Expose in node - Enables dynamic data swapping by exposing this audio clip in the play node. See Dynamic Data Swapping, below.
* Start time - The time the audio clip starts playing
* Length - The amount of time the audio clip will play for. If Length is longer then the source audio clip's length, the audio will loop. If it is shorter, the clip will end playback early.
Time Offset - Shifts the playback time of the audio clip within the timeline element. For example, if Time Offset is set to 1 second, the "beginning" of the timeline element will be 1 second into the audio clip's playback.

#### MIDI File
MIDI Files can be dragged into the timeline from the Unity Project browser, or by right clicking on the timeline and selecting Add MIDI File. Each MIDI note plays as an event through the Event Out output in the Play node's Combined Audio Bus, as well as the track's Event Out if the track is exposed in the Play node. Each event will have the note number, channel, and velocity of the note that created it.

Advanced options for the MIDI File can be accessed by right clicking and selecting edit. The following options are available:
* MIDI File -The MIDI File attached to this timeline element. This field lets you swap out the MIDI File for this timeline element.
* Expose in node - Enables dynamic data swapping by exposing this MIDI file in the play node. See Dynamic Data Swapping, below.
* Start time - The time the MIDI file starts playing
* Length - The amount of time the MIDI file will play for. If Length is longer then the source file's length, it will loop. If it is shorter, the file will end playback early.
Time Offset - Shifts the playback time of the MIDI file within the timeline element. For example, if Time Offset is set to 1 second, the "beginning" of the timeline element will be 1 second into the MIDI file's playback.

#### Event
Event timeline elements provide a simple way of triggering events from the timeline. To add an event, right click on the timeline and select Add Event. Timeline events are played back on the Event Out in Play node's Combined Audio Bus, as well as the Track Event out if the track is exposed. To access the Event element's options, right click on the event and select Edit. The following options are available:
* Label - The label used to identify this event on the timeline
* Time - The time this event occurs.
* Event Parameters - The event parameters to pass on with this event when triggered. Note that parameter names must be unique for each event.

#### Loop
Loop timeline elements enable looping within the timeline. To add a loop, right click on the timeline and select Add Loop. Loops add a boolean input to the Play node called Can Exit Loop. When the Can Exit Loop input is false, playback loops within the range of the Loop element. When Can Exit Loop is true, playback will proceed.

Advanced options for the loop can be accessed by right clicking on the loop range and selecting Edit. The following options are available:
* Label - The name used to identify the loop in the timeline and the Play node
* Time - The start time for the loop
* Length - The length of the loop's time range.

### The End of Playback Marker
In addition to timeline elements, the Track Area also contains the end of playback marker. This marker is a vertical line representing the time of playback where the Play Finished Output of the Play node is called.

Note that timeline elements that extend past the end of playback marker will continue to play, even if the end of playback marker has been reached. This is useful in situations where, for example, reverb is baked into the audio file being played and needs to be accounted for. The next node in the sequence needs to be triggered, but the audio clip needs to keep playing to avoid cutting off the reverb.

### Time Signature and BPM Area
At the top of the window is the BPM and Time signature bar. Time signatures and BPMs can be used in conjunction with snapping to align timeline elements. The left side of the bar contains the starting BPM and the starting time signature for the timeline. BPM and time signature change markesrs can be used to change the BPM or time signature mid-track. To add a new marker right click on the bar (at the top for BPM, at the bottom for time signatures) and add the marker.

### Snapping
At the bottom right of the Play Node Timeline are the snapping controls. Snapping can be performed using a variety of time divisions, all based on the BPM and Time signatures set in the time signature and BPM area. Timeline elements can also snap to the start and end of other timeline elements. To activate snapping, hold CTRL while dragging.

### Playback controls
The bottom left of the timeline window contains the timeline's playback controls. Playback can be previewed both in edit mode and play mode.

Next to the play back controls are settings for Audio Preview and Event Preview. These options perform the following functions:
* Audio Preview - If Play Audio Using Node Settings is selected, audio will play through the Audio Outs connected to the play node when previewing. If the user wants to preview audio without playing it through Audio Outs, they can set the window to Play All Audio.
* Event Preview - By default, Events are disabled when previewing playback in the timeline window. This is to prevent unintended activation of subsequent nodes in the sound graph. If the user wishes preview playback including subsequent nodes, set this dropdown to Play On Node.

### Dynamic Data Swapping
Dynamic data swapping enables the user to swap out Audio Clips and MIDI files at runtime. To enable this feature, add a Audio Clip or MIDI file to the timeline, right click on it, select edit, and check the Expose In Node option. The Play node will now have an input for swapping out the Audio Clip or MIDI file. When Expose In Node is checked, a Label option is also displayed in the timeline element options. This label identifies this item in the Play node and the Timeline.

### Other controls
* Hold shift and click to select multiple items
* Hold ctrl and drag to enable snapping
* The delete key deletes items
* Hold ctrl and scroll to zoom

## Combined Audio Bus
![Combined-Audio-Bus](/IMG/Combined-Audio-Bus.png)



The Combined Audio Bus controls playback for the entire node, and outputs all audio and events for the node. Here are the inputs and outputs for the Combine Audio Bus

### Inputs
Input | Type | Description
------------ | ------|-------
Play | Event | When triggered, this event begins playback of the node. Note that if the Play node is already playing, calling this event will not stop that previous playback. Instead, they will play in parallel.
Pause | Event | When triggered, this event pauses the node
Resume | Event | If this node is paused, triggering this event resumes playback
Stop | Event | When triggered, this event stops all playback on this node.

### Outputs
Output | Type| Description
------------ | -------|------
Play Finished | Event | This event is called when the timeline reaches the End of Playback Marker.
Audio Out | Audio | All audio output from this node (across all tracks) flows through this output. Note that this output can be connected directly to an Audio Out, or audio can be "sent" by clicking the dropdown next to this output and selecting a target Audio Out. This function helps unclutter the Sound Graph.
Event Out | Event | All Events (including MIDI events) triggered in this node (across all tracks) are sent through this output

### Options
Option | Type | Description
--- | --- | ---
Volume | Float | The main volume across all tracks, ranging from 0 to 1
Pan | Float | The main pan across all tracks ranging from -1 (left) to 1 (right)
Play finished | Dropdown | Chooses when the Play Finished output event is triggered. When "At Specified Time is selected", the node will trigger the event once the End of Playback Marker is reached in the timeline. When "When All Clips Are Finished is selected", the event is triggered when all playback items in the timeline have finished playing 

## Play Node Track Controls
![Play-Node-Track-Controls](/IMG/Play-Node-Track-Controls.png)

When tracks in the Timeline are marked as exposed, they display some options and outputs in the play node

### Inputs

| Input  | Type  | Description                                      |
| ------ | ----- | ------------------------------------------------ |
| Volume | Float | The volume of the audio of this individual track |
| Pan    | Float | The panning of this individual track             |

### Outputs
Output | Type| Description
------------ | -------|------
Audio Out | Audio | All audio output from this track flows through this output. Note that this output can be connected directly to an Audio Out, or audio can be "sent" by clicking the dropdown next to this output and selecting a target Audio Out. This function helps unclutter the Sound Graph.
Event Out | Event | All Events (including MIDI events) triggered in this track are sent through this output

### Options
Option | Type | Description
--- | --- | ---
Volume | Float | The volume for this track, ranging from 0 to 1
Pan | Float | The pan for this track ranging from -1 (left) to 1 (right)

## Play Node Loop Controls

![Play-Node-Loop-Controls](/IMG/Play-Node-Loop-Controls.png)

When a loop is added to the timeline, the loop is exposed in the Play node. Set the input Boolean to True to exit the loop

## Play Node Exposed Track Items

![Play-Node-Exposed-Track-Items](/IMG/Play-Node-Exposed-Track-Items.png)

When an exposed MIDI or Audio clip is added to the timeline, an input appears on the Play node. Set the value of the input to control what is played back in the timeline.