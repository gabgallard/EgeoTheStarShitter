using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Core;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Tools
{
    internal delegate object[] EventParametersGetter(MidiEvent midiEvent, MidiFileCsvConversionSettings settings);
}
