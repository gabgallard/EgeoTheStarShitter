using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class PropertyGetBuild : FieldBase
    {

        bool isAutoProperty = false;

        List<ArbitraryBuilder> content = new List<ArbitraryBuilder>();


        public PropertyGetBuild()
        {
            isAutoProperty = true;
        }

        public PropertyGetBuild(bool isAutoProperty)
        {
            this.isAutoProperty = isAutoProperty;
        }

        public PropertyGetBuild(List<ArbitraryBuilder> content)
        {
            this.content = content;
        }
        public PropertyGetBuild(ArbitraryLineBuilder content)
        {
            this.content = new List<ArbitraryBuilder>(new ArbitraryLineBuilder[] { content });
        }

        public void AddLine(string newLine)
        {
            content.Add(new ArbitraryLineBuilder(newLine));
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            if (isAutoProperty)
            {
                lines.Add(Indent(indent) + "get;");
                return lines;
            }
            else
            {
                lines.Add(Indent(indent) + "get {");
                indent++;
                foreach (ArbitraryBuilder builder in content)
                {
                    lines = builder.WriteLines(lines, indent);
                }
                indent--;
                lines.Add(Indent(indent) + "}");
                return lines;
            }
        }
    }
}
