using System.Collections.Generic;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction
{
    internal interface ITempoMapValuesCache
    {
        #region Properties

        IEnumerable<TempoMapLine> InvalidateOnLines { get; }

        #endregion

        #region Methods

        void Invalidate(TempoMap tempoMap);

        #endregion
    }
}
