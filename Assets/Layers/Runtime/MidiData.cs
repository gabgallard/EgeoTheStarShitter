//using ABXY.Layers.ThirdParty.MidiJack;

namespace ABXY.Layers.Runtime
{
    public class MidiData
    {//TODO: Limit all of these
        public int noteNumber;
        public MidiChannel channelNumber;
        public float velocity;

        public enum MidiChannel
        {
            Ch1,    // 0
            Ch2,    // 1
            Ch3,
            Ch4,
            Ch5,
            Ch6,
            Ch7,
            Ch8,
            Ch9,
            Ch10,
            Ch11,
            Ch12,
            Ch13,
            Ch14,
            Ch15,
            Ch16,
            All     // 16
        }

        public static MidiData defaultMidiFlowInfo
        {
            get
            {
                return new MidiData(0, 0, 0f);
            }
        }

        public MidiData(int noteNumber, MidiChannel channelNumber, float velocity)
        {
            this.noteNumber = noteNumber;
            this.channelNumber = channelNumber;
            this.velocity = velocity;
        }

        public MidiData()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2} ", MidiUtils.NoteNumberToName(noteNumber), channelNumber, velocity);
        }


    }
}
