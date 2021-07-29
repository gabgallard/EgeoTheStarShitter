using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Tools
{
    /// <summary>
    /// Settings according to which notes should be quantized.
    /// </summary>
    public class NotesQuantizingSettings : LengthedObjectsQuantizingSettings<Note>
    {
    }

    /// <summary>
    /// Provides methods to quantize notes time.
    /// </summary>
    /// <remarks>
    /// See <see href="xref:a_quantizer">Quantizer</see> article on Wiki to learn more.
    /// </remarks>
    public class NotesQuantizer : LengthedObjectsQuantizer<Note, NotesQuantizingSettings>
    {
    }
}
