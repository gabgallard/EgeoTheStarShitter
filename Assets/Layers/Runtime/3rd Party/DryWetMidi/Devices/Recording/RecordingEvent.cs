using System;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Core;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Devices
{
    internal sealed class RecordingEvent
    {
        #region Constructor

        public RecordingEvent(MidiEvent midiEvent, TimeSpan time)
        {
            Event = midiEvent;
            Time = time;
        }

        #endregion

        #region Properties

        public MidiEvent Event { get; }

        public TimeSpan Time { get; }

        #endregion
    }
}
