using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode
{
    public static class PlaynodeEditorUtils
    {
    




        public static float DrawSlider(Rect position, string label, float value, float min, float max, float entryAreaWidth)
        {
            Rect labelRect = new Rect(position.x + EditorGUIUtility.standardVerticalSpacing, 
                position.y, 
                EditorGUIUtility.labelWidth - (2f * EditorGUIUtility.standardVerticalSpacing), 
                position.height);
            Rect valueRect = new Rect(position.x + position.width - entryAreaWidth + EditorGUIUtility.standardVerticalSpacing, 
                position.y, 
                entryAreaWidth - (2f * EditorGUIUtility.standardVerticalSpacing), position.height);
            Rect sliderRect = new Rect(position.x + labelRect.width + EditorGUIUtility.standardVerticalSpacing,
                position.y, 
                position.width - labelRect.width - valueRect.width - (3f* EditorGUIUtility.standardVerticalSpacing), position.height);
            EditorGUI.LabelField(labelRect, label);
            value = GUI.HorizontalSlider(sliderRect, value, min, max);
            value = EditorGUI.FloatField(valueRect, value);
            value = Mathf.Clamp(value, min, max);
            return value;
        }
    
    }
}
