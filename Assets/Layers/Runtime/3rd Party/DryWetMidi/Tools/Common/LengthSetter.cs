using System;
using System.Collections.Generic;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;

namespace ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Tools
{
    internal static class LengthSetter
    {
        #region Constants

        private static readonly Dictionary<Type, Action<ILengthedObject, long>> LengthSetters =
            new Dictionary<Type, Action<ILengthedObject, long>>
            {
                [typeof(Note)] = (obj, length) => ((Note)obj).Length = length,
                [typeof(Chord)] = (obj, length) => ((Chord)obj).Length = length
            };

        #endregion

        #region Methods

        public static void SetObjectLength<TObject>(TObject obj, long time)
            where TObject : ILengthedObject
        {
            LengthSetters[obj.GetType()](obj, time);
        }

        #endregion
    }
}
