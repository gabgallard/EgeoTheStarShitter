using UnityEngine;

namespace ABXY.Layers.Editor.Analyzer.Style
{
    public abstract class AnalyzerStyle
    {
        public abstract Color mainBackground { get; }
        public abstract Color secondaryBackground { get; }
        public abstract Color primaryTimelineElementColor { get; }
        public abstract Color secondaryTimelineElementColor { get; }

        public abstract Color32 eventHighlightColor { get; }

        public abstract Color32 headerColor { get; }

        public abstract Color gridLineColor { get; }

        public abstract Color textColor { get; }

        public abstract Color highlightColor { get; }
        public abstract Color dragAreaColor { get; }

        public abstract Color endTimeColor { get; }
        public abstract Color endTimeHighlightColor { get; }

        public abstract Color buttonColor { get; }

        private GUIStyle _toolbarButtonStyle;
        public GUIStyle toolbarButtonStyle
        {
            get
            {
                if (_toolbarButtonStyle == null)
                {
                    _toolbarButtonStyle = new GUIStyle(GUI.skin.button);
                    _toolbarButtonStyle.padding = new RectOffset(2, 2, 2, 2);
                }
                return _toolbarButtonStyle;
            }
        }


        private Texture2D _playButton;

        public Texture playButton
        {
            get
            {
                if (_playButton == null)
                    _playButton = Resources.Load<Texture2D>("Symphony/Play");
                return _playButton;
            }
        }

        private Texture2D _pauseButton;

        public Texture pauseButton
        {
            get
            {
                if (_pauseButton == null)
                    _pauseButton = Resources.Load<Texture2D>("Symphony/Pause");
                return _pauseButton;
            }
        }

        private Texture2D _rewindButton;

        public Texture rewindButton
        {
            get
            {
                if (_rewindButton == null)
                    _rewindButton = Resources.Load<Texture2D>("Symphony/Rewind");
                return _rewindButton;
            }
        }

        public abstract GUIStyle headerLabelStyle
        {
            get;
        }

        public abstract GUIStyle centeredHeaderLabelStyle
        {
            get;
        }

    }
}
