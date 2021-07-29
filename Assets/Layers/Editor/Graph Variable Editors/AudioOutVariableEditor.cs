using System;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.FlowTypes;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class AudioOutVariableEditor : GraphVariableEditor
    {
        public override Type handlesType => typeof(AudioFlow);


        public override string GetPrettyTypeName()
        {
            return "Audio Out";
        }

    }
}
