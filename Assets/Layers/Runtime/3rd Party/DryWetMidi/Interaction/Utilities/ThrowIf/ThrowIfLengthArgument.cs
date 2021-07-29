using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Common;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction
{
    internal static class ThrowIfLengthArgument
    {
        #region Methods

        internal static void IsNegative(string parameterName, long length)
        {
            ThrowIfArgument.IsNegative(parameterName, length, "Length is negative.");
        }

        #endregion
    }
}
