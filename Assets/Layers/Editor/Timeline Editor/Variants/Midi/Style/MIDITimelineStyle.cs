using ABXY.Layers.Editor.Timeline_Editor.Variants.Style;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.Midi.Style
{
    public class MIDITimelineStyle : TimelineStyle
    {
        private LightTimeline lightStyle = new LightTimeline();
        private DarkTimeline darkStyle = new DarkTimeline();


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


        private TimelineStyle GetCurrentStyle()
        {
            if (EditorGUIUtility.isProSkin)
                return darkStyle;
            else
                return lightStyle;
        }
    }
}
