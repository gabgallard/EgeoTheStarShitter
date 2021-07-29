using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class DelegateBuilder : FieldBase
    {
        private List<ParameterBuilder> parameters = new List<ParameterBuilder>();
        public string returnType { get; private set; }
        public FieldBuilder.AccessibilityValues accessibility { get; private set; }

        public DelegateBuilder(string returnType,  FieldBuilder.AccessibilityValues accessibility, string methodName)
        {
            this.returnType = returnType;
            this.name = methodName;
            this.accessibility = accessibility;
        }

        public DelegateBuilder(string returnType,  FieldBuilder.AccessibilityValues accessibility, string methodName, params ParameterBuilder[] parameters)
        {
            this.returnType = returnType;
            this.name = methodName;
            this.accessibility = accessibility;
            this.parameters = new List<ParameterBuilder>(parameters);
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
            string delegateString = Indent(indent) + accessibility.ToString().ToLower() + " delegate " +  returnType + " " + name + "( ";
            for (int index = 0; index < parameters.Count; index++)
            {
                delegateString += parameters[index].WriteLines(new List<string>(), 0)[0] + (index + 1 != parameters.Count ? ", " : " ");
            }
            delegateString += ");";
            lines.Add(delegateString);
            return lines;
        }
    }
}