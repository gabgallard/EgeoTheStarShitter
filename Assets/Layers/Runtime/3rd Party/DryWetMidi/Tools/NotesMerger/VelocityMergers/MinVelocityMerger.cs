using System;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Common;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Tools
{
    internal sealed class MinVelocityMerger : VelocityMerger
    {
        #region Overrides

        public override void Merge(SevenBitNumber velocity)
        {
            _velocity = (SevenBitNumber)Math.Min(_velocity, velocity);
        }

        #endregion
    }
}
