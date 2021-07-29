using System;
using ABXY.Layers.Runtime;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Graph_Variable_Editors
{
    public class IntVariableEditor : GraphVariableEditor, InputNodeInspector, PlayerInspector
    {

        public override Type handlesType => typeof(int);

        // Value in input
        public void DrawInputNodeValue(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = EditorGUI.IntField(position, label, (int)edit.objectValue);
        }
        public float CalculateInputNodeValueHeight(VariableEdit edit, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }



        //Value in player
        public void DrawInPlayerInspector(Rect position, string label, VariableEdit edit)
        {
            edit.objectValue = EditorGUI.IntField(position, label, (int)edit.objectValue);
        }

        public float CalculateHeightInPlayerInspector(VariableEdit variable, string label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        public object GetDefaultValue()
        {
            return 0;
        }

        public override string GetPrettyTypeName()
        {
            return "Number (Integer)";
        }

    }
}
    
