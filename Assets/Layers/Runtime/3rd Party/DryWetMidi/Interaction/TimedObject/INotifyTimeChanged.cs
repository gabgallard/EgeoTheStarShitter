using System;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction
{
    public interface INotifyTimeChanged
    {
        #region Events

        event EventHandler<TimeChangedEventArgs> TimeChanged;

        #endregion
    }
}
