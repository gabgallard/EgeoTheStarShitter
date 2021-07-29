---
typora-root-url:../
---

# Audio Out Node

![Audio-Out](/IMG/Audio-Out.png)

## About

Nodes that generate audio, like the Play or Sampler nodes, send audio to Audio Out nodes for playback. The Audio Out node has all of the controls determining how audio is played back, like the final playback volume, pan, position, the output mixer, spatialization, etc.

## Inputs
Input | Type | Description
------------ | ------|-------
Audio In | Audio | The audio that is played back through this Audio Out. Note that nodes that are using this Audio Out for playback don't necessarily need to be connected to this input. Audio outputs can "send" audio without a visible connection to prevent the graph from getting cluttered. Audio outputs that are sending this way are counted to the right of this input's label. This input is hidden when the Settings Source option is set to Input.
Volume | Float | This is the playback volume, ranging from 0 to 1. Note that nodes that output audio may have their own volume controls that factor into their final volume. This input is hidden when the Settings Source option is set to Input.
Pitch | Float | This control enables the user to pitch-shift incoming volume. It ranges from 0 to 3. This input is hidden when the Settings Source option is set to Input.
Stereo Pan | Float | This control influences how much of the sound plays through the left or right speakers. Note that nodes that output audio may have their own pan controls that influence their final panning. This input is hidden when the Settings Source option is set to Input.
Audio Mixer Group | Audio Mixer Group | The audio mixer group that incoming audio will play through. This input is hidden when the Settings Source option is set to Input.
Bypass Effects | Boolean | Incoming audio will bypass effects when true. The effects may be from filter components or global listener filters. This input is hidden when the Settings Source option is set to Input.
Bypass Listener Effects | Boolean | Bypasses global effects on the audio listener when true. This input is hidden when the Settings Source option is set to Input.
Bypass Reverb Zones | Boolean | When True, reverb zones are not applied to audio played through this Audio Out. This input is hidden when the Settings Source option is set to Input.
Priority | Integer | Unity virtualizes audio sources when more audio sources are in use then available hardware channels. This control influences which audio sources are virtualized first. It ranges from 0 to 255. This input is hidden when the Settings Source option is set to Input.
World Position | Vector3 | The world position that incoming audio will play at. Note that this option is only available when the Play at Transform input has no value. This input is hidden when the Settings Source option is set to Input.
Play at Transform | Transform | If set, audio will playback at the position of this transform. Note that this option is only available when the World Position input has no value. This input is hidden when the Settings Source option is set to Input.
Reverb Zone Mix | Float or Curve | The amount that the incoming audio will be mixed a reverb zone. This can be specified as a float, or as a curve representing the reverb mix versus the distance from the listener to the audio source. This input is hidden when the Settings Source option is set to Input.
Spatial Blend | Float or Curve | How much the incoming audio is subject to spatialization. A value of 0 results in fully 2D audio, and a value of 1 results in fully spatialized audio. This can be specified as a float, or as a curve representing the spatial blend versus the distance from the listener to the audio source. This input is hidden when the Settings Source option is set to Input.
Spread | Float or Curve | The spread angle of the audio in degrees. This can be specified as a float, or as a curve representing the spread angle versus the distance from the listener to the audio source. This input is hidden when the Settings Source option is set to Input.
Volume Rolloff settings | Float or Curve | The rate at which incoming audio attenuates. This can be specified as a logarithmic curve, a linear falloff, or as a custom curve. This input is hidden when the Settings Source option is set to Input.

Note that each of these options can be set, either as incoming values on inputs, on the node itself, or as parameters in the events triggering audio playback on the nodes sending audio to the Audio Out.

## Options
Option | Type| Description
------------ | -------|------
Settings Source | Setting Source Selector | The settings for this Audio Out may either be set on the node itself, or be passed in using the Audio Settings input. This is useful for sharing settings across Audio Out nodes.
Share Settings | Share Settings Selector | There are two modes for how playback settings are handled, Share Settings and Instance Settings. In share settings mode, the inputs are read continuously and are immediately applied to all audio playing through the Audio Out. For example, if the input to the World Position input is continuously changing, all audio currently playing through the Audio Out will play back at that position and follow it as it changes. In Instance Settings mode, Inputs are read each time new audio plays through the Audio Out, and the new values are only applied to that new audio. For example, if World Position is set, and one piece of audio is sent to the Audio Out, then the World position is changed and another piece of audio is sent, the two pieces of audio will play back in different positions.
3D Audio | Curves | This dropdown curve editor allows the user to manually edit the Reverb Zone Mix, Spatial Blend, Spread, and Volume Rolloff curves.



