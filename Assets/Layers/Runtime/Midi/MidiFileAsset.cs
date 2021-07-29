using System.Collections.Generic;
using System.IO;
using System.Linq;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Core;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using UnityEngine;

namespace ABXY.Layers.Runtime.Midi
{
    [CreateAssetMenu(fileName = "MIDI Asset", menuName = "MIDI Asset")]
    public class MidiFileAsset : ScriptableObject
    {
        [SerializeField]
        private byte[] fileContents;

        [SerializeField]
        private bool _editable = true;

        public bool editable { get { return _editable; } }

        [SerializeField]
        private long _endTime = 0;

        /// <summary>
        /// End time of this asset in ticks
        /// </summary>
        public long endTime { get { return _endTime; } }

        [SerializeField]
        private double _endTimeSeconds = 0;
        public double endTimeSeconds { get { return _endTimeSeconds; } }

        public void LoadBytes(byte[] bytes)
        {
            this.fileContents = bytes;
        }

        public MidiFile GetMidi()
        {
            if (fileContents == null || fileContents.Length == 0)
            {
                MidiFile newMidiFile = new MidiFile(
                    new TrackChunk(
                        new SetTempoEvent(500000)),
                    new TrackChunk());
                _endTime = TimeConverter.ConvertFrom(new BarBeatTicksTimeSpan(1), newMidiFile.GetTempoMap());
                _endTimeSeconds = (TimeConverter.ConvertTo(_endTime, TimeSpanType.Metric, newMidiFile.GetTempoMap()) as MetricTimeSpan).TotalMicroseconds / 1000000f;
                MemoryStream newFileStream = new MemoryStream();
                newMidiFile.Write(newFileStream);
                fileContents = newFileStream.ToArray();

            }
            MemoryStream stream = new MemoryStream();
            stream.Write(fileContents, 0, fileContents.Length);
            stream.Position = 0;
            return MidiFile.Read(stream);
        }

        public void SaveMidi(MidiFile changedFile, TempoMapManager tempoMapManager, List<NotesManager> notesManagers)
        {
#if UNITY_EDITOR
            MemoryStream stream = new MemoryStream();
            changedFile.Write(stream);
            fileContents = stream.ToArray();

            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);

            if (Path.GetExtension(assetPath) != ".asset")
            {
                File.WriteAllBytes(assetPath, fileContents);

            }

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public List<Note> GetNotes()
        {
            MidiFile midiFile = GetMidi();
            return midiFile.GetNotes().ToList();
        }

        public TempoMap GetTempoMap()
        {
            MidiFile midiFile = GetMidi();
            return midiFile.GetTempoMap();
        }
    }
}
