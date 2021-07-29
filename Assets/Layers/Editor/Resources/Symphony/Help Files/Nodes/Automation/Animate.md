---
typora-root-url:../
---

# Animate Node

![Animate Node](/IMG/Animate-Node.png)

## About

Animates an output value over time, according to an animation curve. When the Start event is triggered, the the value on the Output port will change according to the curve. Triggering stop will reset the value.

## Inputs
Input | Type | Description
------------ | -------|------
Start | Event| Begins animating the output value
Stop | Event | Stops animating the output value, and resets the time on the curve to zero
Animation Curve | AnimationCurve | The animation curve representing the output value of the node over time.

## Outputs
Output | Type | Description
------------ | ------|-------
Value | Float | The resulting value of the animation curve over time
