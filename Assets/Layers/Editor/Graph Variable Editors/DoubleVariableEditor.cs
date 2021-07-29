using System;
using ABXY.Layers.Runtime;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class DoubleVariableEditor : GraphVariableEditor, InputNodeInspector, PlayerInspector
    {
        public override Type handlesType => typeof(double);

        public object GetDefaultValue()
        {
            return 0.0;
        }

        public override string GetPrettyTypeName()
        {
            return "Number (Double)";
        }

        //value input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = EditorGUI.DoubleField(position, label, (double)edit.objectValue);
        }

        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        // Player
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = EditorGUI.DoubleField(position, label, (double)edit.objectValue);
        }

        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
