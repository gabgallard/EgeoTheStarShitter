using ABXY.Layers.Runtime.Timeline.Playnode;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SortedQueue
{
    List<KeyValuePair<double, PlaynodeDataItem>> playQueue = new List<KeyValuePair<double, PlaynodeDataItem>>();


    public void Insert(double time, PlaynodeDataItem item)
    {
        int insertionIndex = GetInsertIndex(time);
        playQueue.Insert(insertionIndex, new KeyValuePair<double, PlaynodeDataItem>(time, item));
    }


    /// <summary>
    /// Gets all items at or after the given time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public List<KeyValuePair<double, PlaynodeDataItem>> GetItemsScheduledAtOrAfter(double time)
    {
        if (playQueue.Count == 0)
            return new List<KeyValuePair<double, PlaynodeDataItem>>();
        int startIndex = GetInsertIndex(time);
        return playQueue.Skip(startIndex).ToList();
    }

    /// <summary>
    /// Get all items at or after the given time, and are before time + time period
    /// </summary>
    /// <param name="time"></param>
    /// <param name="timePeriod"></param>
    /// <returns></returns>
    public List<KeyValuePair<double, PlaynodeDataItem>> GetItemsScheduledAtOrAfter(double time, double timePeriod)
    {
        if (playQueue.Count == 0)
            return new List<KeyValuePair<double, PlaynodeDataItem>>();
        int startIndex = GetInsertIndex(time);
        int endIndex = GetInsertIndex(time + timePeriod);
        return playQueue.Take(endIndex).Skip(startIndex).ToList();
    }

    /// <summary>
    /// Get all items at that is scehduled to play during time and time+timePeriod
    /// </summary>
    /// <param name="time"></param>
    /// <param name="timePeriod"></param>
    /// <returns></returns>
    public List<KeyValuePair<double, PlaynodeDataItem>> GetItemsScheduledDuring(double time, double timePeriod)
    {
        return playQueue.Where(x => x.Value.startTime <= time + timePeriod && x.Value.startTime + x.Value.length > time).ToList();
    }

    /// <summary>
    /// Get all items at that is scehduled to play during time and time+timePeriod
    /// </summary>
    /// <param name="time"></param>
    /// <param name="timePeriod"></param>
    /// <returns></returns>
    public List<PlaynodeDataItem> GetItemsScheduledDuring(double time, double timePeriod, System.Predicate<PlaynodeDataItem> predicate)
    {
        return playQueue.Where(x => x.Value.startTime <= time + timePeriod && x.Value.startTime + x.Value.length > time && predicate(x.Value)).Select(y=>y.Value).ToList();
    }

    private int GetInsertIndex(double keyValue)
    {
        return BinarySearchInsertionIndex(0, playQueue.Count-1, keyValue);
    }

    // Returns index of x if it is present in
    // arr[l..r], else return -1
    private int BinarySearchInsertionIndex(int l, int r, double x)
    {
        if (r >= l)
        {
            int mid = l + (r - l) / 2;

            // If the element is present at the
            // middle itself
            if (playQueue[mid].Key == x)
                return mid;

            // If element is smaller than mid, then
            // it can only be present in left subarray
            if (playQueue[mid].Key > x)
                return BinarySearchInsertionIndex(l, mid - 1, x);

            // Else the element can only be present
            // in right subarray
            return BinarySearchInsertionIndex(mid + 1, r, x);
        }

        // We reach here when element is not present
        // in array
        return l;
    }
}
