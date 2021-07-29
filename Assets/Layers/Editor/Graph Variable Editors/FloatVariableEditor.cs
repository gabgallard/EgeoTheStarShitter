using System;
using ABXY.Layers.Runtime;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class FloatVariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector
    {
  

        public override Type handlesType => typeof(float);


        // Value input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = EditorGUI.FloatField(position, label, (float)edit.objectValue);
        }
        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }



        // Player inspector
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {

            edit.objectValue = EditorGUI.FloatField(position, label, (float)edit.objectValue);
        }
        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        public object GetDefaultValue()
        {
            return 0f;
        }

        public override string GetPrettyTypeName()
        {
            return "Number (Float)";
        }

    }
}
