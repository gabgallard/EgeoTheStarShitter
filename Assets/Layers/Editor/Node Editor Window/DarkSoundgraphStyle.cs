using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editor_Window
{
    public class DarkSoundgraphStyle : SoundGraphStyle
    {
        public override Color32 nodeBackgroundColor => new Color32(95,95,95,255);

        public override Color32 headerColor => new Color32(44, 44, 44, 255);

        private GUIStyle _headerLabelStyle;
        public override GUIStyle headerLabelStyle
        {
            get
            {
                if (_headerLabelStyle == null)
                {
                    _headerLabelStyle = new GUIStyle(NodeEditorResources.styles.nodeHeader);
                    _headerLabelStyle.normal.textColor = Color.white;
                    _headerLabelStyle.fontStyle = FontStyle.Normal;
                    _headerLabelStyle.padding = new RectOffset(0, 0, -3, 0);
                }
                return _headerLabelStyle;
            }
        }


        public override Color popupBackgroundColor => new Color32(59, 59, 59, 150);

        public override Color32 TriggerButtonActiveColor => new Color32(78, 78, 78, 255);
        public override Color32 TriggerButtonInactiveColor => new Color32(95, 95, 95, 255);


        private GUIStyle _popupMainText;
        public override GUIStyle popupMainText
        {
            get
            {
                if (_popupMainText == null)
                {
                    _popupMainText = new GUIStyle(EditorStyles.label);
                    _popupMainText.alignment = TextAnchor.MiddleLeft;
                }
                return _popupMainText;
            }
        }

        private GUIStyle _popupSecondaryText;
        public override GUIStyle popupSecondaryText
        {
            get
            {
                if (_popupSecondaryText == null)
                {
                    _popupSecondaryText = new GUIStyle(EditorStyles.label);
                    _popupSecondaryText.alignment = TextAnchor.MiddleLeft;
                    _popupSecondaryText.normal.textColor = new Color32(160, 160, 160, 255);
                }
                return _popupSecondaryText;
            }
        }

        private GUIStyle _popupHeaderStyle;
        public override GUIStyle popupHeaderStyle
        {
            get
            {
                if (_popupHeaderStyle == null)
                {
                    _popupHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
                    _popupHeaderStyle.alignment = TextAnchor.MiddleLeft;
                }
                return _popupHeaderStyle;
            }
        }



        public override Texture2D grid
        {
            get
            {
                if (_lightGrid == null)
                {
                    Color32 color = LayersSettings.GetOrCreateSettings().enableGreenScreen ? (Color32)new Color(0f, 1f, 0f, 1f) : new Color32(59, 59, 59, 255);
                    if (LayersSettings.GetOrCreateSettings().enableScreenshot)
                        color = Color.white;

                    _lightGrid = NodeEditorResources.GenerateGridTexture(color, color);
                }
                return _lightGrid;
            }
        }

        private GUIStyle _soundGraphNameLabelStyle;
        public override GUIStyle soundGraphNameLabelStyle
        {
            get
            {
                if (_soundGraphNameLabelStyle == null)
                {
                    _soundGraphNameLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                    _soundGraphNameLabelStyle.fontSize += 2;
                    _soundGraphNameLabelStyle.normal.textColor = Color.white;
                    _soundGraphNameLabelStyle.clipping = TextClipping.Overflow;
                }
                return _soundGraphNameLabelStyle;
            }
        }

        private GUIStyle _codeRegenLabelStyle;
        public override GUIStyle codeRegenLabelStyle
        {
            get
            {
                if (_codeRegenLabelStyle == null)
                {
                    _codeRegenLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                    _codeRegenLabelStyle.fontSize += 2;
                    _codeRegenLabelStyle.normal.textColor = Color.white;
                    _codeRegenLabelStyle.clipping = TextClipping.Overflow;
                    _codeRegenLabelStyle.alignment = TextAnchor.MiddleCenter;
                }
                return _codeRegenLabelStyle;
            }
        }

        public override Color32 nodeHighlightBackground => new Color(0.19215f, 0.69804f, 1f);

        public override Color32 nodeMidLightBackground => new Color(0.19215f, 0.69804f, 1f) * 0.8f;

        private Texture2D _lightGrid;
    }
}
