using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Midi;
using ABXY.Layers.Runtime.Nodes.Playback;
using UnityEngine;

namespace ABXY.Layers.Runtime.Timeline.Playnode
{
    [System.Serializable]
    public class PlaynodeDataItem : TimelineDataItem
    {
        [SerializeField]
        private Object backingObject;

        [SerializeField]
        private int _rowNumber;
        public override int rowNumber { 
            get => Mathf.Clamp(_rowNumber + rowNumberOffset, 0, int.MaxValue); 
            set => _rowNumber = Mathf.Clamp(value, 0 , int.MaxValue); 
        }

        public override bool resizableInInterface => playnodeDataItemType != PlaynodeDataItemTypes.Event && !(exposed && oneShot);

        [SerializeField]
        public bool exposed;

        /// <summary>
        /// When exposed, input items can either loop over the given time, or play once. This controls which option is used
        /// </summary>
        [SerializeField]
        public bool oneShot = false;

        //public override bool momentary => isEventItem;
        public override TimelineItemType rangeTypes {
            get
            {
                if (playnodeDataItemType == PlaynodeDataItemTypes.Audio && exposed && oneShot)
                    return TimelineItemType.Momentary;

                if (playnodeDataItemType == PlaynodeDataItemTypes.MIDI && exposed && oneShot)
                    return TimelineItemType.Momentary;

                switch (playnodeDataItemType)
                {
                    case PlaynodeDataItemTypes.Audio:
                        return TimelineItemType.Normal;
                    case PlaynodeDataItemTypes.MIDI:
                        return TimelineItemType.Normal;
                    case PlaynodeDataItemTypes.Event:
                        return TimelineItemType.Momentary;
                    case PlaynodeDataItemTypes.Loop:
                        return TimelineItemType.Ranged;
                }
                return TimelineItemType.Normal;
            }
        }//=> playnodeDataItemType == PlaynodeDataItemTypes.Event? TimelineItemType.Momentary: TimelineItemType.Normal;

        public int numberRepetitions
        {
            get
            {
                if (DataSourceLength - interiorStartTime == 0)
                    return 1;
                return Mathf.CeilToInt((float)(length/(DataSourceLength - interiorStartTime)));
            }
        }

        public double DataSourceLength
        {
            get
            {
                if (exposed && oneShot)
                {
                    return length;
                }

                if (backingObject == null)
                return length;

                if (backingObject.GetType() == typeof(MidiFileAsset))
                    return (backingObject as MidiFileAsset).endTimeSeconds;
                else
                    return (backingObject as AudioClip).length;
            }
        }

        [SerializeField]
        private double _length = -1;
        public override double length {
            get {
                if (exposed && oneShot)
                {
                    if (_length == -1) // then unitialized
                        _length = 5;
                    object backingObject = GetBackingObject();
                    if (backingObject is MidiFileAsset)
                        return (backingObject as MidiFileAsset).endTimeSeconds;
                    else if (backingObject is AudioClip)
                        return (backingObject as AudioClip).length;
                    return 0;
                }

                if (_length == -1) //then uninitialized. Just doing this so I don't kill my old graphs
                    _length = DataSourceLength;

                

                return _length;
            }
            set {
                _length = value;
            }
        }

        [SerializeField]
        private double _startTime;
        public override double startTime {
            get => _startTime;
            set => _startTime = Mathf.Clamp((float)value, 0, float.MaxValue); }

        [SerializeField]
        private double _interiorStartTime;
        public override double interiorStartTime {
            get {
                if (exposed && oneShot)
                    return 0;

                return Mathf.Repeat((float)_interiorStartTime, (float)DataSourceLength); ; 
            }
            set {
                value = Mathf.Repeat((float)value, (float)DataSourceLength);
                _interiorStartTime = value; 
            }
        }


        public enum PlaynodeDataItemTypes { Audio, MIDI, Event, Loop}
        // event item data
        [SerializeField]
        private PlaynodeDataItemTypes _playnodeDataItemTypes = PlaynodeDataItemTypes.Audio;
        public PlaynodeDataItemTypes playnodeDataItemType { get { return _playnodeDataItemTypes; } }

        [SerializeField]
        public string eventLabel="Event";
        [SerializeField]
        public List<GraphVariable> eventParameters = new List<GraphVariable>();

        [SerializeField]
        private string _itemID = "";

        public string itemID
        {
            get
            {
                return _itemID;
            }
        }

        [SerializeField]
        private PlayNode owningPlayNode;

        private PlaynodeDataItem()
        {

        }

        public PlaynodeDataItem(double time, PlaynodeDataItemTypes dataType, PlayNode owningPlayNode)
        {
            this._playnodeDataItemTypes = dataType;
            this.startTime = time;
            _itemID = System.Guid.NewGuid().ToString();
            if (dataType == PlaynodeDataItemTypes.Event)
                this.backingObject = Resources.Load("Layers/Single Beat");
            else if (dataType == PlaynodeDataItemTypes.Loop)
            {
                this.backingObject = Resources.Load("Layers/Empty MIDI File");
                eventLabel = "Loop";
                length = 1;
                owningPlayNode.AddDynamicInput(typeof(bool), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, itemID);
            }else if (dataType == PlaynodeDataItemTypes.Audio)
                owningPlayNode?.AddDynamicInput(typeof(AudioClip), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, itemID);
            else if (dataType == PlaynodeDataItemTypes.MIDI)
                owningPlayNode?.AddDynamicInput(typeof(MidiFileAsset), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, itemID);

            this.owningPlayNode = owningPlayNode;
        }
        public PlaynodeDataItem(double time, MidiFileAsset midiFile, PlayNode owningPlayNode)
        {
            backingObject = midiFile;
            this.startTime = time;
            this._length = DataSourceLength;
            _playnodeDataItemTypes = PlaynodeDataItemTypes.MIDI;
            _itemID = System.Guid.NewGuid().ToString();
            this.owningPlayNode = owningPlayNode;
            eventLabel = "Midi Timeline Item";
            owningPlayNode?.AddDynamicInput(typeof(MidiFileAsset), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, itemID);
        }

        public PlaynodeDataItem(double time, AudioClip audiofile, PlayNode owningPlayNode)
        {
            backingObject = audiofile;
            this.startTime = time;
            this._length = DataSourceLength;
            _playnodeDataItemTypes = PlaynodeDataItemTypes.Audio;
            _itemID = System.Guid.NewGuid().ToString();
            this.owningPlayNode = owningPlayNode;
            eventLabel = "Audio Timeline Item";
            owningPlayNode?.AddDynamicInput(typeof(AudioClip), Node.ConnectionType.Override, Node.TypeConstraint.Inherited, itemID);
        }

        public string dataName { 
            get {
                if (exposed)
                    return eventLabel + " <Exposed>";
                else if (backingObject == null )
                    return "<EMPTY>";
                return backingObject.name; 
            } 
        }

   

        public object GetBackingObject()
        {
            if (exposed)
                return owningPlayNode.GetInputValue(itemID, null);
            else
                return backingObject;
        }

        public override void ApplyTransformations()
        {
            rowNumber = _rowNumber + rowNumberOffset;
            rowNumberOffset = 0;
        }

        public override void OnDestroy()
        {
            if (owningPlayNode != null && owningPlayNode.GetInputPort(itemID) != null)
                owningPlayNode?.RemoveDynamicPort(itemID);
        }

        public override TimelineDataItem Copy()
        {
            ApplyTransformations();
            PlaynodeDataItem newItem = new PlaynodeDataItem();
            newItem.backingObject = backingObject;
            newItem.eventLabel = eventLabel;
            newItem.eventParameters = new List<GraphVariable>();
            foreach (GraphVariable variable in eventParameters)
                newItem.eventParameters.Add((GraphVariable)variable.FullCopy());
            newItem.exposed = exposed;
            newItem.interiorStartTime = interiorStartTime;
            newItem._itemID = System.Guid.NewGuid().ToString();
            newItem._length = _length;
            newItem._playnodeDataItemTypes = _playnodeDataItemTypes;
            newItem._rowNumber = _rowNumber;
            newItem._startTime = _startTime;
            return newItem;
        }
    }
}
