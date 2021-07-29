using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Tools
{
    /// <summary>
    /// Settings according to which notes should be randomized.
    /// </summary>
    public sealed class NotesRandomizingSettings : LengthedObjectsRandomizingSettings<Note>
    {
    }

    /// <summary>
    /// Provides methods to randomize notes time.
    /// </summary>
    public sealed class NotesRandomizer : LengthedObjectsRandomizer<Note, NotesRandomizingSettings>
    {
    }
}
