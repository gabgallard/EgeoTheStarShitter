using System.Collections.Generic;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using ABXY.Layers.Runtime.Timeline;

namespace ABXY.Layers.Editor.Timeline_Editor.Structure
{
    public abstract class TimeLineDataSource
    {
        public abstract List<TimelineDataItem> GetTimelineDataItems();
        public abstract void ApplyTimelineDataChanges(List<TimelineDataItem> items);

        public abstract List<TimeSignatureDataItem> GetTimeSignatureItems();

        public abstract void SetTimeSignatureChanges(List<TimeSignatureDataItem> timeSignatures);

        public abstract List<BPMDataItem> GetBPMItems();

        public abstract void SetBPMItems(List<BPMDataItem> items);


        public abstract TimeSignatureDataItem GetDefaultTimeSignature();
        public abstract void SetDefaultTimeSignature(TimeSignatureDataItem defaultTimeSignature);

        public abstract BPMDataItem GetDefaultBPM();
        public abstract void SetDefaultBPM(BPMDataItem defaultBPM);

        public abstract List<TimeLineRowDataItem> GetDataRows();

        public abstract void SetDataRows(List<TimeLineRowDataItem> rows);

        /// <summary>
        /// In seconds
        /// </summary>
        /// <returns></returns>
        public abstract double GetEndTime();

        public abstract void SetEndTime(double endTime);

        public abstract TempoMap GetTempoMap();

        public abstract TimelineDataItem OnAddTimelineObject();

        public abstract void OnRemoveTimelineObject(TimelineDataItem removedItem);

        public abstract double GetCurrentPlaybackTime();

        public abstract void SetCurrentPlaybackTime(double time);

        public bool editable;

        public bool changed;
    }
}
