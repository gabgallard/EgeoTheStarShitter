namespace ABXY.Layers.Runtime.Timeline
{
    [System.Serializable]
    public abstract class TimelineDataItem
    {
        public virtual double startTime { get; set; }
        public virtual double length { get; set; }

        public virtual int rowNumber { get; set; }

        public virtual int rowNumberOffset { get; set; }

        public virtual bool resizableInInterface { get; }

        public virtual double interiorStartTime { get; set; }

        public enum TimelineItemType { Normal, Momentary, Ranged}
        public virtual TimelineItemType rangeTypes { get; }

        public virtual void ApplyTransformations()
        {

        }

        public virtual void OnDestroy()
        {

        }

        public abstract TimelineDataItem Copy();
    }
}
