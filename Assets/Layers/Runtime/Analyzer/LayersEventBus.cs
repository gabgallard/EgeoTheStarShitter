using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayersEventBus
{
    public static System.Action<LayersAnalyzerEvent> onEventRaised;

    public static void RaiseEvent(LayersAnalyzerEvent levent)
    {
        onEventRaised?.Invoke(levent);
    }
}
