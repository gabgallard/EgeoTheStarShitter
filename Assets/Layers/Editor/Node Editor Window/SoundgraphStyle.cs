using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editor_Window
{
    public abstract class SoundGraphStyle
    {

        public abstract Color32 nodeBackgroundColor { get; }

        public abstract Color32 headerColor { get; }

        public abstract Color32 nodeHighlightBackground { get; }

        public abstract Color32 nodeMidLightBackground { get; }

        public abstract Color32 TriggerButtonInactiveColor { get; }
        public abstract Color32 TriggerButtonActiveColor { get; }

        public abstract GUIStyle soundGraphNameLabelStyle
        {
            get;
        }

        public abstract GUIStyle codeRegenLabelStyle
        {
            get;
        }

        public abstract GUIStyle headerLabelStyle
        {
            get;
        }

        public abstract Color popupBackgroundColor
        {
            get;
        }

        public abstract GUIStyle popupMainText { get; }

        public abstract GUIStyle popupSecondaryText { get; }

        public abstract GUIStyle popupHeaderStyle { get; }

        public abstract Texture2D grid
        {
            get;
        }

    }
}
