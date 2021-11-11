using System.IO;
using System.Text.RegularExpressions;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.ThirdParty.Malee.List;
using ABXY.Layers.ThirdParty.MidiJack;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes.Playback;
using UnityEditor;
using UnityEngine;
using ABXY.Layers.Runtime.Nodes;

namespace ABXY.Layers.Editor.Node_Editors.Playback
{
    [NodeEditor.CustomNodeEditor(typeof(SamplerTrackNode))]
    public class SamplerTrackNodeEditor : FlowNodeEditor
    {
        ReorderableList noteClipList;
        public override void OnCreate()
        {
            base.OnCreate();
            noteClipList = new ReorderableList(serializedObjectTree.FindProperty("samples"));
            noteClipList.pageSize = 12;
            noteClipList.paginate = true;
            noteClipList.drawElementCallback += DrawElementDelegate;
            noteClipList.getElementHeightCallback += GetElementHeight;
            noteClipList.onAddCallback += OnAdd;
            noteClipList.onRemoveCallback += OnRemove;
        }

        private void OnRemove(ReorderableList list)
        {
            list.Remove(list.Selected);
            listeningForNextKey = false;
            noteNumberProp = null;
        }

        private void OnAdd(ReorderableList list)
        {
            list.AddItem();
            listeningForNextKey = false;
            noteNumberProp = null;
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            serializedObjectTree.UpdateIfRequiredOrScript();
            //NodeEditorGUILayout.PortPair(target.GetInputPort("MidiIn"), target.GetOutputPort("AudioOut"));

            NodeEditorGUIDraw.PortField(layout.DrawLine(),new GUIContent("MIDI In"), target.GetInputPort("MidiIn"), serializedObjectTree);
            Rect audioOutRect = layout.LastRect();
            audioOutRect = new Rect(audioOutRect.x + (audioOutRect.width / 2f), audioOutRect.y, audioOutRect.width / 2f, audioOutRect.height);
            DrawAudioOutSelector(audioOutRect, new GUIContent("Audio out"), target.GetOutputPort("AudioOut"), serializedObjectTree.FindProperty("audioOutSendID"), 5f);

            LayersGUIUtilities.IncomingParameterSelector(layout.DrawLine(), target as FlowNode, serializedObjectTree.FindProperty("parameterName"), "MidiIn", typeof(MidiData));

            if (serializedObjectTree.FindProperty("expanded").boolValue)
            {
                if (GUI.Button(layout.DrawLine(), "Load folder"))
                {
                    string result = EditorUtility.OpenFolderPanel("Media folder", Application.dataPath, "");
                    foreach (string filePath in Directory.EnumerateFiles(result))
                    {
                        if (filePath.StartsWith(Application.dataPath))
                        {
                            string truncatedFilePath = filePath.Substring(Application.dataPath.Length - 6, filePath.Length - Application.dataPath.Length + 6);
                            truncatedFilePath = truncatedFilePath.Replace("\\", "/");
                            AudioClip clip = (AudioClip)AssetDatabase.LoadAssetAtPath(truncatedFilePath, typeof(AudioClip));

                            if (clip != null)
                            {
                                int noteNum = 0;
                                Match match = Regex.Match(Path.GetFileName(truncatedFilePath), "[0-9]+");
                                if (match.Success)
                                    int.TryParse(match.Value, out noteNum);

                                SerializedProperty newClipProp = noteClipList.AddItem();
                                newClipProp.FindPropertyRelative("keyNum").intValue = noteNum;
                                newClipProp.FindPropertyRelative("audioClip").objectReferenceValue = clip;
                            }
                        }
                    }
                }

                noteClipList.DoList(layout.Draw(noteClipList.GetHeight()), new GUIContent("Samples"));
            }
            serializedObjectTree.ApplyModifiedProperties();
        }

        private void DrawElementDelegate(Rect rect, SerializedPropertyTree element, GUIContent label, bool selected, bool focused)
        {
            float labelSize = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = rect.width/2f;

            SerializedPropertyTree noteNumberProp = element.FindPropertyRelative("keyNum");
            Rect controlRect = new Rect(rect.x, rect.y, rect.width - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);
            Rect numberRect = new Rect(controlRect.x, controlRect.y, controlRect.width - 35, controlRect.height);
            noteNumberProp.intValue = EditorGUI.IntField(numberRect, new GUIContent("Note"), noteNumberProp.intValue);

            EditorGUI.BeginDisabledGroup(true);
            Rect noteRect = new Rect(controlRect.x + controlRect.width - 35, controlRect.y, 35, controlRect.height);
            EditorGUI.TextField(noteRect, MidiUtils.NoteNumberToName(noteNumberProp.intValue));
            EditorGUI.EndDisabledGroup();

            // key listening
            EditorGUI.BeginDisabledGroup(listeningForNextKey && noteNumberProp.propertyPath != this.noteNumberProp.propertyPath);
            Rect listenButtonRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, rect.width, EditorGUIUtility.singleLineHeight);
            listeningForNextKey = GUI.Toggle(listenButtonRect, listeningForNextKey, "Listen for key","Button");
            if (listeningForNextKey)
            {
                int increment = MidiDriver.Instance.TotalMessageCount;//Just forcing an update
            }

            if (wasListeningForNextKey != listeningForNextKey && listeningForNextKey)
            {
                this.noteNumberProp = noteNumberProp;
                MidiMaster.noteOnDelegate += OnKeyPressed;
            }
            else if (wasListeningForNextKey != listeningForNextKey && !listeningForNextKey)
            {
                this.noteNumberProp = null;
                MidiMaster.noteOnDelegate -= OnKeyPressed;
            }
            wasListeningForNextKey = listeningForNextKey;
            EditorGUI.EndDisabledGroup();



            Rect audioRect = new Rect(rect.x, rect.y + 2f*EditorGUIUtility.singleLineHeight + 2f*EditorGUIUtility.standardVerticalSpacing, rect.width - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(audioRect, element.FindPropertyRelative("audioClip"));
            EditorGUIUtility.labelWidth = labelSize;
        }

        private float GetElementHeight(SerializedPropertyTree element)
        {
            return 3f * EditorGUIUtility.singleLineHeight + 4f * EditorGUIUtility.standardVerticalSpacing;
        }

        public override int GetWidth()
        {
            return 250;
        }

        private bool listeningForNextKey = false;
        private bool wasListeningForNextKey = false;
        SerializedProperty noteNumberProp;
        private void OnKeyPressed(MidiChannel channel, int noteNumber, float velocity)
        {
            MidiMaster.noteOnDelegate -= OnKeyPressed;
            listeningForNextKey = false;
            wasListeningForNextKey = false;
            noteNumberProp.intValue = noteNumber;
            serializedObjectTree.ApplyModifiedProperties();
        }

        protected override bool CanExpand()
        {
            return true;
        }
    }
}
