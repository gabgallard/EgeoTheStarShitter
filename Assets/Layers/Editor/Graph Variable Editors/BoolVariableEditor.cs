using System;
using ABXY.Layers.Runtime;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class BoolVariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector
    {
        public override Type handlesType => typeof(bool);

        public object GetDefaultValue()
        {
            return false;
        }

        // Default value
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = LayersGUIUtilities.DrawRightAlignedCheckbox(position, label, (bool)edit.objectValue);
        }

        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }



        // Player value
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = LayersGUIUtilities.DrawRightAlignedCheckbox(position, label, (bool)edit.objectValue);
        }
        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }



        public override string GetPrettyTypeName()
        {
            return "Boolean";
        }

    }
}
