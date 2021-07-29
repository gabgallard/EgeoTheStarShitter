using System;
using ABXY.Layers.Runtime;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class StringVariableEditor : GraphVariableEditor, PlayerInspector, InputNodeInspector
    {
        public override Type handlesType => typeof(string);

        public object GetDefaultValue()
        {
            return "";
        }


        //Default value input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = EditorGUI.TextField(position, new GUIContent(label), (string)edit.objectValue);
        }
        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

       

        //Default value Player
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = EditorGUI.TextField(position, new GUIContent(label), (string)edit.objectValue);
        }
        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        public override string GetPrettyTypeName()
        {
            return "String";
        }

    }
}
