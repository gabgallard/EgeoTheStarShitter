using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Composing
{
    internal abstract class StepAction : PatternAction
    {
        #region Constructor

        public StepAction(ITimeSpan step)
        {
            Step = step;
        }

        #endregion

        #region Properties

        public ITimeSpan Step { get; }

        #endregion
    }
}
