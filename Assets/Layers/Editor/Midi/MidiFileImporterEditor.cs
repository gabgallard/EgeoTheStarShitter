using ABXY.Layers.Editor.Timeline_Editor.Variants.Midi;
using ABXY.Layers.Runtime.Midi;
using UnityEditor;

using UnityEngine;

namespace ABXY.Layers.Editor.Midi
{
    [CustomEditor(typeof(MidiFileImporter))]
    public class MidiFileImporterEditor : UnityEditor.AssetImporters.ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("editable"));
            if (GUILayout.Button("Open"))
            {
                MidiTimelineWindow.ShowMIDITimeline(AssetDatabase.LoadAssetAtPath((target as MidiFileImporter).assetPath, typeof(MidiFileAsset)) as MidiFileAsset);
                //PianoRollWindow.ShowPianoRoll(AssetDatabase.LoadAssetAtPath((target as MidiFileImporter).assetPath, typeof(MidiFileAsset)) as MidiFileAsset);
            }
            base.ApplyRevertGUI();
        }
    }
}