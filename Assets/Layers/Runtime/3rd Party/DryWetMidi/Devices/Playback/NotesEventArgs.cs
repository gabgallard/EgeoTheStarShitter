using System;
using System.Collections.Generic;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Devices
{
    /// <summary>
    /// Holds notes collection for <see cref="Playback.NotesPlaybackStarted"/> and
    /// <see cref="Playback.NotesPlaybackFinished"/>.
    /// </summary>
    public sealed class NotesEventArgs : EventArgs
    {
        #region Constructor

        internal NotesEventArgs(params Note[] notes)
        {
            Notes = notes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets notes collection that started or finished to play by a <see cref="Playback"/>.
        /// </summary>
        public IEnumerable<Note> Notes { get; }

        #endregion
    }
}
