using ABXY.Layers.Editor.ThirdParty.Editor.Scripts.Layout;
using ABXY.Layers.Editor.ThirdParty.Editor.Scripts.Renderer;
using Markdig;
using Markdig.Extensions.JiraLinks;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts
{
    public class MarkdownViewer
    {
        private GUISkin         mSkin            = null;
        private string          mText            = string.Empty;
        private string          mCurrentPath     = string.Empty;
        private HandlerImages   mHandlerImages   = new HandlerImages();
        private HandlerNavigate mHandlerNavigate = new HandlerNavigate();

        private Vector2         mScrollPos       = Vector2.zero;
        private Layout.Layout          mLayout          = null;
        private bool            mRaw             = false;

        private static History  mHistory         = new History();

        public MarkdownViewer( GUISkin skin, string path, string content )
        {
            mSkin        = skin;
            mCurrentPath = path;
            mText        = content;

            mHistory.OnOpen( mCurrentPath );
            mLayout = ParseDocument();

            mHandlerImages.CurrentPath   = mCurrentPath;

            mHandlerNavigate.CurrentPath = mCurrentPath;
            mHandlerNavigate.History     = mHistory;
            mHandlerNavigate.FindBlock   = ( id ) => mLayout.Find( id );
            mHandlerNavigate.ScrollTo    = ( pos ) => mScrollPos.y = pos;
        }


        //------------------------------------------------------------------------------

        public bool Update()
        {
            return mHandlerImages.UpdateRequests();
        }


        //------------------------------------------------------------------------------

        Layout.Layout ParseDocument()
        {
            var context  = new Context( mSkin, mHandlerImages, mHandlerNavigate );
            var builder  = new LayoutBuilder( context );
            var renderer = new RendererMarkdown( builder );

            var pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAutoLinks()
            ;

            if( !string.IsNullOrEmpty( Preferences.JIRA ) )
            {
                pipelineBuilder.UseJiraLinks( new JiraLinkOptions( Preferences.JIRA ) );
            }
            pipelineBuilder.UsePipeTables(new Markdig.Extensions.Tables.PipeTableOptions());

            var pipeline = pipelineBuilder.Build();
            pipeline.Setup( renderer );

            var doc = Markdown.Parse( mText, pipeline );
            renderer.Render( doc );

            return builder.GetLayout();
        }


        //------------------------------------------------------------------------------

#if !UNITY_2018
        private float mHeight = 1.0f;
#endif

        protected Rect GetEditorRect()
        {
#if !UNITY_2018

            // calculate working space
            if( Event.current.type == EventType.Layout )
            {
                var offset = 32.0f + 96.0f + 4.0f; // tab height + asset labels height + offset
                mHeight = ( Screen.height - offset ) / 1.5115f;
            }

            // reserve space (we are going to paint on it directly)
            GUILayout.Space( mHeight );

            // return working rect
            return new Rect( 0.0f, 4.0f, EditorGUIUtility.currentViewWidth, mHeight );
#else
            GUILayout.FlexibleSpace();
            var rectContainer = GUILayoutUtility.GetLastRect();
            rectContainer.width = EditorGUIUtility.currentViewWidth;
            return rectContainer;
#endif
        }

        //------------------------------------------------------------------------------

        public void Draw()
        {
            GUI.skin    = mSkin;
            GUI.enabled = true;

            // content rect

            var rectContainer = GetEditorRect();


            // clear background

            var rectFullScreen = new Rect( 0.0f, rectContainer.yMin - 4.0f, Screen.width, Screen.height );
            GUI.DrawTexture( rectFullScreen, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false );

            // scroll window

            var padLeft     = 8.0f;
            var padRight    = 4.0f;
            var padHoriz    = padLeft + padRight;
            var scrollWidth = GUI.skin.verticalScrollbar.fixedWidth;
            var minWidth    = rectContainer.width - scrollWidth - padHoriz;
            var maxHeight   = ContentHeight( minWidth );

            var hasScrollbar =  maxHeight >= rectContainer.height;
            var contentWidth = hasScrollbar ? minWidth : rectContainer.width - padHoriz;
            var rectContent  = new Rect( -padLeft, 0.0f, contentWidth, maxHeight );

            // draw content

            using( var scroll = new GUI.ScrollViewScope( rectContainer, mScrollPos, rectContent ) )
            {
                mScrollPos = scroll.scrollPosition;

                if( mRaw )
                {
                    rectContent.width = minWidth - GUI.skin.button.fixedWidth;
                    DrawRaw( rectContent );
                }
                else
                {
                    DrawMarkdown( rectContent );
                }
            }

            DrawToolbar( rectContainer, hasScrollbar ? scrollWidth + padRight : padRight );
        }

        //------------------------------------------------------------------------------

        float ContentHeight( float width )
        {
            return mRaw ? GUI.skin.GetStyle( "raw" ).CalcHeight( new GUIContent( mText ), width ) : mLayout.Height;
        }

        //------------------------------------------------------------------------------

        void DrawToolbar( Rect rect, float marginRight )
        {
            var style  = GUI.skin.button;
            var size   = style.fixedHeight;
            var btn    = new Rect( rect.xMax - size - marginRight, rect.yMin, size, size );

#if SYMPHONY_DEV
            if( GUI.Button( btn, string.Empty, GUI.skin.GetStyle( mRaw ? "btnRaw" : "btnFile" ) ) )
            {
                mRaw = !mRaw;
            }
#endif

            if( mRaw == false )
            {
                if( mHistory.CanForward )
                {
                    btn.x -= size;

                    if( GUI.Button( btn, string.Empty, GUI.skin.GetStyle( "btnForward" ) ) )
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>( mHistory.Forward() );
                    }
                }

                if( mHistory.CanBack )
                {
                    btn.x -= size;

                    if( GUI.Button( btn, string.Empty, GUI.skin.GetStyle( "btnBack" ) ) )
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>( mHistory.Back() );
                    }
                }
            }
        }

        //------------------------------------------------------------------------------

        void DrawRaw( Rect rect )
        {
            EditorGUI.SelectableLabel( rect, mText, GUI.skin.GetStyle( "raw" ) );
        }

        //------------------------------------------------------------------------------

        void DrawMarkdown( Rect rect )
        {
            switch( Event.current.type )
            {
                case EventType.Ignore:
                    break;
#if SYMPHONY_DEV
                case EventType.ContextClick:
                    var menu = new GenericMenu();
                    menu.AddItem( new GUIContent( "View Source" ), false, () => mRaw = !mRaw );
                    menu.ShowAsContext();

                    break;
#endif
                case EventType.Layout:
                    mLayout.Arrange( rect.width );
                    break;

                default:
                    mLayout.Draw();
                    break;
            }
        }
    }
}
