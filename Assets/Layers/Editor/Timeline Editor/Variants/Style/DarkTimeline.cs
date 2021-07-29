using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.Style
{
    public class DarkTimeline : TimelineStyle
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
    }
}
