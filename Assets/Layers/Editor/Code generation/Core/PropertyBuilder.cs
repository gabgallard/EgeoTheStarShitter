using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class PropertyBuilder : FieldBase
    {
        PropertyGetBuild getBuilder;
        PropertySetBuild setBuilder;

        public FieldBuilder.AccessibilityValues accessibility { get; private set; }

        public string type { get; private set; }
        public bool isStatic { get; private set; }


        public PropertyBuilder(FieldBuilder.AccessibilityValues accessibility, bool isStatic, string type, string name, PropertyGetBuild getBuilder, PropertySetBuild setBuilder)
        {
            this.getBuilder = getBuilder;
            this.setBuilder = setBuilder;
            this.accessibility = accessibility;
            this.type = type;
            this.isStatic = isStatic;
            this.name = name;
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            string staticString = isStatic ? "static " : "";
            lines.Add(Indent(indent) + accessibility.ToString().ToLower() + " " + staticString  + type + " " + name + "{");
            indent++;
            if (getBuilder != null)
                lines = getBuilder.WriteLines(lines, indent);

            if (setBuilder != null)
                lines = setBuilder.WriteLines(lines, indent);
            indent--;
            lines.Add(Indent(indent) + "}");
            return lines;
        }
    }
}
