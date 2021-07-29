using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editor_Window
{
    public class SoundgraphCombinedStyle : SoundGraphStyle
    {
        private LightSoundgraphStyle lightStyle = new LightSoundgraphStyle();
        private DarkSoundgraphStyle darkStyle = new DarkSoundgraphStyle();

        public override Color32 nodeBackgroundColor => GetCurrentStyle().nodeBackgroundColor;

        public override Color32 headerColor => GetCurrentStyle().headerColor;

        public override GUIStyle headerLabelStyle => GetCurrentStyle().headerLabelStyle;

        public override Texture2D grid => GetCurrentStyle().grid;

        public override Color popupBackgroundColor => GetCurrentStyle().popupBackgroundColor;
        public override Color32 TriggerButtonActiveColor => GetCurrentStyle().TriggerButtonActiveColor;
        public override Color32 TriggerButtonInactiveColor => GetCurrentStyle().TriggerButtonInactiveColor;

        public override GUIStyle popupMainText => GetCurrentStyle().popupMainText;

        public override GUIStyle popupSecondaryText => GetCurrentStyle().popupSecondaryText;

        public override GUIStyle popupHeaderStyle => GetCurrentStyle().popupHeaderStyle;

        public override GUIStyle soundGraphNameLabelStyle => GetCurrentStyle().soundGraphNameLabelStyle;

        public override GUIStyle codeRegenLabelStyle => GetCurrentStyle().codeRegenLabelStyle;

        public override Color32 nodeHighlightBackground => GetCurrentStyle().nodeHighlightBackground;

        public override Color32 nodeMidLightBackground => GetCurrentStyle().nodeMidLightBackground;

        // private DarkTimeline darkStyle = new DarkTimeline();
        private GUIStyle _messageStyle;

        public GUIStyle messageStyle
        {
            get
            {
                if (_messageStyle == null)
                {
                    _messageStyle = new GUIStyle(EditorStyles.label);
                    _messageStyle.wordWrap = true;
                    _messageStyle.padding = new RectOffset(5, 5, 5, 5);
                }
                return _messageStyle;
            }
        }


        private SoundGraphStyle GetCurrentStyle()
        {
            if (EditorGUIUtility.isProSkin)
                return darkStyle;
            else
                return lightStyle;
        }
    }
}
