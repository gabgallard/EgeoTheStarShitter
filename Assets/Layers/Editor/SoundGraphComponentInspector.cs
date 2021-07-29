using ABXY.Layers.Editor.Node_Editor_Window;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor
{
    [CustomEditor(typeof(SoundGraph))]
    public class SoundGraphComponentInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                (target as SoundGraph).subgraphNode = null;// so playback previews work properly
                SoundGraphEditorWindow.Open(target as NodeGraph);
            }
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("comment"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
