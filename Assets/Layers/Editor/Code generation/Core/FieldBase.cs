//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public abstract class FieldBase
    {
        public abstract List<string> WriteLines(List<string> lines, int indent);

        public abstract FieldBase ReadLines(List<string> lines);

        public string name { get; protected set; }

        protected string Indent(int indent)
        {
            string space = "";
            for (int index = 0; index < indent * 4; index++)
                space += " ";
            return space;
        }


        protected int CloseBracketLocation(List<string> lines)
        {
            bool firstBracketFound = false;
            int level = 0;
            for (int index = 0; index < lines.Count; index++)
            {
                string line = lines[index];
                if (line.Contains("{"))
                {
                    level++;
                    firstBracketFound = true;
                }
                if (line.Contains("}"))
                {
                    level--;
                    if (level <= 0 && firstBracketFound)
                        return index;
                }
            }
            return -1;
        }

        protected bool ContainsOpenBracket(string line)
        {
            return line.Contains("{");
        }

        protected bool ContainsCloseBracket(string line)
        {
            return line.Contains("}");

        }

        protected List<string> GetSubset(List<string> list, int start, int end)
        {
            return list.GetRange(start, end - start);
        }

        protected List<string> RemoveSubset(List<string> list, int start, int end)
        {
            list.RemoveRange(start, end - start);
            return list;
        }

        public enum Visibility { PRIVATE, PUBLIC, PROTECTED}

        public static FieldBase Interpret(List<string> lines, FieldBase parentField)
        {

            while (lines.Count >= 0)
            {
                FieldBase value = new NamespaceBuilder("").ReadLines(lines);
                if (value != null)
                {
                    if (parentField == null)
                        parentField = value;
                    else
                    {
                        (parentField as FieldContainer).AddField(value);
                    }
                    return parentField;

                }

                value = new ClassBuilder("", true).ReadLines(lines);
                if (value != null)
                {
                    if (parentField == null)
                        parentField = value;
                    else
                    {
                        (parentField as FieldContainer).AddField(value);
                    }
                    return parentField;

                }

                value = new EnumBuilder("").ReadLines(lines);
                if (value != null && parentField != null)
                    (parentField as FieldContainer).AddField(value);
                else
                {
                    lines.Clear();
                    return null;
                }
            }
            return parentField;

        }

        public virtual void Merge(FieldBase field)
        {

        }

        public override string ToString()
        {
            return GetType() + " '" + name + "'";
        }
    }
}
