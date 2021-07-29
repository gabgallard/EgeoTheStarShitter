using ABXY.Layers.Runtime.Timeline;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemEditWindow : EditorWindow
{
    protected static ItemEditWindow lastOpenedEditWindow;

    public static void CloseLastWindowIfOpen()
    {
        lastOpenedEditWindow?.Close();
        lastOpenedEditWindow = null;

    }

    public static void OnTimelineObjectRemoved(TimelineDataItem dataItem)
    {
        lastOpenedEditWindow?.TimelineObjectRemovedInternal(dataItem);
    }

    protected virtual void TimelineObjectRemovedInternal(TimelineDataItem dataItem)
    {

    }
}
