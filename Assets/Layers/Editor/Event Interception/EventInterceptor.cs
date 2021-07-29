using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventInterceptor
{
    static Event blockedEvent = null;
    public static void Begin(bool active)
    {
        blockedEvent = null;


        if (active && Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout)
        {
            blockedEvent = new Event( Event.current);
            Event.current.Use();
        }
    }

    public static void End()
    {
        if (blockedEvent != null)
        {
            Event.current = blockedEvent;
            blockedEvent = null;
        }
    }
}
