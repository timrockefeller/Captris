using System.Collections.Generic;
using System;

public enum PlayEventType
{
    // OWNER_TYPE[_SUB]
    EMPTY_EVENT,
    

}


public class EventDispatcher
{

    public EventDispatcher()
    {
        events = new Dictionary<PlayEventType, List<Action>>();
    }
    private Dictionary<PlayEventType, List<Action>> events;


    public void AddEventListener(PlayEventType type, Action callback)
    {
        if (events[type] == null)
            events[type] = new List<Action>();
        events[type].Add(callback);
    }
    public void RemoveEventListener(PlayEventType type, Action callback)
    {
        if (events[type] == null)return;
        events[type].Remove(callback);
    }

    public void SendEvent(PlayEventType type)
    {
        if (events[type] == null)return;
        foreach (var _event in events[type])
        {
            _event();
        }
    }


}