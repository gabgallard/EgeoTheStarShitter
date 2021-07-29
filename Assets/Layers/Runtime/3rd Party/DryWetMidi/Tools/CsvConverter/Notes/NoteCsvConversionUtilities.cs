using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Common;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Tools
{
    internal static class NoteCsvConversionUtilities
    {
        #region Methods

        public static object FormatNoteNumber(SevenBitNumber noteNumber, NoteNumberFormat noteNumberFormat)
        {
            switch (noteNumberFormat)
            {
                case NoteNumberFormat.NoteNumber:
                    return noteNumber;
                case NoteNumberFormat.Letter:
                    return MusicTheory.Note.Get(noteNumber);
            }

            return null;
        }

        #endregion
    }
}
