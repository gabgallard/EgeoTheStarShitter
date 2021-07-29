//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class FieldBuilder : FieldBase
    {
        public enum AccessibilityValues { Public, Private, Protected}
        public AccessibilityValues accessibility { get; private set; }

        public string startingValue { get; private set; }
        public string type { get; private set; }
        public bool isReadonly { get; private set; }
        public bool isStatic { get; private set; }

        public FieldBuilder(AccessibilityValues accessibility, bool isStatic, bool isReadonly, string type, string name, string startingValue)
        {
            this.accessibility = accessibility;
            this.name = name;
            this.startingValue = startingValue;
            this.type = type;
            this.isReadonly = isReadonly;
            this.isStatic = isStatic;
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }
        
        public override List<string> WriteLines(List<string> lines, int indent)
        {
            string readOnlyString = isReadonly ? " readonly " : " ";
            string staticString = isStatic ? " static " : "";
            lines.Add(Indent(indent) + accessibility.ToString().ToLower() + staticString + readOnlyString + type + " " + name + " = " + startingValue + ";");
            return lines;
        }
    }
}
