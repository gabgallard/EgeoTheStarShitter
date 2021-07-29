using ABXY.Layers.Editor.ThirdParty.Xnode;
using UnityEngine;
using UnityEditor;

namespace ABXY.Layers.Editor.Analyzer.Style
{
    public class AnalyzerLightStyle : AnalyzerStyle
    {
        public override Color mainBackground => new Color32(193, 193, 193, 255);

        public override Color secondaryBackground => new Color32(219,219,219,255);

        public override Color primaryTimelineElementColor => new Color32(161,161,161,255);

        public override Color secondaryTimelineElementColor => new Color32(219, 219, 219, 255);

        public override Color gridLineColor => new Color32(131,131,131,255);

        public override Color textColor => Color.black;

        public override Color highlightColor => new Color32(158,0,147,255);

        public override Color dragAreaColor => new Color32(129, 129, 129, 255);

        public override Color endTimeColor => new Color32(79,79,79,255);
        public override Color endTimeHighlightColor => new Color32(158, 0, 147, 255);

        public override Color buttonColor => new Color32(79, 79, 79, 255);

        public override Color32 eventHighlightColor => new Color(0.225f, 0.929f, 0.590f);

        public override Color32 headerColor => new Color32(222, 222, 222, 255);

        private GUIStyle _headerLabelStyle;
        public override GUIStyle headerLabelStyle
        {
            get
            {
                if (_headerLabelStyle == null)
                {
                    _headerLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                    _headerLabelStyle.normal.textColor = Color.black;
                    _headerLabelStyle.fontStyle = FontStyle.Normal;
                    _headerLabelStyle.padding = new RectOffset(0, 0, -3, 0);
                    _headerLabelStyle.alignment = TextAnchor.MiddleLeft;
                }
                return _headerLabelStyle;
            }
        }

        private GUIStyle _centeredHeaderLabelStyle;
        public override GUIStyle centeredHeaderLabelStyle
        {
            get
            {
                if (_centeredHeaderLabelStyle == null)
                {
                    _centeredHeaderLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                    _centeredHeaderLabelStyle.normal.textColor = Color.black;
                    _centeredHeaderLabelStyle.fontStyle = FontStyle.Normal;
                    _centeredHeaderLabelStyle.padding = new RectOffset(0, 0, -3, 0);
                    _centeredHeaderLabelStyle.alignment = TextAnchor.MiddleCenter;
                }
                return _centeredHeaderLabelStyle;
            }
        }
    }
}
