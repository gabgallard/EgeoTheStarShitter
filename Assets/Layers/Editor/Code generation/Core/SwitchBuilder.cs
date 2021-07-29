using System.Collections.Generic;

namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class SwitchBuilder : FieldBase
    {

        private List<SwitchOptionBuilder> switchOptions = new List<SwitchOptionBuilder>();

        public string switchVarName { get; private set; }

        public SwitchBuilder(string switchVarName, List<SwitchOptionBuilder> switchOptions)
        {
            this.switchOptions = switchOptions;
            this.switchVarName = switchVarName;
        }

        public SwitchBuilder(string switchVarName)
        {
            this.switchVarName = switchVarName;
        }

        public void AddOption(SwitchOptionBuilder option)
        {
            switchOptions.Add(option);
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            lines.Add(Indent(indent) + "switch(" + switchVarName + "){");
            indent++;
            foreach (SwitchOptionBuilder option in switchOptions)
            {
                lines = option.WriteLines(lines, indent);
            }
            indent--;
            lines.Add(Indent(indent) + "}");
            return lines;
        }
    }
}
