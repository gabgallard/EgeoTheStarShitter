//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class ParameterBuilder : FieldBase
    {
        public string type { get; private set; }

        public ParameterBuilder(string type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            lines.Add(type + " " + name);
            return lines;
        }
    }
}