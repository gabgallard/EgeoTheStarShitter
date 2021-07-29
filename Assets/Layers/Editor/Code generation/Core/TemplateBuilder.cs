using Scriban;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Editor.Code_generation.Core
{
    public class TemplateBuilder : FieldBase
    {
        string templateString;
        object model;

        public TemplateBuilder(string template, object model)
        {
            this.templateString = template ?? throw new ArgumentNullException(nameof(template));
            this.model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            try
            {
                var template = Template.Parse(templateString);
                var result = template.Render(model);

                lines.Add(result);
            }catch(System.Exception e)
            {
                Debug.LogError(e);
            }

            return lines;
        }
    }
}