using ABXY.Layers.Editor.Timeline_Editor.Variants.Style;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Analyzer.Style
{
    public class CombinedAnalyzerStyle : AnalyzerStyle
    {

        private AnalyzerLightStyle lightStyle = new AnalyzerLightStyle();
        private AnalyzerDarkStyle darkStyle = new AnalyzerDarkStyle();


        public override Color mainBackground => GetCurrentStyle().mainBackground;

        public override Color secondaryBackground => GetCurrentStyle().secondaryBackground;

        public override Color primaryTimelineElementColor => GetCurrentStyle().primaryTimelineElementColor;

        public override Color secondaryTimelineElementColor => GetCurrentStyle().secondaryTimelineElementColor;

        public override Color gridLineColor => GetCurrentStyle().gridLineColor;

        public override Color textColor => GetCurrentStyle().textColor;

        public override Color highlightColor => GetCurrentStyle().highlightColor;

        public override Color dragAreaColor => GetCurrentStyle().dragAreaColor;

        public override Color endTimeColor => GetCurrentStyle().endTimeColor;

        public override Color endTimeHighlightColor => GetCurrentStyle().endTimeHighlightColor;
        public override Color buttonColor => GetCurrentStyle().buttonColor;


        private Texture2D _midiIcon = null;
        public Texture2D midiIcon
        {
            get
            {
                if (_midiIcon == null)
                    _midiIcon = Resources.Load<Texture2D>("Symphony/MIDI Icon");
                return _midiIcon;
            }
        }

        private Texture2D _audioIcon = null;

        public Texture2D audioIcon
        {
            get
            {
                if (_audioIcon == null)
                    _audioIcon = Resources.Load<Texture2D>("Symphony/Audio Icon");
                return _audioIcon;
            }
        }

        private Texture2D _deleteIcon = null;

        public Texture2D deleteIcon
        {
            get
            {
                if (_deleteIcon == null)
                    _deleteIcon = Resources.Load<Texture2D>("Symphony/Delete Icon");
                return _deleteIcon;
            }
        }

        private GUIStyle _closeButtonStyle;
        public GUIStyle closeButtonStyle
        {
            get
            {
                if (_closeButtonStyle == null)
                {
                    _closeButtonStyle = new GUIStyle(GUI.skin.button);
                    _closeButtonStyle.normal.background = deleteIcon;
                    _closeButtonStyle.margin = new RectOffset(0, 0, 0, 0);
                    _closeButtonStyle.border = new RectOffset(0, 0, 0, 0);
                }
                return _closeButtonStyle;
            }
        }

        private Texture2D _visibleIcon = null;

        public Texture2D visibleIcon
        {
            get
            {
                if (_visibleIcon == null)
                    _visibleIcon = Resources.Load<Texture2D>("Symphony/Expose In Node Graph");
                return _visibleIcon;
            }
        }

        private Texture2D _hiddenIcon = null;

        public Texture2D hiddenIcon
        {
            get
            {
                if (_hiddenIcon == null)
                    _hiddenIcon = Resources.Load<Texture2D>("Symphony/Hidden In Node Graph");
                return _hiddenIcon;
            }
        }

        private Texture2D _timelineItemBackground;

        public Texture timelineItemBackground
        {
            get
            {
                if (_timelineItemBackground == null)
                    _timelineItemBackground = Resources.Load<Texture2D>("Symphony/Timeline Item Background");
                return _timelineItemBackground;
            }
        }


        private Texture2D _eventItemBackground;

        public Texture eventItemBackground
        {
            get
            {
                if (_eventItemBackground == null)
                    _eventItemBackground = Resources.Load<Texture2D>("Symphony/Event Dot");
                return _eventItemBackground;
            }
        }

        private GUIStyle _exposeInNodeGraphButton;
        public GUIStyle exposeInNodeGraphButton
        {
            get
            {
                if (_exposeInNodeGraphButton == null)
                {
                    _exposeInNodeGraphButton = new GUIStyle(GUI.skin.button);
                    _exposeInNodeGraphButton.normal.background = hiddenIcon;
                    _exposeInNodeGraphButton.onNormal.background = visibleIcon;
                    _exposeInNodeGraphButton.active.background = visibleIcon;
                    _exposeInNodeGraphButton.onActive.background = hiddenIcon;
                    _exposeInNodeGraphButton.onFocused.background = visibleIcon;
                    _exposeInNodeGraphButton.margin = new RectOffset(0, 0, 0, 0);
                    _exposeInNodeGraphButton.border = new RectOffset(0, 0, 0, 0);
                }
                return _exposeInNodeGraphButton;
            }
        }

        public override Color32 eventHighlightColor => GetCurrentStyle().eventHighlightColor;
        public override Color32 headerColor => GetCurrentStyle().headerColor;

        public override GUIStyle headerLabelStyle => GetCurrentStyle().headerLabelStyle;

        public override GUIStyle centeredHeaderLabelStyle => GetCurrentStyle().centeredHeaderLabelStyle;

        public Color errorColor => Color.red;

        private AnalyzerStyle GetCurrentStyle()
        {
            if (EditorGUIUtility.isProSkin)
                return darkStyle;
            else
                return lightStyle;
        }
    }
}
