---
typora-root-url:../
---

# Wait for Event Node

![Wait-For-Event](/IMG/Wait-For-Event.png)

## About

When the Start input is triggered, this node waits until the the given graph event has also been triggered. Once this has occurred, the End output is triggered

## Inputs
Input | Type | Description
------------ | ------|-------
Start | Event | Once this event is triggered, the node begins waiting for the given event to be triggered
 Reset | Event | If this node is waiting for an event, triggering this event resets the node to normal 

## Outputs
Output | Type| Description
------------ | -------|------
End | Event | Once the node is set to wait for an event, and then the given event has been triggered, this output event will trigger and the node will be set to normal 

## Options

| Output   | Type           | Description                                                  |
| -------- | -------------- | ------------------------------------------------------------ |
| Unlabled | Event Selector | When this node is triggered, it waits for this event to be triggered. Once this occurs, the End output is triggered |
