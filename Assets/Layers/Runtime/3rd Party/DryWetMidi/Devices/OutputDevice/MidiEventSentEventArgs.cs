using System;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Core;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Devices
{
    /// <summary>
    /// Provides data for the <see cref="IOutputDevice.EventSent"/> event.
    /// </summary>
    public sealed class MidiEventSentEventArgs : EventArgs
    {
        #region Constructor

        public MidiEventSentEventArgs(MidiEvent midiEvent)
        {
            Event = midiEvent;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets MIDI event sent to <see cref="IOutputDevice"/>.
        /// </summary>
        public MidiEvent Event { get; }

        #endregion
    }
}
