using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.Style
{
    public class LightTimeline : TimelineStyle
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
    }
}
