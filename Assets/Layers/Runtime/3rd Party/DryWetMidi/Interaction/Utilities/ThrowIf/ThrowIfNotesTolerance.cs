using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Common;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction
{
    internal static class ThrowIfNotesTolerance
    {
        #region Methods

        internal static void IsNegative(string parameterName, long notesTolerance)
        {
            ThrowIfArgument.IsNegative(parameterName, notesTolerance, "Notes tolerance is negative.");
        }

        #endregion
    }
}
