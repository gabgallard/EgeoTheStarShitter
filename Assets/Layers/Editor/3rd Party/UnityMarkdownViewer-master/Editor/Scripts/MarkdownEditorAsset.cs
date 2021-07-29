using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts
{
#if SYMPHONY_DEV
    [CustomEditor( typeof(  MarkdownAsset ) )]
#endif
    public class MarkdownEditorAsset : UnityEditor.Editor
    {
        public GUISkin Skin;

        MarkdownViewer mViewer;

        protected void OnEnable()
        {
            var content = ( target as MarkdownAsset ).text;
            var path    = AssetDatabase.GetAssetPath( target );

            mViewer = new MarkdownViewer( Skin, path, content );
            EditorApplication.update += UpdateRequests;
        }

        protected void OnDisable()
        {
            EditorApplication.update -= UpdateRequests;
            mViewer = null;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        protected override void OnHeaderGUI()
        {
            //base.OnHeaderGUI(); 
        }

        public override void OnInspectorGUI()
        {
            mViewer.Draw();
        }


        void UpdateRequests()
        {
            if( mViewer.Update() )
            {
                Repaint();
            }
        }
    }
}
