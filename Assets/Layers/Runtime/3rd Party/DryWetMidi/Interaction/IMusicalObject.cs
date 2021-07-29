using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Common;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction
{
    /// <summary>
    /// Musical objects that can be played.
    /// </summary>
    public interface IMusicalObject
    {
        #region Properties

        /// <summary>
        /// Gets the channel which should be used to play an object.
        /// </summary>
        FourBitNumber Channel { get; }

        #endregion
    }
}
