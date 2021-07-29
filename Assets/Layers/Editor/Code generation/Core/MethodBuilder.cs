//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class MethodBuilder : FieldBase
    {


        private List<ParameterBuilder> parameters = new List<ParameterBuilder>();
        private List<FieldBase> content = new List<FieldBase>();
        public string returnType { get; private set; }
        public FieldBuilder.AccessibilityValues accessibility { get; private set; }

        public bool isStatic { get; private set; }

        public bool isOverride { get; private set; }

        public MethodBuilder(string returnType, bool isStatic, bool isOverride, FieldBuilder.AccessibilityValues accessibility, string methodName)
        {
            this.returnType = returnType;
            this.name = methodName;
            this.isStatic = isStatic;
            this.isOverride = isOverride;
            this.accessibility = accessibility;
        }

        public MethodBuilder(string returnType, bool isStatic, bool isOverride,FieldBuilder.AccessibilityValues accessibility, string methodName, List<FieldBase> content, params ParameterBuilder[] parameters)
        {
            this.returnType = returnType;
            this.name = methodName;
            this.content = content;
            this.isStatic = isStatic;
            this.isOverride = isOverride;
            this.accessibility = accessibility;
            this.parameters = new List<ParameterBuilder>(parameters);
        }

        public void AddContent(FieldBase content)
        {
            this.content.Add(content);
        }

        public void AddParameter(ParameterBuilder parameter)
        {
            this.parameters.Add(parameter);
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            string staticString = isStatic ? "static " : ""; 
            string overrideString = isOverride ? "override " : "";
            string methodStart = Indent(indent) + accessibility.ToString().ToLower() + " " + staticString + overrideString + returnType + " " + name + "( " ;
            for (int index = 0; index < parameters.Count; index++)
            {
                methodStart += parameters[index].WriteLines(new List<string>(), 0)[0] + (index + 1 != parameters.Count ? ", " : " ");
            }
            methodStart += "){";
            lines.Add(methodStart);

            indent++;
            foreach (FieldBase contentItem in content)
                lines = contentItem.WriteLines(lines, indent);

            indent--;
            lines.Add(Indent(indent) + "}");
            return lines;
        }
    }
}
