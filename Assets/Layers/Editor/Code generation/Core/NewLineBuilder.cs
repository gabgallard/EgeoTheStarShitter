using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class NewLineBuilder : FieldBase
    {
        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            lines.Add(Indent(indent));
            return lines;
        }
    }
}