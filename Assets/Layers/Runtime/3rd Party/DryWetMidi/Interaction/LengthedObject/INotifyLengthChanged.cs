using System;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction
{
    public interface INotifyLengthChanged
    {
        #region Events

        event EventHandler<LengthChangedEventArgs> LengthChanged;

        #endregion
    }
}
