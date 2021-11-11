using ABXY.Layers.Runtime.Timeline.Playnode;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityAsync;
using UnityEngine;

public class Tape
{
    private bool running = false;

    private Object schedulerObject;


    SortedQueue queue = new SortedQueue();

    public Tape(Object schedulerObject)
    {
        this.schedulerObject = schedulerObject ?? throw new System.ArgumentNullException(nameof(schedulerObject));
    }

    public void Schedule(double startTime, List<PlaynodeDataItem> items)
    {

        foreach(PlaynodeDataItem item in items)
        {
            Schedule(startTime, item);
        }
    }

    public void Schedule(double startTime, PlaynodeDataItem item)
    {

        if (!running)
            Run();
        queue.Insert(startTime + item.startTime, item);
    }

    private async void Run()
    {
        running = true;
        await Await.BackgroundSyncContext();

        while (running)
        {
            double currentTime = AudioSettings.dspTime;
            List<KeyValuePair<double, PlaynodeDataItem>> newData = queue.GetItemsScheduledAtOrAfter(currentTime, 0.05f);

            foreach (var datum in newData)
                Debug.Log(datum.Key);

            await Await.SecondsRealtime(0.05f).ConfigureAwait(schedulerObject, FrameScheduler.FixedUpdate);
        }
    }

}
