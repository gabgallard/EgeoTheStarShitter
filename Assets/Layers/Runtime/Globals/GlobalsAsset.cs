using ABXY.Layers.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Graph Globals", menuName = "Sound Graph Globals")]
public class GlobalsAsset : ScriptableObject
{
    [SerializeField]
    List<GraphVariable> variables = new List<GraphVariable>();

    [SerializeField]
    List<GraphEvent> events = new List<GraphEvent>();

    [SerializeField]
    private string _assetID = "";
    public string assetID
    {
        get
        {
            if (_assetID == "")
                ResetID();
            return _assetID;
        }
    }

    /// <summary>
    /// Called whenever an event is called in this graph
    /// </summary>
    private System.Action<string, double, Dictionary<string, object>> onEventCalled;

    internal void ResetID()
    {
        _assetID = System.Guid.NewGuid().ToString();
    }


    public List<string> GetVariableNames()
    {
        List<string> names = new List<string>();
        names.AddRange(variables.Select(x => x.name));
        return names;
    }

    public List<GraphVariable> GetAllVariables()
    {
        return variables;
    }


    public List<string> GetEventNames()
    {
        List<string> names = new List<string>();
        names.AddRange(events.Select(x => x.eventName));
        return names;
    }

    public List<GraphEvent> GetAllEvents()
    {
        List<GraphEvent> names = new List<GraphEvent>();
        names.AddRange(events);
        return names;
    }

    public void CallEventByID(string id, double dspTime)
    {
        CallEventByID(id, dspTime, new Dictionary<string, object>());
    }

    public void CallEventByID(string id, double dspTime, Dictionary<string, object> data)
    {
        GraphEvent gevent = GetEventByID(id);
        if (gevent != null)
            CallEvent(gevent.eventName, dspTime, data);
    }

    public void CallEvent(string eventName, double dspTime, Dictionary<string, object> data)
    {
        GraphEvent graphEvent = GetEvent(eventName);
        if (graphEvent == null)
        {
            Debug.LogError("Event named " + eventName + " doesn't exist");
            return;
        }
        onEventCalled.Invoke(eventName, dspTime, data);
        graphEvent.onGraphEventCalled?.Invoke(dspTime, data);
    }

    public void CallEvent(string eventName, double dspTime)
    {
        CallEvent(eventName, dspTime, new Dictionary<string, object>());
    }

    public void RegisterEventListener(string eventName, UnityEngine.Events.UnityAction<double, Dictionary<string, object>> onEventQueue)
    {
        GraphEvent graphEvent = GetEvent(eventName);
        if (graphEvent == null)
        {
            Debug.LogError("Event named " + eventName + " doesn't exist");
            return;
        }
        graphEvent.onGraphEventCalled.AddListener(onEventQueue);
    }

    public void RegisterEventListenerById(string eventID, UnityEngine.Events.UnityAction<double, Dictionary<string, object>> onEventQueue)
    {
        GraphEvent graphEvent = GetEventByID(eventID);
        if (graphEvent == null)
        {
            Debug.LogError("Event with ID " + eventID + " doesn't exist");
            return;
        }
        graphEvent.onGraphEventCalled.AddListener(onEventQueue);
    }

    public void ClearAllEvents()
    {
        List<GraphEvent> allEvents = GetAllEvents();
        foreach (GraphEvent gevent in allEvents)
            gevent.onGraphEventCalled.RemoveAllListeners();
    }

    public void UnregisterEventListener(string eventName, UnityEngine.Events.UnityAction<double, Dictionary<string, object>> onEventQueue)
    {
        GraphEvent graphEvent = GetEvent(eventName);
        if (graphEvent == null)
        {
            Debug.LogError("Event named " + eventName + " doesn't exist");
            return;
        }
        graphEvent.onGraphEventCalled.RemoveListener(onEventQueue);
    }

    public void UnregisterEventListenerByID(string eventID, UnityEngine.Events.UnityAction<double, Dictionary<string, object>> onEventQueue)
    {
        GraphEvent graphEvent = GetEventByID(eventID);
        if (graphEvent == null)
        {
            Debug.LogError("Event with ID " + eventID + " doesn't exist");
            return;
        }
        graphEvent.onGraphEventCalled.RemoveListener(onEventQueue);
    }

    public void RegisterEventListener(System.Action<string, double, Dictionary<string, object>> onEventQueue)
    {
        onEventCalled -= onEventQueue;
        onEventCalled += onEventQueue;
    }


    public void UnregisterEventListener(System.Action<string, double, Dictionary<string, object>> onEventQueue)
    {
        onEventCalled -= onEventQueue;
    }

    public GraphEvent GetEvent(string eventName)
    {
        return events.FirstOrDefault();
    }

    public GraphEvent GetEventByID(string eventID)
    {
        return events.FirstOrDefault();
    }

    public bool HasEventWithID(string eventID)
    {
        return GetEventByID(eventID) != null;
    }

    public T GetVariable<T>(string name)
    {
        GraphVariable graphVar = GetGraphVariable(name);
        if (graphVar == null)
        {
            Debug.Log("Variable named " + name + " does not exist");
            return default(T);
        }


        object value = graphVar.Value();
        if (value != null && typeof(T).IsAssignableFrom(value.GetType()))
            return (T)value;
        else
        {
            Debug.LogError("Attempted to get the value of " + name + " as type " + typeof(T).Name + ", but " + name + " is of type " + value.GetType());
            return default(T);
        }
    }

    public object GetVariableValue(string name)
    {

        GraphVariable graphVar = GetGraphVariable(name);

        if (graphVar == null)
        {
            Debug.Log("Variable named " + name + " does not exist");
            return null;
        }

        return graphVar.Value();
    }

    public object GetVariableValueByID(string name)
    {


        GraphVariable graphVar = GetGraphVariableByID(name);
        if (graphVar == null)
        {
            Debug.Log("Variable named " + name + " does not exist");
            return null;
        }


        return graphVar.Value();

    }

    public void SetVariable<T>(string name, T value)
    {

        GraphVariable graphVar = GetGraphVariable(name);
        if (graphVar == null)
        {
            Debug.Log("Variable named " + name + " does not exist");
            return;
        }

        graphVar.SetValue(value);
    }

    public void SetVariable(string name, object value)
    {
        GraphVariable graphVar = GetGraphVariable(name);
        if (graphVar == null)
        {
            Debug.Log("Variable named " + name + " does not exist");
            return;
        }

        graphVar.SetValue(value);
    }

    public void SetVariableByID(string id, object value)
    {
        GraphVariable graphVar = GetGraphVariableByID(id);
        if (graphVar == null)
        {
            Debug.Log("Variable with id " + id + " does not exist");
            return;
        }

        graphVar.SetValue(value);
    }

    public GraphVariable GetGraphVariable(string name)
    {
        return variables.Find(y => y.name == name);
    }

    public GraphVariable GetGraphVariableByID(string id)
    {
        return variables.Find(y => y.variableID == id);
    }

    public bool HasGraphVariableWithID(string id)
    {
        return GetGraphVariableByID(id) != null;
    }
}
