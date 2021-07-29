//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class FileBuilder : FieldContainer
    {
        private List<FieldBase> preamble = new List<FieldBase>();

        private List<FieldBase> fields = new List<FieldBase>();

        public bool isEmpty { get { return preamble.Count == 0 && fields.Count == 0; } }

        public override void AddField(FieldBase newField)
        {
            fields.Add(newField);
        }

        public void AddPreambleField(FieldBase newField)
        {
            preamble.Add(newField);
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            return null;
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            foreach (FieldBase field in preamble)
            {
                lines = field.WriteLines(lines, 0);
            }

            foreach (FieldBase field in fields)
            {
                lines = field.WriteLines(lines, 0);
            }
            return lines;
        }

        public override void Merge(FieldBase field)
        {
            if (field.GetType() == typeof(FileBuilder))
            {
                FileBuilder castField = field as FileBuilder;

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
    }

}