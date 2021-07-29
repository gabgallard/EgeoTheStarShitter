using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Common;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Timeline;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.Midi
{
    public class MidiDataItem : TimelineDataItem
    {

        private long underlyingLength;
        public override double length {
            get
            {
                return underlyingLength;
            }
            set
            {

                double newValue = Mathf.Clamp((float)value, 0, long.MaxValue);
                underlyingNote.Length = (long)newValue;
                underlyingLength = (long)newValue;
            }
        }

        public override bool resizableInInterface => true;
        public override int rowNumber {
            get => (int)Mathf.Clamp(128 - underlyingNote.NoteNumber + offset,0,127);
            set => underlyingNote.NoteNumber = (ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Common.SevenBitNumber)Mathf.Clamp( 128 - value,0,127); 
        }

        private long underlyingTime;
        public override double startTime {
            get
            {
                return underlyingTime;
            }
            set
            {
                double newValue = Mathf.Clamp((float)value, 0, long.MaxValue);
                underlyingNote.Time = (long)newValue;
                underlyingTime = (long)newValue;
            }
        }

        private int offset;
        public override int rowNumberOffset { get => offset; set => offset = value; }

        public MidiUtils.channels channelNumber
        {
            get
            {
                return (MidiUtils.channels)((int)underlyingNote.Channel);
            }
        }

        public Note underlyingNote { get; private set; }


        public SevenBitNumber velocity
        {
            get
            {
                return underlyingNote.Velocity;
            }
            set
            {
                underlyingNote.Velocity = value;
            }
        }

        NotesManager sourceNotesManager;

        public MidiDataItem(Note note, NotesManager sourceNotesManager)
        {
            underlyingNote = note;
            underlyingLength = note.Length;
            underlyingTime = note.Time;
            this.sourceNotesManager = sourceNotesManager;
        }

        public override void ApplyTransformations()
        {
            underlyingNote.NoteNumber = (SevenBitNumber)(Mathf.Clamp( underlyingNote.NoteNumber - offset,0,int.MaxValue));
            offset = 0;
        }

        public void Delete()
        {
            sourceNotesManager.Notes.Remove(underlyingNote);
        }

        public override TimelineDataItem Copy()
        {
            throw new System.NotImplementedException();
        }
    }
}
