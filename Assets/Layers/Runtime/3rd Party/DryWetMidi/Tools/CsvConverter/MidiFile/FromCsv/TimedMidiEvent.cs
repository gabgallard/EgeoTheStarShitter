using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Core;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Tools
{
    internal sealed class TimedMidiEvent
    {
        #region Constructor

        public TimedMidiEvent(ITimeSpan time, MidiEvent midiEvent)
        {
            Time = time;
            Event = midiEvent;
        }

        #endregion

        #region Properties

        public ITimeSpan Time { get; }

        public MidiEvent Event { get; }

        #endregion
    }
}
