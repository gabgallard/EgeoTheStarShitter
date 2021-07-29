using ABXY.Layers.Runtime.Nodes.Playback;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor
{
    public class PlaynodePreviewSettings : EditorWindow
    {
    
        private PlayNode targetNode;

        public static void ShowSelector(PlayNode targetNode)
        {
            PlaynodePreviewSettings selectorwindow = EditorWindow.CreateInstance<PlaynodePreviewSettings>();
            selectorwindow.ShowAuxWindow();
            selectorwindow.targetNode = targetNode;
        }

        private void OnGUI()
        {
            if (targetNode == null)
            {
                this.Close();
                return;
            }

            LayersGUIUtilities.DrawDropdown(new GUIContent("Audio Preview"), targetNode.previewAudioType, (newValue) => {
                targetNode.previewAudioType = (PlayNode.AudioPreviewTypes)newValue;
            });

            LayersGUIUtilities.DrawDropdown(new GUIContent("Event Preview"), targetNode.eventPreviewType, (newValue) => {
                targetNode.eventPreviewType = (PlayNode.EventPreviewTypes)newValue;
            });

        

        }

    
    }
}
