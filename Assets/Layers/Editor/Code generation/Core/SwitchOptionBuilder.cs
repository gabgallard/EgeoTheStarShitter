using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class SwitchOptionBuilder : FieldBase
    {
        public string optionText{ get; private set; }

        private List<ArbitraryBuilder> content = new List<ArbitraryBuilder>();

        public bool insertBreak { get; private set; }

        public SwitchOptionBuilder(string optionText, List<ArbitraryBuilder> content, bool insertBreak)
        {
            this.optionText = optionText;
            this.content = content;
            this.insertBreak = insertBreak;
        }

        public SwitchOptionBuilder(string optionText, bool insertBreak)
        {
            this.optionText = optionText;
            this.insertBreak = insertBreak;
        }


        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            lines.Add(Indent(indent) + "case "+optionText+":");
            indent++;
            foreach(ArbitraryBuilder line in content)
            {
                lines = line.WriteLines(lines, indent);
            }
            if (insertBreak)
                lines.Add(Indent(indent) + "break;");

            return lines;
        }

        public void AddContent(ArbitraryBuilder builder)
        {
            content.Add(builder);
        }
    }
}
