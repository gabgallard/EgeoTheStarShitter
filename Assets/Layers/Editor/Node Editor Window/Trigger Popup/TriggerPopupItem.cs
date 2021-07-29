using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editor_Window
{
    public class TriggerPopupItem
    {
        public string itemName { get; private set; }
        public string secondaryText { get; private set; }
        public System.Action onClick { get; private set; }

        SoundGraphEditorWindow editorWindow;

        private System.Guid itemGuid;

        private static System.Guid selectedID;

        public bool isHovered
        {
            get
            {
                return selectedID == itemGuid;
            }
            set
            {
                selectedID = itemGuid;
            }
        }

        public TriggerPopupItem(string itemName,string secondaryText, Action onClick, SoundGraphEditorWindow editorWindow)
        {
            this.itemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            this.secondaryText = secondaryText;
            this.onClick = onClick;
            this.editorWindow = editorWindow ?? throw new ArgumentNullException(nameof(editorWindow));
            itemGuid = System.Guid.NewGuid();
        }

        public void DoLayout()
        {
            Rect drawRect = EditorGUILayout.GetControlRect(false, 40);
            if (Event.current.type == EventType.Repaint)
            {
                Color32 currentColor = isHovered ? editorWindow.style.TriggerButtonActiveColor : editorWindow.style.TriggerButtonInactiveColor;
                EditorGUI.DrawRect(drawRect, currentColor);

                float margin = 14f;

                float labelWidth = Mathf.Min(EditorStyles.label.CalcSize(new GUIContent(itemName)).x, drawRect.width - (margin * 2f));
                Rect labelRect = new Rect(drawRect.x + margin, drawRect.y, labelWidth, drawRect.height);
                EditorGUI.LabelField(labelRect, itemName, editorWindow.style.popupMainText);


                Rect descriptorRect = new Rect(drawRect.x + margin + labelWidth + margin, drawRect.y, drawRect.width - (margin * 3f) - labelWidth, 
                    drawRect.height);
                EditorGUI.LabelField(descriptorRect, secondaryText, editorWindow.style.popupSecondaryText);
                
            }

            if (drawRect.Contains(Event.current.mousePosition))
            {
                isHovered = true;
                if (Event.current.type == EventType.MouseDown)
                {
                    onClick?.Invoke();
                    Event.current.Use();
                }
            }
        }
    }
}