using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using UnityEngine;

namespace ABXY.Layers.Runtime.Timeline
{
    [System.Serializable]
    public class TimeSignatureDataItem 
    {
        [SerializeField]
        private int backingNumerator = 4;
        public int Numerator {
            get
            {
                return backingNumerator;
            }
            set
            {
                tsObject = new TimeSignature(value, Denominator);
                backingNumerator = value;
            }
        }

        private int backingDenominator = 4;
        public int Denominator
        {
            get
            {
                return backingDenominator;
            }
            set
            {
                tsObject = new TimeSignature(Numerator, value);
                backingDenominator = value;
            }
        }

        public double time;

        private TimeSignature tsObject;

        public TimeSignatureDataItem(double time, TimeSignature timeSignature)
        {
            tsObject = timeSignature;
            backingNumerator = timeSignature.Numerator;
            backingDenominator = timeSignature.Denominator;
            this.time = time;
        }

        public TimeSignatureDataItem()
        {
            tsObject = TimeSignature.Default;
        }

        public TimeSignature ToTimeSignature()
        {
            if (tsObject == null)
            {
                tsObject = new TimeSignature(backingNumerator, backingDenominator);
            }
            return tsObject;
        }
    }
}
