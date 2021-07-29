using System.IO;
using System.Text.RegularExpressions;
using ABXY.Layers.Editor.ThirdParty.Editor.Scripts;
using ABXY.Layers.Runtime.Nodes;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editors
{
    [CustomEditor(typeof(FlowNode), true)]
    public class FlowNodeHelpInspector : UnityEditor.Editor
    {
        MarkdownViewer mViewer;
        public GUISkin Skin;
        private void OnEnable()
        {
            string helpFilePath = GetHelpFileResourcePath();
            Object target = Resources.Load(helpFilePath);
            if (target == null)
                return;

            string fullPath = AssetDatabase.GetAssetPath(target);
            string content = (target as TextAsset).text;
            //removing headers
            content = Regex.Replace(content, "(?<!-)---[\r\n]+[a-zA-Z:./-]+[\r\n]+---(?!-)", "");

            //Fixing image paths
            content = Regex.Replace(content, "/IMG/", "../../IMG/");

            if (File.Exists(fullPath))
                mViewer = new MarkdownViewer(Skin, fullPath, content);
        }
        public override void OnInspectorGUI()
        {
#if SYMPHONY_DEV
        DrawDefaultInspector();
#else

            mViewer?.Draw();

            mViewer?.Update();

#endif
        }

        private string GetHelpFileResourcePath()
        {
            return "Symphony/Help Files/"+(string)target.GetType().GetMethod("GetHelpFileResourcePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(target, null);
        }
    }
}
