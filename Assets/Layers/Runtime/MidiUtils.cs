using System.Collections.Generic;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using UnityEngine;

namespace ABXY.Layers.Runtime
{
    public static class MidiUtils
    {

        public enum channels { Channel1 , Channel2 , Channel3 , Channel4 , Channel5 , Channel6 , Channel7 , Channel8 , Channel9 , Channel10 , Channel11 , Channel12 , Channel13 , Channel14 , Channel15 , Channel16 , AllChannels }

        public static string[] gridLabels = new string[] { "1\u2215128", "1\u221564", "1\u221532", "1\u221516", "1\u22158", "1\u22154", "1\u22152", "1", "Bar", "Beat"};

        public static float[] gridDivisor = new float[] {32, 16, 8, 4, 2,1, 0.5f, 0.25f,0.125f, 0.0625f };

        public static ITimeSpan[] gridTimeSpans = 
            new ITimeSpan[] {new MusicalTimeSpan(128), MusicalTimeSpan.SixtyFourth, MusicalTimeSpan.ThirtySecond, MusicalTimeSpan.Sixteenth, MusicalTimeSpan.Eighth, MusicalTimeSpan.Quarter, MusicalTimeSpan.Half, MusicalTimeSpan.Whole, new BarBeatFractionTimeSpan(1), new BarBeatFractionTimeSpan(0,1) };

        private static List<string> noteLetters = new List<string>(new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" });

        public static string NoteNumberToName(int number)
        {
            int realIndex = number - 24;
            int keyNumber = Mathf.FloorToInt((number-12) / 12f);
            return noteLetters[(int)Mathf.Repeat(realIndex, 12)] + keyNumber.ToString();
        }

        public static string NoteNumberToMenuFormattedName(int number)
        {
            int realIndex = number - 24;
            int keyNumber = Mathf.FloorToInt((number - 12) / 12f);
            return $"Octave {keyNumber.ToString()}/{noteLetters[(int)Mathf.Repeat(realIndex, 12)]}" ;
        }

        public static int NameToNoteNumber(string name)
        {
            bool isSharp = name.Substring(1, 1) == "#";
            int letterNumber = noteLetters.IndexOf(isSharp? name.Substring(0, 2): name.Substring(0, 1));
            int keyNumber = isSharp ? int.Parse(name.Substring(2, name.Length - 2)): int.Parse(name.Substring(1, name.Length - 1));

            int notenumber = letterNumber +(keyNumber*12) + 12 ;
            return notenumber;
        }

        public static bool IsSharp(int number)
        {
            return IsSharp(NoteNumberToName(number));
        }

        public static bool IsSharp(string noteName)
        {
            bool isSharp = noteName.Substring(1, 1) == "#";
            return isSharp;
        }
    }
}
