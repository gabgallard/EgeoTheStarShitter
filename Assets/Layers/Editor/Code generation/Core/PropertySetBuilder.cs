using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class PropertySetBuild : FieldBase
    {

        bool isAutoProperty = false;

        List<ArbitraryBuilder> content = new List<ArbitraryBuilder>();

        public PropertySetBuild()
        {
            isAutoProperty = true;
        }

        public PropertySetBuild(List<ArbitraryBuilder> content)
        {
            this.content = content;
        }

        public PropertySetBuild(ArbitraryLineBuilder content)
        {
            this.content = new List<ArbitraryBuilder>(new ArbitraryLineBuilder[] { content });
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            if (isAutoProperty)
            {
                lines.Add(Indent(indent) + "set;");
                return lines;
            }
            else
            {
                lines.Add(Indent(indent) + "set {");
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
