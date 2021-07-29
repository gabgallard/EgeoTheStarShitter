using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.Timeline_Editor.Structure;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Core;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using ABXY.Layers.Runtime.Midi;
using ABXY.Layers.Runtime.Timeline;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.Midi
{
    public class MidiDataSource : TimeLineDataSource
    {
        public MidiFileAsset midiAsset { get; private set; }

        private TempoMapManager tempoMapManager = null;
        private MidiFile midiFile;

        private List<TimeLineRowDataItem> rows = new List<TimeLineRowDataItem>( new TimeLineRowDataItem[128]);

        private List<TimelineDataItem> dataItems = new List<TimelineDataItem>();

        private List<TimeSignatureDataItem> timeSignatures = new List<TimeSignatureDataItem>();
        private List<BPMDataItem> bpms = new List<BPMDataItem>();

        private BPMDataItem defaultBPM;

        private TimeSignatureDataItem defaultTS;

        private List<NotesManager> notesManagers = new List<NotesManager>();
        private NotesManager defaultNotesManager { get { return notesManagers.FirstOrDefault(); } }

        private double endTime;

        public MidiDataSource(MidiFileAsset midiAsset)
        {
            this.midiAsset = midiAsset;
            midiFile = midiAsset.GetMidi();
            editable = midiAsset.editable;

            this.tempoMapManager = midiFile.ManageTempoMap();
            for (int index = 0; index < rows.Count; index++)
                rows[index] = new TimeLineRowDataItem();
            endTime = midiAsset.endTimeSeconds;

            foreach (TrackChunk chunk in midiFile.GetTrackChunks())
            {
                NotesManager notesManager = chunk.ManageNotes();
                notesManagers.Add(notesManager);
                foreach(Note note in notesManager.Notes)
                {
                    dataItems.Add(new MidiDataItem(note, notesManager));
                }
            }

            timeSignatures = tempoMapManager.TempoMap.TimeSignature.Where(x=>x.Time != 0).Select(x => new TimeSignatureDataItem(x.Time, x.Value)).ToList();
            bpms = tempoMapManager.TempoMap.Tempo.Where(x => x.Time != 0).Select(x => new BPMDataItem(x.Time, x.Value)).ToList();

            defaultBPM = new BPMDataItem(tempoMapManager.TempoMap.Tempo.AtTime(0));
            defaultTS = new TimeSignatureDataItem(0, tempoMapManager.TempoMap.TimeSignature.AtTime(0));
        }

    

        public override BPMDataItem GetDefaultBPM()
        {
            return defaultBPM;
        }

        public override void SetDefaultBPM(BPMDataItem defaultBPM)
        {
            this.defaultBPM = defaultBPM;
        
            RebuildTempoMap();
        }

        public override TimeSignatureDataItem GetDefaultTimeSignature()
        {
            return defaultTS;
        }

        public override void SetDefaultTimeSignature(TimeSignatureDataItem defaultTimeSignature)
        {
            this.defaultTS = defaultTimeSignature;
            RebuildTempoMap();
        }


        public override List<TimelineDataItem> GetTimelineDataItems()
        {
            return dataItems;
        }

        public override void ApplyTimelineDataChanges(List<TimelineDataItem> items)
        {
            dataItems = items;
        }

        public override List<TimeSignatureDataItem> GetTimeSignatureItems()
        {
            return timeSignatures;
        }

    

   

        public override void SetTimeSignatureChanges(List<TimeSignatureDataItem> timeSignatures)
        {
            this.timeSignatures = timeSignatures;

            RebuildTempoMap();
        }

        public override List<TimeLineRowDataItem> GetDataRows()
        {
            return rows;
        }

        public override void SetDataRows(List<TimeLineRowDataItem> rows)
        {
        
        }

        public override double GetEndTime()
        {
            return endTime;
        }

        public override void SetEndTime(double endTime)
        {
            this.endTime = endTime;
        }

        public override TempoMap GetTempoMap()
        {
            return tempoMapManager.TempoMap;
        }

        public override List<BPMDataItem> GetBPMItems()
        {
            return bpms;
        }

        public override void SetBPMItems(List<BPMDataItem> items)
        {
            bpms = items;
            RebuildTempoMap();
        }

        private void RebuildTempoMap()
        {
            tempoMapManager.ReplaceTempoMap(TempoMap.Create(new TicksPerQuarterNoteTimeDivision((tempoMapManager.TempoMap.TimeDivision as TicksPerQuarterNoteTimeDivision).TicksPerQuarterNote)
                , GetDefaultBPM().ToTempo(), GetDefaultTimeSignature().ToTimeSignature()));

            foreach (TimeSignatureDataItem item in timeSignatures)
            {
                if (item.time != 0)
                    tempoMapManager.SetTimeSignature((long)item.time, item.ToTimeSignature());
            }

            foreach(BPMDataItem item in bpms)
            {
                if (item.time != 0)
                    tempoMapManager.SetTempo((long)item.time, item.ToTempo());
            }
        }


        public void DoSave()
        {
            //OnMenu_Create();
            changed = false;
            //tempoMapManager.Dispose();
            tempoMapManager.Dispose();
            notesManagers.ForEach(x => x.Dispose());


            tempoMapManager = midiFile.ManageTempoMap();

            RebuildTempoMap();

            tempoMapManager.Dispose();

            SerializedObject so = new SerializedObject(midiAsset);
            so.FindProperty("_endTime").longValue = (long)endTime * 1000000;

            so.FindProperty("_endTimeSeconds").doubleValue = endTime;
            so.ApplyModifiedProperties();

            midiAsset.SaveMidi(midiFile, tempoMapManager, notesManagers);
        }

        public override TimelineDataItem OnAddTimelineObject()
        {
            Note note = new Note((ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Common.SevenBitNumber)0);
            defaultNotesManager.Notes.Add(note);
            TimelineDataItem dataItem = new MidiDataItem(note, defaultNotesManager);
            dataItems.Add(dataItem);
            return dataItem;
        }

        public override void OnRemoveTimelineObject(TimelineDataItem removedItem)
        {
            (removedItem as MidiDataItem).Delete();
            dataItems.Remove(removedItem);
        }

        public void Dispose()
        {
            foreach (NotesManager manager in notesManagers)
            {
                manager.Dispose();
            }
            tempoMapManager.Dispose();
        }

        private double currentPlaybackTime = 0;
        public override double GetCurrentPlaybackTime()
        {
            return currentPlaybackTime;//TODO replace stub
        }

        public override void SetCurrentPlaybackTime(double time)
        {
            currentPlaybackTime = Mathf.Clamp((float)time, 0f, float.MaxValue);
        }
    }
}
