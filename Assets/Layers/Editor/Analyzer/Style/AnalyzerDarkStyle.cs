using ABXY.Layers.Editor.ThirdParty.Xnode;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Analyzer.Style
{
    public class AnalyzerDarkStyle : AnalyzerStyle
    {
        public override Color mainBackground => new Color32(59,59,59,255);

        public override Color secondaryBackground => new Color32(95, 95, 95, 255);

        public override Color primaryTimelineElementColor => new Color32(46, 46, 46, 255);

        public override Color secondaryTimelineElementColor => new Color32(95, 95, 95, 255);

        public override Color gridLineColor => new Color32(152, 152, 152, 255);

        public override Color textColor => new Color32(152, 152, 152, 255);

        public override Color highlightColor => new Color32(49, 178, 255, 255);

        public override Color dragAreaColor => new Color32(59, 59, 59, 255);

        public override Color endTimeColor => new Color32(46, 46, 46, 255); 
        public override Color endTimeHighlightColor => new Color32(49, 178, 255, 255);

        public override Color buttonColor => new Color32(152, 152, 152, 255);

        public override Color32 eventHighlightColor => new Color(0.225f, 0.929f, 0.590f);
        public override Color32 headerColor => new Color32(44, 44, 44, 255);


        private GUIStyle _headerLabelStyle;
        public override GUIStyle headerLabelStyle
        {
            get
            {
                if (_headerLabelStyle == null)
                {
                    _headerLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                    _headerLabelStyle.normal.textColor = Color.white;
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
                    _centeredHeaderLabelStyle.normal.textColor = Color.white;
                    _centeredHeaderLabelStyle.fontStyle = FontStyle.Normal;
                    _centeredHeaderLabelStyle.padding = new RectOffset(0, 0, -3, 0);
                    _centeredHeaderLabelStyle.alignment = TextAnchor.MiddleCenter;
                }
                return _centeredHeaderLabelStyle;
            }
        }

    }
}
