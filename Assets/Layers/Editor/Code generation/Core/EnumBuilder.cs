//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class EnumBuilder : FieldBase
    {
        private List<string> enumValues = new List<string>();

        private bool useFlags = true;

        public EnumBuilder(string enumName)
        {
            this.name = enumName;
        }

        public EnumBuilder(string enumName, bool useFlags)
        {
            this.name = enumName;
            this.useFlags = useFlags;
        }

        public EnumBuilder AddValue(string enumValue)
        {
            if (!enumValues.Contains(enumValue))
                enumValues.Add(enumValue);
            return this;
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            if (useFlags)
                WriteLinesWithFlags(lines, indent);
            else
                WriteLinesWithoutFlags(lines, indent);
            return lines;
        }

        private void WriteLinesWithFlags(List<string> lines, int indent)
        {
            lines.Add(Indent(indent) + "[System.Flags]");
            string enumString = "";
            enumString += Indent(indent) + "public enum " + name + " {";
            enumString += " Nothing = 0";
            enumString += ", Everything = ~0, ";

            for (int index = 0; index < enumValues.Count; index++)
            {
                string value = enumValues[index];
                enumString += value + " = 1<<" + (index);
                if (index < enumValues.Count - 1)
                    enumString += ", ";
            }
            enumString += "};";

            lines.Add(enumString);
        }
        private void WriteLinesWithoutFlags(List<string> lines, int indent)
        {
            string enumString = "";
            enumString += Indent(indent) + "public enum " + name + " {";

            for (int index = 0; index < enumValues.Count; index++)
            {
                string value = enumValues[index];
                enumString += value;
                if (index < enumValues.Count - 1)
                    enumString += ", ";
            }
            enumString += "};";

            lines.Add(enumString);
        }
        public override FieldBase ReadLines(List<string> lines)
        {
            if (lines.Count >= 2)
            {
                bool hasFlags = lines[0].Contains("[System.Flags]");
                bool isEnum = Regex.Match(lines[1], "public enum [a-zA-Z0-9]+ {.+};").Success;
                if (hasFlags && isEnum)
                {
                    string enumName = Regex.Match(lines[1], "(?<=enum )[a-zA-Z0-9]+(?= *{)").Value;

                    Regex regex = new Regex("(?<=[{,])[ a-zA-Z0-9]+(?=[=])");
                    MatchCollection enumValues = regex.Matches(lines[1]);

                    EnumBuilder builder = new EnumBuilder(enumName);

                    foreach (Match match in enumValues)
                    {
                        string value = match.Value.Replace(" ", "");
                        if (value != "Nothing" && value != "Everything")
                            builder.AddValue(value);
                    }
                    lines = RemoveSubset(lines, 0, 2);
                    return builder;
                }
            }
            return null;
        }

        public override void Merge(FieldBase field)
        {
            if (field.GetType() == typeof(EnumBuilder))
            {
                EnumBuilder castedField = field as EnumBuilder;
                foreach (string value in castedField.enumValues)
                {
                    if (!enumValues.Contains(value))
                        enumValues.Add(value);
                }
            }

        }
    }
}