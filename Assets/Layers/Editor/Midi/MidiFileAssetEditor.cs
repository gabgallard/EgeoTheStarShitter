using ABXY.Layers.Editor.Timeline_Editor.Variants.Midi;
using ABXY.Layers.Runtime.Midi;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Midi
{
    [CustomEditor(typeof(MidiFileAsset))]
    public class MidiFileAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(false);
            if (GUILayout.Button("Open"))
            {
                MidiTimelineWindow.ShowMIDITimeline(target as MidiFileAsset);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
