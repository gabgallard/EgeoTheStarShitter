using System.IO;
using System.Linq;
using ABXY.Layers.ThirdParty.Melanchall.DryWetMidi.Interaction;
using ABXY.Layers.Runtime.Midi;
using UnityEditor;

using UnityEngine;

namespace ABXY.Layers.Editor.Midi
{
    [UnityEditor.AssetImporters.ScriptedImporter(1, "mid")]
    public class MidiFileImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        [SerializeField]
        private bool editable = false;
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            byte[] contents = File.ReadAllBytes(ctx.assetPath);
            MidiFileAsset midiFile = AssetDatabase.LoadAssetAtPath<MidiFileAsset>(ctx.assetPath);

            long endTime = 0;
            double endTimeSeconds = 0f;
            if (midiFile == null)
            {
                midiFile = ScriptableObject.CreateInstance<MidiFileAsset>();
                midiFile.LoadBytes(contents);


                Note lastNote = midiFile.GetNotes().Last();
                endTime = lastNote.Time + lastNote.Length;
                endTimeSeconds = (TimeConverter.ConvertTo(endTime, TimeSpanType.Metric, midiFile.GetTempoMap()) as MetricTimeSpan).TotalMicroseconds / 1000000f;
            }
            else
            {
                endTime = midiFile.endTime;
                endTimeSeconds = midiFile.endTimeSeconds;
            }
        

            SerializedObject serializedFile = new SerializedObject(midiFile);
            serializedFile.FindProperty("_editable").boolValue = editable;
            serializedFile.FindProperty("_endTime").longValue = endTime;
            serializedFile.FindProperty("_endTimeSeconds").doubleValue = endTimeSeconds;
            serializedFile.ApplyModifiedProperties();

            midiFile.LoadBytes(contents);

            ctx.AddObjectToAsset("main obj", midiFile);
            ctx.SetMainObject(midiFile);
        }
    }
}