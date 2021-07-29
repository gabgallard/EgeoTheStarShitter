using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.Timeline_Editor.Structure;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Core;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using ABXY.Layers.Runtime.Timeline;
using ABXY.Layers.Runtime.Timeline.Playnode;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode
{
    public class PlayNodeDataSource : TimeLineDataSource
    {
        public global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode backingPlaynode { get; private set; }

        private TempoMapManager tempoMapManager;

    

        public PlayNodeDataSource(global::ABXY.Layers.Runtime.Nodes.Playback.PlayNode backingPlaynode)
        {
            this.backingPlaynode = backingPlaynode;
            this.editable = true;
            tempoMapManager = new TempoMapManager();
            RebuildTempoMap();
        }

        public override void ApplyTimelineDataChanges(List<TimelineDataItem> items)
        {
            backingPlaynode.timelineData = items.Select(x=>(PlaynodeDataItem)x).ToList();
        }

        public override List<BPMDataItem> GetBPMItems()
        {
            return backingPlaynode.bpms;
        }

        public override List<TimeLineRowDataItem> GetDataRows()
        {
            if (backingPlaynode.tracks.Count == 0)
                backingPlaynode.tracks.Add(new PlaynodeTrackItem(backingPlaynode));
            return backingPlaynode.tracks.Select(x => (TimeLineRowDataItem)x).ToList();
        }

        private static int PlaynodeTrackItemComparator(PlaynodeDataItem a, PlaynodeDataItem b)
        {
            if (a== null)
            {
                if (b == null)
                {
                    // If ais null and b is null, theb're
                    // equal.
                    return 0;
                }
                else
                {
                    // If ais null and b is not null, b
                    // is greater.
                    return -1;
                }
            }
            else
            {
                // If ais not null...
                //
                if (b == null)
                    // ...and b is null, a is greater.
                {
                    return 1;
                }
                else
                {
                    return (int)a.playnodeDataItemType > (int)b.playnodeDataItemType ? 1:-1;
                }
            }
        }

        public override BPMDataItem GetDefaultBPM()
        {
            return backingPlaynode.defaultBPM;
        }

        public override TimeSignatureDataItem GetDefaultTimeSignature()
        {
            return backingPlaynode.defaultTimeSignature;
        }

        public override double GetEndTime()
        {
            return backingPlaynode.endTime;
        }

        public override TempoMap GetTempoMap()
        {
            return tempoMapManager.TempoMap;
        }

        public override List<TimelineDataItem> GetTimelineDataItems()
        {
            backingPlaynode.timelineData.Sort(PlaynodeTrackItemComparator);
            return backingPlaynode.timelineData.Select(x=>(TimelineDataItem)x).ToList();
        }

        public override List<TimeSignatureDataItem> GetTimeSignatureItems()
        {
            return backingPlaynode.timeSignatures;
        }

        public override TimelineDataItem OnAddTimelineObject()
        {
            return null;
        }

        public override void OnRemoveTimelineObject(TimelineDataItem removedItem)
        {
            backingPlaynode.timelineData.Remove((PlaynodeDataItem)removedItem);
        }

        public override void SetBPMItems(List<BPMDataItem> items)
        {
            backingPlaynode.bpms = items;
            RebuildTempoMap();
        }

        public override void SetDataRows(List<TimeLineRowDataItem> rows)
        {
            backingPlaynode.tracks = rows.Select(x=> (PlaynodeTrackItem)x).ToList();
        }

        public override void SetDefaultBPM(BPMDataItem defaultBPM)
        {
            backingPlaynode.defaultBPM = defaultBPM;
            RebuildTempoMap();
        }

        public override void SetDefaultTimeSignature(TimeSignatureDataItem defaultTimeSignature)
        {
            backingPlaynode.defaultTimeSignature = defaultTimeSignature;
            RebuildTempoMap();
        }

        public override void SetEndTime(double endTime)
        {
            backingPlaynode.endTime = endTime;
        }

        public override void SetTimeSignatureChanges(List<TimeSignatureDataItem> timeSignatures)
        {
            backingPlaynode.timeSignatures = timeSignatures;
            RebuildTempoMap();
        }

        private void RebuildTempoMap()
        {
            tempoMapManager.ReplaceTempoMap(TempoMap.Create(new TicksPerQuarterNoteTimeDivision((tempoMapManager.TempoMap.TimeDivision as TicksPerQuarterNoteTimeDivision).TicksPerQuarterNote)
                , GetDefaultBPM().ToTempo(), GetDefaultTimeSignature().ToTimeSignature()));

            foreach (TimeSignatureDataItem item in GetTimeSignatureItems())
            {
                if (item.time != 0)
                    tempoMapManager.SetTimeSignature((long)item.time, item.ToTimeSignature());
            }

            foreach (BPMDataItem item in GetBPMItems())
            {
                if (item.time != 0)
                    tempoMapManager.SetTempo((long)item.time, item.ToTempo());
            }
        }

        private double currentTime;

        public override double GetCurrentPlaybackTime()
        {
            return currentTime;
        }

        public override void SetCurrentPlaybackTime(double time)
        {
            currentTime = Mathf.Clamp((float)time, 0f, float.MaxValue);
        }
    }
}
