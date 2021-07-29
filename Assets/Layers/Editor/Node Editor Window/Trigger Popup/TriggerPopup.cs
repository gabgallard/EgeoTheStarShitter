using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Editor.ThirdParty.Xnode;
using ABXY.Layers.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityAsync;
using System.Threading.Tasks;

namespace ABXY.Layers.Editor.Node_Editor_Window {
    public class TriggerPopup
    {
        private enum States { Hidden, EventList, EventParams }

        private States state { 
            get; 
            set; 
        }


        private EventList eventList;
        private ParameterList parameterList;


        SoundGraphEditorWindow editorWindow;

        /// <summary>
        /// Used to change the number of items unity is drawing without throwing 
        /// a repaint error
        /// </summary>
        private System.Action deferredForLayoutActions;

        public int eventCount { get { return eventList.count; } }

        public TriggerPopup(SoundGraphEditorWindow editorWindow)
        {
            this.editorWindow = editorWindow;
            eventList = new EventList(editorWindow, this);
            parameterList = new ParameterList(editorWindow,this);
        }

        public void Show()
        {
            eventList.LoadItems();

            if (eventList.count == 1)
                Show((editorWindow.graph as SoundGraph).GetAllEvents().First());
            else
            {
                deferredForLayoutActions += () =>
                {
                    state = States.EventList;
                };
            }
        }


        public void Show(GraphEvent gevent)
        {
            deferredForLayoutActions += ()=>{
                eventList.LoadItems();
                parameterList.LoadItems(gevent);

                if (gevent.parameters.Count == 0)// then just run it
                {
                    RunEvent(gevent);
                    Hide();
                }
                else
                {
                    
                    state = States.EventParams;
                }
            };
            

        }

        public void Hide()
        {
            state = States.Hidden;
        }



        public void CheckForSpacebarEvent()
        {
            if (state == States.Hidden && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
            {
                Show();
                Event.current.Use();
            }
        }

        public void Draw()
        {
            if (Event.current.type == EventType.Layout)
            {
                deferredForLayoutActions?.Invoke();
                deferredForLayoutActions = null;
            }

            

            if (editorWindow.graph == null)
                state = States.Hidden;

            if (state == States.Hidden)
                return;

            EditorGUI.DrawRect(new Rect(0, 0, editorWindow.position.width, editorWindow.position.height), editorWindow.style.popupBackgroundColor);
            var nodeBodyStyle =  NodeEditorResources.styles.nodeBody;

            float popupHeight = state == States.EventList? eventList.CalculateEventListHeight(): parameterList.CalculateParameterListHeight();
            float popupWidth = 400;
            Rect totalBoxRect = new Rect((editorWindow.position.width - popupWidth)/2f, (editorWindow.position.height  - popupHeight) / 2f, popupWidth, popupHeight);

            Color originalGUIColor = GUI.color;
            GUI.color = editorWindow.style.nodeBackgroundColor;

            GUI.Box(totalBoxRect, "", nodeBodyStyle);
            GUI.color = originalGUIColor;

            string windowLabel = state == States.EventList ? eventList.windowName : parameterList.windowName;
            Rect headerRect = new Rect(totalBoxRect.x, totalBoxRect.y, totalBoxRect.width, 40);
            DrawHeader(headerRect, windowLabel);

            float headerHeight = 30f;
            float lrMargins = 6f;

            Rect listRect = new Rect(totalBoxRect.x + lrMargins, totalBoxRect.y + headerHeight, totalBoxRect.width-(2f* lrMargins), totalBoxRect.height - headerHeight);

            if (state == States.EventList)
                eventList.DrawEventList(listRect);
            else if (state == States.EventParams)
            {
                parameterList.DrawParameterList(listRect);
            }


            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.UpArrow)
                SelectPrevious();


            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.DownArrow)
                SelectNext();


            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                Back();

            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Space))
            {
                Enter();
            }

            //blocking clicks through background
            if (state != States.Hidden && Event.current.type == EventType.MouseDown)
                Event.current.Use();
        }

        private void DrawHeader(Rect headerArea, string text)
        {
            float margin = 15f;
            Rect labelRect = new Rect(headerArea.x + margin, headerArea.y-2, headerArea.width - (2f * margin), headerArea.height);
            EditorGUI.LabelField(labelRect, text, editorWindow.style.popupHeaderStyle);

            Rect closeRect = new Rect(headerArea.x + headerArea.width - 25, headerArea.y, headerArea.height, headerArea.height);
            if (GUI.Button(closeRect, "X", editorWindow.style.popupHeaderStyle))
                Hide();
        }

        private void Enter()
        {
            eventList.Enter();
        }

        private void Back()
        {
            if (state == States.EventList)
                eventList.Back();
            else if (state == States.EventParams)
                parameterList.Back();
            Event.current.Use();
        }

        private void SelectPrevious()
        {
            eventList.SelectPrevious();
        }

        private void SelectNext()
        {
            eventList.SelectNext();
        }


        public void RunEvent(GraphEvent gevent)
        {
            RunEvent(gevent, null);
        }
        public void RunEvent(GraphEvent gevent, List<GraphVariable> parameters)
        {
            Hide();
            Dictionary<string, object> parametersOut = new Dictionary<string, object>();

            if (parameters != null)
            {
                foreach (GraphVariable parameter in parameters)
                    parametersOut.Add(parameter.name, parameter.DefaultValue());
            }

            SoundGraph soundGraph = editorWindow.graph as SoundGraph;

            if (soundGraph == null)
                return;

            soundGraph.CallEventByID(gevent.eventID, AudioSettings.dspTime, parametersOut,0);
            
        }

    }
}