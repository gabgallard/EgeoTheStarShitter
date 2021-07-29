---
typora-root-url:../
---

# Click Track Node

![Click-Track](/IMG/Click-Track.png)

## About

This node outputs a click track signal based on the inputs.

## Inputs
Input | Type | Description
------------ | ------|-------
Enter | Event | Begins playback of the click track
BPM | Float | The beats per minute of the signal this node generates
Number of Bars | Integer | The number of bars this node will play
Beats Per Bar | Integer | The number of beats per bar this node will play
Clicks Per Beat | Integer | The number of clicks per beat this bar will play


## Outputs
Output | Type| Description
------------ | -------|------
Track Finished | Event | Once playback of this click track has finished, this event is triggered
On Bar | Event | This event is triggered whenever a bar in the click track is completed
On Beat | Event | This event is triggered whenever a beat in the click track is completed
On Click | Event | This event is triggered whenever a click in the click track is played

## Options
Option | Type | Description
--- | --- | ---
Play Click | Boolean | When checked, this node will audibly play the click track



