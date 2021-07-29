using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using UnityEngine;

namespace ABXY.Layers.Runtime.Timeline
{
    [System.Serializable]
    public class BPMDataItem
    {
        public double time;

        [SerializeField]
        private long backingBPM = 120;
        public long bpm { 
            get {
                return backingBPM;
            }
            set
            {
                tempoObject = Tempo.FromBeatsPerMinute((int)value);
                backingBPM = value;
            }
        }

        private Tempo tempoObject;

        public BPMDataItem(Tempo tempo)
        {
            this.tempoObject = tempo;
            this.backingBPM = tempo.BeatsPerMinute;
        }

        public BPMDataItem(double time, Tempo tempo)
        {
            this.tempoObject = tempo;
            this.time = time;
            this.backingBPM = tempo.BeatsPerMinute;
        }

        public BPMDataItem()
        {
            this.tempoObject = Tempo.Default;
        }

        public Tempo ToTempo()
        {
            if (tempoObject == null)
                tempoObject = Tempo.FromBeatsPerMinute((int)backingBPM);
            return tempoObject;
        }
    }
}
