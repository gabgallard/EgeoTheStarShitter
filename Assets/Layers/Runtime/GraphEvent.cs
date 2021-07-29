using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime
{
    [System.Serializable]
    public class GraphEvent
    {
        public string eventName;

        [SerializeField]
        public GraphEventCalled onGraphEventCalled = new GraphEventCalled();

        /// <summary>
        /// Clears after being called
        /// </summary>
        [SerializeField]
        public GraphEventCalled onGraphEventCalledEphemeral = new GraphEventCalled();

        [System.Serializable]
        public class GraphEventCalled : UnityEngine.Events.UnityEvent<double, Dictionary<string,object>> { }



        [SerializeField]
        public bool expose = false;

        [SerializeField]
        public bool expanded = false;

        [SerializeField]
        public string eventID = System.Guid.Empty.ToString();

        [SerializeField]
        public List<EventParameterDef> parameters = new List<EventParameterDef>();

        public void CallEvents(double time, Dictionary<string, object> data)
        {
            onGraphEventCalled?.Invoke(time,data);
            InvokeEphemerals(time, data);
        }

        public void InvokeEphemerals(double time, Dictionary<string, object> data)
        {
            onGraphEventCalledEphemeral?.Invoke(time, data);
            onGraphEventCalledEphemeral = new GraphEventCalled();
        }

        public static void CopyEvent(GraphEvent from, GraphEvent to)
        {
            to.parameters = from.parameters;
            to.eventName = from.eventName;
        }

        public GraphEvent Copy()
        {
            GraphEvent newEvent = new GraphEvent();
            newEvent.eventName = eventName;
            newEvent.eventID = eventID;
            newEvent.parameters = parameters;
            return newEvent;
        }

        [System.Serializable]
        public struct EventParameterDef
        {
            public string parameterName;
            public string parameterTypeName;

            public EventParameterDef(string parameterName, string parameterTypeName)
            {
                this.parameterName = parameterName;
                this.parameterTypeName = parameterTypeName;
            }
        }
    }
}
