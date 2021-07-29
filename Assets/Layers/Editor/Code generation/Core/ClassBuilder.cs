//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ABXY.Layers.Editor.Code_generation.Core
{

    public class ClassBuilder : FieldContainer, IEnumerable<FieldBase>
    {
        public enum ClassType { Static, Nonstatic }

        public bool partial { get; private set; }

        public bool isStatic { get; private set; }

        private List<FieldBase> fields = new List<FieldBase>();

        private string extends = "";

        public ClassBuilder(string className, bool partial, string extends, bool isStatic)
        {
            this.name = className;
            this.partial = partial;
            this.extends = extends;
            this.isStatic = isStatic;
        }

        public ClassBuilder(string className, bool partial, string extends)
        {
            this.name = className;
            this.partial = partial;
            this.extends = extends;
        }

        public ClassBuilder(string className, bool partial)
        {
            this.name = className;
            this.partial = partial;
            this.extends = "";
        }

        public override void AddField(FieldBase newField)
        {
            fields.Add(newField);
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            // Drawing the class name;
            string firstLine = "";
            firstLine += Indent(indent) + "public ";
            if (isStatic)
                firstLine += "static ";

            if (partial)
                firstLine += "partial ";
            firstLine += "class ";
            firstLine += name;

            if (!string.IsNullOrEmpty(extends))
                firstLine += ": " + extends;

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

        public override FieldBase ReadLines(List<string> lines)
        {
            if (lines.Count >= 3)
            {
                string firstLine = lines[0];
                bool isClass = Regex.Match(firstLine, "public (partial )?class [a-zA-Z0-9]+").Success;

                string nextLine = lines[1];
                bool hasOpenBracket = nextLine.Contains("{");

                int closeBracketIndex = CloseBracketLocation(lines);
                bool hasCloseBracket = closeBracketIndex != -1;

                if (isClass && hasOpenBracket && hasCloseBracket)
                {
                    List<string> insetLines = GetSubset(lines, 2, closeBracketIndex);

                    lines = RemoveSubset(lines, 0, closeBracketIndex + 1);

                    string className = Regex.Match(firstLine, "(?<=class )[a-zA-Z0-9]+").Value;
                    bool isPartial = firstLine.Contains("partial");

                    ClassBuilder newClass = new ClassBuilder(className, isPartial);
                    while (insetLines.Count > 0)
                        Interpret(insetLines, newClass);
                    return newClass;
                }
            }
            return null;
        }

        public override void Merge(FieldBase field)
        {
            if (field.GetType() == typeof(ClassBuilder))
            {
                ClassBuilder castField = field as ClassBuilder;
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

        public IEnumerator<FieldBase> GetEnumerator()
        {
            foreach (FieldBase field in fields)
            {
                yield return field;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (FieldBase field in fields)
            {
                yield return field;
            }
        }
    }
}
