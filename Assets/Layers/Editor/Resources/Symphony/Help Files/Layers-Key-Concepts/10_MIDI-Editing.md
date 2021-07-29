---
typora-root-url:../
---

# MIDI Editing

## About

Layers has full MIDI Import and editing support. MIDI files can be directly imported as an asset. New MIDI files can be created by right clicking in the Project browser and selecting Create->MIDI Asset.

## Editing MIDI Assets
To begin MIDI editing, select the MIDI asset you wish to edit. If the MIDI asset was created externally, set the Editable property to true in the Inspector, then click Apply. Then, press the Open button to open the MIDI asset in the Layers MIDI Editor.


## The MIDI Editor
The MIDI Editor is where MIDI assets can be edited and saved. Here are the components of the MIDI Editor

![MIDIEditorExample.png](/IMG/MIDIEditorExample.png)

### The Piano Roll
In the middle of the MIDI Editor is the Piano Roll. This functions similarly to a piano roll in a Digital Audio Workstation. Notes can be dragged around the note area and resized by dragging on the note edges. To add a new note, click and drag in an empty area in the Piano Roll

On the left hand side of the Piano Roll are series of keys, meant to make identifying notes easier. Natural notes are labeled, and clicking on each key will sound out the note.

### The End of Playback Marker
In addition to Notes, the Piano Roll also contains the end of playback marker. This marker is a vertical line representing the end of playback for the file. Any notes past this point will be ignored when played back in nodes, such as the [Play](/Docs/Nodes/Playback/Play) node.

### Time Signature and BPM Area
At the top of the window is the BPM and Time signature bar. Time signatures and BPMs control the rate of playback for the MIDI asset, and can be used in conjunction with snapping to align timeline elements. The left side of the bar contains the starting BPM and the starting time signature for the timeline. BPM and time signature change markesrs can be used to change the BPM or time signature mid-track. To add a new marker right click on the bar (at the top for BPM, at the bottom for time signatures) and add the marker.

### Velocities
At the bottom of the Piano Roll is the Velocities area. When a note is selected in the Piano Roll, its velocity shows in the velocity area. Simply drag the velocity to change the value.

### Snapping
At the bottom right of the MIDI Editor are the snapping controls. Snapping can be performed using a variety of time divisions, all based on the BPM and Time signatures set in the time signature and BPM area. Timeline elements can also snap to the start and end of other timeline elements. To activate snapping, hold CTRL while dragging.

### Playback controls
The bottom left of the timeline window contains the timeline's playback controls. Playback can be previewed both in edit mode and play mode.

Next to the play back controls are the global settings for Audio Preview in the MIDI Window. Any Sound Graph can be used to preview MIDI in a MIDI Window. The only requirement is that the Sound Graph have one named Sound Graph Event that triggers the playback of a note. When the MIDI Window triggers that event to play a note, that MIDI notes information will be included with that event. See [Graph Inputs](/Docs/Nodes/Signal-Sources/Graph-Inputs) for more information.

### Other controls
* Hold shift and click to select multiple items
* Hold ctrl and drag to enable snapping
* The delete key deletes items
* Hold ctrl and scroll to zoom
