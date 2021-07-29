//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class NamespaceBuilder : FieldContainer
    {
        List<FieldBase> fields = new List<FieldBase>();

        public NamespaceBuilder(string name)
        {
            this.name = name;
        }

        public override void Merge(FieldBase field)
        {
            if (field.GetType() == typeof(NamespaceBuilder))
            {
                NamespaceBuilder castField = field as NamespaceBuilder;
                foreach (FieldBase child in castField.fields)
                {
                    FieldBase preexistingField = fields.FindLast(x => x.name == child.name);
                    if (preexistingField != null)
                    {
                        preexistingField.Merge(child);
                    }
                    else
                    {
                        fields.Add(child);
                    }
                }
            }
        }

        public override void AddField(FieldBase newField)
        {
            fields.Add(newField);
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            if (lines.Count >= 3)
            {
                string firstLine = lines[0];
                bool isNamespace = Regex.Match(firstLine, "namespace [a-zA-Z.]+").Success;

                string nextLine = lines[1];
                bool hasOpenBracket = nextLine.Contains("{");

                int closeBracketIndex = CloseBracketLocation(lines);
                bool hasCloseBracket = closeBracketIndex != -1;

                if (isNamespace && hasOpenBracket && hasCloseBracket)
                {
                    List<string> insetLines = GetSubset(lines, 2, closeBracketIndex);

                    lines = RemoveSubset(lines, 0, closeBracketIndex + 1);

                    string namespaceName = Regex.Match(firstLine, "(?<=namespace )[a-zA-Z.]+").Value;

                    NamespaceBuilder newNamespace = new NamespaceBuilder(namespaceName);
                    while (insetLines.Count > 0)
                        Interpret(insetLines, newNamespace);
                    return newNamespace;
                }
            }
            return null;
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            // Drawing the class name;
            string firstLine = "";
            firstLine += Indent(indent) + "namespace ";
            firstLine += name;
            lines.Add(firstLine);
            lines.Add(Indent(indent) + "{");

            indent++;

            foreach (FieldBase field in fields)
            {
                lines = field.WriteLines(lines, indent);
            }

            indent--;

            lines.Add(Indent(indent) + "}");

            return lines;
        }
    }
}   