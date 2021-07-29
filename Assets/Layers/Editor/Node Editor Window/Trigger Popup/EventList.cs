using ABXY.Layers.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ABXY.Layers.Editor.Node_Editor_Window
{
    public class EventList
    {
        List<TriggerPopupItem> triggerItems = new List<TriggerPopupItem>();

        SoundGraphEditorWindow editorWindow;
        TriggerPopup popup;

        public int count { get { return triggerItems.Count; } }

        public string windowName { get { return "Trigger Event"; } }

        public EventList(SoundGraphEditorWindow editorWindow, TriggerPopup popup)
        {
            this.editorWindow = editorWindow;
            this.popup = popup;
        }

        public void LoadItems()
        {
            triggerItems.Clear();
            foreach (var gevent in (editorWindow.graph as SoundGraph).GetAllEvents())
            {
                System.Action callFunction = () => { popup.Show(gevent); };

                string argString = "";
                if (gevent.parameters.Count != 0)
                    argString = $"({string.Join(",", gevent.parameters.Select(x => $"{x.parameterTypeName} {x.parameterName}"))})";

                triggerItems.Add(new TriggerPopupItem(gevent.eventName, argString,callFunction, editorWindow));
            }

            if (triggerItems.Count > 0)
                triggerItems.First().isHovered = true;

        }

        public float CalculateEventListHeight()
        {
            return (triggerItems.Count() * 40f) + 60;
        }

        public void DrawEventList(Rect drawRect)
        {
            GUILayout.BeginArea(drawRect);
            for (int index = 0; index < triggerItems.Count; index++)
            {
                var item = triggerItems[index];
                item.DoLayout();
            }
            GUILayout.EndArea();

        }

        public void SelectPrevious()
        {
            int selectionIndex = triggerItems.FindIndex(x => x.isHovered);
            if (selectionIndex > 0)
                triggerItems[selectionIndex - 1].isHovered = true;
        }

        public void SelectNext()
        {
            int selectionIndex = triggerItems.FindIndex(x => x.isHovered);
            if (selectionIndex >= 0 && selectionIndex < triggerItems.Count - 1)
                triggerItems[selectionIndex + 1].isHovered = true;
        }

        public void Enter()
        {
            int selectionIndex = triggerItems.FindIndex(x => x.isHovered);

            if (selectionIndex >= 0 && selectionIndex < triggerItems.Count)
                triggerItems[selectionIndex].onClick?.Invoke();
        }

        public void Back()
        {
            popup.Hide();
        }
    }
}